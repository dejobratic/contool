using Contool.Models;
using CsvHelper;
using System.Dynamic;
using System.Globalization;

namespace Contool.Services;

internal class CsvInputReader : IInputReader
{
    public DataSource DataSource => DataSource.Csv;

    public async Task<Content> ReadAsync(string path, CancellationToken cancellationToken)
    {
        var input = new Content();

        using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var reader = new StreamReader(stream);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        // Read header
        await csv.ReadAsync();
        csv.ReadHeader();
        input.SetHeadings(csv.HeaderRecord!);

        // Read rows
        while (await csv.ReadAsync())
        {
            cancellationToken.ThrowIfCancellationRequested();

            var row = new ExpandoObject() as IDictionary<string, object?>;

            foreach (var header in input.Headings)
            {
                row[header.Value] = csv.GetField(header.Value);
            }

            input.AddRow(row);
        }

        return input;
    }
}
