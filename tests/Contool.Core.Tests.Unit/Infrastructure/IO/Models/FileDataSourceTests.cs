using Contool.Core.Infrastructure.IO.Models;

namespace Contool.Core.Tests.Unit.Infrastructure.IO.Models;

public class FileDataSourceTests
{
    [Theory]
    [InlineData(".csv", "CSV")]
    [InlineData(".xlsx", "EXCEL")]
    [InlineData(".json", "JSON")]
    [InlineData(".CSV", "CSV")]
    [InlineData(".XLSX", "EXCEL")]
    [InlineData(".JSON", "JSON")]
    public void GivenValidExtension_WhenCreatingFromExtension_ThenReturnsCorrectDataSource(
        string extension, string expectedDataSourceName)
    {
        // Act
        var dataSource = DataSource.From(extension);

        // Assert
        Assert.Equal(expectedDataSourceName, dataSource.Name);
        Assert.IsType<FileDataSource>(dataSource);
        
        var fileDataSource = (FileDataSource)dataSource;
        Assert.Equal(extension.ToLowerInvariant(), fileDataSource.Extension.ToLowerInvariant());
    }

    [Theory]
    [InlineData("data.csv", "CSV")]
    [InlineData("data.xlsx", "EXCEL")]
    [InlineData("data.json", "JSON")]
    [InlineData("DATA.CSV", "CSV")]
    [InlineData("DATA.XLSX", "EXCEL")]
    [InlineData("DATA.JSON", "JSON")]
    public void GivenFileWithValidExtension_WhenCreatingFromFileName_ThenReturnsCorrectDataSource(
        string fileName, string expectedDataSourceName)
    {
        // Arrange
        var extension = Path.GetExtension(fileName);

        // Act
        var dataSource = DataSource.From(extension);

        // Assert
        Assert.Equal(expectedDataSourceName, dataSource.Name);
    }

    [Theory]
    [InlineData(".txt")]
    [InlineData(".pdf")]
    [InlineData(".doc")]
    [InlineData("")]
    public void GivenInvalidExtension_WhenCreatingFromExtension_ThenThrowsException(string extension)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => DataSource.From(extension));
    }

    [Fact]
    public void GivenNullExtension_WhenCreatingFromExtension_ThenThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => DataSource.From(null!));
    }

    [Theory]
    [InlineData("folder/data.csv", "CSV")]
    [InlineData("C:\\folder\\data.xlsx", "EXCEL")]
    [InlineData("/home/user/data.json", "JSON")]
    [InlineData("../data.csv", "CSV")]
    public void GivenFileWithPath_WhenCreatingFromFilePath_ThenExtractsExtensionCorrectly(
        string filePath, string expectedDataSourceName)
    {
        // Arrange
        var extension = Path.GetExtension(filePath);

        // Act
        var dataSource = DataSource.From(extension);

        // Assert
        Assert.Equal(expectedDataSourceName, dataSource.Name);
    }

    [Fact]
    public void GivenFileDataSource_WhenCheckingInheritance_ThenInheritsFromDataSource()
    {
        // Arrange
        var fileDataSource = new FileDataSource("CSV", ".csv");

        // Act & Assert
        Assert.IsAssignableFrom<DataSource>(fileDataSource);
    }

    [Fact]
    public void GivenFileDataSource_WhenCheckingProperties_ThenHasCorrectProperties()
    {
        // Arrange
        var fileDataSource = new FileDataSource("CSV", ".csv");

        // Act & Assert
        Assert.Equal("CSV", fileDataSource.Name);
        Assert.Equal(".csv", fileDataSource.Extension);
    }

    [Fact]
    public void GivenFileDataSource_WhenCallingToString_ThenReturnsCorrectFormat()
    {
        // Arrange
        var fileDataSource = new FileDataSource("CSV", ".csv");

        // Act
        var result = fileDataSource.ToString();

        // Assert
        Assert.Equal("CSV (.csv)", result);
    }

    [Fact]
    public void GivenStaticDataSources_WhenChecking_ThenAllAreFileDataSources()
    {
        // Act & Assert
        Assert.IsType<FileDataSource>(DataSource.Csv);
        Assert.IsType<FileDataSource>(DataSource.Excel);
        Assert.IsType<FileDataSource>(DataSource.Json);
    }

    [Fact]
    public void GivenStaticDataSources_WhenCheckingExtensions_ThenHaveCorrectExtensions()
    {
        // Act & Assert
        var csvFile = (FileDataSource)DataSource.Csv;
        var excelFile = (FileDataSource)DataSource.Excel;
        var jsonFile = (FileDataSource)DataSource.Json;

        Assert.Equal(".csv", csvFile.Extension);
        Assert.Equal(".xlsx", excelFile.Extension);
        Assert.Equal(".json", jsonFile.Extension);
    }

    [Theory]
    [InlineData("CSV")]
    [InlineData("EXCEL")]
    [InlineData("JSON")]
    public void GivenDataSourceName_WhenCreatingFromName_ThenReturnsCorrectDataSource(string name)
    {
        // Act
        var dataSource = DataSource.From(name);

        // Assert
        Assert.Equal(name, dataSource.Name);
        Assert.IsType<FileDataSource>(dataSource);
    }
}