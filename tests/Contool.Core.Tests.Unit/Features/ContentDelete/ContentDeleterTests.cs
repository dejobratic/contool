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
    public async Task GivenValidInput_WhenDeleteAsync_ThenDeletesEntriesInBatches()
    {
        // Arrange
        var input = CreateDeleterInput();
        
        // Act
        await _sut.DeleteAsync(input, CancellationToken.None);
        
        // Assert
        _batchProcessorMock.Verify(x => x.ProcessAsync(
            It.IsAny<IAsyncEnumerable<Entry<dynamic>>>(),
            50,
            It.IsAny<Func<IReadOnlyList<Entry<dynamic>>, CancellationToken, Task>>(),
            It.IsAny<Func<Entry<dynamic>, bool>>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GivenValidInput_WhenDeleteAsync_ThenReportsProgressCorrectly()
    {
        // Arrange
        var input = CreateDeleterInput();
        
        // Act
        await _sut.DeleteAsync(input, CancellationToken.None);
        
        // Assert
        _progressReporterMock.Verify(x => x.Start(Operation.Delete, It.IsAny<Func<int>>()), Times.Once);
        _progressReporterMock.Verify(x => x.Complete(), Times.Once);
    }

    [Fact]
    public async Task GivenValidInput_WhenDeleteAsync_ThenCallsContentfulServiceDeleteEntries()
    {
        // Arrange
        var input = CreateDeleterInput();
        
        // Act
        await _sut.DeleteAsync(input, CancellationToken.None);
        
        // Assert
        _batchProcessorMock.Verify(x => x.ProcessAsync(
            It.IsAny<IAsyncEnumerable<Entry<dynamic>>>(),
            It.IsAny<int>(),
            It.IsAny<Func<IReadOnlyList<Entry<dynamic>>, CancellationToken, Task>>(),
            It.IsAny<Func<Entry<dynamic>, bool>>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GivenCancellationToken_WhenDeleteAsync_ThenRespectsCancellation()
    {
        // Arrange
        var input = CreateDeleterInput();
        
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        
        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            _sut.DeleteAsync(input, cts.Token));
    }

    [Fact]
    public async Task GivenBatchProcessorThrowsException_WhenDeleteAsync_ThenBubblesException()
    {
        // Arrange
        var input = CreateDeleterInput();
        _batchProcessorMock.Setup(x => x.ProcessAsync(
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
    public async Task GivenEmptyEntries_WhenDeleteAsync_ThenHandlesEmptyCollection()
    {
        // Arrange
        var input = CreateDeleterInputWithEmptyEntries();
        
        // Act
        await _sut.DeleteAsync(input, CancellationToken.None);
        
        // Assert
        _batchProcessorMock.Verify(x => x.ProcessAsync(
            It.IsAny<IAsyncEnumerable<Entry<dynamic>>>(),
            It.IsAny<int>(),
            It.IsAny<Func<IReadOnlyList<Entry<dynamic>>, CancellationToken, Task>>(),
            It.IsAny<Func<Entry<dynamic>, bool>>(),
            It.IsAny<CancellationToken>()), Times.Once);
        _progressReporterMock.Verify(x => x.Start(It.IsAny<Operation>(), It.IsAny<Func<int>>()), Times.Once);
        _progressReporterMock.Verify(x => x.Complete(), Times.Once);
    }

    [Fact]
    public async Task GivenLargeNumberOfEntries_WhenDeleteAsync_ThenProcessesInBatchesOfFifty()
    {
        // Arrange
        var input = CreateDeleterInputWithLargeDataset(300);
        
        // Act
        await _sut.DeleteAsync(input, CancellationToken.None);
        
        // Assert
        _batchProcessorMock.Verify(x => x.ProcessAsync(
            It.IsAny<IAsyncEnumerable<Entry<dynamic>>>(),
            50,
            It.IsAny<Func<IReadOnlyList<Entry<dynamic>>, CancellationToken, Task>>(),
            It.IsAny<Func<Entry<dynamic>, bool>>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GivenContentfulServiceThrowsException_WhenDeleteAsync_ThenBubblesException()
    {
        // Arrange
        var input = CreateDeleterInputWithFailingService();
        
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _sut.DeleteAsync(input, CancellationToken.None));
    }

    [Fact]
    public async Task GivenIncludeArchivedFalse_WhenDeleteAsync_ThenFiltersOutArchivedEntries()
    {
        // Arrange
        var input = CreateDeleterInputWithArchivedEntries(includeArchived: false);
        
        // Act
        await _sut.DeleteAsync(input, CancellationToken.None);
        
        // Assert
        _batchProcessorMock.Verify(x => x.ProcessAsync(
            It.IsAny<IAsyncEnumerable<Entry<dynamic>>>(),
            It.IsAny<int>(),
            It.IsAny<Func<IReadOnlyList<Entry<dynamic>>, CancellationToken, Task>>(),
            It.IsAny<Func<Entry<dynamic>, bool>>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GivenIncludeArchivedTrue_WhenDeleteAsync_ThenIncludesArchivedEntries()
    {
        // Arrange
        var input = CreateDeleterInputWithArchivedEntries(includeArchived: true);
        
        // Act
        await _sut.DeleteAsync(input, CancellationToken.None);
        
        // Assert
        _batchProcessorMock.Verify(x => x.ProcessAsync(
            It.IsAny<IAsyncEnumerable<Entry<dynamic>>>(),
            It.IsAny<int>(),
            It.IsAny<Func<IReadOnlyList<Entry<dynamic>>, CancellationToken, Task>>(),
            It.IsAny<Func<Entry<dynamic>, bool>>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GivenMixedArchivedAndNonArchivedEntries_WhenDeleteAsyncWithIncludeArchivedFalse_ThenProcessesOnlyNonArchivedEntries()
    {
        // Arrange
        var input = CreateDeleterInputWithMixedArchivedEntries(includeArchived: false);
        
        // Act
        await _sut.DeleteAsync(input, CancellationToken.None);
        
        // Assert
        _batchProcessorMock.Verify(x => x.ProcessAsync(
            It.IsAny<IAsyncEnumerable<Entry<dynamic>>>(),
            It.IsAny<int>(),
            It.IsAny<Func<IReadOnlyList<Entry<dynamic>>, CancellationToken, Task>>(),
            It.IsAny<Func<Entry<dynamic>, bool>>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    private ContentDeleterInput CreateDeleterInput()
    {
        var testEntries = CreateTestEntries();
        
        _contentfulServiceMock
            .Setup(x => x.GetEntriesAsync("test-content-type", null, PagingMode.SkipForward, It.IsAny<CancellationToken>()))
            .Returns(testEntries);
        
        return new ContentDeleterInput
        {
            ContentTypeId = "test-content-type",
            ContentfulService = _contentfulServiceMock.Object,
            IncludeArchived = false
        };
    }

    private ContentDeleterInput CreateDeleterInputWithEmptyEntries()
    {
        var emptyEntries = new MockAsyncEnumerableWithTotal<Entry<dynamic>>([]);
        
        _contentfulServiceMock
            .Setup(x => x.GetEntriesAsync("test-content-type", null, PagingMode.SkipForward, It.IsAny<CancellationToken>()))
            .Returns(emptyEntries);
        
        return new ContentDeleterInput
        {
            ContentTypeId = "test-content-type",
            ContentfulService = _contentfulServiceMock.Object,
            IncludeArchived = false
        };
    }

    private ContentDeleterInput CreateDeleterInputWithLargeDataset(int count)
    {
        var entries = Enumerable.Range(1, count)
            .Select(i => EntryBuilder.CreateBlogPost($"entry{i}"))
            .ToArray();
        
        var testEntries = new MockAsyncEnumerableWithTotal<Entry<dynamic>>(entries);
        
        _contentfulServiceMock
            .Setup(x => x.GetEntriesAsync("test-content-type", null, PagingMode.SkipForward, It.IsAny<CancellationToken>()))
            .Returns(testEntries);
        
        return new ContentDeleterInput
        {
            ContentTypeId = "test-content-type",
            ContentfulService = _contentfulServiceMock.Object,
            IncludeArchived = false
        };
    }

    private ContentDeleterInput CreateDeleterInputWithFailingService()
    {
        _contentfulServiceMock
            .Setup(x => x.GetEntriesAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<PagingMode>(), It.IsAny<CancellationToken>()))
            .Throws(new InvalidOperationException("Service failure"));
        
        return new ContentDeleterInput
        {
            ContentTypeId = "test-content-type",
            ContentfulService = _contentfulServiceMock.Object,
            IncludeArchived = false
        };
    }

    private ContentDeleterInput CreateDeleterInputWithArchivedEntries(bool includeArchived)
    {
        var testEntries = new MockAsyncEnumerableWithTotal<Entry<dynamic>>(
        [
            EntryBuilder.CreateArchivedBlogPost("archived1"),
            EntryBuilder.CreateArchivedBlogPost("archived2"),
            EntryBuilder.CreateArchivedProduct("archived3"),
        ]);
        
        _contentfulServiceMock
            .Setup(x => x.GetEntriesAsync("test-content-type", null, PagingMode.SkipForward, It.IsAny<CancellationToken>()))
            .Returns(testEntries);
        
        return new ContentDeleterInput
        {
            ContentTypeId = "test-content-type",
            ContentfulService = _contentfulServiceMock.Object,
            IncludeArchived = includeArchived,
        };
    }

    private ContentDeleterInput CreateDeleterInputWithMixedArchivedEntries(bool includeArchived)
    {
        var testEntries = new MockAsyncEnumerableWithTotal<Entry<dynamic>>(
        [
            EntryBuilder.CreateBlogPost("entry1"),
            EntryBuilder.CreateArchivedBlogPost("archived1"),
            EntryBuilder.CreateProduct("entry2"),
            EntryBuilder.CreateArchivedProduct("archived2"),
        ]);
        
        _contentfulServiceMock
            .Setup(x => x.GetEntriesAsync("test-content-type", null, PagingMode.SkipForward, It.IsAny<CancellationToken>()))
            .Returns(testEntries);
        
        return new ContentDeleterInput
        {
            ContentTypeId = "test-content-type",
            ContentfulService = _contentfulServiceMock.Object,
            IncludeArchived = includeArchived
        };
    }

    private static MockAsyncEnumerableWithTotal<Entry<dynamic>> CreateTestEntries() 
        => new(
        [
            EntryBuilder.CreateBlogPost("entry1"),
            EntryBuilder.CreateBlogPost("entry2"),
            EntryBuilder.CreateBlogPost("entry3"),
        ]);
}