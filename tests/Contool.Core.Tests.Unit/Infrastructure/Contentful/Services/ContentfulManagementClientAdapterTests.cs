using Contentful.Core;
using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Contool.Core.Infrastructure.Contentful.Services;
using Contool.Core.Tests.Unit.Helpers;
using MockLite;

namespace Contool.Core.Tests.Unit.Infrastructure.Contentful.Services;

public class ContentfulManagementClientAdapterTests
{
    private readonly ContentfulManagementClientAdapter _sut;
    
    private readonly Mock<IContentfulManagementClient> _clientMock = new();

    public ContentfulManagementClientAdapterTests()
    {
        _sut = new ContentfulManagementClientAdapter(_clientMock.Object);
    }

    [Fact]
    public async Task GivenValidSpaceId_WhenGetSpaceAsync_ThenReturnsSpace()
    {
        // Arrange
        var spaceId = "test-space";
        var expectedSpace = new Space
        {
            SystemProperties = new SystemProperties { Id = spaceId },
            Name = "Test Space"
        };
        _clientMock.Setup(x => x.GetSpace(spaceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedSpace);

        // Act
        var actual = await _sut.GetSpaceAsync(spaceId, CancellationToken.None);

        // Assert
        _clientMock.Verify(x => x.GetSpace(spaceId, CancellationToken.None));
        Assert.Equal(expectedSpace, actual);
    }

    [Fact]
    public async Task GivenValidEnvironmentId_WhenGetEnvironmentAsync_ThenReturnsEnvironment()
    {
        // Arrange
        var environmentId = "test-environment";
        var expectedEnvironment = new ContentfulEnvironment
        {
            SystemProperties = new SystemProperties { Id = environmentId }
        };
        _clientMock.Setup(x => x.GetEnvironment(environmentId, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedEnvironment);

        // Act
        var actual = await _sut.GetEnvironmentAsync(environmentId, CancellationToken.None);

        // Assert
        _clientMock.Verify(x => x.GetEnvironment(environmentId, It.IsAny<string>(), CancellationToken.None));
        Assert.Equal(expectedEnvironment, actual);
    }

    [Fact]
    public async Task WhenGetCurrentUser_ThenReturnsUser()
    {
        // Arrange
        var expectedUser = new User
        {
            SystemProperties = new SystemProperties { Id = "user-1" },
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com"
        };
        _clientMock.Setup(x => x.GetCurrentUser(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUser);

        // Act
        var actual = await _sut.GetCurrentUser(CancellationToken.None);

        // Assert
        _clientMock.Verify(x => x.GetCurrentUser(CancellationToken.None));
        Assert.Equal(expectedUser, actual);
    }

    [Fact]
    public async Task WhenGetLocalesCollectionAsync_ThenReturnsLocales()
    {
        // Arrange
        var expectedLocales = LocaleBuilder.CreateMultiple();
        var expectedCollection = new ContentfulCollection<Locale>
        {
            Items = expectedLocales,
            Total = expectedLocales.Count
        };
        _clientMock.Setup(x => x.GetLocalesCollection(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedCollection);

        // Act
        var actual = await _sut.GetLocalesCollectionAsync(CancellationToken.None);

        // Assert
        _clientMock.Verify(x => x.GetLocalesCollection(It.IsAny<string>(), CancellationToken.None));
        Assert.Equal(expectedLocales, actual);
    }

    [Fact]
    public async Task GivenValidContentTypeId_WhenGetContentTypeAsync_ThenReturnsContentType()
    {
        // Arrange
        var contentTypeId = "test-content-type";
        var expectedContentType = ContentTypeBuilder.CreateBlogPost(contentTypeId, "Test Content Type");
        _clientMock.Setup(x => x.GetContentType(contentTypeId, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedContentType);

        // Act
        var actual = await _sut.GetContentTypeAsync(contentTypeId, CancellationToken.None);

        // Assert
        _clientMock.Verify(x => x.GetContentType(contentTypeId, It.IsAny<string>(), CancellationToken.None));
        Assert.Equal(expectedContentType, actual);
    }

    [Fact]
    public async Task GivenNonExistentContentTypeId_WhenGetContentTypeAsync_ThenReturnsNull()
    {
        // Arrange
        var contentTypeId = "non-existent-content-type";
        _clientMock.Setup(x => x.GetContentType(contentTypeId, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ContentType?)null);

        // Act
        var actual = await _sut.GetContentTypeAsync(contentTypeId, CancellationToken.None);

        // Assert
        _clientMock.Verify(x => x.GetContentType(contentTypeId, It.IsAny<string>(), CancellationToken.None));
        Assert.Null(actual);
    }

    [Fact]
    public async Task WhenGetContentTypesAsync_ThenReturnsContentTypes()
    {
        // Arrange
        var expectedContentTypes = new List<ContentType>
        {
            ContentTypeBuilder.CreateBlogPost("blogPost", "Blog Post"),
            ContentTypeBuilder.CreateProduct("product", "Product")
        };
        _clientMock.Setup(x => x.GetContentTypes(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedContentTypes);

        // Act
        var actual = await _sut.GetContentTypesAsync(CancellationToken.None);

        // Assert
        _clientMock.Verify(x => x.GetContentTypes(It.IsAny<string>(), CancellationToken.None));
        Assert.Equal(expectedContentTypes, actual);
    }

    [Fact]
    public async Task GivenValidContentType_WhenCreateOrUpdateContentTypeAsync_ThenReturnsContentType()
    {
        // Arrange
        var contentType = ContentTypeBuilder.CreateBlogPost("blogPost", "Blog Post");
        var expectedContentType = ContentTypeBuilder.CreateBlogPost("blogPost", "Blog Post");
        expectedContentType.SystemProperties.Version = 2;
        _clientMock.Setup(x => x.CreateOrUpdateContentType(contentType, It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedContentType);

        // Act
        var actual = await _sut.CreateOrUpdateContentTypeAsync(contentType, CancellationToken.None);

        // Assert
        _clientMock.Verify(x => x.CreateOrUpdateContentType(contentType, It.IsAny<string>(), It.IsAny<int?>(), CancellationToken.None));
        Assert.Equal(expectedContentType, actual);
    }

    [Fact]
    public async Task GivenValidContentTypeIdAndVersion_WhenActivateContentTypeAsync_ThenReturnsActivatedContentType()
    {
        // Arrange
        var contentTypeId = "test-content-type";
        var version = 1;
        var expectedContentType = ContentTypeBuilder.CreateBlogPost(contentTypeId, "Test Content Type");
        expectedContentType.SystemProperties.Version = version + 1;
        _clientMock.Setup(x => x.ActivateContentType(contentTypeId, version, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedContentType);

        // Act
        var actual = await _sut.ActivateContentTypeAsync(contentTypeId, version, CancellationToken.None);

        // Assert
        _clientMock.Verify(x => x.ActivateContentType(contentTypeId, version, It.IsAny<string>(), CancellationToken.None));
        Assert.Equal(expectedContentType, actual);
    }

    [Fact]
    public async Task GivenValidContentTypeId_WhenDeactivateContentTypeAsync_ThenCompletesSuccessfully()
    {
        // Arrange
        var contentTypeId = "test-content-type";

        // Act
        await _sut.DeactivateContentTypeAsync(contentTypeId, CancellationToken.None);

        // Assert
        _clientMock.Verify(x => x.DeactivateContentType(contentTypeId, It.IsAny<string>(), CancellationToken.None));
    }

    [Fact]
    public async Task GivenValidContentTypeId_WhenDeleteContentTypeAsync_ThenCompletesSuccessfully()
    {
        // Arrange
        var contentTypeId = "test-content-type";

        // Act
        await _sut.DeleteContentTypeAsync(contentTypeId, CancellationToken.None);

        // Assert
        _clientMock.Verify(x => x.DeleteContentType(contentTypeId, It.IsAny<string>(), CancellationToken.None));
    }

    [Fact]
    public async Task GivenValidQueryString_WhenGetEntriesCollectionAsync_ThenReturnsEntries()
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
        _clientMock.Setup(x => x.GetEntriesCollection<Entry<dynamic>>(It.IsAny<string>(), queryString, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedCollection);

        // Act
        var actual = await _sut.GetEntriesCollectionAsync(queryString, CancellationToken.None);

        // Assert
        _clientMock.Verify(x => x.GetEntriesCollection<Entry<dynamic>>(It.IsAny<string>(), queryString, CancellationToken.None));
        Assert.Equal(expectedCollection.Items, actual.Items);
        Assert.Equal(expectedCollection.Total, actual.Total);
    }

    [Fact]
    public async Task GivenValidEntry_WhenCreateOrUpdateEntryAsync_ThenReturnsEntry()
    {
        // Arrange
        var entry = EntryBuilder.CreateBlogPost("entry1", "blogPost");
        var version = 1;
        var expectedEntry = EntryBuilder.CreateBlogPost("entry1", "blogPost");
        expectedEntry.SystemProperties.Version = version + 1;
        _clientMock.Setup(x => x.CreateOrUpdateEntry(entry, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedEntry);

        // Act
        var actual = await _sut.CreateOrUpdateEntryAsync(entry, version, CancellationToken.None);

        // Assert
        _clientMock.Verify(x => x.CreateOrUpdateEntry(entry, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int?>(), CancellationToken.None));
        Assert.Equal(expectedEntry, actual);
    }

    [Fact]
    public async Task GivenValidEntryIdAndVersion_WhenPublishEntryAsync_ThenReturnsPublishedEntry()
    {
        // Arrange
        var entryId = "test-entry";
        var version = 1;
        var expectedEntry = EntryBuilder.CreateBlogPost(entryId, "blogPost");
        expectedEntry.SystemProperties.Version = version + 1;
        _clientMock.Setup(x => x.PublishEntry(entryId, version, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedEntry);

        // Act
        var actual = await _sut.PublishEntryAsync(entryId, version, CancellationToken.None);

        // Assert
        _clientMock.Verify(x => x.PublishEntry(entryId, version, It.IsAny<string>(), CancellationToken.None));
        Assert.Equal(expectedEntry, actual);
    }

    [Fact]
    public async Task GivenValidEntryIdAndVersion_WhenUnpublishEntryAsync_ThenReturnsUnpublishedEntry()
    {
        // Arrange
        var entryId = "test-entry";
        var version = 2;
        var expectedEntry = EntryBuilder.CreateBlogPost(entryId, "blogPost");
        expectedEntry.SystemProperties.Version = version + 1;
        _clientMock.Setup(x => x.UnpublishEntry(entryId, version, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedEntry);

        // Act
        var actual = await _sut.UnpublishEntryAsync(entryId, version, CancellationToken.None);

        // Assert
        _clientMock.Verify(x => x.UnpublishEntry(entryId, version, It.IsAny<string>(), CancellationToken.None));
        Assert.Equal(expectedEntry, actual);
    }

    [Fact]
    public async Task GivenValidEntryIdAndVersion_WhenArchiveEntryAsync_ThenReturnsArchivedEntry()
    {
        // Arrange
        var entryId = "test-entry";
        var version = 1;
        var expectedEntry = EntryBuilder.CreateBlogPost(entryId, "blogPost");
        expectedEntry.SystemProperties.Version = version + 1;
        _clientMock.Setup(x => x.ArchiveEntry(entryId, version, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedEntry);

        // Act
        var actual = await _sut.ArchiveEntryAsync(entryId, version, CancellationToken.None);

        // Assert
        _clientMock.Verify(x => x.ArchiveEntry(entryId, version, It.IsAny<string>(), CancellationToken.None));
        Assert.Equal(expectedEntry, actual);
    }

    [Fact]
    public async Task GivenValidEntryIdAndVersion_WhenUnarchiveEntryAsync_ThenReturnsUnarchivedEntry()
    {
        // Arrange
        var entryId = "test-entry";
        var version = 1;
        var expectedEntry = EntryBuilder.CreateBlogPost(entryId, "blogPost");
        expectedEntry.SystemProperties.Version = version + 1;
        _clientMock.Setup(x => x.UnarchiveEntry(entryId, version, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedEntry);

        // Act
        var actual = await _sut.UnarchiveEntryAsync(entryId, version, CancellationToken.None);

        // Assert
        _clientMock.Verify(x => x.UnarchiveEntry(entryId, version, It.IsAny<string>(), CancellationToken.None));
        Assert.Equal(expectedEntry, actual);
    }

    [Fact]
    public async Task GivenValidEntryIdAndVersion_WhenDeleteEntryAsync_ThenCompletesSuccessfully()
    {
        // Arrange
        var entryId = "test-entry";
        var version = 1;

        // Act
        await _sut.DeleteEntryAsync(entryId, version, CancellationToken.None);

        // Assert
        _clientMock.Verify(x => x.DeleteEntry(entryId, version, It.IsAny<string>(), CancellationToken.None));
    }

    [Fact]
    public async Task GivenCancelledToken_WhenGetSpaceAsync_ThenThrowsOperationCanceledException()
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
    public async Task GivenCancelledToken_WhenGetEnvironmentAsync_ThenThrowsOperationCanceledException()
    {
        // Arrange
        var environmentId = "test-environment";
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            _sut.GetEnvironmentAsync(environmentId, cts.Token));
    }

    [Fact]
    public async Task GivenCancelledToken_WhenGetCurrentUser_ThenThrowsOperationCanceledException()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            _sut.GetCurrentUser(cts.Token));
    }

    [Fact]
    public async Task GivenCancelledToken_WhenGetLocalesCollectionAsync_ThenThrowsOperationCanceledException()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            _sut.GetLocalesCollectionAsync(cts.Token));
    }

    [Fact]
    public async Task GivenCancelledToken_WhenGetContentTypeAsync_ThenThrowsOperationCanceledException()
    {
        // Arrange
        var contentTypeId = "test-content-type";
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            _sut.GetContentTypeAsync(contentTypeId, cts.Token));
    }

    [Fact]
    public async Task GivenCancelledToken_WhenGetContentTypesAsync_ThenThrowsOperationCanceledException()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            _sut.GetContentTypesAsync(cts.Token));
    }

    [Fact]
    public async Task GivenCancelledToken_WhenCreateOrUpdateContentTypeAsync_ThenThrowsOperationCanceledException()
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
    public async Task GivenCancelledToken_WhenActivateContentTypeAsync_ThenThrowsOperationCanceledException()
    {
        // Arrange
        var contentTypeId = "test-content-type";
        var version = 1;
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            _sut.ActivateContentTypeAsync(contentTypeId, version, cts.Token));
    }

    [Fact]
    public async Task GivenCancelledToken_WhenDeactivateContentTypeAsync_ThenThrowsOperationCanceledException()
    {
        // Arrange
        var contentTypeId = "test-content-type";
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            _sut.DeactivateContentTypeAsync(contentTypeId, cts.Token));
    }

    [Fact]
    public async Task GivenCancelledToken_WhenDeleteContentTypeAsync_ThenThrowsOperationCanceledException()
    {
        // Arrange
        var contentTypeId = "test-content-type";
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            _sut.DeleteContentTypeAsync(contentTypeId, cts.Token));
    }

    [Fact]
    public async Task GivenCancelledToken_WhenGetEntriesCollectionAsync_ThenThrowsOperationCanceledException()
    {
        // Arrange
        var queryString = "content_type=blogPost";
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            _sut.GetEntriesCollectionAsync(queryString, cts.Token));
    }

    [Fact]
    public async Task GivenCancelledToken_WhenCreateOrUpdateEntryAsync_ThenThrowsOperationCanceledException()
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
    public async Task GivenCancelledToken_WhenPublishEntryAsync_ThenThrowsOperationCanceledException()
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
    public async Task GivenCancelledToken_WhenUnpublishEntryAsync_ThenThrowsOperationCanceledException()
    {
        // Arrange
        var entryId = "test-entry";
        var version = 1;
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            _sut.UnpublishEntryAsync(entryId, version, cts.Token));
    }

    [Fact]
    public async Task GivenCancelledToken_WhenArchiveEntryAsync_ThenThrowsOperationCanceledException()
    {
        // Arrange
        var entryId = "test-entry";
        var version = 1;
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            _sut.ArchiveEntryAsync(entryId, version, cts.Token));
    }

    [Fact]
    public async Task GivenCancelledToken_WhenUnarchiveEntryAsync_ThenThrowsOperationCanceledException()
    {
        // Arrange
        var entryId = "test-entry";
        var version = 1;
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            _sut.UnarchiveEntryAsync(entryId, version, cts.Token));
    }

    [Fact]
    public async Task GivenCancelledToken_WhenDeleteEntryAsync_ThenThrowsOperationCanceledException()
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
    public async Task GivenClientThrowsException_WhenGetSpaceAsync_ThenPropagatesException()
    {
        // Arrange
        var spaceId = "test-space";
        var expectedException = new InvalidOperationException("Test exception");
        _clientMock.Setup(x => x.GetSpace(spaceId, It.IsAny<CancellationToken>()))
            .Throws(expectedException);

        // Act & Assert
        var actualException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _sut.GetSpaceAsync(spaceId, CancellationToken.None));
        
        Assert.Equal(expectedException.Message, actualException.Message);
    }

    [Fact]
    public async Task GivenClientThrowsException_WhenGetContentTypeAsync_ThenPropagatesException()
    {
        // Arrange
        var contentTypeId = "test-content-type";
        var expectedException = new InvalidOperationException("Test exception");
        _clientMock.Setup(x => x.GetContentType(contentTypeId, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Throws(expectedException);

        // Act & Assert
        var actualException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _sut.GetContentTypeAsync(contentTypeId, CancellationToken.None));
        
        Assert.Equal(expectedException.Message, actualException.Message);
    }

    [Fact]
    public async Task GivenClientThrowsException_WhenPublishEntryAsync_ThenPropagatesException()
    {
        // Arrange
        var entryId = "test-entry";
        var version = 1;
        var expectedException = new InvalidOperationException("Test exception");
        _clientMock.Setup(x => x.PublishEntry(entryId, version, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Throws(expectedException);

        // Act & Assert
        var actualException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _sut.PublishEntryAsync(entryId, version, CancellationToken.None));
        
        Assert.Equal(expectedException.Message, actualException.Message);
    }
}