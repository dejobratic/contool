using Contool.Core.Infrastructure.Utils.Models;
using Contool.Core.Tests.Unit.Helpers;
using Contool.Core.Tests.Unit.Mocks;

namespace Contool.Core.Tests.Unit.Infrastructure.Utils.Models;

public class AsyncEnumerableWithTotalTests
{
    [Fact]
    public async Task GivenSourceWithItems_WhenEnumerated_ThenItemsAreReturnedCorrectly()
    {
        // Arrange
        var asyncEnumerableWithTotal = new AsyncEnumerableWithTotal<int>(
            source: AsyncEnumerableFactory.From([1, 2, 3]),
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
            source: AsyncEnumerableFactory.From([1, 2, 3]),
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
        var progressReporter = new MockProgressReporter();

        var asyncEnumerableWithTotal = new AsyncEnumerableWithTotal<int>(
            source: AsyncEnumerableFactory.From([1, 2, 3]),
            getTotal: () => 3,
            progressReporter);

        // Act
        _ = await asyncEnumerableWithTotal.ToListAsync();

        // Assert
        Assert.Equal(3, progressReporter.IncrementCount);
        Assert.True(progressReporter.IsComplete);
    }
}