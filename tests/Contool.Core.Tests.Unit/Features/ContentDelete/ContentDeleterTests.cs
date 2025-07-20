using Contentful.Core.Models;
using Contool.Core.Features.ContentDelete;
using Contool.Core.Infrastructure.Contentful.Services;
using Contool.Core.Infrastructure.Contentful.Utils;
using Contool.Core.Infrastructure.Utils.Models;
using Contool.Core.Infrastructure.Utils.Services;
using Contool.Core.Tests.Unit.Helpers;
using MockLite;

namespace Contool.Core.Tests.Unit.Features.ContentDelete;

public class ContentDeleterTests
{
    private readonly ContentDeleter _sut;
    
    private readonly Mock<IBatchProcessor> _batchProcessorMock = new();
    private readonly Mock<IProgressReporter> _progressReporterMock = new();
    private readonly Mock<IContentfulService> _contentfulServiceMock = new();

    public ContentDeleterTests()
    {
        _batchProcessorMock.SetupDefaults();
        _progressReporterMock.SetupDefaults();
        _contentfulServiceMock.SetupDefaults();
        
        _sut = new ContentDeleter(_batchProcessorMock.Object, _progressReporterMock.Object);
    }

    [Fact]
    public async Task GivenValidInput_WhenDeleting_ThenDeletesEntriesInBatches()
    {
        // Arrange
        SetupEntriesToRead();
        
        var input = CreateDeleterInput();
        
        // Act
        await _sut.DeleteAsync(input, CancellationToken.None);
        
        // Assert
        _batchProcessorMock.Verify(
            x => x.ProcessAsync(
                It.IsAny<IAsyncEnumerable<Entry<dynamic>>>(),
                50,
                It.IsAny<Func<IReadOnlyList<Entry<dynamic>>, CancellationToken, Task>>(),
                It.IsAny<Func<Entry<dynamic>, bool>>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GivenValidInput_WhenDeleting_ThenReportsProgressCorrectly()
    {
        // Arrange
        SetupEntriesToRead();
        
        var input = CreateDeleterInput();
        
        // Act
        await _sut.DeleteAsync(input, CancellationToken.None);
        
        // Assert
        _progressReporterMock.Verify(x => x.Start(Operation.Delete, It.IsAny<Func<int>>()), Times.Once);
        _progressReporterMock.Verify(x => x.Complete(), Times.Once);
    }

    [Fact]
    public async Task GivenValidInput_WhenDeleting_ThenCallsContentfulServiceDeleteEntries()
    {
        // Arrange
        const int entriesCount = 150;
        
        SetupEntriesToRead(entriesCount: entriesCount);
        
        var input = CreateDeleterInput();
        
        // Act
        await _sut.DeleteAsync(input, CancellationToken.None);
        
        // Assert
        _contentfulServiceMock.Verify(
            x => x.DeleteEntriesAsync(
                It.Is<IReadOnlyList<Entry<dynamic>>>(entries => entries.Count == entriesCount),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GivenBatchProcessorThrowsException_WhenDeleting_ThenBubblesException()
    {
        // Arrange
        var input = CreateDeleterInput();
        
        _batchProcessorMock.Setup(
            x => x.ProcessAsync(
                It.IsAny<IAsyncEnumerable<Entry<dynamic>>>(),
                It.IsAny<int>(),
                It.IsAny<Func<IReadOnlyList<Entry<dynamic>>, CancellationToken, Task>>(),
                It.IsAny<Func<Entry<dynamic>, bool>>(),
                It.IsAny<CancellationToken>()))
            .Throws(new InvalidOperationException("Batch processing failed"));
        
        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _sut.DeleteAsync(input, CancellationToken.None));
        
        Assert.Equal("Batch processing failed", exception.Message);
    }

    [Fact]
    public async Task GivenContentfulServiceThrowsException_WhenDeleting_ThenBubblesException()
    {
        // Arrange
        var input = CreateDeleterInput();
        
        _contentfulServiceMock
            .Setup(x => x.GetEntriesAsync(
                It.IsAny<string>(),
                It.IsAny<int?>(),
                It.IsAny<PagingMode>(),
                It.IsAny<CancellationToken>()))
            .Throws(new InvalidOperationException("Entry read failed"));
        
        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _sut.DeleteAsync(input, CancellationToken.None));
        
        Assert.Equal("Entry read failed", exception.Message);
    }

    private ContentDeleterInput CreateDeleterInput(
        string contentTypeId = "blogPost")
    {
        return new ContentDeleterInput
        {
            ContentTypeId = contentTypeId,
            ContentfulService = _contentfulServiceMock.Object,
            IncludeArchived = false
        };
    }

    private void SetupEntriesToRead(
        string contentTypeId =  "blogPost",
        int entriesCount = 150)
    {
        var entries = CreateBlogPostEntryAsyncEnumerable(entriesCount);
        
        _contentfulServiceMock
            .Setup(x => x.GetEntriesAsync(
               contentTypeId,
                null,
                PagingMode.SkipForward,
                It.IsAny<CancellationToken>()))
            .Returns(entries);
    }

    private static MockAsyncEnumerableWithTotal<Entry<dynamic>> CreateBlogPostEntryAsyncEnumerable(int count)
    {
        var entries = Enumerable.Range(1, count)
            .Select(i => EntryBuilder.CreateBlogPost(id: $"entry{i}"))
            .ToArray();
        
        return new MockAsyncEnumerableWithTotal<Entry<dynamic>>(entries);
    }
}