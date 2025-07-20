using Contentful.Core.Models;
using Contool.Core.Infrastructure.Contentful.Services;
using Contool.Core.Infrastructure.Utils.Models;
using Contool.Core.Tests.Unit.Helpers;
using MockLite;

namespace Contool.Core.Tests.Unit.Infrastructure.Contentful.Services;

public class ContentfulEntryOperationServiceTests
{
    private readonly ContentfulEntryOperationService _sut;
    
    private readonly Mock<IContentfulManagementClientAdapter> _clientMock = new Mock<IContentfulManagementClientAdapter>();

    public ContentfulEntryOperationServiceTests()
    {
        _sut = new ContentfulEntryOperationService(_clientMock.Object);
    }

    [Fact]
    public async Task GivenValidEntryNotArchivedNotPublished_WhenCreateOrUpdateEntryAsync_ThenCreatesOrUpdatesEntry()
    {
        // Arrange
        var entry = EntryBuilder.CreateBlogPost("entry1", "blogPost");
        var version = 1;
        var archived = false;
        var publish = false;
        
        var expectedEntry = EntryBuilder.CreateBlogPost("entry1", "blogPost");
        expectedEntry.SystemProperties.Version = version + 1;
        _clientMock.Setup(x => x.CreateOrUpdateEntryAsync(It.IsAny<Entry<dynamic>>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedEntry);

        // Act
        var actual = await _sut.CreateOrUpdateEntryAsync(entry, version, archived, publish, CancellationToken.None);

        // Assert
        Assert.True(actual.IsSuccess);
        Assert.Equal(entry.SystemProperties.Id, actual.EntityId);
        Assert.Equal(Operation.Upload, actual.Operation);
        _clientMock.Verify(x => x.CreateOrUpdateEntryAsync(entry, version, It.IsAny<CancellationToken>()), Times.Once);
        _clientMock.Verify(x => x.UnarchiveEntryAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        _clientMock.Verify(x => x.PublishEntryAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GivenValidEntryArchivedNotPublished_WhenCreateOrUpdateEntryAsync_ThenUnarchivesBeforeUpdate()
    {
        // Arrange
        var entry = EntryBuilder.CreateBlogPost("entry1", "blogPost");
        var version = 1;
        var archived = true;
        var publish = false;
        
        var unarchivedEntry = EntryBuilder.CreateBlogPost("entry1", "blogPost");
        unarchivedEntry.SystemProperties.Version = version + 1;
        
        var updatedEntry = EntryBuilder.CreateBlogPost("entry1", "blogPost");
        updatedEntry.SystemProperties.Version = version + 2;
        
        _clientMock.Setup(x => x.UnarchiveEntryAsync(entry.SystemProperties.Id, version, It.IsAny<CancellationToken>()))
            .ReturnsAsync(unarchivedEntry);
        _clientMock.Setup(x => x.CreateOrUpdateEntryAsync(It.IsAny<Entry<dynamic>>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(updatedEntry);

        // Act
        var actual = await _sut.CreateOrUpdateEntryAsync(entry, version, archived, publish, CancellationToken.None);

        // Assert
        Assert.True(actual.IsSuccess);
        Assert.Equal(entry.SystemProperties.Id, actual.EntityId);
        Assert.Equal(Operation.Upload, actual.Operation);
        _clientMock.Verify(x => x.UnarchiveEntryAsync(entry.SystemProperties.Id, version, It.IsAny<CancellationToken>()), Times.Once);
        _clientMock.Verify(x => x.CreateOrUpdateEntryAsync(entry, unarchivedEntry.SystemProperties.Version ?? 0, It.IsAny<CancellationToken>()), Times.Once);
        _clientMock.Verify(x => x.PublishEntryAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GivenValidEntryNotArchivedWithPublish_WhenCreateOrUpdateEntryAsync_ThenCreatesUpdatesAndPublishes()
    {
        // Arrange
        var entry = EntryBuilder.CreateBlogPost("entry1", "blogPost");
        var version = 1;
        var archived = false;
        var publish = true;
        
        var updatedEntry = EntryBuilder.CreateBlogPost("entry1", "blogPost");
        updatedEntry.SystemProperties.Version = version + 1;
        
        _clientMock.Setup(x => x.CreateOrUpdateEntryAsync(It.IsAny<Entry<dynamic>>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(updatedEntry);
        _clientMock.Setup(x => x.PublishEntryAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(updatedEntry);

        // Act
        var actual = await _sut.CreateOrUpdateEntryAsync(entry, version, archived, publish, CancellationToken.None);

        // Assert
        Assert.True(actual.IsSuccess);
        Assert.Equal(entry.SystemProperties.Id, actual.EntityId);
        Assert.Equal(Operation.Upload, actual.Operation);
        _clientMock.Verify(x => x.CreateOrUpdateEntryAsync(entry, version, It.IsAny<CancellationToken>()), Times.Once);
        _clientMock.Verify(x => x.PublishEntryAsync(entry.SystemProperties.Id, updatedEntry.SystemProperties.Version ?? 0, It.IsAny<CancellationToken>()), Times.Once);
        _clientMock.Verify(x => x.UnarchiveEntryAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GivenValidEntryArchivedWithPublish_WhenCreateOrUpdateEntryAsync_ThenUnarchivesUpdatesAndPublishes()
    {
        // Arrange
        var entry = EntryBuilder.CreateBlogPost("entry1", "blogPost");
        var version = 1;
        var archived = true;
        var publish = true;
        
        var unarchivedEntry = EntryBuilder.CreateBlogPost("entry1", "blogPost");
        unarchivedEntry.SystemProperties.Version = version + 1;
        var updatedEntry = EntryBuilder.CreateBlogPost("entry1", "blogPost");
        updatedEntry.SystemProperties.Version = version + 2;
        
        _clientMock.Setup(x => x.UnarchiveEntryAsync(entry.SystemProperties.Id, version, It.IsAny<CancellationToken>()))
            .ReturnsAsync(unarchivedEntry);
        _clientMock.Setup(x => x.CreateOrUpdateEntryAsync(It.IsAny<Entry<dynamic>>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(updatedEntry);
        _clientMock.Setup(x => x.PublishEntryAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(updatedEntry);

        // Act
        var actual = await _sut.CreateOrUpdateEntryAsync(entry, version, archived, publish, CancellationToken.None);

        // Assert
        Assert.True(actual.IsSuccess);
        Assert.Equal(entry.SystemProperties.Id, actual.EntityId);
        Assert.Equal(Operation.Upload, actual.Operation);
        _clientMock.Verify(x => x.UnarchiveEntryAsync(entry.SystemProperties.Id, version, It.IsAny<CancellationToken>()), Times.Once);
        _clientMock.Verify(x => x.CreateOrUpdateEntryAsync(entry, unarchivedEntry.SystemProperties.Version ?? 0, It.IsAny<CancellationToken>()), Times.Once);
        _clientMock.Verify(x => x.PublishEntryAsync(entry.SystemProperties.Id, updatedEntry.SystemProperties.Version ?? 0, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GivenClientThrowsException_WhenCreateOrUpdateEntryAsync_ThenReturnsFailureResult()
    {
        // Arrange
        var entry = EntryBuilder.CreateBlogPost("entry1", "blogPost");
        var version = 1;
        var archived = false;
        var publish = false;
        
        var expectedException = new InvalidOperationException("Test exception");
        _clientMock.Setup(x => x.CreateOrUpdateEntryAsync(It.IsAny<Entry<dynamic>>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Throws(expectedException);

        // Act
        var actual = await _sut.CreateOrUpdateEntryAsync(entry, version, archived, publish, CancellationToken.None);

        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Equal(entry.SystemProperties.Id, actual.EntityId);
        Assert.Equal(Operation.Upload, actual.Operation);
        Assert.Equal(expectedException, actual.Exception);
    }

    [Fact]
    public async Task GivenDraftEntry_WhenPublishEntryAsync_ThenPublishesEntry()
    {
        // Arrange
        var entry = EntryBuilder.CreateBlogPost("entry1", "blogPost");
        // Make it a draft (not published)
        entry.SystemProperties.PublishedAt = null;
        entry.SystemProperties.PublishedVersion = null;
        
        var publishedEntry = EntryBuilder.CreateBlogPost("entry1", "blogPost");
        publishedEntry.SystemProperties.PublishedAt = DateTime.UtcNow;
        publishedEntry.SystemProperties.PublishedVersion = entry.SystemProperties.Version;
        
        _clientMock.Setup(x => x.PublishEntryAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(publishedEntry);

        // Act
        var actual = await _sut.PublishEntryAsync(entry, CancellationToken.None);

        // Assert
        Assert.True(actual.IsSuccess);
        Assert.Equal(entry.SystemProperties.Id, actual.EntityId);
        Assert.Equal(Operation.Publish, actual.Operation);
        _clientMock.Verify(x => x.PublishEntryAsync(entry.SystemProperties.Id, entry.SystemProperties.Version ?? 0, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GivenPublishedEntry_WhenPublishEntryAsync_ThenReturnsSuccessWithoutCalling()
    {
        // Arrange
        var entry = EntryBuilder.CreateBlogPost("entry1", "blogPost");
        // Make it published
        entry.SystemProperties.PublishedAt = DateTime.UtcNow;
        entry.SystemProperties.PublishedVersion = entry.SystemProperties.Version - 1;

        // Act
        var actual = await _sut.PublishEntryAsync(entry, CancellationToken.None);

        // Assert
        Assert.True(actual.IsSuccess);
        Assert.Equal(entry.SystemProperties.Id, actual.EntityId);
        Assert.Equal(Operation.Publish, actual.Operation);
        _clientMock.Verify(x => x.PublishEntryAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GivenArchivedEntry_WhenPublishEntryAsync_ThenReturnsSuccessWithoutCalling()
    {
        // Arrange
        var entry = EntryBuilder.CreateBlogPost("entry1", "blogPost");
        // Make it archived
        entry.SystemProperties.ArchivedAt = DateTime.UtcNow;

        // Act
        var actual = await _sut.PublishEntryAsync(entry, CancellationToken.None);

        // Assert
        Assert.True(actual.IsSuccess);
        Assert.Equal(entry.SystemProperties.Id, actual.EntityId);
        Assert.Equal(Operation.Publish, actual.Operation);
        _clientMock.Verify(x => x.PublishEntryAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GivenClientThrowsException_WhenPublishEntryAsync_ThenReturnsFailureResult()
    {
        // Arrange
        var entry = EntryBuilder.CreateBlogPost("entry1", "blogPost");
        // Make it a draft (not published)
        entry.SystemProperties.PublishedAt = null;
        entry.SystemProperties.PublishedVersion = null;
        
        var expectedException = new InvalidOperationException("Test exception");
        _clientMock.Setup(x => x.PublishEntryAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Throws(expectedException);

        // Act
        var actual = await _sut.PublishEntryAsync(entry, CancellationToken.None);

        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Equal(entry.SystemProperties.Id, actual.EntityId);
        Assert.Equal(Operation.Publish, actual.Operation);
        Assert.Equal(expectedException, actual.Exception);
    }

    [Fact]
    public async Task GivenPublishedEntry_WhenUnpublishEntryAsync_ThenUnpublishesEntry()
    {
        // Arrange
        var entry = EntryBuilder.CreateBlogPost("entry1", "blogPost");
        // Make it published
        entry.SystemProperties.PublishedAt = DateTime.UtcNow;
        entry.SystemProperties.PublishedVersion = entry.SystemProperties.Version - 1;
        
        var unpublishedEntry = EntryBuilder.CreateBlogPost("entry1", "blogPost");
        unpublishedEntry.SystemProperties.PublishedAt = null;
        unpublishedEntry.SystemProperties.PublishedVersion = null;
        
        _clientMock.Setup(x => x.UnpublishEntryAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(unpublishedEntry);

        // Act
        var actual = await _sut.UnpublishEntryAsync(entry, CancellationToken.None);

        // Assert
        Assert.True(actual.IsSuccess);
        Assert.Equal(entry.SystemProperties.Id, actual.EntityId);
        Assert.Equal(Operation.Unpublish, actual.Operation);
        _clientMock.Verify(x => x.UnpublishEntryAsync(entry.SystemProperties.Id, entry.SystemProperties.Version ?? 0, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GivenUnpublishedEntry_WhenUnpublishEntryAsync_ThenReturnsSuccessWithoutCalling()
    {
        // Arrange
        var entry = EntryBuilder.CreateBlogPost("entry1", "blogPost");
        // Make it unpublished (draft)
        entry.SystemProperties.PublishedAt = null;
        entry.SystemProperties.PublishedVersion = null;

        // Act
        var actual = await _sut.UnpublishEntryAsync(entry, CancellationToken.None);

        // Assert
        Assert.True(actual.IsSuccess);
        Assert.Equal(entry.SystemProperties.Id, actual.EntityId);
        Assert.Equal(Operation.Unpublish, actual.Operation);
        _clientMock.Verify(x => x.UnpublishEntryAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GivenClientThrowsException_WhenUnpublishEntryAsync_ThenReturnsFailureResult()
    {
        // Arrange
        var entry = EntryBuilder.CreateBlogPost("entry1", "blogPost");
        // Make it published
        entry.SystemProperties.PublishedAt = DateTime.UtcNow;
        entry.SystemProperties.PublishedVersion = entry.SystemProperties.Version - 1;
        
        var expectedException = new InvalidOperationException("Test exception");
        _clientMock.Setup(x => x.UnpublishEntryAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Throws(expectedException);

        // Act
        var actual = await _sut.UnpublishEntryAsync(entry, CancellationToken.None);

        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Equal(entry.SystemProperties.Id, actual.EntityId);
        Assert.Equal(Operation.Unpublish, actual.Operation);
        Assert.Equal(expectedException, actual.Exception);
    }

    [Fact]
    public async Task GivenDraftEntry_WhenDeleteEntryAsync_ThenDeletesEntry()
    {
        // Arrange
        var entry = EntryBuilder.CreateBlogPost("entry1", "blogPost");
        // Make it a draft (not published, not archived)
        entry.SystemProperties.PublishedAt = null;
        entry.SystemProperties.PublishedVersion = null;
        entry.SystemProperties.ArchivedAt = null;

        // Act
        var actual = await _sut.DeleteEntryAsync(entry, CancellationToken.None);

        // Assert
        Assert.True(actual.IsSuccess);
        Assert.Equal(entry.SystemProperties.Id, actual.EntityId);
        Assert.Equal(Operation.Delete, actual.Operation);
        _clientMock.Verify(x => x.DeleteEntryAsync(entry.SystemProperties.Id, entry.SystemProperties.Version ?? 0, It.IsAny<CancellationToken>()), Times.Once);
        _clientMock.Verify(x => x.UnpublishEntryAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        _clientMock.Verify(x => x.UnarchiveEntryAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GivenPublishedEntry_WhenDeleteEntryAsync_ThenUnpublishesAndDeletes()
    {
        // Arrange
        var entry = EntryBuilder.CreateBlogPost("entry1", "blogPost");
        // Make it published
        entry.SystemProperties.PublishedAt = DateTime.UtcNow;
        entry.SystemProperties.PublishedVersion = entry.SystemProperties.Version - 1;
        
        var unpublishedEntry = EntryBuilder.CreateBlogPost("entry1", "blogPost");
        unpublishedEntry.SystemProperties.PublishedAt = null;
        unpublishedEntry.SystemProperties.PublishedVersion = null;
        unpublishedEntry.SystemProperties.Version = entry.SystemProperties.Version + 1;
        
        _clientMock.Setup(x => x.UnpublishEntryAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(unpublishedEntry);
        _clientMock.Setup(x => x.DeleteEntryAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var actual = await _sut.DeleteEntryAsync(entry, CancellationToken.None);

        // Assert
        Assert.True(actual.IsSuccess);
        Assert.Equal(entry.SystemProperties.Id, actual.EntityId);
        Assert.Equal(Operation.Delete, actual.Operation);
        _clientMock.Verify(x => x.UnpublishEntryAsync(entry.SystemProperties.Id, entry.SystemProperties.Version ?? 0, It.IsAny<CancellationToken>()), Times.Once);
        _clientMock.Verify(x => x.DeleteEntryAsync(entry.SystemProperties.Id, unpublishedEntry.SystemProperties.Version ?? 0, It.IsAny<CancellationToken>()), Times.Once);
        _clientMock.Verify(x => x.UnarchiveEntryAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GivenArchivedEntry_WhenDeleteEntryAsync_ThenUnarchivesAndDeletes()
    {
        // Arrange
        var entry = EntryBuilder.CreateBlogPost("entry1", "blogPost");
        // Make it archived
        entry.SystemProperties.ArchivedAt = DateTime.UtcNow;
        entry.SystemProperties.PublishedAt = null;
        entry.SystemProperties.PublishedVersion = null;
        
        var unarchivedEntry = EntryBuilder.CreateBlogPost("entry1", "blogPost");
        unarchivedEntry.SystemProperties.ArchivedAt = null;
        unarchivedEntry.SystemProperties.Version = entry.SystemProperties.Version + 1;
        
        _clientMock.Setup(x => x.UnarchiveEntryAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(unarchivedEntry);
        _clientMock.Setup(x => x.DeleteEntryAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var actual = await _sut.DeleteEntryAsync(entry, CancellationToken.None);

        // Assert
        Assert.True(actual.IsSuccess);
        Assert.Equal(entry.SystemProperties.Id, actual.EntityId);
        Assert.Equal(Operation.Delete, actual.Operation);
        _clientMock.Verify(x => x.UnarchiveEntryAsync(entry.SystemProperties.Id, entry.SystemProperties.Version ?? 0, It.IsAny<CancellationToken>()), Times.Once);
        _clientMock.Verify(x => x.DeleteEntryAsync(entry.SystemProperties.Id, unarchivedEntry.SystemProperties.Version ?? 0, It.IsAny<CancellationToken>()), Times.Once);
        _clientMock.Verify(x => x.UnpublishEntryAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GivenClientThrowsException_WhenDeleteEntryAsync_ThenReturnsFailureResult()
    {
        // Arrange
        var entry = EntryBuilder.CreateBlogPost("entry1", "blogPost");
        // Make it a draft (not published, not archived)
        entry.SystemProperties.PublishedAt = null;
        entry.SystemProperties.PublishedVersion = null;
        entry.SystemProperties.ArchivedAt = null;
        
        var expectedException = new InvalidOperationException("Test exception");
        _clientMock.Setup(x => x.DeleteEntryAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Throws(expectedException);

        // Act
        var actual = await _sut.DeleteEntryAsync(entry, CancellationToken.None);

        // Assert
        Assert.False(actual.IsSuccess);
        Assert.Equal(entry.SystemProperties.Id, actual.EntityId);
        Assert.Equal(Operation.Delete, actual.Operation);
        Assert.Equal(expectedException, actual.Exception);
    }

    [Fact]
    public async Task GivenCancelledToken_WhenUnpublishEntryAsync_ThenRespectsCancellation()
    {
        // Arrange
        var entry = EntryBuilder.CreateBlogPost("entry1", "blogPost");
        // Make it published
        entry.SystemProperties.PublishedAt = DateTime.UtcNow;
        entry.SystemProperties.PublishedVersion = entry.SystemProperties.Version - 1;
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        var actual = await _sut.UnpublishEntryAsync(entry, cts.Token);

        // Assert - Should handle cancellation gracefully and return failure
        Assert.False(actual.IsSuccess);
        Assert.Equal(entry.SystemProperties.Id, actual.EntityId);
        Assert.Equal(Operation.Unpublish, actual.Operation);
        Assert.IsType<OperationCanceledException>(actual.Exception);
    }

    [Fact]
    public async Task GivenCancelledToken_WhenDeleteEntryAsync_ThenRespectsCancellation()
    {
        // Arrange
        var entry = EntryBuilder.CreateBlogPost("entry1", "blogPost");
        // Make it a draft (not published, not archived)
        entry.SystemProperties.PublishedAt = null;
        entry.SystemProperties.PublishedVersion = null;
        entry.SystemProperties.ArchivedAt = null;
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        var actual = await _sut.DeleteEntryAsync(entry, cts.Token);

        // Assert - Should handle cancellation gracefully and return failure
        Assert.False(actual.IsSuccess);
        Assert.Equal(entry.SystemProperties.Id, actual.EntityId);
        Assert.Equal(Operation.Delete, actual.Operation);
        Assert.IsType<OperationCanceledException>(actual.Exception);
    }
}