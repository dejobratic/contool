using Contool.Core.Infrastructure.IO.Models;
using CsvHelper;
using System.Globalization;

namespace Contool.Core.Infrastructure.IO.Output;

public class CsvOutputWriter : IOutputWriter
{
    public DataSource DataSource => DataSource.Csv;

    public async Task SaveAsync(string path, IAsyncEnumerable<dynamic> content, CancellationToken cancellationToken)
    {
        await using var writer = new StreamWriter(path);
        await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

        // pull off first row to infer headers
        await using var enumerator = content
            .WithCancellation(cancellationToken)
            .GetAsyncEnumerator();

        if (!await enumerator.MoveNextAsync())
            return; // nothing to write

        if (enumerator.Current is not IDictionary<string, object?> firstRecord)
            throw new InvalidOperationException("Expected each row to be IDictionary<string, object?>");

        // infer header list
        var headers = firstRecord.Keys.ToList();

        // write header
        foreach (var h in headers)
            csv.WriteField(h);

        await csv.NextRecordAsync();

        // helper to write a single record by header order
        static async Task WriteRecordAsync(
            CsvWriter csv,
            IList<string> headers,
            IDictionary<string, object?> record)
        {
            foreach (var h in headers)
                csv.WriteField(record.TryGetValue(h, out var v) ? v : null);

            await csv.NextRecordAsync();
        }

        // write first record
        await WriteRecordAsync(csv, headers, firstRecord);

        // write remaining
        while (await enumerator.MoveNextAsync())
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (enumerator.Current is not IDictionary<string, object?> record)
                throw new InvalidOperationException("Expected each row to be IDictionary<string, object?>");

            await WriteRecordAsync(csv, headers, record);
        }
    }
}
