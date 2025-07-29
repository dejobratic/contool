using Contool.Core.Infrastructure.IO.Models;
using Contool.Core.Infrastructure.Utils.Models;
using CsvHelper;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Contool.Core.Infrastructure.IO.Services;

public class CsvInputReader : IInputReader
{
    public DataSource DataSource => DataSource.Csv;

    public IAsyncEnumerableWithTotal<dynamic> ReadAsync(
        string path,
        CancellationToken cancellationToken)
    {
        var rows = ReadRowsAsync(path, cancellationToken);
        var total = CountDataRows(path);

        var result = new AsyncEnumerableWithTotal<dynamic>(
            rows,
            () => total);
        
        // Set total immediately instead of waiting for enumeration
        typeof(AsyncEnumerableWithTotal<dynamic>)
            .GetProperty("Total")!
            .SetValue(result, total);

        return result;
    }

    private static async IAsyncEnumerable<dynamic> ReadRowsAsync(
        string path,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var reader = new StreamReader(stream);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        if (!await csv.ReadAsync())
            yield break;

        csv.ReadHeader();
        var headers = csv.HeaderRecord!;

        while (await csv.ReadAsync())
        {
            cancellationToken.ThrowIfCancellationRequested();

            var row = new Dictionary<string, object?>(headers.Length);
            foreach (var h in headers)
                row[h] = csv.GetField(h);

            yield return row;
        }
    }

    private static int CountDataRows(string path)
    {
        using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var reader = new StreamReader(stream);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        
        var count = 0;
        if (csv.Read()) // Skip header
        {
            csv.ReadHeader();
            while (csv.Read())
            {
                count++;
            }
        }
        
        return count;
    }
}
