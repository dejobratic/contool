using Contool.Core.Infrastructure.IO.Models;
using Contool.Core.Infrastructure.IO.Services;

namespace Contool.Core.Tests.Unit.Infrastructure.IO.Services;

public class InputReaderFactoryTests
{
    private readonly InputReaderFactory _factory = new(
    [
        new CsvInputReader(),
        new ExcelInputReader(),
        new JsonInputReader(),
    ]);

    [Fact]
    public void GivenCsvDataSource_WhenCreate_ThenReturnsCsvInputReader()
    {
        // Act
        var reader = _factory.Create(DataSource.Csv);

        // Assert
        Assert.NotNull(reader);
        Assert.IsType<CsvInputReader>(reader);
        Assert.Equal(DataSource.Csv, reader.DataSource);
    }

    [Fact]
    public void GivenExcelDataSource_WhenCreate_ThenReturnsExcelInputReader()
    {
        // Act
        var reader = _factory.Create(DataSource.Excel);

        // Assert
        Assert.NotNull(reader);
        Assert.IsType<ExcelInputReader>(reader);
        Assert.Equal(DataSource.Excel, reader.DataSource);
    }

    [Fact]
    public void GivenJsonDataSource_WhenCreate_ThenReturnsJsonInputReader()
    {
        // Act
        var reader = _factory.Create(DataSource.Json);

        // Assert
        Assert.NotNull(reader);
        Assert.IsType<JsonInputReader>(reader);
        Assert.Equal(DataSource.Json, reader.DataSource);
    }

    [Fact]
    public void GivenUnsupportedDataSource_WhenCreate_ThenThrowsException()
    {
        // Arrange
        var unsupportedDataSource = new FileDataSource("UNSUPPORTED", ".xyz");

        // Act & Assert
        Assert.Throws<NotImplementedException>(() => _factory.Create(unsupportedDataSource));
    }

    [Fact]
    public void GivenFactoryImplementsInterface_WhenChecked_ThenImplementsIInputReaderFactory()
    {
        // Arrange & Act & Assert & Assert
        Assert.IsAssignableFrom<IInputReaderFactory>(_factory);
    }

    [Fact]
    public void GivenMultipleCallsForSameDataSource_WhenCreate_ThenReturnsSameInstance()
    {
        // Act
        var reader1 = _factory.Create(DataSource.Csv);
        var reader2 = _factory.Create(DataSource.Csv);

        // Assert
        Assert.NotNull(reader1);
        Assert.NotNull(reader2);
        Assert.Same(reader1, reader2); // Same instance due to dictionary lookup
        Assert.IsType<CsvInputReader>(reader1);
        Assert.IsType<CsvInputReader>(reader2);
    }

    [Fact]
    public void GivenAllSupportedDataSources_WhenCreate_ThenReturnsCorrectTypes()
    {
        // Arrange
        var supportedDataSources = new[]
        {
            DataSource.Csv,
            DataSource.Excel,
            DataSource.Json
        };

        var expectedTypes = new[]
        {
            typeof(CsvInputReader),
            typeof(ExcelInputReader),
            typeof(JsonInputReader)
        };

        // Act & Assert
        for (var i = 0; i < supportedDataSources.Length; i++)
        {
            var reader = _factory.Create(supportedDataSources[i]);
            Assert.NotNull(reader);
            Assert.IsType(expectedTypes[i], reader);
            Assert.Equal(supportedDataSources[i], reader.DataSource);
        }
    }

    [Fact]
    public void GivenEmptyReadersList_WhenCreateFactory_ThenCanCreateButThrowsOnUse()
    {
        // Arrange
        var factoryWithEmptyReaders = new InputReaderFactory([]);

        // Act & Assert
        Assert.Throws<NotImplementedException>(() => 
            factoryWithEmptyReaders.Create(DataSource.Csv));
    }

    [Fact]
    public void GivenNullReadersList_WhenCreateFactory_ThenThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new InputReaderFactory(null!));
    }

    [Fact]
    public void GivenPartialReadersList_WhenCreate_ThenWorksForSupportedTypes()
    {
        // Arrange
        var partialReaders = new List<IInputReader> { new CsvInputReader() };
        var partialFactory = new InputReaderFactory(partialReaders);

        // Act & Assert
        var csvReader = partialFactory.Create(DataSource.Csv);
        Assert.NotNull(csvReader);
        Assert.IsType<CsvInputReader>(csvReader);
        
        Assert.Throws<NotImplementedException>(() => partialFactory.Create(DataSource.Excel));
    }

    [Fact]
    public void GivenReadersList_WhenCreateFactory_ThenBuildsDictionaryCorrectly()
    {
        // Arrange
        var testReaders = new List<IInputReader>
        {
            new CsvInputReader(),
            new ExcelInputReader()
        };

        // Act
        var testFactory = new InputReaderFactory(testReaders);

        // Assert
        var csvReader = testFactory.Create(DataSource.Csv);
        var excelReader = testFactory.Create(DataSource.Excel);
        
        Assert.IsType<CsvInputReader>(csvReader);
        Assert.IsType<ExcelInputReader>(excelReader);
    }
}