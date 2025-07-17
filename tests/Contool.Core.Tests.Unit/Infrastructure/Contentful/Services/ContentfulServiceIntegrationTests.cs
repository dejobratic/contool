using Contentful.Core.Models;
using Contool.Core.Infrastructure.Contentful.Extensions;
using Contool.Core.Infrastructure.Contentful.Services;
using Contool.Core.Infrastructure.Contentful.Utils;
using Contool.Core.Tests.Unit.Helpers;
using MockLite;

namespace Contool.Core.Tests.Unit.Infrastructure.Contentful.Services;

public class ContentfulServiceIntegrationTests
{
    private readonly ContentfulService _sut;

    private readonly Mock<IContentfulManagementClientAdapter> _clientMock;
    private readonly Mock<IContentfulEntryOperationService> _entryOperationServiceMock;

    public ContentfulServiceIntegrationTests()
    {
        _clientMock = new Mock<IContentfulManagementClientAdapter>();
        _entryOperationServiceMock = new Mock<IContentfulEntryOperationService>();
        _sut = new ContentfulService(_clientMock.Object, _entryOperationServiceMock.Object);
    }

    [Fact]
    public async Task GivenValidContentTypeId_WhenGetContentTypeAsync_ThenReturnsContentType()
    {
        // Arrange
        var contentType = ContentTypeBuilder.CreateBlogPost();
        _clientMock.Setup(x => x.GetContentTypeAsync(contentType.SystemProperties.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(contentType);

        // Act
        var result = await _sut.GetContentTypeAsync("blogPost", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("blogPost", result.SystemProperties.Id);
        Assert.Equal("Blog Post", result.Name);
        _clientMock.Verify(x => x.GetContentTypeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GivenInvalidContentTypeId_WhenGetContentTypeAsync_ThenReturnsNull()
    {
        // Arrange
        _clientMock.Setup(x => x.GetContentTypeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ContentType?)null);

        // Act
        var result = await _sut.GetContentTypeAsync("nonexistent", CancellationToken.None);

        // Assert
        Assert.Null(result);
        _clientMock.Verify(x => x.GetContentTypeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GivenService_WhenGetLocalesAsync_ThenReturnsLocales()
    {
        // Arrange
        var locales = LocaleBuilder.CreateMultiple();
        _clientMock.Setup(x => x.GetLocalesCollectionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(locales);

        // Act
        var result = await _sut.GetLocalesAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
        Assert.Contains(result, l => l.Code == "en-US");
        Assert.Contains(result, l => l.Code == "fr-FR");
        Assert.Contains(result, l => l.Code == "es-ES");
        _clientMock.Verify(x => x.GetLocalesCollectionAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GivenService_WhenGetContentTypesAsync_ThenReturnsAllContentTypes()
    {
        // Arrange
        var contentTypes = new[]
        {
            ContentTypeBuilder.CreateBlogPost(),
            ContentTypeBuilder.CreateProduct(),
            ContentTypeBuilder.CreateMinimal()
        };
        _clientMock.Setup(x => x.GetContentTypesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(contentTypes);

        // Act
        var result = await _sut.GetContentTypesAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
        Assert.Contains(result, ct => ct.SystemProperties.Id == "blogPost");
        Assert.Contains(result, ct => ct.SystemProperties.Id == "product");
        Assert.Contains(result, ct => ct.SystemProperties.Id == "minimal");
        _clientMock.Verify(x => x.GetContentTypesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GivenNewContentType_WhenCreateContentTypeAsync_ThenCreatesAndReturnsContentType()
    {
        // Arrange
        var contentType = ContentTypeBuilder.CreateBlogPost();
        _clientMock.Setup(x => x.GetContentTypeAsync(contentType.SystemProperties.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ContentType?)null);
        _clientMock.Setup(x => x.CreateOrUpdateContentTypeAsync(contentType, It.IsAny<CancellationToken>()))
            .ReturnsAsync(contentType);
        _clientMock.Setup(x => x.ActivateContentTypeAsync(contentType.SystemProperties.Id, It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(contentType);

        // Act
        var result = await _sut.CreateContentTypeAsync(contentType, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("blogPost", result.SystemProperties.Id);
        _clientMock.Verify(x => x.CreateOrUpdateContentTypeAsync(contentType, It.IsAny<CancellationToken>()), Times.Once);
        _clientMock.Verify(x => x.ActivateContentTypeAsync(contentType.SystemProperties.Id, It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GivenValidContentTypeId_WhenDeleteContentTypeAsync_ThenDeletesContentType()
    {
        // Arrange
        var contentTypeId = "blogPost";

        // Act
        await _sut.DeleteContentTypeAsync(contentTypeId, CancellationToken.None);

        // Assert
        _clientMock.Verify(x => x.DeactivateContentTypeAsync(contentTypeId, It.IsAny<CancellationToken>()), Times.Once);
        _clientMock.Verify(x => x.DeleteContentTypeAsync(contentTypeId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GivenValidContentTypeId_WhenGetEntriesAsync_ThenReturnsAsyncEnumerableWithTotal()
    {
        // Arrange
        var entries = new[]
        {
            EntryBuilder.CreateBlogPost("entry1", "blogPost"),
            EntryBuilder.CreateBlogPost("entry2", "blogPost"),
            EntryBuilder.CreateBlogPost("entry3", "blogPost")
        };
        var contentfulCollection = new ContentfulCollection<Entry<dynamic>>
        {
            Items = entries.ToList(),
            Total = entries.Length
        };
        _clientMock.Setup(x => x.GetEntriesCollectionAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(contentfulCollection);

        // Act
        var result = _sut.GetEntriesAsync("blogPost", pageSize: 10, PagingMode.SkipForward, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        var entriesList = await result.ToListAsync();
        Assert.Equal(3, entriesList.Count);
        Assert.Equal(3, result.Total);
    }

    [Fact]
    public async Task GivenLargeDataSet_WhenGetEntriesAsync_ThenHandlesPagingCorrectly()
    {
        // Arrange
        const int totalEntries = 150;
        var entries = Enumerable.Range(1, totalEntries)
            .Select(i => EntryBuilder.CreateBlogPost($"entry{i}", "blogPost"))
            .ToArray();
        var contentfulCollection = new ContentfulCollection<Entry<dynamic>>
        {
            Items = entries.ToList(),
            Total = entries.Length
        };
        _clientMock.Setup(x => x.GetEntriesCollectionAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(contentfulCollection);

        // Act
        var result = _sut.GetEntriesAsync("blogPost", pageSize: 50, PagingMode.SkipForward, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        var entriesList = await result.ToListAsync();
        Assert.Equal(totalEntries, entriesList.Count);
        Assert.Equal(totalEntries, result.Total);
    }

    [Fact]
    public async Task GivenValidEntries_WhenCreateOrUpdateEntriesAsync_ThenCreatesOrUpdatesEntries()
    {
        // Arrange
        var entries = new[]
        {
            EntryBuilder.CreateBlogPost("entry1", "blogPost"),
            EntryBuilder.CreateBlogPost("entry2", "blogPost")
        };
        
        // Set up the client mock to return empty lookups
        _clientMock.Setup(x => x.GetExistingEntriesLookupByIdAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<string, Entry<dynamic>>());
        _clientMock.Setup(x => x.GetUnpublishedOrMissingReferencedEntriesIdsLookup(It.IsAny<IEnumerable<Entry<dynamic>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<string, HashSet<string>>());

        // Act
        await _sut.CreateOrUpdateEntriesAsync(entries, publish: false, CancellationToken.None);

        // Assert
        _entryOperationServiceMock.Verify(
            x => x.CreateOrUpdateEntryAsync(
                It.IsAny<Entry<dynamic>>(), 
                It.IsAny<int>(), 
                It.IsAny<bool>(), 
                It.IsAny<bool>(), 
                It.IsAny<CancellationToken>()), 
            Times.Exactly(2));
    }

    [Fact]
    public async Task GivenValidEntriesWithPublish_WhenCreateOrUpdateEntriesAsync_ThenPublishesEntries()
    {
        // Arrange
        var entries = new[]
        {
            EntryBuilder.CreateBlogPost("entry1", "blogPost")
        };
        
        // Set up the client mock to return empty lookups (enabling publish)
        _clientMock.Setup(x => x.GetExistingEntriesLookupByIdAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<string, Entry<dynamic>>());
        _clientMock.Setup(x => x.GetUnpublishedOrMissingReferencedEntriesIdsLookup(It.IsAny<IEnumerable<Entry<dynamic>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<string, HashSet<string>> { { "entry1", new HashSet<string>() } });

        // Act
        await _sut.CreateOrUpdateEntriesAsync(entries, publish: true, CancellationToken.None);

        // Assert
        _entryOperationServiceMock.Verify(
            x => x.CreateOrUpdateEntryAsync(
                It.IsAny<Entry<dynamic>>(), 
                It.IsAny<int>(), 
                It.IsAny<bool>(), 
                true, // publish should be true
                It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task GivenValidEntries_WhenPublishEntriesAsync_ThenPublishesEntries()
    {
        // Arrange
        var entries = new[]
        {
            EntryBuilder.CreateBlogPost("entry1", "blogPost"),
            EntryBuilder.CreateBlogPost("entry2", "blogPost")
        };

        // Act
        await _sut.PublishEntriesAsync(entries, CancellationToken.None);

        // Assert
        _entryOperationServiceMock.Verify(
            x => x.PublishEntryAsync(It.IsAny<Entry<dynamic>>(), It.IsAny<CancellationToken>()), 
            Times.Exactly(2));
    }

    [Fact]
    public async Task GivenValidEntries_WhenUnpublishEntriesAsync_ThenUnpublishesEntries()
    {
        // Arrange
        var entries = new[]
        {
            EntryBuilder.CreateBlogPost("entry1", "blogPost"),
            EntryBuilder.CreateBlogPost("entry2", "blogPost")
        };

        // Act
        await _sut.UnpublishEntriesAsync(entries, CancellationToken.None);

        // Assert
        _entryOperationServiceMock.Verify(
            x => x.UnpublishEntryAsync(It.IsAny<Entry<dynamic>>(), It.IsAny<CancellationToken>()), 
            Times.Exactly(2));
    }

    [Fact]
    public async Task GivenValidEntries_WhenDeleteEntriesAsync_ThenDeletesEntries()
    {
        // Arrange
        var entries = new[]
        {
            EntryBuilder.CreateBlogPost("entry1", "blogPost"),
            EntryBuilder.CreateBlogPost("entry2", "blogPost")
        };

        // Act
        await _sut.DeleteEntriesAsync(entries, CancellationToken.None);

        // Assert
        _entryOperationServiceMock.Verify(
            x => x.DeleteEntryAsync(It.IsAny<Entry<dynamic>>(), It.IsAny<CancellationToken>()), 
            Times.Exactly(2));
    }

    [Fact]
    public async Task GivenCancellationToken_WhenGetContentTypeAsync_ThenRespectsCancellation()
    {
        // Arrange
        var contentType = ContentTypeBuilder.CreateBlogPost();
        _clientMock.Setup(x => x.GetContentTypeAsync(contentType.SystemProperties.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(contentType);

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            _sut.GetContentTypeAsync("blogPost", cts.Token));
    }

    [Fact]
    public async Task GivenCancellationToken_WhenGetLocalesAsync_ThenRespectsCancellation()
    {
        // Arrange
        var locales = LocaleBuilder.CreateMultiple();
        _clientMock.Setup(x => x.GetLocalesCollectionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(locales);

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            _sut.GetLocalesAsync(cts.Token));
    }

    [Fact]
    public async Task GivenCancellationToken_WhenGetEntriesAsync_ThenRespectsCancellation()
    {
        // Arrange
        var entries = new[]
        {
            EntryBuilder.CreateBlogPost("entry1", "blogPost")
        };
        var contentfulCollection = new ContentfulCollection<Entry<dynamic>>
        {
            Items = entries.ToList(),
            Total = entries.Length
        };
        _clientMock.Setup(x => x.GetEntriesCollectionAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(contentfulCollection);

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
            await _sut.GetEntriesAsync("blogPost", cancellationToken: cts.Token).ToListAsync());
    }

    [Fact]
    public void GivenService_WhenCheckingInterface_ThenImplementsIContentfulService()
    {
        // Arrange & Act
        var implementsInterface = _sut is IContentfulService;

        // Assert
        Assert.True(implementsInterface);
    }

    [Fact]
    public async Task GivenEmptyEntries_WhenCreateOrUpdateEntriesAsync_ThenHandlesEmptyCollection()
    {
        // Arrange
        var emptyEntries = Array.Empty<Entry<dynamic>>();
        
        // Set up the client mock to return empty lookups
        _clientMock.Setup(x => x.GetExistingEntriesLookupByIdAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<string, Entry<dynamic>>());
        _clientMock.Setup(x => x.GetUnpublishedOrMissingReferencedEntriesIdsLookup(It.IsAny<IEnumerable<Entry<dynamic>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<string, HashSet<string>>());

        // Act
        await _sut.CreateOrUpdateEntriesAsync(emptyEntries, publish: false, CancellationToken.None);

        // Assert
        _entryOperationServiceMock.Verify(
            x => x.CreateOrUpdateEntryAsync(
                It.IsAny<Entry<dynamic>>(), 
                It.IsAny<int>(), 
                It.IsAny<bool>(), 
                It.IsAny<bool>(), 
                It.IsAny<CancellationToken>()), 
            Times.Never);
    }

    [Fact]
    public async Task GivenService_WhenMultipleOperationsPerformed_ThenAllOperationsWorkCorrectly()
    {
        // Arrange
        var contentType = ContentTypeBuilder.CreateBlogPost();
        var locales = LocaleBuilder.CreateMultiple();
        var entries = new[]
        {
            EntryBuilder.CreateBlogPost("entry1", "blogPost"),
            EntryBuilder.CreateBlogPost("entry2", "blogPost")
        };

        _clientMock.Setup(x => x.GetContentTypeAsync(contentType.SystemProperties.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(contentType);
        _clientMock.Setup(x => x.GetLocalesCollectionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(locales);
        var contentfulCollection = new ContentfulCollection<Entry<dynamic>>
        {
            Items = entries.ToList(),
            Total = entries.Length
        };
        _clientMock.Setup(x => x.GetEntriesCollectionAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(contentfulCollection);
        
        // Set up lookups for CreateOrUpdateEntriesAsync
        _clientMock.Setup(x => x.GetExistingEntriesLookupByIdAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<string, Entry<dynamic>>());
        _clientMock.Setup(x => x.GetUnpublishedOrMissingReferencedEntriesIdsLookup(It.IsAny<IEnumerable<Entry<dynamic>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<string, HashSet<string>>());

        // Act
        var retrievedContentType = await _sut.GetContentTypeAsync("blogPost", CancellationToken.None);
        var retrievedLocales = await _sut.GetLocalesAsync(CancellationToken.None);
        var retrievedEntries = await _sut.GetEntriesAsync("blogPost", cancellationToken: CancellationToken.None).ToListAsync();
        await _sut.CreateOrUpdateEntriesAsync(entries, publish: false, CancellationToken.None);
        await _sut.PublishEntriesAsync(entries, CancellationToken.None);

        // Assert
        Assert.NotNull(retrievedContentType);
        Assert.NotNull(retrievedLocales);
        Assert.Equal(2, retrievedEntries.Count);
        _entryOperationServiceMock.Verify(
            x => x.CreateOrUpdateEntryAsync(
                It.IsAny<Entry<dynamic>>(), 
                It.IsAny<int>(), 
                It.IsAny<bool>(), 
                It.IsAny<bool>(), 
                It.IsAny<CancellationToken>()), 
            Times.Exactly(2));
        _entryOperationServiceMock.Verify(
            x => x.PublishEntryAsync(It.IsAny<Entry<dynamic>>(), It.IsAny<CancellationToken>()), 
            Times.Exactly(2));
    }
}