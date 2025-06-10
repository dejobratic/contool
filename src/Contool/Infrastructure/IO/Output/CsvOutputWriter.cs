using Contool.Infrastructure.IO.Models;
using CsvHelper;
using System.Globalization;

namespace Contool.Infrastructure.IO.Output;

internal class CsvOutputWriter : IOutputWriter
{
    public DataSource DataSource => DataSource.Csv;

    public async Task SaveAsync(OutputContent output, CancellationToken cancellationToken)
    {
        if (output.Headings.Length == 0)
            throw new InvalidOperationException("No headings found in output.");

        await using var writer = new StreamWriter(output.FullPath);
        await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

        // Write header
        foreach (var header in output.Headings)
        {
            csv.WriteField(header);
        }

        await csv.NextRecordAsync();

        // Write records
        foreach (var row in output.Rows)
        {
            if (row is not IDictionary<string, object?> record)
                throw new InvalidOperationException("Expected dictionary for data row.");

            foreach (var header in output.Headings)
            {
                record.TryGetValue(header.Value, out var value);
                csv.WriteField(value);
            }

            await csv.NextRecordAsync();
        }
    }
}
