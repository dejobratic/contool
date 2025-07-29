using Contentful.Core.Models;
using Contool.Core.Features.ContentUnpublish;
using Contool.Core.Infrastructure.Contentful.Services;
using Contool.Core.Infrastructure.Contentful.Utils;
using Contool.Core.Infrastructure.Utils.Models;
using Contool.Core.Infrastructure.Utils.Services;
using Contool.Core.Tests.Unit.Helpers;
using MockLite;
using Locale = Contentful.Core.Models.Management.Locale;

namespace Contool.Core.Tests.Unit.Features.ContentUnpublish;

public class ContentUnpublisherTests
{
    private readonly ContentUnpublisher _sut;
    
    private readonly Mock<IBatchProcessor> _batchProcessorMock = new();
    private readonly Mock<IProgressReporter> _progressReporterMock = new();

    public ContentUnpublisherTests()
    {
        _batchProcessorMock.SetupDefaults();
        _progressReporterMock.SetupDefaults();
        
        _sut = new ContentUnpublisher(_batchProcessorMock.Object, _progressReporterMock.Object);
    }

    [Fact]
    public async Task GivenValidInput_WhenUnpublishAsync_ThenUnpublishesEntriesInBatches()
    {
        // Arrange
        var input = CreateUnpublisherInput();
        
        // Act
        await _sut.UnpublishAsync(input, CancellationToken.None);
        
        // Assert
        _batchProcessorMock.Verify(x => x.ProcessAsync(
            It.IsAny<IAsyncEnumerable<object>>(),
            50,
            It.IsAny<Func<IReadOnlyList<object>, CancellationToken, Task>>(),
            It.IsAny<Func<object, bool>>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GivenValidInput_WhenUnpublishAsync_ThenReportsProgressCorrectly()
    {
        // Arrange
        var input = CreateUnpublisherInput();
        
        // Act
        await _sut.UnpublishAsync(input, CancellationToken.None);
        
        // Assert
        _progressReporterMock.Verify(x => x.Start(Operation.Unpublish, It.IsAny<Func<int>>()), Times.Once);
        _progressReporterMock.Verify(x => x.Complete(), Times.Once);
    }

    [Fact]
    public async Task GivenValidInput_WhenUnpublishAsync_ThenCallsContentfulServiceUnpublishEntries()
    {
        // Arrange
        var input = CreateUnpublisherInput();
        
        // Act
        await _sut.UnpublishAsync(input, CancellationToken.None);
        
        // Assert
        _batchProcessorMock.Verify(x => x.ProcessAsync(
            It.IsAny<IAsyncEnumerable<object>>(),
            50,
            It.IsAny<Func<IReadOnlyList<object>, CancellationToken, Task>>(),
            It.IsAny<Func<object, bool>>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GivenBatchProcessorThrowsException_WhenUnpublishAsync_ThenBubblesException()
    {
        // Arrange
        var input = CreateUnpublisherInput();
        _batchProcessorMock.Setup(x => x.ProcessAsync(
            It.IsAny<IAsyncEnumerable<object>>(),
            It.IsAny<int>(),
            It.IsAny<Func<IReadOnlyList<object>, CancellationToken, Task>>(),
            It.IsAny<Func<object, bool>>(),
            It.IsAny<CancellationToken>()))
            .Throws(new InvalidOperationException("Batch processing failed"));
        
        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _sut.UnpublishAsync(input, CancellationToken.None));
        
        Assert.Equal("Batch processing failed", exception.Message);
    }

    [Fact]
    public async Task GivenEmptyEntries_WhenUnpublishAsync_ThenHandlesEmptyCollection()
    {
        // Arrange
        var input = CreateUnpublisherInputWithEmptyEntries();
        
        // Act
        await _sut.UnpublishAsync(input, CancellationToken.None);
        
        // Assert
        _batchProcessorMock.Verify(x => x.ProcessAsync(
            It.IsAny<IAsyncEnumerable<object>>(),
            50,
            It.IsAny<Func<IReadOnlyList<object>, CancellationToken, Task>>(),
            It.IsAny<Func<object, bool>>(),
            It.IsAny<CancellationToken>()), Times.Once);
        _progressReporterMock.Verify(x => x.Start(Operation.Unpublish, It.IsAny<Func<int>>()), Times.Once);
        _progressReporterMock.Verify(x => x.Complete(), Times.Once);
    }

    [Fact]
    public async Task GivenLargeNumberOfEntries_WhenUnpublishAsync_ThenProcessesInBatchesOfFifty()
    {
        // Arrange
        var input = CreateUnpublisherInputWithLargeDataset(200);
        
        // Act
        await _sut.UnpublishAsync(input, CancellationToken.None);
        
        // Assert
        _batchProcessorMock.Verify(x => x.ProcessAsync(
            It.IsAny<IAsyncEnumerable<object>>(),
            50,
            It.IsAny<Func<IReadOnlyList<object>, CancellationToken, Task>>(),
            It.IsAny<Func<object, bool>>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GivenContentfulServiceThrowsException_WhenUnpublishAsync_ThenBubblesException()
    {
        // Arrange
        var input = CreateUnpublisherInputWithFailingService();
        
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _sut.UnpublishAsync(input, CancellationToken.None));
    }

    private static ContentUnpublisherInput CreateUnpublisherInput()
    {
        var mockService = new Mock<IContentfulService>();
        mockService.SetupDefaults();
        var testEntries = CreateTestEntries();
        mockService.Setup(x => x.GetEntriesAsync("test-content-type", null, PagingMode.SkipForward, It.IsAny<CancellationToken>()))
            .Returns(testEntries);
        
        return new ContentUnpublisherInput
        {
            ContentTypeId = "test-content-type",
            ContentfulService = mockService.Object
        };
    }

    private static ContentUnpublisherInput CreateUnpublisherInputWithEmptyEntries()
    {
        var mockService = new Mock<IContentfulService>();
        mockService.SetupDefaults();
        var emptyEntries = new MockAsyncEnumerableWithTotal<Entry<dynamic>>(Array.Empty<Entry<dynamic>>());
        mockService.Setup(x => x.GetEntriesAsync("test-content-type", null, PagingMode.SkipForward, It.IsAny<CancellationToken>()))
            .Returns(emptyEntries);
        
        return new ContentUnpublisherInput
        {
            ContentTypeId = "test-content-type",
            ContentfulService = mockService.Object
        };
    }

    private static ContentUnpublisherInput CreateUnpublisherInputWithLargeDataset(int count)
    {
        var mockService = new Mock<IContentfulService>();
        mockService.SetupDefaults();
        var entries = Enumerable.Range(1, count)
            .Select(i => EntryBuilder.CreateBlogPost($"entry{i}", "blogPost"))
            .ToArray();
        var testEntries = new MockAsyncEnumerableWithTotal<Entry<dynamic>>(entries);
        mockService.Setup(x => x.GetEntriesAsync("test-content-type", null, PagingMode.SkipForward, It.IsAny<CancellationToken>()))
            .Returns(testEntries);
        
        return new ContentUnpublisherInput
        {
            ContentTypeId = "test-content-type",
            ContentfulService = mockService.Object
        };
    }

    private static ContentUnpublisherInput CreateUnpublisherInputWithFailingService()
    {
        var failingService = new Mock<IContentfulService>();
        failingService.SetupDefaults();
        failingService.Setup(x => x.GetEntriesAsync("test-content-type", null, PagingMode.SkipForward, It.IsAny<CancellationToken>()))
            .Throws(new InvalidOperationException("Service failure"));
        
        return new ContentUnpublisherInput
        {
            ContentTypeId = "test-content-type",
            ContentfulService = failingService.Object
        };
    }

    private static IAsyncEnumerableWithTotal<Entry<dynamic>> CreateTestEntries()
    {
        var entries = new[]
        {
            EntryBuilder.CreateBlogPost("entry1", "blogPost"),
            EntryBuilder.CreateBlogPost("entry2", "blogPost"),
            EntryBuilder.CreateBlogPost("entry3", "blogPost")
        };

        return new MockAsyncEnumerableWithTotal<Entry<dynamic>>(entries);
    }

}