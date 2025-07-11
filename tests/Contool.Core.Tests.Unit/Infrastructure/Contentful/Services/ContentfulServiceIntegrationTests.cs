using Contentful.Core.Models;
using Contool.Core.Infrastructure.Contentful.Services;
using Contool.Core.Infrastructure.Contentful.Utils;
using Contool.Core.Tests.Unit.Helpers;
using Contool.Core.Tests.Unit.Mocks;

namespace Contool.Core.Tests.Unit.Infrastructure.Contentful.Services;

public class ContentfulServiceIntegrationTests
{
    private readonly ContentfulService _sut;

    private readonly MockContentfulManagementClientAdapter _clientMock = new();
    private readonly MockEntryOperationService _entryOperationServiceMock = new();

    public ContentfulServiceIntegrationTests()
    {
        _sut = new ContentfulService(_clientMock, _entryOperationServiceMock);
    }

    [Fact]
    public async Task GivenValidContentTypeId_WhenGetContentTypeAsync_ThenReturnsContentType()
    {
        // Arrange
        var contentType = ContentTypeBuilder.CreateBlogPost();
        _clientMock.SetupContentType(contentType);

        // Act
        var result = await _sut.GetContentTypeAsync("blogPost", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("blogPost", result.SystemProperties.Id);
        Assert.Equal("Blog Post", result.Name);
        Assert.True(_clientMock.GetContentTypeAsyncWasCalled);
    }

    [Fact]
    public async Task GivenInvalidContentTypeId_WhenGetContentTypeAsync_ThenReturnsNull()
    {
        // Arrange
        _clientMock.SetupContentType(null);

        // Act
        var result = await _sut.GetContentTypeAsync("nonexistent", CancellationToken.None);

        // Assert
        Assert.Null(result);
        Assert.True(_clientMock.GetContentTypeAsyncWasCalled);
    }

    [Fact]
    public async Task GivenService_WhenGetLocalesAsync_ThenReturnsLocales()
    {
        // Arrange
        var locales = LocaleBuilder.CreateMultiple();
        _clientMock.SetupLocales(locales);

        // Act
        var result = await _sut.GetLocalesAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
        Assert.Contains(result, l => l.Code == "en-US");
        Assert.Contains(result, l => l.Code == "fr-FR");
        Assert.Contains(result, l => l.Code == "es-ES");
        Assert.True(_clientMock.GetLocalesAsyncWasCalled);
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
        _clientMock.SetupContentTypes(contentTypes);

        // Act
        var result = await _sut.GetContentTypesAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
        Assert.Contains(result, ct => ct.SystemProperties.Id == "blogPost");
        Assert.Contains(result, ct => ct.SystemProperties.Id == "product");
        Assert.Contains(result, ct => ct.SystemProperties.Id == "minimal");
        Assert.True(_clientMock.GetContentTypesAsyncWasCalled);
    }

    [Fact]
    public async Task GivenNewContentType_WhenCreateContentTypeAsync_ThenCreatesAndReturnsContentType()
    {
        // Arrange
        var contentType = ContentTypeBuilder.CreateBlogPost();
        _clientMock.SetupCreatedContentType(contentType);

        // Act
        var result = await _sut.CreateContentTypeAsync(contentType, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("blogPost", result.SystemProperties.Id);
        Assert.True(_clientMock.CreateContentTypeAsyncWasCalled);
    }

    [Fact]
    public async Task GivenValidContentTypeId_WhenDeleteContentTypeAsync_ThenDeletesContentType()
    {
        // Arrange
        var contentTypeId = "blogPost";

        // Act
        await _sut.DeleteContentTypeAsync(contentTypeId, CancellationToken.None);

        // Assert
        Assert.True(_clientMock.DeleteContentTypeAsyncWasCalled);
        Assert.Equal(contentTypeId, _clientMock.LastDeletedContentTypeId);
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
        _clientMock.SetupEntries(entries);

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
        _clientMock.SetupEntries(entries);

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

        // Act
        await _sut.CreateOrUpdateEntriesAsync(entries, publish: false, CancellationToken.None);

        // Assert
        Assert.True(_clientMock.CreateOrUpdateEntriesAsyncWasCalled);
        Assert.Equal(2, _clientMock.LastCreatedOrUpdatedEntries?.Count());
        Assert.False(_clientMock.LastPublishFlag);
    }

    [Fact]
    public async Task GivenValidEntriesWithPublish_WhenCreateOrUpdateEntriesAsync_ThenPublishesEntries()
    {
        // Arrange
        var entries = new[]
        {
            EntryBuilder.CreateBlogPost("entry1", "blogPost")
        };

        // Act
        await _sut.CreateOrUpdateEntriesAsync(entries, publish: true, CancellationToken.None);

        // Assert
        Assert.True(_clientMock.CreateOrUpdateEntriesAsyncWasCalled);
        Assert.True(_clientMock.LastPublishFlag);
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
        Assert.True(_clientMock.PublishEntriesAsyncWasCalled);
        Assert.Equal(2, _clientMock.LastPublishedEntries?.Count());
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
        Assert.True(_clientMock.UnpublishEntriesAsyncWasCalled);
        Assert.Equal(2, _clientMock.LastUnpublishedEntries?.Count());
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
        Assert.True(_clientMock.DeleteEntriesAsyncWasCalled);
        Assert.Equal(2, _clientMock.LastDeletedEntries?.Count());
    }

    [Fact]
    public async Task GivenCancellationToken_WhenGetContentTypeAsync_ThenRespectsCancellation()
    {
        // Arrange
        var contentType = ContentTypeBuilder.CreateBlogPost();
        _clientMock.SetupContentType(contentType);

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
        _clientMock.SetupLocales(locales);

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
        _clientMock.SetupEntries(entries);

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

        // Act
        await _sut.CreateOrUpdateEntriesAsync(emptyEntries, publish: false, CancellationToken.None);

        // Assert
        Assert.True(_clientMock.CreateOrUpdateEntriesAsyncWasCalled);
        Assert.Empty(_clientMock.LastCreatedOrUpdatedEntries ?? []);
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

        _clientMock.SetupContentType(contentType);
        _clientMock.SetupLocales(locales);
        _clientMock.SetupEntries(entries);

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
        Assert.True(_clientMock.CreateOrUpdateEntriesAsyncWasCalled);
        Assert.True(_clientMock.PublishEntriesAsyncWasCalled);
    }
}