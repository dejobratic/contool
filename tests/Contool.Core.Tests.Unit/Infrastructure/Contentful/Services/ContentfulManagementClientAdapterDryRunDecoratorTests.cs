using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Contool.Core.Infrastructure.Contentful.Services;
using Contool.Core.Tests.Unit.Helpers;
using MockLite;

namespace Contool.Core.Tests.Unit.Infrastructure.Contentful.Services;

public class ContentfulManagementClientAdapterDryRunDecoratorTests
{
    private readonly ContentfulManagementClientAdapterDryRunDecorator _sut;
    
    private readonly Mock<IContentfulManagementClientAdapter> _innerMock;

    public ContentfulManagementClientAdapterDryRunDecoratorTests()
    {
        _innerMock = new Mock<IContentfulManagementClientAdapter>();
        _sut = new ContentfulManagementClientAdapterDryRunDecorator(_innerMock.Object);
    }

    [Fact]
    public async Task GivenValidSpaceId_WhenGetSpaceAsync_ThenCallsInnerAdapter()
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
    public async Task GivenValidEnvironmentId_WhenGetEnvironmentAsync_ThenCallsInnerAdapter()
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
    public async Task WhenGetCurrentUser_ThenCallsInnerAdapter()
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
    public async Task WhenGetLocalesCollectionAsync_ThenCallsInnerAdapter()
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
    public async Task GivenValidContentTypeId_WhenGetContentTypeAsync_ThenCallsInnerAdapter()
    {
        // Arrange
        var contentTypeId = "test-content-type";
        var expectedContentType = ContentTypeBuilder.Create()
            .WithId(contentTypeId)
            .WithName("Test Content Type")
            .Build();
        _innerMock.Setup(x => x.GetContentTypeAsync(contentTypeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedContentType);

        // Act
        var actual = await _sut.GetContentTypeAsync(contentTypeId, CancellationToken.None);

        // Assert
        _innerMock.Verify(x => x.GetContentTypeAsync(contentTypeId, It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(expectedContentType, actual);
    }

    [Fact]
    public async Task WhenGetContentTypesAsync_ThenCallsInnerAdapter()
    {
        // Arrange
        var expectedContentTypes = new List<ContentType>
        {
            ContentTypeBuilder.CreateBlogPost(),
            ContentTypeBuilder.CreateProduct()
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
    public async Task GivenValidContentType_WhenCreateOrUpdateContentTypeAsync_ThenReturnsSameContentTypeWithoutCallingInner()
    {
        // Arrange
        var contentType = ContentTypeBuilder.CreateBlogPost();

        // Act
        var actual = await _sut.CreateOrUpdateContentTypeAsync(contentType, CancellationToken.None);

        // Assert
        _innerMock.Verify(x => x.CreateOrUpdateContentTypeAsync(It.IsAny<ContentType>(), It.IsAny<CancellationToken>()), Times.Never);
        Assert.Equal(contentType, actual);
    }

    [Fact]
    public async Task GivenValidContentTypeIdAndVersion_WhenActivateContentTypeAsync_ThenReturnsMockContentTypeWithoutCallingInner()
    {
        // Arrange
        var contentTypeId = "test-content-type";
        var version = 1;

        // Act
        var actual = await _sut.ActivateContentTypeAsync(contentTypeId, version, CancellationToken.None);

        // Assert
        _innerMock.Verify(x => x.ActivateContentTypeAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        Assert.Equal(contentTypeId, actual.SystemProperties.Id);
        Assert.Equal(version, actual.SystemProperties.Version);
    }

    [Fact]
    public async Task GivenValidContentTypeId_WhenDeactivateContentTypeAsync_ThenCompletesWithoutCallingInner()
    {
        // Arrange
        var contentTypeId = "test-content-type";

        // Act
        await _sut.DeactivateContentTypeAsync(contentTypeId, CancellationToken.None);

        // Assert
        _innerMock.Verify(x => x.DeactivateContentTypeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GivenValidContentTypeId_WhenDeleteContentTypeAsync_ThenCompletesWithoutCallingInner()
    {
        // Arrange
        var contentTypeId = "test-content-type";

        // Act
        await _sut.DeleteContentTypeAsync(contentTypeId, CancellationToken.None);

        // Assert
        _innerMock.Verify(x => x.DeleteContentTypeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GivenValidQueryString_WhenGetEntriesCollectionAsync_ThenCallsInnerAdapter()
    {
        // Arrange
        var queryString = "content_type=blogPost&limit=10";
        var entries = new List<Entry<dynamic>>
        {
            EntryBuilder.CreateBlogPost("entry1"),
            EntryBuilder.CreateBlogPost("entry2")
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
    public async Task GivenValidEntry_WhenCreateOrUpdateEntryAsync_ThenReturnsSameEntryWithoutCallingInner()
    {
        // Arrange
        var entry = EntryBuilder.CreateBlogPost("entry1");
        var version = 1;

        // Act
        var actual = await _sut.CreateOrUpdateEntryAsync(entry, version, CancellationToken.None);

        // Assert
        _innerMock.Verify(x => x.CreateOrUpdateEntryAsync(It.IsAny<Entry<dynamic>>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        Assert.Equal(entry, actual);
    }

    [Fact]
    public async Task GivenValidEntryIdAndVersion_WhenPublishEntryAsync_ThenReturnsMockEntryWithoutCallingInner()
    {
        // Arrange
        var entryId = "test-entry";
        var version = 1;

        // Act
        var actual = await _sut.PublishEntryAsync(entryId, version, CancellationToken.None);

        // Assert
        _innerMock.Verify(x => x.PublishEntryAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        Assert.Equal(entryId, actual.SystemProperties.Id);
        Assert.Equal(version, actual.SystemProperties.Version);
    }

    [Fact]
    public async Task GivenValidEntryIdAndVersion_WhenUnpublishEntryAsync_ThenReturnsMockEntryWithoutCallingInner()
    {
        // Arrange
        var entryId = "test-entry";
        var version = 2;

        // Act
        var actual = await _sut.UnpublishEntryAsync(entryId, version, CancellationToken.None);

        // Assert
        _innerMock.Verify(x => x.UnpublishEntryAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        Assert.Equal(entryId, actual.SystemProperties.Id);
        Assert.Equal(version, actual.SystemProperties.Version);
    }

    [Fact]
    public async Task GivenValidEntryIdAndVersion_WhenArchiveEntryAsync_ThenReturnsMockEntryWithoutCallingInner()
    {
        // Arrange
        var entryId = "test-entry";
        var version = 1;

        // Act
        var actual = await _sut.ArchiveEntryAsync(entryId, version, CancellationToken.None);

        // Assert
        _innerMock.Verify(x => x.ArchiveEntryAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        Assert.Equal(entryId, actual.SystemProperties.Id);
        Assert.Equal(version, actual.SystemProperties.Version);
    }

    [Fact]
    public async Task GivenValidEntryIdAndVersion_WhenUnarchiveEntryAsync_ThenReturnsMockEntryWithoutCallingInner()
    {
        // Arrange
        var entryId = "test-entry";
        var version = 1;

        // Act
        var actual = await _sut.UnarchiveEntryAsync(entryId, version, CancellationToken.None);

        // Assert
        _innerMock.Verify(x => x.UnarchiveEntryAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        Assert.Equal(entryId, actual.SystemProperties.Id);
        Assert.Equal(version, actual.SystemProperties.Version);
    }

    [Fact]
    public async Task GivenValidEntryIdAndVersion_WhenDeleteEntryAsync_ThenCompletesWithoutCallingInner()
    {
        // Arrange
        var entryId = "test-entry";
        var version = 1;

        // Act
        await _sut.DeleteEntryAsync(entryId, version, CancellationToken.None);

        // Assert
        _innerMock.Verify(x => x.DeleteEntryAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GivenInnerAdapterThrowsException_WhenGetSpaceAsync_ThenPropagatesException()
    {
        // Arrange
        var spaceId = "test-space";
        var expectedException = new InvalidOperationException("Test exception");
        _innerMock.Setup(x => x.GetSpaceAsync(spaceId, It.IsAny<CancellationToken>()))
            .Throws(expectedException);

        // Act & Assert
        var actualException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _sut.GetSpaceAsync(spaceId, CancellationToken.None));
        
        Assert.Equal(expectedException.Message, actualException.Message);
    }

    [Fact]
    public async Task GivenInnerAdapterThrowsException_WhenGetContentTypeAsync_ThenPropagatesException()
    {
        // Arrange
        var contentTypeId = "test-content-type";
        var expectedException = new InvalidOperationException("Test exception");
        _innerMock.Setup(x => x.GetContentTypeAsync(contentTypeId, It.IsAny<CancellationToken>()))
            .Throws(expectedException);

        // Act & Assert
        var actualException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _sut.GetContentTypeAsync(contentTypeId, CancellationToken.None));
        
        Assert.Equal(expectedException.Message, actualException.Message);
    }

    [Fact]
    public async Task GivenInnerAdapterThrowsException_WhenGetEntriesCollectionAsync_ThenPropagatesException()
    {
        // Arrange
        var queryString = "content_type=blogPost";
        var expectedException = new InvalidOperationException("Test exception");
        _innerMock.Setup(x => x.GetEntriesCollectionAsync(queryString, It.IsAny<CancellationToken>()))
            .Throws(expectedException);

        // Act & Assert
        var actualException = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _sut.GetEntriesCollectionAsync(queryString, CancellationToken.None));
        
        Assert.Equal(expectedException.Message, actualException.Message);
    }
}