using Contool.Core.Infrastructure.IO.Models;

namespace Contool.Core.Tests.Unit.Infrastructure.IO.Models;

public class DataSourceTests
{
    [Fact]
    public void GivenDataSource_WhenAccessingStaticInstances_ThenReturnsCorrectValues()
    {
        // Act & Assert
        Assert.Equal("CSV", DataSource.Csv.Name);
        Assert.Equal("EXCEL", DataSource.Excel.Name);
        Assert.Equal("JSON", DataSource.Json.Name);
    }

    [Fact]
    public void GivenDataSource_WhenComparingInstances_ThenEqualityWorksCorrectly()
    {
        // Arrange
        var csv1 = DataSource.Csv;
        var csv2 = DataSource.Csv;
        var excel = DataSource.Excel;

        // Act & Assert
        Assert.Equal(csv1, csv2);
        Assert.NotEqual(csv1, excel);
        Assert.True(csv1 == csv2);
        Assert.True(csv1 != excel);
    }

    [Fact]
    public void GivenDataSource_WhenCallingToString_ThenReturnsName()
    {
        // Act & Assert
        Assert.Equal("CSV (.csv)", DataSource.Csv.ToString());
        Assert.Equal("EXCEL (.xlsx)", DataSource.Excel.ToString());
        Assert.Equal("JSON (.json)", DataSource.Json.ToString());
    }

    [Fact]
    public void GivenDataSource_WhenGettingHashCode_ThenConsistentForSameInstance()
    {
        // Arrange
        var csv1 = DataSource.Csv;
        var csv2 = DataSource.Csv;

        // Act & Assert
        Assert.Equal(csv1.GetHashCode(), csv2.GetHashCode());
    }

    [Fact]
    public void GivenDataSource_WhenCheckingType_ThenIsCorrectType()
    {
        // Act & Assert
        Assert.IsType<DataSource>(DataSource.Csv, exactMatch: false);
        Assert.IsType<DataSource>(DataSource.Excel, exactMatch: false);
        Assert.IsType<DataSource>(DataSource.Json, exactMatch: false);
    }

    [Theory]
    [InlineData("CSV")]
    [InlineData("EXCEL")]
    [InlineData("JSON")]
    public void GivenDataSourceName_WhenValidating_ThenNamesAreCorrect(string expectedName)
    {
        // Arrange
        var dataSource = expectedName switch
        {
            "CSV" => DataSource.Csv,
            "EXCEL" => DataSource.Excel,
            "JSON" => DataSource.Json,
            _ => throw new ArgumentException("Invalid data source name")
        };

        // Act & Assert
        Assert.Equal(expectedName, dataSource.Name);
    }

    [Fact]
    public void GivenAllDataSources_WhenCollecting_ThenHasExpectedCount()
    {
        // Arrange
        var allDataSources = new[] { DataSource.Csv, DataSource.Excel, DataSource.Json };

        // Act & Assert
        Assert.Equal(3, allDataSources.Length);
        Assert.All(allDataSources, ds => Assert.NotNull(ds));
        Assert.All(allDataSources, ds => Assert.NotNull(ds.Name));
    }
}