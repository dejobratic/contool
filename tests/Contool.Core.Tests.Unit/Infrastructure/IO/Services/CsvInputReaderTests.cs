using Contool.Core.Infrastructure.IO.Models;
using Contool.Core.Infrastructure.IO.Services;
using System.Text;

namespace Contool.Core.Tests.Unit.Infrastructure.IO.Services;

public class CsvInputReaderTests : IDisposable
{
    private readonly CsvInputReader _reader;
    private readonly string _tempDirectory;

    public CsvInputReaderTests()
    {
        _reader = new CsvInputReader();
        _tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempDirectory);
    }

    [Fact]
    public void GivenCsvInputReader_WhenInstantiated_ThenDataSourceIsCorrect()
    {
        // Act
        var dataSource = _reader.DataSource;

        // Assert
        Assert.Equal(DataSource.Csv, dataSource);
    }

    [Fact]
    public async Task GivenValidCsvFile_WhenRead_ThenReturnsCorrectData()
    {
        // Arrange
        var csvContent = "Name,Age,Email\nJohn Doe,30,john@example.com\nJane Smith,25,jane@example.com\nBob Johnson,35,bob@example.com";
        var filePath = CreateTempCsvFile("valid.csv", csvContent);

        // Act
        var result = _reader.ReadAsync(filePath, CancellationToken.None);

        // Assert
        Assert.Equal(3, result.Total);
        
        var records = await result.ToListAsync();
        Assert.Equal(3, records.Count);

        // Verify first record
        var firstRecord = records[0] as IDictionary<string, object>;
        Assert.NotNull(firstRecord);
        Assert.Equal("John Doe", firstRecord["Name"]);
        Assert.Equal("30", firstRecord["Age"]);
        Assert.Equal("john@example.com", firstRecord["Email"]);
    }

    [Fact]
    public async Task GivenEmptyCsvFile_WhenRead_ThenReturnsEmptyResult()
    {
        // Arrange
        var csvContent = "Name,Age,Email\n";
        var filePath = CreateTempCsvFile("empty.csv", csvContent);

        // Act
        var result = _reader.ReadAsync(filePath, CancellationToken.None);

        // Assert
        Assert.Equal(0, result.Total);
        
        var records = await result.ToListAsync();
        Assert.Empty(records);
    }

    [Fact]
    public async Task GivenCsvWithSpecialCharacters_WhenRead_ThenHandlesCorrectly()
    {
        // Arrange
        var csvContent = "Name,Description,Price\n\"Product \"\"A\"\"\",\"Contains, commas and \"\"quotes\"\"\",29.99\nProduct B,Simple description,15.50";
        var filePath = CreateTempCsvFile("special.csv", csvContent);

        // Act
        var result = _reader.ReadAsync(filePath, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Total);
        
        var records = await result.ToListAsync();
        Assert.Equal(2, records.Count);

        var firstRecord = records[0] as IDictionary<string, object>;
        Assert.NotNull(firstRecord);
        Assert.Equal("Product \"A\"", firstRecord["Name"]);
        Assert.Equal("Contains, commas and \"quotes\"", firstRecord["Description"]);
    }

    [Fact]
    public async Task GivenCsvWithMissingFields_WhenRead_ThenHandlesMissingValues()
    {
        // Arrange
        var csvContent = "Name,Age,Email\nJohn Doe,30,john@example.com\nJane Smith,,jane@example.com\nBob Johnson,35,";
        var filePath = CreateTempCsvFile("missing.csv", csvContent);

        // Act
        var result = _reader.ReadAsync(filePath, CancellationToken.None);

        // Assert
        var records = await result.ToListAsync();
        Assert.Equal(3, records.Count);

        var secondRecord = records[1] as IDictionary<string, object>;
        Assert.NotNull(secondRecord);
        Assert.Equal("Jane Smith", secondRecord["Name"]);
        Assert.Equal("", secondRecord["Age"]); // Empty string for missing value
        Assert.Equal("jane@example.com", secondRecord["Email"]);
    }

    [Fact]
    public async Task GivenLargeCsvFile_WhenRead_ThenStreamsEfficiently()
    {
        // Arrange
        var csvBuilder = new StringBuilder();
        csvBuilder.AppendLine("Id,Name,Value");
        
        const int recordCount = 100; // Reduced for faster testing
        for (int i = 1; i <= recordCount; i++)
        {
            csvBuilder.AppendLine($"{i},Record {i},{i * 10}");
        }
        
        var filePath = CreateTempCsvFile("large.csv", csvBuilder.ToString());

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
            Assert.Equal(processedCount.ToString(), recordDict["Id"]);
        }
        
        Assert.Equal(recordCount, processedCount);
    }

    [Fact]
    public async Task GivenCancellationToken_WhenCancelled_ThenThrowsOperationCancelledException()
    {
        // Arrange
        const string csvContent = "Name,Age,Email\nJohn Doe,30,john@example.com\nJane Smith,25,jane@example.com";
        var filePath = CreateTempCsvFile("cancel.csv", csvContent);
        
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        // Act & Assert
        var result = _reader.ReadAsync(filePath, cts.Token);
        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
        {
            await foreach (var _ in result)
            {
                // This should throw immediately due to cancellation
            }
        });
    }

    [Fact]
    public async Task GivenCsvWithUtf8Characters_WhenRead_ThenPreservesEncoding()
    {
        // Arrange
        var csvContent = "Name,Location,Description\nAndré,Montréal,Café au lait\nJosé,México,Niño pequeño";
        var filePath = CreateTempCsvFile("utf8.csv", csvContent);

        // Act
        var result = _reader.ReadAsync(filePath, CancellationToken.None);

        // Assert
        var records = await result.ToListAsync();
        Assert.Equal(2, records.Count);

        var firstRecord = records[0] as IDictionary<string, object>;
        Assert.NotNull(firstRecord);
        Assert.Equal("André", firstRecord["Name"]);
        Assert.Equal("Montréal", firstRecord["Location"]);
        Assert.Equal("Café au lait", firstRecord["Description"]);
    }

    [Fact]
    public async Task GivenNonExistentFile_WhenRead_ThenThrowsException()
    {
        // Arrange
        var nonExistentPath = Path.Combine(_tempDirectory, "nonexistent.csv");

        // Act & Assert
        await Assert.ThrowsAnyAsync<Exception>(async () =>
        {
            var result = _reader.ReadAsync(nonExistentPath, CancellationToken.None);
            
            await foreach (var _ in result)
            {
                // Should throw when trying to read non-existent file
            }
        });
    }

    private string CreateTempCsvFile(string fileName, string content)
    {
        var filePath = Path.Combine(_tempDirectory, fileName);
        File.WriteAllText(filePath, content, Encoding.UTF8);
        return filePath;
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDirectory))
        {
            Directory.Delete(_tempDirectory, true);
        }
    }
}