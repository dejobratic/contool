using Contentful.Core.Models;
using Contool.Core.Features.ContentDownload;
using Contool.Core.Infrastructure.Contentful.Services;
using Contool.Core.Infrastructure.IO.Services;
using Contool.Core.Infrastructure.Utils.Models;
using Contool.Core.Tests.Unit.Helpers;
using Contool.Core.Tests.Unit.Mocks;

namespace Contool.Core.Tests.Unit.Features.ContentDownload;

public class ContentDownloadCommandHandlerTests
{
    private readonly ContentDownloadCommandHandler _sut;
    
    private readonly MockContentEntrySerializerFactory _serializerFactoryMock = new();
    private readonly MockContentfulServiceBuilder _serviceBuilderMock = new();
    private readonly MockOutputWriterFactory _outputWriterFactoryMock = new();
    private readonly MockContentDownloader _contentDownloaderMock = new();

    public ContentDownloadCommandHandlerTests()
    {
        _sut = new ContentDownloadCommandHandler(
            _serializerFactoryMock,
            _serviceBuilderMock,
            _outputWriterFactoryMock,
            _contentDownloaderMock);
    }

    [Fact]
    public async Task GivenValidCommand_WhenHandleAsync_ThenProcessesCompleteWorkflow()
    {
        // Arrange
        var command = new ContentDownloadCommand
        {
            ContentTypeId = "blogPost",
            OutputPath = "/test/output",
            OutputFormat = "csv",
            SpaceId = "test-space",
            EnvironmentId = "test-environment"
        };

        var mockService = new MockContentfulService();
        var mockSerializer = new MockContentEntrySerializer();
        var mockOutputWriter = new MockOutputWriter();

        // Setup test entries
        var testEntries = CreateTestEntries();
        mockService.SetupEntries(testEntries);

        _serviceBuilderMock.SetupService(mockService);
        _serializerFactoryMock.SetupSerializer(mockSerializer);
        _outputWriterFactoryMock.SetupOutputWriter(mockOutputWriter);

        // Act
        await _sut.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(_serviceBuilderMock.BuildWasCalled);
        Assert.Equal(command.SpaceId, _serviceBuilderMock.LastSpaceId);
        Assert.Equal(command.EnvironmentId, _serviceBuilderMock.LastEnvironmentId);
        
        Assert.True(_serializerFactoryMock.CreateAsyncWasCalled);
        Assert.Equal(command.ContentTypeId, _serializerFactoryMock.LastContentTypeId);
        
        Assert.True(_outputWriterFactoryMock.CreateWasCalled);
        Assert.True(_contentDownloaderMock.DownloadAsyncWasCalled);
    }

    [Fact]
    public async Task GivenExcelFormat_WhenHandleAsync_ThenCreatesExcelOutputWriter()
    {
        // Arrange
        var command = new ContentDownloadCommand
        {
            ContentTypeId = "blogPost",
            OutputPath = "/test/output",
            OutputFormat = "excel",
            SpaceId = "test-space",
            EnvironmentId = "test-environment"
        };

        var mockService = new MockContentfulService();
        var mockSerializer = new MockContentEntrySerializer();
        var mockOutputWriter = new MockOutputWriter();

        // Setup test entries
        var testEntries = CreateTestEntries();
        mockService.SetupEntries(testEntries);

        _serviceBuilderMock.SetupService(mockService);
        _serializerFactoryMock.SetupSerializer(mockSerializer);
        _outputWriterFactoryMock.SetupOutputWriter(mockOutputWriter);

        // Act
        await _sut.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(_outputWriterFactoryMock.CreateWasCalled);
        Assert.NotNull(_outputWriterFactoryMock.LastDataSource);
    }

    [Fact]
    public async Task GivenJsonFormat_WhenHandleAsync_ThenCreatesJsonOutputWriter()
    {
        // Arrange
        var command = new ContentDownloadCommand
        {
            ContentTypeId = "blogPost",
            OutputPath = "/test/output",
            OutputFormat = "json",
            SpaceId = "test-space",
            EnvironmentId = "test-environment"
        };

        var mockService = new MockContentfulService();
        var mockSerializer = new MockContentEntrySerializer();
        var mockOutputWriter = new MockOutputWriter();

        // Setup test entries
        var testEntries = CreateTestEntries();
        mockService.SetupEntries(testEntries);

        _serviceBuilderMock.SetupService(mockService);
        _serializerFactoryMock.SetupSerializer(mockSerializer);
        _outputWriterFactoryMock.SetupOutputWriter(mockOutputWriter);

        // Act
        await _sut.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(_outputWriterFactoryMock.CreateWasCalled);
    }

    [Fact]
    public async Task GivenCancellationToken_WhenHandleAsync_ThenRespectsCancellation()
    {
        // Arrange
        var command = new ContentDownloadCommand
        {
            ContentTypeId = "blogPost",
            OutputPath = "/test/output",
            OutputFormat = "csv",
            SpaceId = "test-space",
            EnvironmentId = "test-environment"
        };

        var mockService = new MockContentfulService();
        var mockSerializer = new MockContentEntrySerializer();
        var mockOutputWriter = new MockOutputWriter();

        // Setup test entries
        var testEntries = CreateTestEntries();
        mockService.SetupEntries(testEntries);

        _serviceBuilderMock.SetupService(mockService);
        _serializerFactoryMock.SetupSerializer(mockSerializer);
        _outputWriterFactoryMock.SetupOutputWriter(mockOutputWriter);

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            _sut.HandleAsync(command, cts.Token));
    }

    [Fact]
    public async Task GivenSerializerFactoryThrowsException_WhenHandleAsync_ThenBubblesException()
    {
        // Arrange
        var command = new ContentDownloadCommand
        {
            ContentTypeId = "blogPost",
            OutputPath = "/test/output",
            OutputFormat = "csv",
            SpaceId = "test-space",
            EnvironmentId = "test-environment"
        };

        var mockService = new MockContentfulService();
        _serviceBuilderMock.SetupService(mockService);
        _serializerFactoryMock.SetupToThrow(new InvalidOperationException("Serializer creation failed"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _sut.HandleAsync(command, CancellationToken.None));
        
        Assert.Equal("Serializer creation failed", exception.Message);
    }

    [Fact]
    public async Task GivenContentDownloaderThrowsException_WhenHandleAsync_ThenBubblesException()
    {
        // Arrange
        var command = new ContentDownloadCommand
        {
            ContentTypeId = "blogPost",
            OutputPath = "/test/output",
            OutputFormat = "csv",
            SpaceId = "test-space",
            EnvironmentId = "test-environment"
        };

        var mockService = new MockContentfulService();
        var mockSerializer = new MockContentEntrySerializer();
        var mockOutputWriter = new MockOutputWriter();

        // Setup test entries
        var testEntries = CreateTestEntries();
        mockService.SetupEntries(testEntries);

        _serviceBuilderMock.SetupService(mockService);
        _serializerFactoryMock.SetupSerializer(mockSerializer);
        _outputWriterFactoryMock.SetupOutputWriter(mockOutputWriter);
        _contentDownloaderMock.SetupToThrow(new IOException("Download failed"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<IOException>(() =>
            _sut.HandleAsync(command, CancellationToken.None));
        
        Assert.Equal("Download failed", exception.Message);
    }

    [Fact]
    public async Task GivenValidCommand_WhenHandleAsync_ThenPassesCorrectInputToDownloader()
    {
        // Arrange
        var command = new ContentDownloadCommand
        {
            ContentTypeId = "blogPost",
            OutputPath = "/test/output",
            OutputFormat = "csv",
            SpaceId = "test-space",
            EnvironmentId = "test-environment"
        };

        var mockService = new MockContentfulService();
        var mockSerializer = new MockContentEntrySerializer();
        var mockOutputWriter = new MockOutputWriter();

        // Setup test entries
        var testEntries = CreateTestEntries();
        mockService.SetupEntries(testEntries);

        _serviceBuilderMock.SetupService(mockService);
        _serializerFactoryMock.SetupSerializer(mockSerializer);
        _outputWriterFactoryMock.SetupOutputWriter(mockOutputWriter);

        // Act
        await _sut.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(_contentDownloaderMock.DownloadAsyncWasCalled);
        Assert.NotNull(_contentDownloaderMock.LastInput);
        Assert.Equal(command.ContentTypeId, _contentDownloaderMock.LastInput.ContentTypeId);
        Assert.Equal(mockOutputWriter, _contentDownloaderMock.LastInput.OutputWriter);
        Assert.NotNull(_contentDownloaderMock.LastInput.Output);
    }

    private static IAsyncEnumerableWithTotal<Entry<dynamic>> CreateTestEntries()
    {
        var entries = new[]
        {
            EntryBuilder.CreateBlogPost("entry1", "blogPost"),
            EntryBuilder.CreateBlogPost("entry2", "blogPost"),
            EntryBuilder.CreateBlogPost("entry3", "blogPost")
        };

        return new AsyncEnumerableWithTotal<Entry<dynamic>>(
            source: AsyncEnumerableFactory.From(entries),
            getTotal: () => entries.Length);
    }
}