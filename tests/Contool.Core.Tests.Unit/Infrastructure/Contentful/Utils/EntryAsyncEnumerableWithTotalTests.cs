using Contentful.Core.Models;
using Contool.Core.Infrastructure.Contentful.Utils;

namespace Contool.Core.Tests.Unit.Infrastructure.Contentful.Utils;

public class EntryAsyncEnumerableWithTotalTests
{
    [Fact]
    public async Task GivenValidEntries_WhenEnumerated_ThenAllEntriesAreReturned()
    {
        // Arrange
        var entries = new List<Entry<string>>
        {
            new() { Fields = "Entry1" },
            new() { Fields = "Entry2" }
        };

        var page1 = new ContentfulCollection<Entry<string>>
        {
            Items = entries,
            Total = entries.Count
        };

        var asyncEnumerable = new EntryAsyncEnumerableWithTotal<string>(
            getEntriesAsync: new MockGetEntriesAsync(page1).Execute,
            query: new EntryQueryBuilder().Limit(2),
            pagingMode: PagingMode.SkipForward);

        // Act
        var actual = await asyncEnumerable.ToListAsync();

        // Assert
        Assert.Equivalent(entries, actual);
        Assert.Equal(entries.Count, asyncEnumerable.Total);
    }

    [Fact]
    public async Task GivenEmptyEntries_WhenEnumerated_ThenNoEntriesAreReturned()
    {
        // Arrange
        var page1 = new ContentfulCollection<Entry<string>>
        {
            Items = [],
            Total = 0
        };

        var asyncEnumerable = new EntryAsyncEnumerableWithTotal<string>(
            getEntriesAsync: new MockGetEntriesAsync(page1).Execute,
            query: new EntryQueryBuilder().Limit(2),
            pagingMode: PagingMode.SkipForward);

        // Act
        var actual = await asyncEnumerable.ToListAsync();

        // Assert
        Assert.Empty(actual);
        Assert.Equal(0, asyncEnumerable.Total);
    }

    [Fact]
    public async Task GivenMultiplePages_WhenEnumerated_ThenAllEntriesAreReturnedAcrossPages()
    {
        // Arrange
        var page1 = new ContentfulCollection<Entry<string>>
        {
            Items =
            [
                new Entry<string> { Fields = "Entry1" },
                new Entry<string> { Fields = "Entry2" }
            ],
            Total = 4
        };

        var page2 = new ContentfulCollection<Entry<string>>
        {
            Items =
            [
                new Entry<string> { Fields = "Entry3" },
                new Entry<string> { Fields = "Entry4" }
            ],
            Total = 4
        };

        var asyncEnumerable = new EntryAsyncEnumerableWithTotal<string>(
            getEntriesAsync: new MockGetEntriesAsync(page1, page2).Execute,
            query: new EntryQueryBuilder().Limit(2),
            pagingMode: PagingMode.SkipForward);

        // Act
        var actual = await asyncEnumerable.ToListAsync();

        // Assert
        var expected = new List<Entry<string>>
        {
            new() { Fields = "Entry1" },
            new() { Fields = "Entry2" },
            new() { Fields = "Entry3" },
            new() { Fields = "Entry4" }
        };

        Assert.Equivalent(expected, actual);
        Assert.Equal(4, asyncEnumerable.Total);
    }

    private class MockGetEntriesAsync(params ContentfulCollection<Entry<string>>[] pages)
    {
        private readonly Queue<ContentfulCollection<Entry<string>>> _pages = new(pages);

        public Task<ContentfulCollection<Entry<string>>> Execute(string query, CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrEmpty(query);
            return Task.FromResult(_pages.Count > 0 ? _pages.Dequeue() : null!);
        }
    }
}