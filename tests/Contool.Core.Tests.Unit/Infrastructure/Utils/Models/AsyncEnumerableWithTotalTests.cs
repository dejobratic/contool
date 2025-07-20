using Contool.Core.Infrastructure.Utils.Models;
using Contool.Core.Infrastructure.Utils.Services;
using Contool.Core.Tests.Unit.Helpers;
using MockLite;

namespace Contool.Core.Tests.Unit.Infrastructure.Utils.Models;

public class AsyncEnumerableWithTotalTests
{
    private static readonly int[] SourceArray = [1, 2, 3];

    [Fact]
    public async Task GivenSourceWithItems_WhenEnumerated_ThenItemsAreReturnedCorrectly()
    {
        // Arrange
        var asyncEnumerableWithTotal = new AsyncEnumerableWithTotal<int>(
            source: SourceArray.ToAsyncEnumerable(),
            getTotal: () => 3);

        // Act
        var actual = await asyncEnumerableWithTotal.ToListAsync();

        // Assert
        Assert.Equal([1, 2, 3], actual);
    }

    [Fact]
    public async Task GivenSourceWithItems_WhenFirstItemIsEnumerated_ThenTotalIsSetCorrectly()
    {
        // Arrange
        var asyncEnumerableWithTotal = new AsyncEnumerableWithTotal<int>(
            source: SourceArray.ToAsyncEnumerable(),
            getTotal: () => 3);

        // Act
        await foreach (var _ in asyncEnumerableWithTotal)
            break;

        // Assert
        Assert.Equal(3, asyncEnumerableWithTotal.Total);
    }

    [Fact]
    public async Task GivenProgressReporter_WhenItemsAreEnumerated_ThenProgressIsUpdated()
    {
        // Arrange
        var progressReporterMock = new Mock<IProgressReporter>();
        progressReporterMock.SetupDefaults();

        var asyncEnumerableWithTotal = new AsyncEnumerableWithTotal<int>(
            source: SourceArray.ToAsyncEnumerable(),
            getTotal: () => 3,
            progressReporterMock.Object);

        // Act
        _ = await asyncEnumerableWithTotal.ToListAsync();

        // Assert
        progressReporterMock.Verify(x => x.Increment(), Times.Exactly(3));
        progressReporterMock.Verify(x => x.Complete(), Times.Once);
    }
}