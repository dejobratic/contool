using Contool.Core.Infrastructure.IO.Models;
using Contool.Core.Infrastructure.Utils.Models;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Contool.Core.Infrastructure.IO.Services;

public class JsonInputReader : IInputReader
{
    public DataSource DataSource => DataSource.Json;

    public IAsyncEnumerableWithTotal<dynamic> ReadAsync(string path, CancellationToken cancellationToken)
    {
        var rows = ReadRowsAsync(path, cancellationToken);
        var total = GetTotalCount(path);

        var result = new AsyncEnumerableWithTotal<dynamic>(
            rows,
            () => total);
        
        // Set total immediately instead of waiting for enumeration
        typeof(AsyncEnumerableWithTotal<dynamic>)
            .GetProperty("Total")!
            .SetValue(result, total);

        return result;
    }

    private static int GetTotalCount(string path)
    {
        var jsonText = File.ReadAllText(path);
        using var document = JsonDocument.Parse(jsonText);
        
        if (document.RootElement.ValueKind == JsonValueKind.Array)
        {
            return document.RootElement.GetArrayLength();
        }
        
        return 1; // Single object
    }

    private static async IAsyncEnumerable<dynamic> ReadRowsAsync(
        string path,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
        
        if (document.RootElement.ValueKind == JsonValueKind.Array)
        {
            foreach (var element in document.RootElement.EnumerateArray())
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return JsonElementToDictionary(element);
            }
        }
        else
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return JsonElementToDictionary(document.RootElement);
        }
    }

    private static Dictionary<string, object?> JsonElementToDictionary(JsonElement element)
    {
        var dictionary = new Dictionary<string, object?>();
        
        if (element.ValueKind == JsonValueKind.Object)
        {
            foreach (var property in element.EnumerateObject())
            {
                dictionary[property.Name] = GetJsonValue(property.Value);
            }
        }
        
        return dictionary;
    }

    private static object? GetJsonValue(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.String => element.GetString(),
            JsonValueKind.Number => element.GetDouble(), // Always use double for consistency
            JsonValueKind.True => true,  
            JsonValueKind.False => false,
            JsonValueKind.Null => null,
            JsonValueKind.Array => element.EnumerateArray().Select(GetJsonValue).ToArray(),
            JsonValueKind.Object => JsonElementToDictionary(element),
            _ => element.GetRawText()
        };
    }
}
