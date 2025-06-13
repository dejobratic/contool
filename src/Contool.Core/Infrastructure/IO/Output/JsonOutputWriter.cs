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

    public async Task SaveAsync(OutputContent output, CancellationToken cancellationToken)
    {
        if (output.Headings.Length == 0)
            throw new InvalidOperationException("No headings found in output.");

        var outputPath = Path.Combine(output.FullPath);

        // Normalize rows into a list of dictionaries with values ordered by output.Headings
        var normalizedRows = new List<Dictionary<string, object?>>();

        foreach (var row in output.Rows)
        {
            if (row is not IDictionary<string, object?> record)
                throw new InvalidOperationException("Expected dictionary for data row.");

            var normalized = new Dictionary<string, object?>();
            foreach (var header in output.Headings)
            {
                record.TryGetValue(header.Value, out var value);
                normalized[header.Value] = value;
            }

            normalizedRows.Add(normalized);
        }

        await using var stream = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.None);
        await JsonSerializer.SerializeAsync(stream, normalizedRows, JsonOptions, cancellationToken);
    }
}
