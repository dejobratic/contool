using Contool.Core.Infrastructure.IO.Models;
using Contool.Core.Infrastructure.Utils.Models;

namespace Contool.Core.Tests.Unit.Infrastructure.IO.Models;

public class OutputContentTests
{
    [Fact]
    public void GivenValidInputs_WhenConstructorIsCalled_ThenPropertiesAreSetCorrectly()
    {
        // Arrange
        var path = Path.Combine("C:", "Test");
        const string name = "Output";
        const string type = "CSV";
        var content = new MockAsyncEnumerableWithTotal<dynamic>();

        // Act
        var actual = new OutputContent(path, name, type, content);

        // Assert
        Assert.Equal(Path.Combine("C:", "Test", "Output.csv"), actual.FullPath);
        Assert.Equal(DataSource.Csv, actual.DataSource);
        Assert.Equal(content, actual.Content);
    }

    [Fact]
    public void GivenInvalidType_WhenConstructorIsCalled_ThenArgumentExceptionIsThrown()
    {
        // Arrange
        const string path = "C:\\Test";
        const string name = "Output";
        const string type = "Unsupported";
        var content = new MockAsyncEnumerableWithTotal<dynamic>();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new OutputContent(path, name, type, content));
        Assert.Equal("Unsupported data source: Unsupported", exception.Message);
    }

    private class MockAsyncEnumerableWithTotal<T> : IAsyncEnumerableWithTotal<T>
    {
        public int Total => 0;

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            => AsyncEnumerable.Empty<T>().GetAsyncEnumerator(cancellationToken);
    }
}