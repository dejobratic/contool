using Contentful.Core.Models;
using Contool.Core.Infrastructure.Contentful.Services;
using Contool.Core.Tests.Unit.Helpers;
using MockLite;

namespace Contool.Core.Tests.Unit.Infrastructure.Contentful.Services;

public class ContentfulServiceTests
{
    private readonly ContentfulService _sut;
    
    private readonly Mock<IContentfulManagementClientAdapter> _clientMock = new();
    private readonly Mock<IContentfulEntryOperationService> _entryOperationServiceMock = new();
    private readonly Mock<IContentfulEntryBulkOperationService> _entryBulkOperationServiceMock = new();

    public ContentfulServiceTests()
    {
        _sut = new ContentfulService(
            _clientMock.Object,
            _entryOperationServiceMock.Object,
            _entryBulkOperationServiceMock.Object);
    }

    [Fact]
    public async Task GivenValidRequest_WhenGetLocalesAsync_ThenReturnsLocales()
    {
        // Arrange
        var expectedLocales = LocaleBuilder.CreateMultiple();
        
        _clientMock
            .Setup(x => x.GetLocalesCollectionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedLocales);

        // Act
        var actual = await _sut.GetLocalesAsync(CancellationToken.None);

        // Assert
        _clientMock.Verify(x => x.GetLocalesCollectionAsync(CancellationToken.None), Times.Once);
        Assert.Equal(expectedLocales, actual);
    }

    [Fact]
    public async Task GivenValidContentTypeId_WhenGetContentTypeAsync_ThenReturnsContentType()
    {
        // Arrange
        var contentTypeId = "test-content-type";
        var expectedContentType = ContentTypeBuilder.CreateBlogPost();
        _clientMock.Setup(x => x.GetContentTypeAsync(contentTypeId, CancellationToken.None))
            .ReturnsAsync(expectedContentType);

        // Act
        var actual = await _sut.GetContentTypeAsync(contentTypeId, CancellationToken.None);

        // Assert
        _clientMock.Verify(x => x.GetContentTypeAsync(contentTypeId, CancellationToken.None), Times.Once);
        Assert.Equal(expectedContentType, actual);
    }

    [Fact]
    public async Task GivenValidRequest_WhenGetContentTypesAsync_ThenReturnsContentTypes()
    {
        // Arrange
        var expectedContentTypes = new List<ContentType>
        {
            ContentTypeBuilder.CreateBlogPost(),
            ContentTypeBuilder.CreateProduct()
        };
        _clientMock.Setup(x => x.GetContentTypesAsync(CancellationToken.None))
            .ReturnsAsync(expectedContentTypes);

        // Act
        var actual = await _sut.GetContentTypesAsync(CancellationToken.None);

        // Assert
        _clientMock.Verify(x => x.GetContentTypesAsync(CancellationToken.None), Times.Once);
        Assert.Equal(expectedContentTypes, actual);
    }

    [Fact]
    public async Task GivenValidContentTypeId_WhenDeleteContentTypeAsync_ThenDeactivatesAndDeletesContentType()
    {
        // Arrange
        var contentTypeId = "content-type-to-delete";

        // Act
        await _sut.DeleteContentTypeAsync(contentTypeId, CancellationToken.None);

        // Assert
        _clientMock.Verify(x => x.DeactivateContentTypeAsync(contentTypeId, CancellationToken.None), Times.Once);
        _clientMock.Verify(x => x.DeleteContentTypeAsync(contentTypeId, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task GivenValidContentTypeId_WhenGetEntriesAsync_ThenReturnsAsyncEnumerable()
    {
        // Arrange
        var contentTypeId = "test-content-type";
        var entries = new List<Entry<dynamic>>
        {
            EntryBuilder.CreateBlogPost("entry1", contentTypeId),
            EntryBuilder.CreateBlogPost("entry2", contentTypeId)
        };
        
        var entriesCollection = new ContentfulCollection<Entry<dynamic>>
        {
            Items = entries,
            Total = entries.Count
        };
        
        _clientMock.Setup(x => x.GetEntriesCollectionAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entriesCollection);

        // Act
        var actual = _sut.GetEntriesAsync(contentTypeId);

        // Assert
        Assert.NotNull(actual);
        
        // Test enumeration
        var enumeratedEntries = new List<Entry<dynamic>>();
        await foreach (var entry in actual)
        {
            enumeratedEntries.Add(entry);
        }
        
        Assert.Equal(entries.Count, enumeratedEntries.Count);
    }

    [Fact]
    public async Task GivenValidEntries_WhenCreateOrUpdateEntriesAsync_ThenProcessesAllEntries()
    {
        // Arrange
        var entries = new List<Entry<dynamic>>
        {
            EntryBuilder.CreateBlogPost("entry1", "blogPost"),
            EntryBuilder.CreateBlogPost("entry2", "blogPost")
        };
        
        // Mock the client calls that ContentfulService depends on
        _clientMock.Setup(x => x.GetEntriesCollectionAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ContentfulCollection<Entry<dynamic>>
            {
                Items = entries, // Return the entries themselves for lookup
                Total = entries.Count
            });

        // Act
        await _sut.CreateOrUpdateEntriesAsync(entries, publish: false, CancellationToken.None);

        // Assert
        _clientMock.Verify(x => x.GetEntriesCollectionAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        _entryOperationServiceMock.Verify(x => x.CreateOrUpdateEntryAsync(
            It.IsAny<Entry<dynamic>>(),
            It.IsAny<int>(),
            It.IsAny<CancellationToken>()), Times.Exactly(entries.Count));
    }

    [Fact]
    public async Task GivenValidEntries_WhenPublishEntriesAsync_ThenPublishesAllEntries()
    {
        // Arrange
        var entries = new List<Entry<dynamic>>
        {
            EntryBuilder.CreateBlogPost("entry1", "blogPost"),
            EntryBuilder.CreateBlogPost("entry2", "blogPost")
        };
        
        // Make entries unpublished (draft state) so they can be published
        foreach (var entry in entries)
        {
            entry.SystemProperties.PublishedAt = null;
            entry.SystemProperties.PublishedVersion = null;
            entry.SystemProperties.Version = 1; // Ensure version is set
            entry.SystemProperties.ArchivedAt = null; // Ensure not archived
        }

        // Act
        await _sut.PublishEntriesAsync(entries, CancellationToken.None);

        // Assert
        _entryBulkOperationServiceMock.Verify(x => x.PublishEntriesAsync(It.IsAny<IReadOnlyList<Entry<dynamic>>>(), It.IsAny<CancellationToken>()), Times.Once);
        _entryOperationServiceMock.Verify(x => x.PublishEntryAsync(It.IsAny<Entry<dynamic>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GivenValidEntries_WhenUnpublishEntriesAsync_ThenUnpublishesAllEntries()
    {
        // Arrange
        var entries = new List<Entry<dynamic>>
        {
            EntryBuilder.CreateBlogPost("entry1", "blogPost"),
            EntryBuilder.CreateBlogPost("entry2", "blogPost")
        };
        
        // Make entries published so they can be unpublished
        foreach (var entry in entries)
        {
            entry.SystemProperties.PublishedAt = DateTime.UtcNow;
            entry.SystemProperties.PublishedVersion = entry.SystemProperties.Version - 1;
        }

        // Act
        await _sut.UnpublishEntriesAsync(entries, CancellationToken.None);

        // Assert
        _entryBulkOperationServiceMock.Verify(x => x.UnpublishEntriesAsync(It.IsAny<IReadOnlyList<Entry<dynamic>>>(), It.IsAny<CancellationToken>()), Times.Once);
        _entryOperationServiceMock.Verify(x => x.UnpublishEntryAsync(It.IsAny<Entry<dynamic>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GivenValidEntries_WhenDeleteEntriesAsync_ThenDeletesAllEntries()
    {
        // Arrange
        var entries = new List<Entry<dynamic>>
        {
            EntryBuilder.CreateBlogPost("entry1", "blogPost"),
            EntryBuilder.CreateBlogPost("entry2", "blogPost")
        };
        
        // Make some entries published so bulk unpublish is called
        entries[0].SystemProperties.PublishedAt = DateTime.UtcNow;
        entries[0].SystemProperties.PublishedVersion = entries[0].SystemProperties.Version - 1;

        // Act
        await _sut.DeleteEntriesAsync(entries, CancellationToken.None);

        // Assert
        _entryBulkOperationServiceMock.Verify(x => x.UnpublishEntriesAsync(It.IsAny<IReadOnlyList<Entry<dynamic>>>(), It.IsAny<CancellationToken>()), Times.Once);
        _entryOperationServiceMock.Verify(x => x.DeleteEntryAsync(It.IsAny<Entry<dynamic>>(), It.IsAny<CancellationToken>()), Times.Exactly(entries.Count));
    }

    [Fact]
    public async Task GivenEmptyEntriesCollection_WhenCreateOrUpdateEntriesAsync_ThenCompletesWithoutError()
    {
        // Arrange
        var entries = new List<Entry<dynamic>>();
        
        // Mock the client calls
        _clientMock.Setup(x => x.GetEntriesCollectionAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ContentfulCollection<Entry<dynamic>>
            {
                Items = new List<Entry<dynamic>>(),
                Total = 0
            });

        // Act
        await _sut.CreateOrUpdateEntriesAsync(entries, publish: false, CancellationToken.None);

        // Assert
        _entryOperationServiceMock.Verify(x => x.CreateOrUpdateEntryAsync(
            It.IsAny<Entry<dynamic>>(),
            It.IsAny<int>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }
}