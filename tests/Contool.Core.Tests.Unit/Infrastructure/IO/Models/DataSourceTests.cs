using Contool.Core.Infrastructure.IO.Models;

namespace Contool.Core.Tests.Unit.Infrastructure.IO.Models;

public class DataSourceTests
{
    [Fact]
    public void GivenValidName_WhenFromIsCalled_ThenCorrectDataSourceIsReturned()
    {
        // Arrange
        var validName = "CSV";

        // Act
        var actual = DataSource.From(validName);

        // Assert
        Assert.Equal(DataSource.Csv, actual);
    }

    [Fact]
    public void GivenValidExtension_WhenFromIsCalled_ThenCorrectDataSourceIsReturned()
    {
        // Arrange
        var validExtension = ".json";

        // Act
        var actual = DataSource.From(validExtension);

        // Assert
        Assert.Equal(DataSource.Json, actual);
    }

    [Fact]
    public void GivenInvalidValue_WhenFromIsCalled_ThenArgumentExceptionIsThrown()
    {
        // Arrange
        var invalidValue = "Unsupported";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => DataSource.From(invalidValue));
        Assert.Equal("Unsupported data source: Unsupported", exception.Message);
    }

    [Fact]
    public void GivenDataSource_WhenToStringIsCalled_ThenNameAndExtensionIsReturned()
    {
        // Arrange
        var dataSource = DataSource.Excel;

        // Act
        var actual = dataSource.ToString();

        // Assert
        Assert.Equal("EXCEL (.xlsx)", actual);
    }
}