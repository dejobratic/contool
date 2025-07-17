using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Contool.Core.Infrastructure.Contentful.Services;
using Contool.Core.Infrastructure.Utils.Models;
using Contool.Core.Infrastructure.Utils.Services;
using Contool.Core.Tests.Unit.Helpers;
using MockLite;

namespace Contool.Core.Tests.Unit.Infrastructure.Contentful.Services;

public class ContentfulManagementClientAdapterOperationTrackerDecoratorTests
{
    private readonly ContentfulManagementClientAdapterOperationTrackerDecorator _sut;
    
    private readonly Mock<IContentfulManagementClientAdapter> _innerMock;
    private readonly Mock<IOperationTracker> _operationTrackerMock;

    public ContentfulManagementClientAdapterOperationTrackerDecoratorTests()
    {
        _innerMock = new Mock<IContentfulManagementClientAdapter>();
        _operationTrackerMock = new Mock<IOperationTracker>();
        _sut = new ContentfulManagementClientAdapterOperationTrackerDecorator(_innerMock.Object, _operationTrackerMock.Object);
    }

    [Fact]
    public async Task GivenValidSpaceId_WhenGetSpaceAsync_ThenCallsInnerAdapterWithoutTracking()
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
        _operationTrackerMock.Verify(x => x.IncrementSuccessCount(It.IsAny<Operation>(), It.IsAny<string>()), Times.Never);
        _operationTrackerMock.Verify(x => x.IncrementErrorCount(It.IsAny<Operation>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GivenValidEnvironmentId_WhenGetEnvironmentAsync_ThenCallsInnerAdapterWithoutTracking()
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
        _operationTrackerMock.Verify(x => x.IncrementSuccessCount(It.IsAny<Operation>(), It.IsAny<string>()), Times.Never);
        _operationTrackerMock.Verify(x => x.IncrementErrorCount(It.IsAny<Operation>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task WhenGetCurrentUser_ThenCallsInnerAdapterWithoutTracking()
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
        _operationTrackerMock.Verify(x => x.IncrementSuccessCount(It.IsAny<Operation>(), It.IsAny<string>()), Times.Never);
        _operationTrackerMock.Verify(x => x.IncrementErrorCount(It.IsAny<Operation>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task WhenGetLocalesCollectionAsync_ThenCallsInnerAdapterWithoutTracking()
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
        _operationTrackerMock.Verify(x => x.IncrementSuccessCount(It.IsAny<Operation>(), It.IsAny<string>()), Times.Never);
        _operationTrackerMock.Verify(x => x.IncrementErrorCount(It.IsAny<Operation>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GivenValidContentTypeId_WhenGetContentTypeAsync_ThenCallsInnerAdapterWithoutTracking()
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
        _operationTrackerMock.Verify(x => x.IncrementSuccessCount(It.IsAny<Operation>(), It.IsAny<string>()), Times.Never);
        _operationTrackerMock.Verify(x => x.IncrementErrorCount(It.IsAny<Operation>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task WhenGetContentTypesAsync_ThenCallsInnerAdapterWithoutTracking()
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
        _operationTrackerMock.Verify(x => x.IncrementSuccessCount(It.IsAny<Operation>(), It.IsAny<string>()), Times.Never);
        _operationTrackerMock.Verify(x => x.IncrementErrorCount(It.IsAny<Operation>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GivenValidContentType_WhenCreateOrUpdateContentTypeAsync_ThenCallsInnerAdapterWithoutTracking()
    {
        // Arrange
        var contentType = ContentTypeBuilder.CreateBlogPost("blogPost", "Blog Post");
        var expectedContentType = ContentTypeBuilder.CreateBlogPost("blogPost", "Blog Post");
        expectedContentType.SystemProperties.Version = 2;
        _innerMock.Setup(x => x.CreateOrUpdateContentTypeAsync(It.IsAny<ContentType>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedContentType);

        // Act
        var actual = await _sut.CreateOrUpdateContentTypeAsync(contentType, CancellationToken.None);

        // Assert
        _innerMock.Verify(x => x.CreateOrUpdateContentTypeAsync(contentType, It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(expectedContentType, actual);
        _operationTrackerMock.Verify(x => x.IncrementSuccessCount(It.IsAny<Operation>(), It.IsAny<string>()), Times.Never);
        _operationTrackerMock.Verify(x => x.IncrementErrorCount(It.IsAny<Operation>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GivenValidContentTypeIdAndVersion_WhenActivateContentTypeAsync_ThenCallsInnerAdapterWithoutTracking()
    {
        // Arrange
        var contentTypeId = "test-content-type";
        var version = 1;
        var expectedContentType = ContentTypeBuilder.CreateBlogPost(contentTypeId, "Test Content Type");
        expectedContentType.SystemProperties.Version = version + 1;
        _innerMock.Setup(x => x.ActivateContentTypeAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedContentType);

        // Act
        var actual = await _sut.ActivateContentTypeAsync(contentTypeId, version, CancellationToken.None);

        // Assert
        _innerMock.Verify(x => x.ActivateContentTypeAsync(contentTypeId, version, It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(expectedContentType, actual);
        _operationTrackerMock.Verify(x => x.IncrementSuccessCount(It.IsAny<Operation>(), It.IsAny<string>()), Times.Never);
        _operationTrackerMock.Verify(x => x.IncrementErrorCount(It.IsAny<Operation>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GivenValidContentTypeId_WhenDeactivateContentTypeAsync_ThenCallsInnerAdapterWithoutTracking()
    {
        // Arrange
        var contentTypeId = "test-content-type";

        // Act
        await _sut.DeactivateContentTypeAsync(contentTypeId, CancellationToken.None);

        // Assert
        _innerMock.Verify(x => x.DeactivateContentTypeAsync(contentTypeId, It.IsAny<CancellationToken>()), Times.Once);
        _operationTrackerMock.Verify(x => x.IncrementSuccessCount(It.IsAny<Operation>(), It.IsAny<string>()), Times.Never);
        _operationTrackerMock.Verify(x => x.IncrementErrorCount(It.IsAny<Operation>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GivenValidContentTypeId_WhenDeleteContentTypeAsync_ThenCallsInnerAdapterWithoutTracking()
    {
        // Arrange
        var contentTypeId = "test-content-type";

        // Act
        await _sut.DeleteContentTypeAsync(contentTypeId, CancellationToken.None);

        // Assert
        _innerMock.Verify(x => x.DeleteContentTypeAsync(contentTypeId, It.IsAny<CancellationToken>()), Times.Once);
        _operationTrackerMock.Verify(x => x.IncrementSuccessCount(It.IsAny<Operation>(), It.IsAny<string>()), Times.Never);
        _operationTrackerMock.Verify(x => x.IncrementErrorCount(It.IsAny<Operation>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GivenValidQueryString_WhenGetEntriesCollectionAsync_ThenCallsInnerAdapterAndTracksReadOperations()
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
        _innerMock.Setup(x => x.GetEntriesCollectionAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedCollection);

        // Act
        var actual = await _sut.GetEntriesCollectionAsync(queryString, CancellationToken.None);

        // Assert
        _innerMock.Verify(x => x.GetEntriesCollectionAsync(queryString, It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(expectedCollection.Items, actual.Items);
        Assert.Equal(expectedCollection.Total, actual.Total);
        
        // Verify tracking for each entry
        _operationTrackerMock.Verify(x => x.IncrementSuccessCount(Operation.Read, It.IsAny<string>()), Times.Exactly(entries.Count));
    }

    [Fact]
    public async Task GivenValidEntry_WhenCreateOrUpdateEntryAsync_ThenCallsInnerAdapterAndTracksUploadOperation()
    {
        // Arrange
        var entry = EntryBuilder.CreateBlogPost("entry1", "blogPost");
        var version = 1;
        var expectedEntry = EntryBuilder.CreateBlogPost("entry1", "blogPost");
        expectedEntry.SystemProperties.Version = version + 1;
        _innerMock.Setup(x => x.CreateOrUpdateEntryAsync(It.IsAny<Entry<dynamic>>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedEntry);

        // Act
        var actual = await _sut.CreateOrUpdateEntryAsync(entry, version, CancellationToken.None);

        // Assert
        _innerMock.Verify(x => x.CreateOrUpdateEntryAsync(entry, version, It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(expectedEntry, actual);
        
        // Verify tracking
        _operationTrackerMock.Verify(x => x.IncrementSuccessCount(Operation.Upload, entry.SystemProperties.Id), Times.Once);
    }

    [Fact]
    public async Task GivenValidEntryIdAndVersion_WhenPublishEntryAsync_ThenCallsInnerAdapterAndTracksPublishOperation()
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
        
        // Verify tracking
        _operationTrackerMock.Verify(x => x.IncrementSuccessCount(Operation.Publish, entryId), Times.Once);
    }

    [Fact]
    public async Task GivenValidEntryIdAndVersion_WhenUnpublishEntryAsync_ThenCallsInnerAdapterAndTracksUnpublishOperation()
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
        
        // Verify tracking
        _operationTrackerMock.Verify(x => x.IncrementSuccessCount(Operation.Unpublish, entryId), Times.Once);
    }

    [Fact]
    public async Task GivenValidEntryIdAndVersion_WhenArchiveEntryAsync_ThenCallsInnerAdapterAndTracksArchiveOperation()
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
        
        // Verify tracking
        _operationTrackerMock.Verify(x => x.IncrementSuccessCount(Operation.Archive, entryId), Times.Once);
    }

    [Fact]
    public async Task GivenValidEntryIdAndVersion_WhenUnarchiveEntryAsync_ThenCallsInnerAdapterAndTracksUnarchiveOperation()
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
        
        // Verify tracking
        _operationTrackerMock.Verify(x => x.IncrementSuccessCount(Operation.Unarchive, entryId), Times.Once);
    }

    [Fact]
    public async Task GivenValidEntryIdAndVersion_WhenDeleteEntryAsync_ThenCallsInnerAdapterAndTracksDeleteOperation()
    {
        // Arrange
        var entryId = "test-entry";
        var version = 1;
        _innerMock.Setup(x => x.DeleteEntryAsync(entryId, version, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _sut.DeleteEntryAsync(entryId, version, CancellationToken.None);

        // Assert
        _innerMock.Verify(x => x.DeleteEntryAsync(entryId, version, It.IsAny<CancellationToken>()), Times.Once);
        
        // Verify tracking
        _operationTrackerMock.Verify(x => x.IncrementSuccessCount(Operation.Delete, entryId), Times.Once);
    }

    [Fact]
    public async Task GivenInnerAdapterThrowsException_WhenCreateOrUpdateEntryAsync_ThenTracksErrorAndPropagatesException()
    {
        // Arrange
        var entry = EntryBuilder.CreateBlogPost("entry1", "blogPost");
        var version = 1;
        var expectedException = new InvalidOperationException("Test exception");
        _innerMock.Setup(x => x.CreateOrUpdateEntryAsync(entry, version, It.IsAny<CancellationToken>()))
            .Throws(expectedException);

        // Act & Assert
        var actualException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _sut.CreateOrUpdateEntryAsync(entry, version, CancellationToken.None));
        
        Assert.Equal(expectedException.Message, actualException.Message);
        
        // Verify error tracking
        _operationTrackerMock.Verify(x => x.IncrementErrorCount(Operation.Upload, entry.SystemProperties.Id), Times.Once);
        _operationTrackerMock.Verify(x => x.IncrementSuccessCount(It.IsAny<Operation>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GivenInnerAdapterThrowsException_WhenPublishEntryAsync_ThenTracksErrorAndPropagatesException()
    {
        // Arrange
        var entryId = "test-entry";
        var version = 1;
        var expectedException = new InvalidOperationException("Test exception");
        _innerMock.Setup(x => x.PublishEntryAsync(entryId, version, It.IsAny<CancellationToken>()))
            .Throws(expectedException);

        // Act & Assert
        var actualException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _sut.PublishEntryAsync(entryId, version, CancellationToken.None));
        
        Assert.Equal(expectedException.Message, actualException.Message);
        
        // Verify error tracking
        _operationTrackerMock.Verify(x => x.IncrementErrorCount(Operation.Publish, entryId), Times.Once);
        _operationTrackerMock.Verify(x => x.IncrementSuccessCount(It.IsAny<Operation>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GivenInnerAdapterThrowsException_WhenDeleteEntryAsync_ThenTracksErrorAndPropagatesException()
    {
        // Arrange
        var entryId = "test-entry";
        var version = 1;
        var expectedException = new InvalidOperationException("Test exception");
        _innerMock.Setup(x => x.DeleteEntryAsync(entryId, version, It.IsAny<CancellationToken>()))
            .Throws(expectedException);

        // Act & Assert
        var actualException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _sut.DeleteEntryAsync(entryId, version, CancellationToken.None));
        
        Assert.Equal(expectedException.Message, actualException.Message);
        
        // Verify error tracking
        _operationTrackerMock.Verify(x => x.IncrementErrorCount(Operation.Delete, entryId), Times.Once);
        _operationTrackerMock.Verify(x => x.IncrementSuccessCount(It.IsAny<Operation>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GivenCancelledToken_WhenCreateOrUpdateEntryAsync_ThenRespectsCancellation()
    {
        // Arrange
        var entry = EntryBuilder.CreateBlogPost("entry1", "blogPost");
        var version = 1;
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            _sut.CreateOrUpdateEntryAsync(entry, version, cts.Token));
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
    public async Task GivenCancelledToken_WhenDeleteEntryAsync_ThenRespectsCancellation()
    {
        // Arrange
        var entryId = "test-entry";
        var version = 1;
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            _sut.DeleteEntryAsync(entryId, version, cts.Token));
    }

    [Fact]
    public async Task GivenMultipleEntries_WhenGetEntriesCollectionAsync_ThenTracksAllReadOperations()
    {
        // Arrange
        var queryString = "content_type=blogPost&limit=10";
        var entries = new List<Entry<dynamic>>
        {
            EntryBuilder.CreateBlogPost("entry1", "blogPost"),
            EntryBuilder.CreateBlogPost("entry2", "blogPost"),
            EntryBuilder.CreateBlogPost("entry3", "blogPost")
        };
        var expectedCollection = new ContentfulCollection<Entry<dynamic>>
        {
            Items = entries,
            Total = entries.Count
        };
        _innerMock.Setup(x => x.GetEntriesCollectionAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedCollection);

        // Act
        await _sut.GetEntriesCollectionAsync(queryString, CancellationToken.None);

        // Assert
        _operationTrackerMock.Verify(x => x.IncrementSuccessCount(Operation.Read, It.IsAny<string>()), Times.Exactly(entries.Count));
    }

    [Fact]
    public async Task GivenEmptyEntriesCollection_WhenGetEntriesCollectionAsync_ThenTracksNoOperations()
    {
        // Arrange
        var queryString = "content_type=blogPost&limit=10";
        var expectedCollection = new ContentfulCollection<Entry<dynamic>>
        {
            Items = [],
            Total = 0
        };
        _innerMock.Setup(x => x.GetEntriesCollectionAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedCollection);

        // Act
        await _sut.GetEntriesCollectionAsync(queryString, CancellationToken.None);

        // Assert
        _operationTrackerMock.Verify(x => x.IncrementSuccessCount(It.IsAny<Operation>(), It.IsAny<string>()), Times.Never);
    }
}