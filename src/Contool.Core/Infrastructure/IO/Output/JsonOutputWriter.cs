using Contool.Core.Infrastructure.IO.Models;
using System.Text.Json;

namespace Contool.Core.Infrastructure.IO.Output;

public class JsonOutputWriter : IOutputWriter
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
    };

    public DataSource DataSource => DataSource.Json;

    public async Task SaveAsync(string path, IAsyncEnumerable<dynamic> content, CancellationToken cancellationToken)
    {
        await using var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
        using var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true });

        writer.WriteStartArray();

        await foreach (var item in content.WithCancellation(cancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (item is not IDictionary<string, object?> record)
                throw new InvalidOperationException("Expected each row to be IDictionary<string, object?>");

            writer.WriteStartObject();

            foreach (var kvp in record)
            {
                writer.WritePropertyName(kvp.Key);
                // Leverage JsonSerializer to handle nulls and complex values
                JsonSerializer.Serialize(writer, kvp.Value, JsonOptions);
            }

            writer.WriteEndObject();
        }

        writer.WriteEndArray();
        await writer.FlushAsync(cancellationToken);
    }
}
