using Contool.Core.Infrastructure.Utils;
using Contool.Core.Tests.Unit.Helpers;

namespace Contool.Core.Tests.Unit.Infrastructure.Utils;

public class BatchProcessorTests
{
    private readonly BatchProcessor _sut = new();

    [Fact]
    public async Task GivenSourceWithMultipleItems_WhenProcessed_ThenBatchesAreCreatedCorrectly()
    {
        // Arrange
        var processedBatches = new List<IReadOnlyList<int>>();

        // Act
        await _sut.ProcessAsync(
            source: AsyncEnumerableFactory.From([1, 2, 3, 4, 5]),
            batchSize: 2,
            batchActionAsync: (batch, _) =>
            {
                processedBatches.Add([.. batch]);
                return Task.CompletedTask;
            });

        // Assert
        Assert.Equal(3, processedBatches.Count);
        Assert.Equal([1, 2], processedBatches[0]);
        Assert.Equal([3, 4], processedBatches[1]);
        Assert.Equal([5], processedBatches[2]);
    }

    [Fact]
    public async Task GivenBatchItemFilter_WhenProcessed_ThenOnlyFilteredItemsAreIncludedInBatches()
    {
        // Arrange
        var processedBatches = new List<IReadOnlyList<int>>();

        // Act
        await _sut.ProcessAsync(
            source: AsyncEnumerableFactory.From([1, 2, 3, 4, 5]),
            batchSize: 2,
            batchActionAsync: (batch, _) =>
            {
                processedBatches.Add([.. batch]);
                return Task.CompletedTask;
            },
            batchItemFilter: item => item % 2 == 0); // Filter even numbers

        // Assert
        Assert.Single(processedBatches);
        Assert.Equal([2, 4], processedBatches[0]);
    }

    [Fact]
    public async Task GivenEmptySource_WhenProcessed_ThenNoBatchesAreCreated()
    {
        // Arrange
        var processedBatches = new List<IReadOnlyList<int>>();

        // Act
        await _sut.ProcessAsync(
            source: AsyncEnumerableFactory.From(Array.Empty<int>()),
            batchSize: 2,
            batchActionAsync: (batch, _) =>
            {
                processedBatches.Add([.. batch]);
                return Task.CompletedTask;
            });

        // Assert
        Assert.Empty(processedBatches);
    }
}