using Contentful.Core.Errors;
using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Contool.Core.Infrastructure.Contentful.Services;
using Contool.Core.Infrastructure.Utils.Services;
using Contool.Core.Tests.Unit.Helpers;
using MockLite;
using System.Net;

namespace Contool.Core.Tests.Unit.Infrastructure.Contentful.Services;

public class ContentfulManagementClientAdapterResiliencyDecoratorTests : IDisposable
{
    private readonly ContentfulManagementClientAdapterResiliencyDecorator _sut;
    
    private readonly Mock<IContentfulManagementClientAdapter> _innerMock;
    private readonly Mock<IResiliencyExecutor> _resiliencyExecutorMock;

    public ContentfulManagementClientAdapterResiliencyDecoratorTests()
    {
        _innerMock = new Mock<IContentfulManagementClientAdapter>();
        _resiliencyExecutorMock = new Mock<IResiliencyExecutor>();
        
        _sut = new ContentfulManagementClientAdapterResiliencyDecorator(_innerMock.Object, _resiliencyExecutorMock.Object);
    }

    [Fact]
    public async Task GivenValidSpaceId_WhenGetSpaceAsync_ThenCallsInnerAdapterWithResiliency()
    {
        // Arrange
        var spaceId = "test-space";
        var expectedSpace = SpaceBuilder.CreateWithId(spaceId);
        _innerMock.Setup(x => x.GetSpaceAsync(spaceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedSpace);

        // Act
        var actual = await _sut.GetSpaceAsync(spaceId, CancellationToken.None);

        // Assert
        _innerMock.Verify(x => x.GetSpaceAsync(spaceId, It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(expectedSpace, actual);
    }

    [Fact]
    public async Task GivenValidEnvironmentId_WhenGetEnvironmentAsync_ThenCallsInnerAdapterWithResiliency()
    {
        // Arrange
        var environmentId = "test-environment";
        var expectedEnvironment = ContentfulEnvironmentBuilder.CreateDefault();
        _innerMock.Setup(x => x.GetEnvironmentAsync(environmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedEnvironment);

        // Act
        var actual = await _sut.GetEnvironmentAsync(environmentId, CancellationToken.None);

        // Assert
        _innerMock.Verify(x => x.GetEnvironmentAsync(environmentId, It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(expectedEnvironment, actual);
    }

    [Fact]
    public async Task WhenGetCurrentUser_ThenCallsInnerAdapterWithResiliency()
    {
        // Arrange
        var expectedUser = UserBuilder.CreateDefault();
        _innerMock.Setup(x => x.GetCurrentUser(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUser);

        // Act
        var actual = await _sut.GetCurrentUser(CancellationToken.None);

        // Assert
        _innerMock.Verify(x => x.GetCurrentUser(It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(expectedUser, actual);
    }

    [Fact]
    public async Task WhenGetLocalesCollectionAsync_ThenCallsInnerAdapterWithResiliency()
    {
        // Arrange
        var expectedLocales = LocaleBuilder.CreateMultiple();
        _innerMock.Setup(x => x.GetLocalesCollectionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedLocales);

        // Act
        var actual = await _sut.GetLocalesCollectionAsync(CancellationToken.None);

        // Assert
        _innerMock.Verify(x => x.GetLocalesCollectionAsync(It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(expectedLocales, actual);
    }

    [Fact]
    public async Task GivenValidContentTypeId_WhenGetContentTypeAsync_ThenCallsInnerAdapterWithResiliency()
    {
        // Arrange
        var contentTypeId = "test-content-type";
        var expectedContentType = ContentTypeBuilder.CreateBlogPost(contentTypeId, "Test Content Type");
        _innerMock.Setup(x => x.GetContentTypeAsync(contentTypeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedContentType);

        // Act
        var actual = await _sut.GetContentTypeAsync(contentTypeId, CancellationToken.None);

        // Assert
        _innerMock.Verify(x => x.GetContentTypeAsync(contentTypeId, It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(expectedContentType, actual);
    }

    [Fact]
    public async Task WhenGetContentTypesAsync_ThenCallsInnerAdapterWithResiliency()
    {
        // Arrange
        var expectedContentTypes = new List<ContentType>
        {
            ContentTypeBuilder.CreateBlogPost("blogPost", "Blog Post"),
            ContentTypeBuilder.CreateProduct("product", "Product")
        };
        _innerMock.Setup(x => x.GetContentTypesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedContentTypes);

        // Act
        var actual = await _sut.GetContentTypesAsync(CancellationToken.None);

        // Assert
        _innerMock.Verify(x => x.GetContentTypesAsync(It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(expectedContentTypes, actual);
    }

    [Fact]
    public async Task GivenValidContentType_WhenCreateOrUpdateContentTypeAsync_ThenCallsInnerAdapterWithResiliency()
    {
        // Arrange
        var contentType = ContentTypeBuilder.CreateBlogPost("blogPost", "Blog Post");
        var expectedContentType = ContentTypeBuilder.CreateBlogPost("blogPost", "Blog Post");
        expectedContentType.SystemProperties.Version = 2;
        _innerMock.Setup(x => x.CreateOrUpdateContentTypeAsync(contentType, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedContentType);

        // Act
        var actual = await _sut.CreateOrUpdateContentTypeAsync(contentType, CancellationToken.None);

        // Assert
        _innerMock.Verify(x => x.CreateOrUpdateContentTypeAsync(contentType, It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(expectedContentType, actual);
    }

    [Fact]
    public async Task GivenValidContentTypeIdAndVersion_WhenActivateContentTypeAsync_ThenCallsInnerAdapterWithResiliency()
    {
        // Arrange
        var contentTypeId = "test-content-type";
        var version = 1;
        var expectedContentType = ContentTypeBuilder.CreateBlogPost(contentTypeId, "Test Content Type");
        expectedContentType.SystemProperties.Version = version + 1;
        _innerMock.Setup(x => x.ActivateContentTypeAsync(contentTypeId, version, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedContentType);

        // Act
        var actual = await _sut.ActivateContentTypeAsync(contentTypeId, version, CancellationToken.None);

        // Assert
        _innerMock.Verify(x => x.ActivateContentTypeAsync(contentTypeId, version, It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(expectedContentType, actual);
    }

    [Fact]
    public async Task GivenValidContentTypeId_WhenDeactivateContentTypeAsync_ThenCallsInnerAdapterWithResiliency()
    {
        // Arrange
        var contentTypeId = "test-content-type";

        // Act
        await _sut.DeactivateContentTypeAsync(contentTypeId, CancellationToken.None);

        // Assert
        _innerMock.Verify(x => x.DeactivateContentTypeAsync(contentTypeId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GivenValidContentTypeId_WhenDeleteContentTypeAsync_ThenCallsInnerAdapterWithResiliency()
    {
        // Arrange
        var contentTypeId = "test-content-type";

        // Act
        await _sut.DeleteContentTypeAsync(contentTypeId, CancellationToken.None);

        // Assert
        _innerMock.Verify(x => x.DeleteContentTypeAsync(contentTypeId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GivenValidQueryString_WhenGetEntriesCollectionAsync_ThenCallsInnerAdapterWithResiliency()
    {
        // Arrange
        var queryString = "content_type=blogPost&limit=10";
        var entries = new List<Entry<dynamic>>
        {
            EntryBuilder.CreateBlogPost("entry1", "blogPost"),
            EntryBuilder.CreateBlogPost("entry2", "blogPost")
        };
        var expectedCollection = new ContentfulCollection<Entry<dynamic>>
        {
            Items = entries,
            Total = entries.Count
        };
        _innerMock.Setup(x => x.GetEntriesCollectionAsync(queryString, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedCollection);

        // Act
        var actual = await _sut.GetEntriesCollectionAsync(queryString, CancellationToken.None);

        // Assert
        _innerMock.Verify(x => x.GetEntriesCollectionAsync(queryString, It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(expectedCollection.Items, actual.Items);
        Assert.Equal(expectedCollection.Total, actual.Total);
    }

    [Fact]
    public async Task GivenValidEntry_WhenCreateOrUpdateEntryAsync_ThenCallsInnerAdapterWithResiliency()
    {
        // Arrange
        var entry = EntryBuilder.CreateBlogPost("entry1", "blogPost");
        var version = 1;
        var expectedEntry = EntryBuilder.CreateBlogPost("entry1", "blogPost");
        expectedEntry.SystemProperties.Version = version + 1;
        _innerMock.Setup(x => x.CreateOrUpdateEntryAsync(entry, version, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedEntry);

        // Act
        var actual = await _sut.CreateOrUpdateEntryAsync(entry, version, CancellationToken.None);

        // Assert
        _innerMock.Verify(x => x.CreateOrUpdateEntryAsync(entry, version, It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(expectedEntry, actual);
    }

    [Fact]
    public async Task GivenValidEntryIdAndVersion_WhenPublishEntryAsync_ThenCallsInnerAdapterWithResiliency()
    {
        // Arrange
        var entryId = "test-entry";
        var version = 1;
        var expectedEntry = EntryBuilder.CreateBlogPost(entryId, "blogPost");
        expectedEntry.SystemProperties.Version = version + 1;
        _innerMock.Setup(x => x.PublishEntryAsync(entryId, version, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedEntry);

        // Act
        var actual = await _sut.PublishEntryAsync(entryId, version, CancellationToken.None);

        // Assert
        _innerMock.Verify(x => x.PublishEntryAsync(entryId, version, It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(expectedEntry, actual);
    }

    [Fact]
    public async Task GivenValidEntryIdAndVersion_WhenUnpublishEntryAsync_ThenCallsInnerAdapterWithResiliency()
    {
        // Arrange
        var entryId = "test-entry";
        var version = 2;
        var expectedEntry = EntryBuilder.CreateBlogPost(entryId, "blogPost");
        expectedEntry.SystemProperties.Version = version + 1;
        _innerMock.Setup(x => x.UnpublishEntryAsync(entryId, version, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedEntry);

        // Act
        var actual = await _sut.UnpublishEntryAsync(entryId, version, CancellationToken.None);

        // Assert
        _innerMock.Verify(x => x.UnpublishEntryAsync(entryId, version, It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(expectedEntry, actual);
    }

    [Fact]
    public async Task GivenValidEntryIdAndVersion_WhenArchiveEntryAsync_ThenCallsInnerAdapterWithResiliency()
    {
        // Arrange
        var entryId = "test-entry";
        var version = 1;
        var expectedEntry = EntryBuilder.CreateBlogPost(entryId, "blogPost");
        expectedEntry.SystemProperties.Version = version + 1;
        _innerMock.Setup(x => x.ArchiveEntryAsync(entryId, version, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedEntry);

        // Act
        var actual = await _sut.ArchiveEntryAsync(entryId, version, CancellationToken.None);

        // Assert
        _innerMock.Verify(x => x.ArchiveEntryAsync(entryId, version, It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(expectedEntry, actual);
    }

    [Fact]
    public async Task GivenValidEntryIdAndVersion_WhenUnarchiveEntryAsync_ThenCallsInnerAdapterWithResiliency()
    {
        // Arrange
        var entryId = "test-entry";
        var version = 1;
        var expectedEntry = EntryBuilder.CreateBlogPost(entryId, "blogPost");
        expectedEntry.SystemProperties.Version = version + 1;
        _innerMock.Setup(x => x.UnarchiveEntryAsync(entryId, version, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedEntry);

        // Act
        var actual = await _sut.UnarchiveEntryAsync(entryId, version, CancellationToken.None);

        // Assert
        _innerMock.Verify(x => x.UnarchiveEntryAsync(entryId, version, It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(expectedEntry, actual);
    }

    [Fact]
    public async Task GivenValidEntryIdAndVersion_WhenDeleteEntryAsync_ThenCallsInnerAdapterWithResiliency()
    {
        // Arrange
        var entryId = "test-entry";
        var version = 1;

        // Act
        await _sut.DeleteEntryAsync(entryId, version, CancellationToken.None);

        // Assert
        _innerMock.Verify(x => x.DeleteEntryAsync(entryId, version, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GivenCancelledToken_WhenGetSpaceAsync_ThenRespectsCancellation()
    {
        // Arrange
        var spaceId = "test-space";
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            _sut.GetSpaceAsync(spaceId, cts.Token));
    }

    [Fact]
    public async Task GivenCancelledToken_WhenCreateOrUpdateContentTypeAsync_ThenRespectsCancellation()
    {
        // Arrange
        var contentType = ContentTypeBuilder.CreateBlogPost("blogPost", "Blog Post");
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            _sut.CreateOrUpdateContentTypeAsync(contentType, cts.Token));
    }

    [Fact]
    public async Task GivenCancelledToken_WhenPublishEntryAsync_ThenRespectsCancellation()
    {
        // Arrange
        var entryId = "test-entry";
        var version = 1;
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            _sut.PublishEntryAsync(entryId, version, cts.Token));
    }

    [Fact]
    public async Task GivenInnerAdapterThrowsNonRetryableException_WhenGetSpaceAsync_ThenPropagatesExceptionImmediately()
    {
        // Arrange
        var spaceId = "test-space";
        var expectedException = new ArgumentException("Test exception");
        _innerMock.Setup(x => x.GetSpaceAsync(spaceId, It.IsAny<CancellationToken>()))
            .Throws(expectedException);

        // Act & Assert
        var actualException = await Assert.ThrowsAsync<ArgumentException>(() =>
            _sut.GetSpaceAsync(spaceId, CancellationToken.None));
        
        Assert.Equal(expectedException.Message, actualException.Message);
    }

    [Fact]
    public async Task GivenInnerAdapterThrowsContentfulException_WhenGetSpaceAsync_ThenRetriesAndEventuallyThrows()
    {
        // Arrange
        var spaceId = "test-space";
        var expectedException = new ContentfulException(400, "Test exception");
        _innerMock.Setup(x => x.GetSpaceAsync(spaceId, It.IsAny<CancellationToken>()))
            .Throws(expectedException);

        // Act & Assert
        var actualException = await Assert.ThrowsAsync<ContentfulException>(() =>
            _sut.GetSpaceAsync(spaceId, CancellationToken.None));
        
        Assert.Equal(expectedException.Message, actualException.Message);
    }

    [Fact]
    public async Task GivenInnerAdapterThrowsRateLimitException_WhenGetSpaceAsync_ThenRetriesAndEventuallyThrows()
    {
        // Arrange
        var spaceId = "test-space";
        var expectedException = new ContentfulRateLimitException("Test exception");
        _innerMock.Setup(x => x.GetSpaceAsync(spaceId, It.IsAny<CancellationToken>()))
            .Throws(expectedException);

        // Act & Assert
        var actualException = await Assert.ThrowsAsync<ContentfulRateLimitException>(() =>
            _sut.GetSpaceAsync(spaceId, CancellationToken.None));
        
        Assert.Equal(expectedException.Message, actualException.Message);
    }

    [Fact]
    public async Task GivenFixedBackoffStrategy_WhenCreatingRetryPolicy_ThenUsesFixedDelays()
    {
        // Arrange

        var fixedBackoffDecorator = new ContentfulManagementClientAdapterResiliencyDecorator(_innerMock.Object, _resiliencyExecutorMock.Object);
        
        var spaceId = "test-space";
        var expectedException = new ContentfulException(400, "Test exception");
        _innerMock.Setup(x => x.GetSpaceAsync(spaceId, It.IsAny<CancellationToken>()))
            .Throws(expectedException);

        // Act & Assert
        var actualException = await Assert.ThrowsAsync<ContentfulException>(() =>
            fixedBackoffDecorator.GetSpaceAsync(spaceId, CancellationToken.None));
        
        Assert.Equal(expectedException.Message, actualException.Message);
    }

    [Fact]
    public async Task GivenNoBackoffStrategy_WhenCreatingRetryPolicy_ThenUsesImmediateRetry()
    {
        // Arrange

        var noBackoffDecorator = new ContentfulManagementClientAdapterResiliencyDecorator(_innerMock.Object, _resiliencyExecutorMock.Object);
        
        var spaceId = "test-space";
        var expectedException = new ContentfulException(400, "Test exception");
        _innerMock.Setup(x => x.GetSpaceAsync(spaceId, It.IsAny<CancellationToken>()))
            .Throws(expectedException);

        // Act & Assert
        var actualException = await Assert.ThrowsAsync<ContentfulException>(() =>
            noBackoffDecorator.GetSpaceAsync(spaceId, CancellationToken.None));
        
        Assert.Equal(expectedException.Message, actualException.Message);
    }

    [Fact]
    public async Task GivenConcurrencyLimit_WhenMultipleRequestsExecuteConcurrently_ThenLimitsConcurrency()
    {
        // Arrange

        var concurrencyLimitDecorator = new ContentfulManagementClientAdapterResiliencyDecorator(_innerMock.Object, _resiliencyExecutorMock.Object);
        
        var spaceId = "test-space";
        var expectedSpace = SpaceBuilder.CreateWithId(spaceId);
        _innerMock.Setup(x => x.GetSpaceAsync(spaceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedSpace);

        // Act
        var tasks = new List<Task<Space>>();
        for (var i = 0; i < 5; i++)
        {
            tasks.Add(concurrencyLimitDecorator.GetSpaceAsync(spaceId, CancellationToken.None));
        }

        var results = await Task.WhenAll(tasks);

        // Assert
        Assert.All(results, result => Assert.Equal(expectedSpace, result));
    }

    public void Dispose()
    {
        // No cleanup needed as ContentfulManagementClientAdapterResiliencyDecorator does not implement IDisposable
    }
}