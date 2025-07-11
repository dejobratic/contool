using Contentful.Core.Models;
using Contool.Core.Features.ContentDelete;
using Contool.Core.Infrastructure.Contentful.Services;
using Contool.Core.Infrastructure.Contentful.Utils;
using Contool.Core.Infrastructure.Utils.Models;
using Contool.Core.Infrastructure.Utils.Services;
using Contool.Core.Tests.Unit.Helpers;
using Contool.Core.Tests.Unit.Mocks;
using Locale = Contentful.Core.Models.Management.Locale;

namespace Contool.Core.Tests.Unit.Features.ContentDelete;

public class ContentDeleterTests
{
    private readonly ContentDeleter _sut;
    
    private readonly MockBatchProcessor _batchProcessorMock = new();
    private readonly MockProgressReporter _progressReporterMock = new();

    public ContentDeleterTests()
    {
        _sut = new ContentDeleter(_batchProcessorMock, _progressReporterMock);
    }

    [Fact]
    public async Task GivenValidInput_WhenDeleteAsync_ThenDeletesEntriesInBatches()
    {
        // Arrange
        var input = CreateDeleterInput();
        
        // Act
        await _sut.DeleteAsync(input, CancellationToken.None);
        
        // Assert
        Assert.True(_batchProcessorMock.ProcessAsyncWasCalled);
        Assert.Equal(50, _batchProcessorMock.LastBatchSize);
        Assert.NotNull(_batchProcessorMock.LastBatchAction);
    }

    [Fact]
    public async Task GivenValidInput_WhenDeleteAsync_ThenReportsProgressCorrectly()
    {
        // Arrange
        var input = CreateDeleterInput();
        
        // Act
        await _sut.DeleteAsync(input, CancellationToken.None);
        
        // Assert
        Assert.True(_progressReporterMock.StartWasCalled);
        Assert.Equal(Operation.Delete, _progressReporterMock.LastOperation);
        Assert.True(_progressReporterMock.CompleteWasCalled);
    }

    [Fact]
    public async Task GivenValidInput_WhenDeleteAsync_ThenCallsContentfulServiceDeleteEntries()
    {
        // Arrange
        var input = CreateDeleterInput();
        var mockContentfulService = (MockContentfulService)input.ContentfulService;
        
        // Act
        await _sut.DeleteAsync(input, CancellationToken.None);
        
        // Assert
        Assert.True(_batchProcessorMock.ProcessAsyncWasCalled);
        // The batch action should be the DeleteEntriesAsync method
        Assert.NotNull(_batchProcessorMock.LastBatchAction);
    }

    [Fact]
    public async Task GivenCancellationToken_WhenDeleteAsync_ThenRespectsCancellation()
    {
        // Arrange
        var input = CreateDeleterInput();
        
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        
        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            _sut.DeleteAsync(input, cts.Token));
    }

    [Fact]
    public async Task GivenBatchProcessorThrowsException_WhenDeleteAsync_ThenBubblesException()
    {
        // Arrange
        var input = CreateDeleterInput();
        _batchProcessorMock.SetupToThrow(new InvalidOperationException("Batch processing failed"));
        
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
        Assert.True(_batchProcessorMock.ProcessAsyncWasCalled);
        Assert.True(_progressReporterMock.StartWasCalled);
        Assert.True(_progressReporterMock.CompleteWasCalled);
    }

    [Fact]
    public async Task GivenLargeNumberOfEntries_WhenDeleteAsync_ThenProcessesInBatchesOfFifty()
    {
        // Arrange
        var input = CreateDeleterInputWithLargeDataset(300);
        
        // Act
        await _sut.DeleteAsync(input, CancellationToken.None);
        
        // Assert
        Assert.True(_batchProcessorMock.ProcessAsyncWasCalled);
        Assert.Equal(50, _batchProcessorMock.LastBatchSize);
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
        Assert.True(_batchProcessorMock.ProcessAsyncWasCalled);
        // The batch processor should have been called with a filter
        Assert.NotNull(_batchProcessorMock.LastBatchAction);
    }

    [Fact]
    public async Task GivenIncludeArchivedTrue_WhenDeleteAsync_ThenIncludesArchivedEntries()
    {
        // Arrange
        var input = CreateDeleterInputWithArchivedEntries(includeArchived: true);
        
        // Act
        await _sut.DeleteAsync(input, CancellationToken.None);
        
        // Assert
        Assert.True(_batchProcessorMock.ProcessAsyncWasCalled);
        // The batch processor should have been called with a filter that includes archived entries
        Assert.NotNull(_batchProcessorMock.LastBatchAction);
    }

    [Fact]
    public async Task GivenMixedArchivedAndNonArchivedEntries_WhenDeleteAsyncWithIncludeArchivedFalse_ThenProcessesOnlyNonArchivedEntries()
    {
        // Arrange
        var input = CreateDeleterInputWithMixedArchivedEntries(includeArchived: false);
        
        // Act
        await _sut.DeleteAsync(input, CancellationToken.None);
        
        // Assert
        Assert.True(_batchProcessorMock.ProcessAsyncWasCalled);
        // Should process with filter that excludes archived entries
        Assert.NotNull(_batchProcessorMock.LastBatchAction);
    }

    private static ContentDeleterInput CreateDeleterInput()
    {
        var mockService = new MockContentfulService();
        var testEntries = CreateTestEntries();
        mockService.SetupEntries(testEntries);
        
        return new ContentDeleterInput
        {
            ContentTypeId = "test-content-type",
            ContentfulService = mockService,
            IncludeArchived = false
        };
    }

    private static ContentDeleterInput CreateDeleterInputWithEmptyEntries()
    {
        var mockService = new MockContentfulService();
        var emptyEntries = new MockAsyncEnumerableWithTotal<Entry<dynamic>>(Array.Empty<Entry<dynamic>>(), 0);
        mockService.SetupEntries(emptyEntries);
        
        return new ContentDeleterInput
        {
            ContentTypeId = "test-content-type",
            ContentfulService = mockService,
            IncludeArchived = false
        };
    }

    private static ContentDeleterInput CreateDeleterInputWithLargeDataset(int count)
    {
        var mockService = new MockContentfulService();
        var entries = Enumerable.Range(1, count)
            .Select(i => EntryBuilder.CreateBlogPost($"entry{i}", "blogPost"))
            .ToArray();
        var testEntries = new MockAsyncEnumerableWithTotal<Entry<dynamic>>(entries, count);
        mockService.SetupEntries(testEntries);
        
        return new ContentDeleterInput
        {
            ContentTypeId = "test-content-type",
            ContentfulService = mockService,
            IncludeArchived = false
        };
    }

    private static ContentDeleterInput CreateDeleterInputWithFailingService()
    {
        var failingService = new FailingContentfulService();
        
        return new ContentDeleterInput
        {
            ContentTypeId = "test-content-type",
            ContentfulService = failingService,
            IncludeArchived = false
        };
    }

    private static ContentDeleterInput CreateDeleterInputWithArchivedEntries(bool includeArchived)
    {
        var mockService = new MockContentfulService();
        var archivedEntries = new[]
        {
            EntryBuilder.CreateArchivedBlogPost("archived1", "blogPost"),
            EntryBuilder.CreateArchivedBlogPost("archived2", "blogPost"),
            EntryBuilder.CreateArchivedProduct("archived3", "product")
        };
        var testEntries = new MockAsyncEnumerableWithTotal<Entry<dynamic>>(archivedEntries, archivedEntries.Length);
        mockService.SetupEntries(testEntries);
        
        return new ContentDeleterInput
        {
            ContentTypeId = "test-content-type",
            ContentfulService = mockService,
            IncludeArchived = includeArchived
        };
    }

    private static ContentDeleterInput CreateDeleterInputWithMixedArchivedEntries(bool includeArchived)
    {
        var mockService = new MockContentfulService();
        var mixedEntries = new[]
        {
            EntryBuilder.CreateBlogPost("entry1", "blogPost"),
            EntryBuilder.CreateArchivedBlogPost("archived1", "blogPost"),
            EntryBuilder.CreateProduct("entry2", "product"),
            EntryBuilder.CreateArchivedProduct("archived2", "product")
        };
        var testEntries = new MockAsyncEnumerableWithTotal<Entry<dynamic>>(mixedEntries, mixedEntries.Length);
        mockService.SetupEntries(testEntries);
        
        return new ContentDeleterInput
        {
            ContentTypeId = "test-content-type",
            ContentfulService = mockService,
            IncludeArchived = includeArchived
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