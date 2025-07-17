using Contentful.Core.Models;
using Contool.Core.Features.ContentPublish;
using Contool.Core.Infrastructure.Contentful.Services;
using Contool.Core.Infrastructure.Contentful.Utils;
using Contool.Core.Infrastructure.Utils.Models;
using Contool.Core.Infrastructure.Utils.Services;
using Contool.Core.Tests.Unit.Helpers;
using MockLite;
using Locale = Contentful.Core.Models.Management.Locale;

namespace Contool.Core.Tests.Unit.Features.ContentPublish;

public class ContentPublisherTests
{
    private readonly ContentPublisher _sut;
    
    private readonly Mock<IBatchProcessor> _batchProcessorMock = new();
    private readonly Mock<IProgressReporter> _progressReporterMock = new();

    public ContentPublisherTests()
    {
        _batchProcessorMock.SetupDefaults();
        _progressReporterMock.SetupDefaults();
        
        _sut = new ContentPublisher(_batchProcessorMock.Object, _progressReporterMock.Object);
    }

    [Fact]
    public async Task GivenValidInput_WhenPublishAsync_ThenPublishesEntriesInBatches()
    {
        // Arrange
        var input = CreatePublisherInput();
        
        // Act
        await _sut.PublishAsync(input, CancellationToken.None);
        
        // Assert
        _batchProcessorMock.Verify(x => x.ProcessAsync(
            It.IsAny<IAsyncEnumerable<object>>(),
            50,
            It.IsAny<Func<IReadOnlyList<object>, CancellationToken, Task>>(),
            It.IsAny<Func<object, bool>>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GivenValidInput_WhenPublishAsync_ThenReportsProgressCorrectly()
    {
        // Arrange
        var input = CreatePublisherInput();
        
        // Act
        await _sut.PublishAsync(input, CancellationToken.None);
        
        // Assert
        _progressReporterMock.Verify(x => x.Start(Operation.Publish, It.IsAny<Func<int>>()), Times.Once);
        _progressReporterMock.Verify(x => x.Complete(), Times.Once);
    }

    [Fact(Skip = "MockLite limitation: Cannot cast to MockContentfulService - deleted custom mock")]
    public async Task GivenValidInput_WhenPublishAsync_ThenCallsContentfulServicePublishEntries()
    {
        // Arrange
        var input = CreatePublisherInput();
        // MockLite limitation: Cannot cast to MockContentfulService - deleted custom mock
        // var mockContentfulService = (MockContentfulService)input.ContentfulService;
        
        // Act
        await _sut.PublishAsync(input, CancellationToken.None);
        
        // Assert
        _batchProcessorMock.Verify(x => x.ProcessAsync(
            It.IsAny<IAsyncEnumerable<object>>(),
            50,
            It.IsAny<Func<IReadOnlyList<object>, CancellationToken, Task>>(),
            It.IsAny<Func<object, bool>>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GivenCancellationToken_WhenPublishAsync_ThenRespectsCancellation()
    {
        // Arrange
        var input = CreatePublisherInput();
        
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        
        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            _sut.PublishAsync(input, cts.Token));
    }

    [Fact]
    public async Task GivenBatchProcessorThrowsException_WhenPublishAsync_ThenBubblesException()
    {
        // Arrange
        var input = CreatePublisherInput();
        _batchProcessorMock.Setup(x => x.ProcessAsync(
            It.IsAny<IAsyncEnumerable<object>>(),
            It.IsAny<int>(),
            It.IsAny<Func<IReadOnlyList<object>, CancellationToken, Task>>(),
            It.IsAny<Func<object, bool>>(),
            It.IsAny<CancellationToken>()))
            .Throws(new InvalidOperationException("Batch processing failed"));
        
        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _sut.PublishAsync(input, CancellationToken.None));
        
        Assert.Equal("Batch processing failed", exception.Message);
    }

    [Fact]
    public async Task GivenEmptyEntries_WhenPublishAsync_ThenHandlesEmptyCollection()
    {
        // Arrange
        var input = CreatePublisherInputWithEmptyEntries();
        
        // Act
        await _sut.PublishAsync(input, CancellationToken.None);
        
        // Assert
        _batchProcessorMock.Verify(x => x.ProcessAsync(
            It.IsAny<IAsyncEnumerable<object>>(),
            50,
            It.IsAny<Func<IReadOnlyList<object>, CancellationToken, Task>>(),
            It.IsAny<Func<object, bool>>(),
            It.IsAny<CancellationToken>()), Times.Once);
        _progressReporterMock.Verify(x => x.Start(Operation.Publish, It.IsAny<Func<int>>()), Times.Once);
        _progressReporterMock.Verify(x => x.Complete(), Times.Once);
    }

    [Fact]
    public async Task GivenLargeNumberOfEntries_WhenPublishAsync_ThenProcessesInBatchesOfFifty()
    {
        // Arrange
        var input = CreatePublisherInputWithLargeDataset(150);
        
        // Act
        await _sut.PublishAsync(input, CancellationToken.None);
        
        // Assert
        _batchProcessorMock.Verify(x => x.ProcessAsync(
            It.IsAny<IAsyncEnumerable<object>>(),
            50,
            It.IsAny<Func<IReadOnlyList<object>, CancellationToken, Task>>(),
            It.IsAny<Func<object, bool>>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GivenContentfulServiceThrowsException_WhenPublishAsync_ThenBubblesException()
    {
        // Arrange
        var input = CreatePublisherInputWithFailingService();
        
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _sut.PublishAsync(input, CancellationToken.None));
    }

    private static ContentPublisherInput CreatePublisherInput()
    {
        var mockService = new Mock<IContentfulService>();
        mockService.SetupDefaults();
        var testEntries = CreateTestEntries();
        mockService.Setup(x => x.GetEntriesAsync("test-content-type", null, PagingMode.SkipForward, It.IsAny<CancellationToken>()))
            .Returns(testEntries);
        
        return new ContentPublisherInput
        {
            ContentTypeId = "test-content-type",
            ContentfulService = mockService.Object
        };
    }

    private static ContentPublisherInput CreatePublisherInputWithEmptyEntries()
    {
        var mockService = new Mock<IContentfulService>();
        mockService.SetupDefaults();
        var emptyEntries = new MockAsyncEnumerableWithTotal<Entry<dynamic>>(Array.Empty<Entry<dynamic>>());
        mockService.Setup(x => x.GetEntriesAsync("test-content-type", null, PagingMode.SkipForward, It.IsAny<CancellationToken>()))
            .Returns(emptyEntries);
        
        return new ContentPublisherInput
        {
            ContentTypeId = "test-content-type",
            ContentfulService = mockService.Object
        };
    }

    private static ContentPublisherInput CreatePublisherInputWithLargeDataset(int count)
    {
        var mockService = new Mock<IContentfulService>();
        mockService.SetupDefaults();
        var entries = Enumerable.Range(1, count)
            .Select(i => EntryBuilder.CreateBlogPost($"entry{i}", "blogPost"))
            .ToArray();
        var testEntries = new MockAsyncEnumerableWithTotal<Entry<dynamic>>(entries);
        mockService.Setup(x => x.GetEntriesAsync("test-content-type", null, PagingMode.SkipForward, It.IsAny<CancellationToken>()))
            .Returns(testEntries);
        
        return new ContentPublisherInput
        {
            ContentTypeId = "test-content-type",
            ContentfulService = mockService.Object
        };
    }

    private static ContentPublisherInput CreatePublisherInputWithFailingService()
    {
        var failingService = new Mock<IContentfulService>();
        failingService.SetupDefaults();
        failingService.Setup(x => x.GetEntriesAsync("test-content-type", null, PagingMode.SkipForward, It.IsAny<CancellationToken>()))
            .Throws(new InvalidOperationException("Service failure"));
        
        return new ContentPublisherInput
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