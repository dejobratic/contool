using Contool.Core.Infrastructure.IO.Models;
using Contool.Core.Infrastructure.IO.Services;
using System.Text;

namespace Contool.Core.Tests.Unit.Infrastructure.IO.Services;

public class JsonInputReaderTests
{
    private readonly JsonInputReader _sut = new();

    [Fact]
    public void GivenJsonInputReader_WhenInstantiated_ThenDataSourceIsCorrect()
    {
        // Act
        var dataSource = _sut.DataSource;

        // Assert
        Assert.Equal(DataSource.Json, dataSource);
    }

    [Fact]
    public async Task GivenJsonInputReader_WhenReadAsync_ThenReturnsCorrectData()
    {
        // Arrange
        var jsonContent = "[\n  {\"name\": \"John\", \"age\": 30},\n  {\"name\": \"Jane\", \"age\": 25}\n]";
        
        var tempPath = Path.GetTempFileName();
        var jsonPath = Path.ChangeExtension(tempPath, ".json");
        File.Delete(tempPath);
        
        await File.WriteAllTextAsync(jsonPath, jsonContent);

        try
        {
            // Act
            var result = _sut.ReadAsync(jsonPath, CancellationToken.None);
            
            // Assert
            Assert.Equal(2, result.Total);
            
            var records = await result.ToListAsync();
            Assert.Equal(2, records.Count);
            
            var firstRecord = records[0] as IDictionary<string, object>;
            Assert.NotNull(firstRecord);
            Assert.Equal("John", firstRecord["name"]);
            Assert.Equal(30.0, firstRecord["age"]);
            
            var secondRecord = records[1] as IDictionary<string, object>;
            Assert.NotNull(secondRecord);
            Assert.Equal("Jane", secondRecord["name"]);
            Assert.Equal(25.0, secondRecord["age"]);
        }
        finally
        {
            if (File.Exists(jsonPath))
                File.Delete(jsonPath);
        }
    }

    [Fact]
    public void GivenJsonInputReader_WhenCheckingInterface_ThenImplementsIInputReader()
    {
        // Arrange & Act
        var implementsInterface = _sut is IInputReader;

        // Assert
        Assert.True(implementsInterface);
    }

    // Note: Additional tests would be added here once JsonInputReader is fully implemented
    // These might include:
    // - Testing array of objects format
    // - Testing nested JSON objects
    // - Testing JSON with different data types
    // - Testing large JSON files
    // - Testing malformed JSON
    // - Testing cancellation token support
    // - Testing UTF-8 encoding support
}