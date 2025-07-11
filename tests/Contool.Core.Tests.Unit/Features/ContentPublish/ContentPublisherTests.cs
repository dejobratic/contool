using Contentful.Core.Models;
using Contool.Core.Features.ContentPublish;
using Contool.Core.Infrastructure.Contentful.Services;
using Contool.Core.Infrastructure.Contentful.Utils;
using Contool.Core.Infrastructure.Utils.Models;
using Contool.Core.Infrastructure.Utils.Services;
using Contool.Core.Tests.Unit.Helpers;
using Contool.Core.Tests.Unit.Mocks;
using Locale = Contentful.Core.Models.Management.Locale;

namespace Contool.Core.Tests.Unit.Features.ContentPublish;

public class ContentPublisherTests
{
    private readonly ContentPublisher _sut;
    
    private readonly MockBatchProcessor _batchProcessorMock = new();
    private readonly MockProgressReporter _progressReporterMock = new();

    public ContentPublisherTests()
    {
        _sut = new ContentPublisher(_batchProcessorMock, _progressReporterMock);
    }

    [Fact]
    public async Task GivenValidInput_WhenPublishAsync_ThenPublishesEntriesInBatches()
    {
        // Arrange
        var input = CreatePublisherInput();
        
        // Act
        await _sut.PublishAsync(input, CancellationToken.None);
        
        // Assert
        Assert.True(_batchProcessorMock.ProcessAsyncWasCalled);
        Assert.Equal(50, _batchProcessorMock.LastBatchSize);
        Assert.NotNull(_batchProcessorMock.LastBatchAction);
    }

    [Fact]
    public async Task GivenValidInput_WhenPublishAsync_ThenReportsProgressCorrectly()
    {
        // Arrange
        var input = CreatePublisherInput();
        
        // Act
        await _sut.PublishAsync(input, CancellationToken.None);
        
        // Assert
        Assert.True(_progressReporterMock.StartWasCalled);
        Assert.Equal(Operation.Publish, _progressReporterMock.LastOperation);
        Assert.True(_progressReporterMock.CompleteWasCalled);
    }

    [Fact]
    public async Task GivenValidInput_WhenPublishAsync_ThenCallsContentfulServicePublishEntries()
    {
        // Arrange
        var input = CreatePublisherInput();
        var mockContentfulService = (MockContentfulService)input.ContentfulService;
        
        // Act
        await _sut.PublishAsync(input, CancellationToken.None);
        
        // Assert
        Assert.True(_batchProcessorMock.ProcessAsyncWasCalled);
        // The batch action should be the PublishEntriesAsync method
        Assert.NotNull(_batchProcessorMock.LastBatchAction);
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
        _batchProcessorMock.SetupToThrow(new InvalidOperationException("Batch processing failed"));
        
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
        Assert.True(_batchProcessorMock.ProcessAsyncWasCalled);
        Assert.True(_progressReporterMock.StartWasCalled);
        Assert.True(_progressReporterMock.CompleteWasCalled);
    }

    [Fact]
    public async Task GivenLargeNumberOfEntries_WhenPublishAsync_ThenProcessesInBatchesOfFifty()
    {
        // Arrange
        var input = CreatePublisherInputWithLargeDataset(150);
        
        // Act
        await _sut.PublishAsync(input, CancellationToken.None);
        
        // Assert
        Assert.True(_batchProcessorMock.ProcessAsyncWasCalled);
        Assert.Equal(50, _batchProcessorMock.LastBatchSize);
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
        var mockService = new MockContentfulService();
        var testEntries = CreateTestEntries();
        mockService.SetupEntries(testEntries);
        
        return new ContentPublisherInput
        {
            ContentTypeId = "test-content-type",
            ContentfulService = mockService
        };
    }

    private static ContentPublisherInput CreatePublisherInputWithEmptyEntries()
    {
        var mockService = new MockContentfulService();
        var emptyEntries = new MockAsyncEnumerableWithTotal<Entry<dynamic>>(Array.Empty<Entry<dynamic>>(), 0);
        mockService.SetupEntries(emptyEntries);
        
        return new ContentPublisherInput
        {
            ContentTypeId = "test-content-type",
            ContentfulService = mockService
        };
    }

    private static ContentPublisherInput CreatePublisherInputWithLargeDataset(int count)
    {
        var mockService = new MockContentfulService();
        var entries = Enumerable.Range(1, count)
            .Select(i => EntryBuilder.CreateBlogPost($"entry{i}", "blogPost"))
            .ToArray();
        var testEntries = new MockAsyncEnumerableWithTotal<Entry<dynamic>>(entries, count);
        mockService.SetupEntries(testEntries);
        
        return new ContentPublisherInput
        {
            ContentTypeId = "test-content-type",
            ContentfulService = mockService
        };
    }

    private static ContentPublisherInput CreatePublisherInputWithFailingService()
    {
        var failingService = new FailingContentfulService();
        
        return new ContentPublisherInput
        {
            ContentTypeId = "test-content-type",
            ContentfulService = failingService
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

        return new MockAsyncEnumerableWithTotal<Entry<dynamic>>(entries, entries.Length);
    }

    private class FailingContentfulService : IContentfulService
    {
        public Task<IEnumerable<Locale>> GetLocalesAsync(CancellationToken cancellationToken) =>
            throw new NotImplementedException();

        public Task<ContentType?> GetContentTypeAsync(string contentTypeId, CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task<IEnumerable<ContentType>> GetContentTypesAsync(CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task<ContentType> CreateContentTypeAsync(ContentType contentType, CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task DeleteContentTypeAsync(string contentTypeId, CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public IAsyncEnumerableWithTotal<Entry<dynamic>> GetEntriesAsync(string contentTypeId, int? pageSize = null, PagingMode pagingMode = PagingMode.SkipForward, CancellationToken cancellationToken = default) =>
            throw new InvalidOperationException("Service failure");

        public Task CreateOrUpdateEntriesAsync(IEnumerable<Entry<dynamic>> entries, bool publish = false, CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task PublishEntriesAsync(IEnumerable<Entry<dynamic>> entries, CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task UnpublishEntriesAsync(IEnumerable<Entry<dynamic>> entries, CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task DeleteEntriesAsync(IEnumerable<Entry<dynamic>> entries, CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();
    }
}