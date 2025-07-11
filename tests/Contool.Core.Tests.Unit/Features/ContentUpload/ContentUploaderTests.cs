using Contentful.Core.Models;
using Contool.Core.Features.ContentUpload;
using Contool.Core.Infrastructure.Contentful.Services;
using Contool.Core.Infrastructure.Utils.Models;
using Contool.Core.Infrastructure.Utils.Services;
using Contool.Core.Tests.Unit.Helpers;
using Contool.Core.Tests.Unit.Mocks;

namespace Contool.Core.Tests.Unit.Features.ContentUpload;

public class ContentUploaderTests
{
    private readonly ContentUploader _uploader;
    private readonly MockBatchProcessor _mockBatchProcessor;
    private readonly MockProgressReporter _mockProgressReporter;
    private readonly MockContentfulService _mockContentfulService;

    public ContentUploaderTests()
    {
        _mockBatchProcessor = new MockBatchProcessor();
        _mockProgressReporter = new MockProgressReporter();
        _uploader = new ContentUploader(_mockBatchProcessor, _mockProgressReporter);
        _mockContentfulService = new MockContentfulService();
    }

    [Fact]
    public async Task GivenValidEntries_WhenUpload_ThenProcessesInBatches()
    {
        // Arrange
        var entries = new[]
        {
            EntryBuilder.CreateBlogPost("entry1"),
            EntryBuilder.CreateBlogPost("entry2"),
            EntryBuilder.CreateBlogPost("entry3"),
        };

        var input = new ContentUploaderInput
        {
            ContentTypeId = "blogPost",
            ContentfulService = _mockContentfulService,
            Entries = new MockAsyncEnumerableWithTotal<Entry<dynamic>>(entries),
            UploadOnlyValidEntries = false,
            PublishEntries = false
        };

        // Act
        await _uploader.UploadAsync(input, CancellationToken.None);

        // Assert
        Assert.True(_mockBatchProcessor.ProcessAsyncWasCalled);
        Assert.Equal(entries.Length, _mockBatchProcessor.ProcessedItemsCount);
        Assert.Equal(50, _mockBatchProcessor.LastBatchSize); // Default batch size
    }

    [Fact]
    public async Task GivenLargeNumberOfEntries_WhenUpload_ThenProcessesInMultipleBatches()
    {
        // Arrange
        const int entryCount = 125;
        var entries = Enumerable.Range(1, entryCount)
            .Select(i => EntryBuilder.CreateBlogPost($"entry{i}"))
            .ToArray();

        var input = new ContentUploaderInput
        {
            ContentTypeId = "blogPost",
            ContentfulService = _mockContentfulService,
            Entries = new MockAsyncEnumerableWithTotal<Entry<dynamic>>(entries),
            UploadOnlyValidEntries = false,
            PublishEntries = false
        };

        // Act
        await _uploader.UploadAsync(input, CancellationToken.None);

        // Assert
        Assert.True(_mockBatchProcessor.ProcessAsyncWasCalled);
        Assert.Equal(entryCount, _mockBatchProcessor.ProcessedItemsCount);
        Assert.True(_mockBatchProcessor.BatchCount >= 3); // Should be at least 3 batches (50, 50, 25)
    }

    [Fact]
    public async Task GivenEmptyEntries_WhenUpload_ThenCompletesWithoutProcessing()
    {
        // Arrange
        var emptyEntries = Array.Empty<Entry<dynamic>>();

        var input = new ContentUploaderInput
        {
            ContentTypeId = "blogPost",
            ContentfulService = _mockContentfulService,
            Entries = new MockAsyncEnumerableWithTotal<Entry<dynamic>>(emptyEntries),
            UploadOnlyValidEntries = false,
            PublishEntries = false
        };

        // Act
        await _uploader.UploadAsync(input, CancellationToken.None);

        // Assert
        Assert.True(_mockBatchProcessor.ProcessAsyncWasCalled);
        Assert.Equal(0, _mockBatchProcessor.ProcessedItemsCount);
    }

    [Fact]
    public async Task GivenPublishEntriesTrue_WhenUpload_ThenCallsPublishAfterUpload()
    {
        // Arrange
        var entries = new[]
        {
            EntryBuilder.CreateBlogPost("entry1")
        };

        var input = new ContentUploaderInput
        {
            ContentTypeId = "blogPost",
            ContentfulService = _mockContentfulService,
            Entries = new MockAsyncEnumerableWithTotal<Entry<dynamic>>(entries),
            UploadOnlyValidEntries = false,
            PublishEntries = true
        };

        // Act
        await _uploader.UploadAsync(input, CancellationToken.None);

        // Assert
        Assert.True(_mockContentfulService.CreateOrUpdateEntriesAsyncWasCalled);
        Assert.True(_mockContentfulService.PublishEntriesAsyncWasCalled);
        Assert.False(_mockContentfulService.UnpublishEntriesAsyncWasCalled);
    }

    [Fact]
    public async Task GivenPublishEntriesFalse_WhenUpload_ThenDoesNotCallPublish()
    {
        // Arrange
        var entries = new[]
        {
            EntryBuilder.CreateBlogPost("entry1")
        };

        var input = new ContentUploaderInput
        {
            ContentTypeId = "blogPost",
            ContentfulService = _mockContentfulService,
            Entries = new MockAsyncEnumerableWithTotal<Entry<dynamic>>(entries),
            UploadOnlyValidEntries = false,
            PublishEntries = false
        };

        // Act
        await _uploader.UploadAsync(input, CancellationToken.None);

        // Assert
        Assert.True(_mockContentfulService.CreateOrUpdateEntriesAsyncWasCalled);
        Assert.False(_mockContentfulService.PublishEntriesAsyncWasCalled);
        Assert.False(_mockContentfulService.UnpublishEntriesAsyncWasCalled);
    }

    [Fact]
    public async Task GivenCancellationToken_WhenCancelled_ThenThrowsOperationCancelledException()
    {
        // Arrange
        var entries = new[]
        {
            EntryBuilder.CreateBlogPost("entry1")
        };

        var input = new ContentUploaderInput
        {
            ContentTypeId = "blogPost",
            ContentfulService = _mockContentfulService,
            Entries = new MockAsyncEnumerableWithTotal<Entry<dynamic>>(entries),
            UploadOnlyValidEntries = false,
            PublishEntries = false
        };

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            _uploader.UploadAsync(input, cts.Token));
    }

    [Fact]
    public async Task GivenProgressReporter_WhenUpload_ThenReportsProgress()
    {
        // Arrange
        var entries = new[]
        {
            EntryBuilder.CreateBlogPost("entry1"),
            EntryBuilder.CreateBlogPost("entry2"),
        };

        var input = new ContentUploaderInput
        {
            ContentTypeId = "blogPost",
            ContentfulService = _mockContentfulService,
            Entries = new MockAsyncEnumerableWithTotal<Entry<dynamic>>(entries),
            UploadOnlyValidEntries = false,
            PublishEntries = false
        };

        // Act
        await _uploader.UploadAsync(input, CancellationToken.None);

        // Assert
        Assert.True(_mockProgressReporter.StartWasCalled);
        Assert.True(_mockProgressReporter.IncrementWasCalled);
        Assert.True(_mockProgressReporter.CompleteWasCalled);
    }

    [Fact]
    public void GivenContentUploader_WhenCheckingInterface_ThenImplementsIContentUploader()
    {
        // Arrange & Act
        var implementsInterface = _uploader is IContentUploader;

        // Assert
        Assert.True(implementsInterface);
    }

    [Fact]
    public async Task GivenBatchProcessorThrowsException_WhenUpload_ThenPropagatesException()
    {
        // Arrange
        _mockBatchProcessor.ShouldThrowException = true;

        var entries = new[]
        {
            EntryBuilder.CreateBlogPost("entry1"),
        };

        var input = new ContentUploaderInput
        {
            ContentTypeId = "blogPost",
            ContentfulService = _mockContentfulService,
            Entries = new MockAsyncEnumerableWithTotal<Entry<dynamic>>(entries),
            UploadOnlyValidEntries = false,
            PublishEntries = false
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _uploader.UploadAsync(input, CancellationToken.None));
    }

    [Fact]
    public async Task GivenCustomBatchSize_WhenUpload_ThenUsesBatchSize()
    {
        // Arrange
        const int customBatchSize = 10;
        var customBatchProcessor = new MockBatchProcessor(customBatchSize);
        var customUploader = new ContentUploader(customBatchProcessor, _mockProgressReporter);

        var entries = new[]
        {
            EntryBuilder.CreateBlogPost("entry1"),
            EntryBuilder.CreateBlogPost("entry2")
        };

        var input = new ContentUploaderInput
        {
            ContentTypeId = "blogPost",
            ContentfulService = _mockContentfulService,
            Entries = new MockAsyncEnumerableWithTotal<Entry<dynamic>>(entries),
            UploadOnlyValidEntries = false,
            PublishEntries = false
        };

        // Act
        await customUploader.UploadAsync(input, CancellationToken.None);

        // Assert
        Assert.Equal(customBatchSize, customBatchProcessor.LastBatchSize);
    }

    [Fact]
    public async Task GivenNullInput_WhenUpload_ThenThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _uploader.UploadAsync(null!, CancellationToken.None));
    }
}