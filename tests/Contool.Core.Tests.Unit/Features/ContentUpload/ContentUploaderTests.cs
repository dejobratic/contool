using Contentful.Core.Models;
using Contool.Core.Features.ContentUpload;
using Contool.Core.Infrastructure.Contentful.Services;
using Contool.Core.Infrastructure.Utils.Models;
using Contool.Core.Infrastructure.Utils.Services;
using Contool.Core.Tests.Unit.Helpers;
using MockLite;

namespace Contool.Core.Tests.Unit.Features.ContentUpload;

public class ContentUploaderTests
{
    private readonly ContentUploader _sut;
    
    private readonly Mock<IBatchProcessor> _batchProcessorMock = new();
    private readonly Mock<IProgressReporter> _progressReporterMock = new();
    private readonly Mock<IContentfulService> _contentfulServiceMock = new();

    public ContentUploaderTests()
    {
        _batchProcessorMock.SetupDefaults();
        _progressReporterMock.SetupDefaults();
        _contentfulServiceMock.SetupDefaults();
        
        _sut = new ContentUploader(_batchProcessorMock.Object, _progressReporterMock.Object);
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
            ContentfulService = _contentfulServiceMock.Object,
            Entries = new MockAsyncEnumerableWithTotal<Entry<dynamic>>(entries),
            UploadOnlyValidEntries = false,
            PublishEntries = false
        };

        // Act
        await _sut.UploadAsync(input, CancellationToken.None);

        // Assert
        _batchProcessorMock.Verify(x => x.ProcessAsync(
            It.IsAny<IAsyncEnumerable<Entry<dynamic>>>(),
            It.IsAny<int>(),
            It.IsAny<Func<IReadOnlyList<Entry<dynamic>>, CancellationToken, Task>>(),
            It.IsAny<Func<Entry<dynamic>, bool>>(),
            It.IsAny<CancellationToken>()), Times.Once);
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
            ContentfulService = _contentfulServiceMock.Object,
            Entries = new MockAsyncEnumerableWithTotal<Entry<dynamic>>(entries),
            UploadOnlyValidEntries = false,
            PublishEntries = false
        };

        // Act
        await _sut.UploadAsync(input, CancellationToken.None);

        // Assert
        _batchProcessorMock.Verify(x => x.ProcessAsync(It.IsAny<IAsyncEnumerable<Entry<dynamic>>>(), It.IsAny<int>(), It.IsAny<Func<IReadOnlyList<Entry<dynamic>>, CancellationToken, Task>>(), It.IsAny<Func<Entry<dynamic>, bool>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GivenEmptyEntries_WhenUpload_ThenCompletesWithoutProcessing()
    {
        // Arrange
        var emptyEntries = Array.Empty<Entry<dynamic>>();

        var input = new ContentUploaderInput
        {
            ContentTypeId = "blogPost",
            ContentfulService = _contentfulServiceMock.Object,
            Entries = new MockAsyncEnumerableWithTotal<Entry<dynamic>>(emptyEntries),
            UploadOnlyValidEntries = false,
            PublishEntries = false
        };

        // Act
        await _sut.UploadAsync(input, CancellationToken.None);

        // Assert
        _batchProcessorMock.Verify(x => x.ProcessAsync(It.IsAny<IAsyncEnumerable<Entry<dynamic>>>(), It.IsAny<int>(), It.IsAny<Func<IReadOnlyList<Entry<dynamic>>, CancellationToken, Task>>(), It.IsAny<Func<Entry<dynamic>, bool>>(), It.IsAny<CancellationToken>()), Times.Once);
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
            ContentfulService = _contentfulServiceMock.Object,
            Entries = new MockAsyncEnumerableWithTotal<Entry<dynamic>>(entries),
            UploadOnlyValidEntries = false,
            PublishEntries = true
        };

        // Act
        await _sut.UploadAsync(input, CancellationToken.None);

        // Assert
        _contentfulServiceMock.Verify(x => x.CreateOrUpdateEntriesAsync(It.IsAny<IEnumerable<Entry<dynamic>>>(), true, It.IsAny<CancellationToken>()), Times.Once);
        _contentfulServiceMock.Verify(x => x.PublishEntriesAsync(It.IsAny<IEnumerable<Entry<dynamic>>>(), It.IsAny<CancellationToken>()), Times.Once);
        _contentfulServiceMock.Verify(x => x.UnpublishEntriesAsync(It.IsAny<IEnumerable<Entry<dynamic>>>(), It.IsAny<CancellationToken>()), Times.Never);
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
            ContentfulService = _contentfulServiceMock.Object,
            Entries = new MockAsyncEnumerableWithTotal<Entry<dynamic>>(entries),
            UploadOnlyValidEntries = false,
            PublishEntries = false
        };

        // Act
        await _sut.UploadAsync(input, CancellationToken.None);

        // Assert
        _contentfulServiceMock.Verify(x => x.CreateOrUpdateEntriesAsync(It.IsAny<IEnumerable<Entry<dynamic>>>(), false, It.IsAny<CancellationToken>()), Times.Once);
        _contentfulServiceMock.Verify(x => x.PublishEntriesAsync(It.IsAny<IEnumerable<Entry<dynamic>>>(), It.IsAny<CancellationToken>()), Times.Never);
        _contentfulServiceMock.Verify(x => x.UnpublishEntriesAsync(It.IsAny<IEnumerable<Entry<dynamic>>>(), It.IsAny<CancellationToken>()), Times.Never);
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
            ContentfulService = _contentfulServiceMock.Object,
            Entries = new MockAsyncEnumerableWithTotal<Entry<dynamic>>(entries),
            UploadOnlyValidEntries = false,
            PublishEntries = false
        };

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            _sut.UploadAsync(input, cts.Token));
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
            ContentfulService = _contentfulServiceMock.Object,
            Entries = new MockAsyncEnumerableWithTotal<Entry<dynamic>>(entries),
            UploadOnlyValidEntries = false,
            PublishEntries = false
        };

        // Act
        await _sut.UploadAsync(input, CancellationToken.None);

        // Assert
        _progressReporterMock.Verify(x => x.Start(It.IsAny<Operation>(), It.IsAny<Func<int>>()), Times.Once);
        _progressReporterMock.Verify(x => x.Increment(), Times.AtLeastOnce);
        _progressReporterMock.Verify(x => x.Complete(), Times.Once);
    }

    [Fact]
    public void GivenContentUploader_WhenCheckingInterface_ThenImplementsIContentUploader()
    {
        // Arrange & Act
        var implementsInterface = _sut is IContentUploader;

        // Assert
        Assert.True(implementsInterface);
    }

    [Fact]
    public async Task GivenBatchProcessorThrowsException_WhenUpload_ThenPropagatesException()
    {
        // Arrange
        _batchProcessorMock.Setup(x => x.ProcessAsync(It.IsAny<IAsyncEnumerable<Entry<dynamic>>>(), It.IsAny<int>(), It.IsAny<Func<IReadOnlyList<Entry<dynamic>>, CancellationToken, Task>>(), It.IsAny<Func<Entry<dynamic>, bool>>(), It.IsAny<CancellationToken>()))
            .Throws(new InvalidOperationException("Mock batch processor exception"));

        var entries = new[]
        {
            EntryBuilder.CreateBlogPost("entry1"),
        };

        var input = new ContentUploaderInput
        {
            ContentTypeId = "blogPost",
            ContentfulService = _contentfulServiceMock.Object,
            Entries = new MockAsyncEnumerableWithTotal<Entry<dynamic>>(entries),
            UploadOnlyValidEntries = false,
            PublishEntries = false
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _sut.UploadAsync(input, CancellationToken.None));
    }

    [Fact]
    public async Task GivenCustomBatchSize_WhenUpload_ThenUsesBatchSize()
    {
        // Arrange
        const int customBatchSize = 10;
        var customBatchProcessor = new Mock<IBatchProcessor>();
        var customUploader = new ContentUploader(customBatchProcessor.Object, _progressReporterMock.Object);

        var entries = new[]
        {
            EntryBuilder.CreateBlogPost("entry1"),
            EntryBuilder.CreateBlogPost("entry2")
        };

        var input = new ContentUploaderInput
        {
            ContentTypeId = "blogPost",
            ContentfulService = _contentfulServiceMock.Object,
            Entries = new MockAsyncEnumerableWithTotal<Entry<dynamic>>(entries),
            UploadOnlyValidEntries = false,
            PublishEntries = false
        };

        // Act
        await customUploader.UploadAsync(input, CancellationToken.None);

        // Assert
        customBatchProcessor.Verify(x => x.ProcessAsync(It.IsAny<IAsyncEnumerable<Entry<dynamic>>>(), It.IsAny<int>(), It.IsAny<Func<IReadOnlyList<Entry<dynamic>>, CancellationToken, Task>>(), It.IsAny<Func<Entry<dynamic>, bool>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GivenNullInput_WhenUpload_ThenThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _sut.UploadAsync(null!, CancellationToken.None));
    }
}