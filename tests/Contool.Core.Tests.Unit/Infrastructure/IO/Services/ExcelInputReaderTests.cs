using Contool.Core.Infrastructure.IO.Models;
using Contool.Core.Infrastructure.IO.Services;
using OfficeOpenXml;

namespace Contool.Core.Tests.Unit.Infrastructure.IO.Services;

public class ExcelInputReaderTests
{
    private readonly ExcelInputReader _reader = new();

    [Fact]
    public void GivenExcelInputReader_WhenInstantiated_ThenDataSourceIsCorrect()
    {
        // Act
        var dataSource = _reader.DataSource;

        // Assert
        Assert.Equal(DataSource.Excel, dataSource);
    }

    [Fact]
    public async Task GivenValidExcelFile_WhenRead_ThenReturnsCorrectData()
    {
        // Arrange
        var filePath = CreateExcelFileAsPath(worksheet =>
        {
            // Headers
            worksheet.Cells[1, 1].Value = "Name";
            worksheet.Cells[1, 2].Value = "Age";
            worksheet.Cells[1, 3].Value = "Email";
            
            // Data rows
            worksheet.Cells[2, 1].Value = "John Doe";
            worksheet.Cells[2, 2].Value = 30;
            worksheet.Cells[2, 3].Value = "john@example.com";
            
            worksheet.Cells[3, 1].Value = "Jane Smith";
            worksheet.Cells[3, 2].Value = 25;
            worksheet.Cells[3, 3].Value = "jane@example.com";
        });

        try
        {
            // Act
            var result = _reader.ReadAsync(filePath, CancellationToken.None);

            // Assert
            Assert.Equal(2, result.Total);
            
            var records = await result.ToListAsync();
            Assert.Equal(2, records.Count);

            // Verify first record
            var firstRecord = records[0] as IDictionary<string, object>;
            Assert.NotNull(firstRecord);
            Assert.Equal("John Doe", firstRecord["Name"]);
            Assert.Equal(30, firstRecord["Age"]);
            Assert.Equal("john@example.com", firstRecord["Email"]);
        }
        finally
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }

    [Fact]
    public async Task GivenExcelWithMixedDataTypes_WhenRead_ThenPreservesTypes()
    {
        // Arrange
        var filePath = CreateExcelFileAsPath(worksheet =>
        {
            worksheet.Cells[1, 1].Value = "Product";
            worksheet.Cells[1, 2].Value = "Price";
            worksheet.Cells[1, 3].Value = "InStock";
            worksheet.Cells[1, 4].Value = "LaunchDate";
            
            worksheet.Cells[2, 1].Value = "Widget A";
            worksheet.Cells[2, 2].Value = 29.99;
            worksheet.Cells[2, 3].Value = true;
            worksheet.Cells[2, 4].Value = new DateTime(2023, 1, 15);
            
            worksheet.Cells[2, 4].Style.Numberformat.Format = "yyyy-mm-dd";
        });

        try
        {
            // Act
            var result = _reader.ReadAsync(filePath, CancellationToken.None);

            // Assert
            var records = await result.ToListAsync();
            Assert.Single(records);

            var record = records[0] as IDictionary<string, object>;
            Assert.NotNull(record);
            Assert.Equal("Widget A", record["Product"]);
            Assert.Equal(29.99, record["Price"]);
            Assert.Equal(true, record["InStock"]);
            Assert.IsType<DateTime>(record["LaunchDate"]);
        }
        finally
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }

    [Fact]
    public async Task GivenExcelWithEmptyCells_WhenRead_ThenHandlesNullValues()
    {
        // Arrange
        var filePath = CreateExcelFileAsPath(worksheet =>
        {
            worksheet.Cells[1, 1].Value = "Name";
            worksheet.Cells[1, 2].Value = "Age";
            worksheet.Cells[1, 3].Value = "Email";
            
            worksheet.Cells[2, 1].Value = "John Doe";
            worksheet.Cells[2, 2].Value = 30;
            worksheet.Cells[2, 3].Value = "john@example.com";
            
            worksheet.Cells[3, 1].Value = "Jane Smith";
            // Age cell is empty
            worksheet.Cells[3, 3].Value = "jane@example.com";
        });

        try
        {
            // Act
            var result = _reader.ReadAsync(filePath, CancellationToken.None);

            // Assert
            var records = await result.ToListAsync();
            Assert.Equal(2, records.Count);

            var secondRecord = records[1] as IDictionary<string, object>;
            Assert.NotNull(secondRecord);
            Assert.Equal("Jane Smith", secondRecord["Name"]);
            Assert.Null(secondRecord["Age"]);
            Assert.Equal("jane@example.com", secondRecord["Email"]);
        }
        finally
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }

    [Fact]
    public async Task GivenExcelWithFormulas_WhenRead_ThenReturnsCalculatedValues()
    {
        // Arrange
        var filePath = CreateExcelFileAsPath(worksheet =>
        {
            worksheet.Cells[1, 1].Value = "Quantity";
            worksheet.Cells[1, 2].Value = "Price";
            worksheet.Cells[1, 3].Value = "Total";
            
            worksheet.Cells[2, 1].Value = 5;
            worksheet.Cells[2, 2].Value = 10.50;
            worksheet.Cells[2, 3].Formula = "A2*B2";
            
            // Calculate formulas
            worksheet.Calculate();
        });

        try
        {
            // Act
            var result = _reader.ReadAsync(filePath, CancellationToken.None);

            // Assert
            var records = await result.ToListAsync();
            Assert.Single(records);

            var record = records[0] as IDictionary<string, object>;
            Assert.NotNull(record);
            Assert.Equal(5, record["Quantity"]);
            Assert.Equal(10.50, record["Price"]);
            Assert.Equal(52.5, record["Total"]);
        }
        finally
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }

    [Fact]
    public async Task GivenExcelWithOnlyHeaders_WhenRead_ThenReturnsEmptyResult()
    {
        // Arrange
        var filePath = CreateExcelFileAsPath(worksheet =>
        {
            worksheet.Cells[1, 1].Value = "Name";
            worksheet.Cells[1, 2].Value = "Age";
            worksheet.Cells[1, 3].Value = "Email";
        });

        try
        {
            // Act
            var result = _reader.ReadAsync(filePath, CancellationToken.None);

            // Assert
            Assert.Equal(0, result.Total);
            
            var records = await result.ToListAsync();
            Assert.Empty(records);
        }
        finally
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }

    [Fact]
    public async Task GivenLargeExcelFile_WhenRead_ThenStreamsEfficiently()
    {
        // Arrange
        const int recordCount = 500;
        var filePath = CreateExcelFileAsPath(worksheet =>
        {
            worksheet.Cells[1, 1].Value = "Id";
            worksheet.Cells[1, 2].Value = "Name";
            worksheet.Cells[1, 3].Value = "Value";
            
            for (int i = 1; i <= recordCount; i++)
            {
                worksheet.Cells[i + 1, 1].Value = i;
                worksheet.Cells[i + 1, 2].Value = $"Record {i}";
                worksheet.Cells[i + 1, 3].Value = i * 10;
            }
        });

        try
        {
            // Act
            var result = _reader.ReadAsync(filePath, CancellationToken.None);

            // Assert
            Assert.Equal(recordCount, result.Total);
            
            var processedCount = 0;
            await foreach (var record in result)
            {
                processedCount++;
                var recordDict = record as IDictionary<string, object>;
                Assert.NotNull(recordDict);
                Assert.Equal(processedCount, recordDict["Id"]);
            }
            
            Assert.Equal(recordCount, processedCount);
        }
        finally
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }

    [Fact]
    public async Task GivenCancellationToken_WhenCancelled_ThenThrowsOperationCancelledException()
    {
        // Arrange
        var filePath = CreateExcelFileAsPath(worksheet =>
        {
            worksheet.Cells[1, 1].Value = "Name";
            worksheet.Cells[2, 1].Value = "John";
            worksheet.Cells[3, 1].Value = "Jane";
        });
        
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        try
        {
            // Act & Assert
            var result = _reader.ReadAsync(filePath, cts.Token);
            await Assert.ThrowsAsync<OperationCanceledException>(async () =>
            {
                await foreach (var record in result)
                {
                    // This should throw immediately due to cancellation
                }
            });
        }
        finally
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }

    [Fact]
    public async Task GivenExcelWithSpecialCharacters_WhenRead_ThenPreservesCharacters()
    {
        // Arrange
        var filePath = CreateExcelFileAsPath(worksheet =>
        {
            worksheet.Cells[1, 1].Value = "Name";
            worksheet.Cells[1, 2].Value = "Description";
            
            worksheet.Cells[2, 1].Value = "André";
            worksheet.Cells[2, 2].Value = "Café & résumé with special chars: ñáéíóú";
        });

        try
        {
            // Act
            var result = _reader.ReadAsync(filePath, CancellationToken.None);

            // Assert
            var records = await result.ToListAsync();
            Assert.Single(records);

            var record = records[0] as IDictionary<string, object>;
            Assert.NotNull(record);
            Assert.Equal("André", record["Name"]);
            Assert.Equal("Café & résumé with special chars: ñáéíóú", record["Description"]);
        }
        finally
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }

    [Fact]
    public async Task GivenExcelWithNumericHeaders_WhenRead_ThenConvertsHeadersToStrings()
    {
        // Arrange
        var filePath = CreateExcelFileAsPath(worksheet =>
        {
            worksheet.Cells[1, 1].Value = 2023;
            worksheet.Cells[1, 2].Value = 2024;
            
            worksheet.Cells[2, 1].Value = "Q1 Data";
            worksheet.Cells[2, 2].Value = "Q2 Data";
        });

        try
        {
            // Act
            var result = _reader.ReadAsync(filePath, CancellationToken.None);

            // Assert
            var records = await result.ToListAsync();
            Assert.Single(records);

            var record = records[0] as IDictionary<string, object>;
            Assert.NotNull(record);
            Assert.True(record.ContainsKey("2023"));
            Assert.True(record.ContainsKey("2024"));
            Assert.Equal("Q1 Data", record["2023"]);
            Assert.Equal("Q2 Data", record["2024"]);
        }
        finally
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }

    [Fact]
    public async Task GivenEmptyExcelFile_WhenRead_ThenThrowsException()
    {
        // Arrange
        var filePath = CreateExcelFileAsPath(worksheet =>
        {
            // Empty worksheet - no headers, no data
        });

        try
        {
            // Act & Assert
            var result = _reader.ReadAsync(filePath, CancellationToken.None);
            
            await Assert.ThrowsAnyAsync<Exception>(async () =>
            {
                await foreach (var record in result)
                {
                    // Should throw when trying to process empty worksheet
                }
            });
        }
        finally
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }

    [Fact]
    public async Task GivenExcelWithDuplicateHeaders_WhenRead_ThenHandlesDuplicates()
    {
        // Arrange
        var filePath = CreateExcelFileAsPath(worksheet =>
        {
            worksheet.Cells[1, 1].Value = "Name";
            worksheet.Cells[1, 2].Value = "Name"; // Duplicate header
            worksheet.Cells[1, 3].Value = "Age";
            
            worksheet.Cells[2, 1].Value = "John";
            worksheet.Cells[2, 2].Value = "Doe";
            worksheet.Cells[2, 3].Value = 30;
        });

        try
        {
            // Act & Assert
            var result = _reader.ReadAsync(filePath, CancellationToken.None);
            
            // This should either handle duplicates gracefully or throw an exception
            // The exact behavior depends on the implementation
            await Assert.ThrowsAnyAsync<Exception>(async () =>
            {
                await foreach (var record in result)
                {
                    // May throw due to duplicate keys in dictionary
                }
            });
        }
        finally
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }

    private static MemoryStream CreateExcelFile(Action<ExcelWorksheet> populateWorksheet)
    {
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Sheet1");
        populateWorksheet(worksheet);
        
        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        return stream;
    }

    private static string CreateExcelFileAsPath(Action<ExcelWorksheet> populateWorksheet)
    {
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Sheet1");
        populateWorksheet(worksheet);
        
        var tempPath = Path.GetTempFileName();
        var excelPath = Path.ChangeExtension(tempPath, ".xlsx");
        File.Delete(tempPath);
        
        package.SaveAs(new FileInfo(excelPath));
        return excelPath;
    }

    private async Task<T> ExecuteWithTempFile<T>(Action<ExcelWorksheet> populateWorksheet, Func<string, Task<T>> testAction)
    {
        var filePath = CreateExcelFileAsPath(populateWorksheet);
        try
        {
            return await testAction(filePath);
        }
        finally
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }

}