using OfficeOpenXml;
using System.Collections;
using System.Dynamic;
using Contool.Core.Infrastructure.IO.Models;

namespace Contool.Core.Infrastructure.IO.Output;

public class ExcelOutputWriter : IOutputWriter
{
    public DataSource DataSource => DataSource.Excel;

    public async Task SaveAsync(string path, IAsyncEnumerable<dynamic> content, CancellationToken cancellationToken)
    {
        ExcelPackage.License.SetNonCommercialPersonal("My Name");
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Sheet1");

        int currentRow = 1;
        bool headerWritten = false;
        string[]? headers = null;

        await foreach (var record in content.WithCancellation(cancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!headerWritten)
            {
                headers = ExtractHeaders(record);
                if (headers.Length == 0) continue; // Skip if no headers found

                WriteHeaders(worksheet, currentRow, headers);
                headerWritten = true;
                currentRow++;
            }

            WriteRecord(worksheet, currentRow, record, headers);
            currentRow++;

            // Yield control periodically for async operation
            if (currentRow % 100 == 0)
                await Task.Yield();
        }

        // Only save if we have data
        if (headerWritten && currentRow > 1)
        {
            // Format the worksheet
            FormatWorksheet(worksheet, currentRow - 1);

            // Ensure the directory exists
            var fileInfo = new FileInfo(path);
            fileInfo.Directory?.Create();

            // Save the file
            await package.SaveAsAsync(fileInfo, cancellationToken);
        }
        else if (!headerWritten)
        {
            // Create an empty file with at least one cell to avoid corruption
            worksheet.Cells[1, 1].Value = "No Data";
            var fileInfo = new FileInfo(path);
            fileInfo.Directory?.Create();
            await package.SaveAsAsync(fileInfo, cancellationToken);
        }
    }

    private static string[] ExtractHeaders(dynamic record)
    {
        return record switch
        {
            ExpandoObject expando => (expando as IDictionary<string, object>).Keys.ToArray(),
            IDictionary<string, object> dict => dict.Keys.ToArray(),
            IDictionary dict => dict.Keys.Cast<string>().ToArray(),
            _ => ExtractHeadersFromObject(record)
        };
    }

    private static string[] ExtractHeadersFromObject(object obj)
    {
        var type = obj.GetType();
        return [.. type.GetProperties()
            .Where(p => p.CanRead)
            .Select(p => p.Name)];
    }

    private static void WriteHeaders(ExcelWorksheet worksheet, int row, string[] headers)
    {
        for (int col = 0; col < headers.Length; col++)
        {
            worksheet.Cells[row, col + 1].Value = headers[col];
        }
    }

    private static void WriteRecord(ExcelWorksheet worksheet, int row, dynamic record, string[]? headers)
    {
        if (headers == null) return;

        switch (record)
        {
            case ExpandoObject expando:
                WriteFromDictionary(worksheet, row, expando as IDictionary<string, object>, headers);
                break;
            case IDictionary<string, object> dict:
                WriteFromDictionary(worksheet, row, dict, headers);
                break;
            case IDictionary dict:
                WriteFromGenericDictionary(worksheet, row, dict, headers);
                break;
            default:
                WriteFromObject(worksheet, row, record, headers);
                break;
        }
    }

    private static void WriteFromDictionary(ExcelWorksheet worksheet, int row, IDictionary<string, object> dict, string[] headers)
    {
        for (int col = 0; col < headers.Length; col++)
        {
            var header = headers[col];
            if (dict.TryGetValue(header, out var value))
            {
                worksheet.Cells[row, col + 1].Value = ConvertValueForExcel(value);
            }
        }
    }

    private static void WriteFromGenericDictionary(ExcelWorksheet worksheet, int row, IDictionary dict, string[] headers)
    {
        for (int col = 0; col < headers.Length; col++)
        {
            var header = headers[col];
            if (dict.Contains(header))
            {
                var value = dict[header];
                worksheet.Cells[row, col + 1].Value = ConvertValueForExcel(value);
            }
        }
    }

    private static void WriteFromObject(ExcelWorksheet worksheet, int row, object obj, string[] headers)
    {
        var type = obj.GetType();
        var properties = type.GetProperties().ToDictionary(p => p.Name, p => p);

        for (int col = 0; col < headers.Length; col++)
        {
            var header = headers[col];
            if (properties.TryGetValue(header, out var property) && property.CanRead)
            {
                var value = property.GetValue(obj);
                worksheet.Cells[row, col + 1].Value = ConvertValueForExcel(value);
            }
        }
    }

    private static object? ConvertValueForExcel(object? value)
    {
        return value switch
        {
            null => null,
            DBNull => null,
            DateTime dt => dt,
            DateTimeOffset dto => dto.DateTime,
            DateOnly dateOnly => dateOnly.ToDateTime(TimeOnly.MinValue),
            TimeOnly timeOnly => timeOnly.ToTimeSpan(),
            decimal => value,
            double d when double.IsNaN(d) || double.IsInfinity(d) => null,
            float f when float.IsNaN(f) || float.IsInfinity(f) => null,
            double => value,
            float => value,
            int => value,
            long => value,
            short => value,
            byte => value,
            bool => value,
            string str when string.IsNullOrEmpty(str) => null,
            string str => str.Trim(),
            // Handle collections by converting to comma-separated string
            IEnumerable<object> enumerable when value is not string =>
                string.Join(", ", enumerable.Where(x => x != null).Select(x => x.ToString()?.Trim()).Where(x => !string.IsNullOrEmpty(x))),
            IEnumerable enumerable when value is not string =>
                string.Join(", ", enumerable.Cast<object>().Where(x => x != null).Select(x => x.ToString()?.Trim()).Where(x => !string.IsNullOrEmpty(x))),
            // Default to string representation, but clean it up
            _ => value.ToString()?.Trim()
        };
    }

    private static void FormatWorksheet(ExcelWorksheet worksheet, int totalRows)
    {
        if (totalRows <= 0 || worksheet.Dimension == null) return;

        var endColumn = worksheet.Dimension.End.Column;
        var range = worksheet.Cells[1, 1, totalRows, endColumn];

        // Format header row
        if (totalRows > 0)
        {
            var headerRange = worksheet.Cells[1, 1, 1, endColumn];
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            headerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        }

        // Add borders to all cells with data
        range.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
        range.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
        range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
        range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

        // Auto-fit columns - use the actual range
        worksheet.Cells[1, 1, totalRows, endColumn].AutoFitColumns();

        // Freeze header row only if we have data
        if (totalRows > 1)
        {
            worksheet.View.FreezePanes(2, 1);
        }
    }
}