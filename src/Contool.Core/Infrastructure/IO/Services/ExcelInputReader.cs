using Contool.Core.Infrastructure.IO.Models;
using Contool.Core.Infrastructure.Utils.Models;
using OfficeOpenXml;
using System.Dynamic;
using System.Runtime.CompilerServices;

namespace Contool.Core.Infrastructure.IO.Services;

public class ExcelInputReader : IInputReader
{
    public DataSource DataSource => DataSource.Excel;

    public IAsyncEnumerableWithTotal<dynamic> ReadAsync(string path, CancellationToken cancellationToken)
    {
        return new AsyncEnumerableWithTotal<dynamic>(
            ReadExcelAsync(path, cancellationToken),
            () => GetTotalRowCount(path));
    }

    private static async IAsyncEnumerable<dynamic> ReadExcelAsync(string path, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ExcelPackage.License.SetNonCommercialPersonal("My Name");
        using var package = new ExcelPackage(new FileInfo(path));
        var worksheet = package.Workbook.Worksheets[0]; // Use first worksheet

        if (worksheet.Dimension == null)
            yield break;

        // Read header row
        var headerRow = 1;
        var headers = new List<string>();

        for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
        {
            var headerValue = worksheet.Cells[headerRow, col].Value?.ToString() ?? $"Column{col}";
            headers.Add(headerValue);
        }

        // Read data rows
        for (int row = headerRow + 1; row <= worksheet.Dimension.End.Row; row++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var record = new ExpandoObject() as IDictionary<string, object>;
            bool hasData = false;

            for (int col = 1; col <= headers.Count; col++)
            {
                var cellValue = worksheet.Cells[row, col].Value;
                var headerName = headers[col - 1];

                if (cellValue != null)
                {
                    hasData = true;
                    record[headerName] = ConvertCellValue(cellValue)!;
                }
                else
                {
                    record[headerName] = null!;
                }
            }

            // Only yield rows that have at least some data
            if (hasData)
                yield return record;

            // Yield control periodically for async operation
            if (row % 100 == 0)
                await Task.Yield();
        }
    }

    private static int GetTotalRowCount(string path)
    {
        using var package = new ExcelPackage(new FileInfo(path));
        var worksheet = package.Workbook.Worksheets[0];

        if (worksheet.Dimension == null)
            return 0;

        // Count non-empty rows (excluding header)
        int totalRows = 0;
        for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
        {
            bool hasData = false;
            for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
            {
                if (worksheet.Cells[row, col].Value != null)
                {
                    hasData = true;
                    break;
                }
            }
            if (hasData)
                totalRows++;
        }

        return totalRows;
    }

    private static object? ConvertCellValue(object cellValue)
    {
        return cellValue switch
        {
            double d => d,
            decimal dec => dec,
            int i => i,
            long l => l,
            bool b => b,
            string s => s,
            // Default to string representation
            _ => cellValue.ToString()
        };
    }
}