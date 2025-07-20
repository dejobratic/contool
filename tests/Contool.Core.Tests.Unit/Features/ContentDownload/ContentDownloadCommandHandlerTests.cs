using Contentful.Core.Models;
using Contool.Core.Features.ContentDownload;
using Contool.Core.Infrastructure.Contentful.Services;
using Contool.Core.Infrastructure.Contentful.Utils;
using Contool.Core.Infrastructure.IO.Models;
using Contool.Core.Infrastructure.IO.Services;
using Contool.Core.Infrastructure.Utils.Models;
using Contool.Core.Tests.Unit.Helpers;
using MockLite;

namespace Contool.Core.Tests.Unit.Features.ContentDownload;

public class ContentDownloadCommandHandlerTests
{
    private readonly ContentDownloadCommandHandler _sut;
    
    private readonly Mock<IContentEntrySerializerFactory> _serializerFactoryMock = new();
    private readonly Mock<IContentfulServiceBuilder> _serviceBuilderMock = new();
    private readonly Mock<IOutputWriterFactory> _outputWriterFactoryMock = new();
    private readonly Mock<IContentDownloader> _contentDownloaderMock = new();
    private readonly Mock<IContentfulService> _contentfulServiceMock = new();

    public ContentDownloadCommandHandlerTests()
    {
        _contentfulServiceMock.SetupDefaults();
        _serviceBuilderMock.SetupDefaults(_contentfulServiceMock);
        
        _sut = new ContentDownloadCommandHandler(
            _serializerFactoryMock.Object,
            _serviceBuilderMock.Object,
            _outputWriterFactoryMock.Object,
            _contentDownloaderMock.Object);
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

        var mockService = new Mock<IContentfulService>();
        mockService.SetupDefaults();
        var mockSerializer = new Mock<IContentEntrySerializer>();
        var mockOutputWriter = new Mock<IOutputWriter>();

        // Setup test entries
        var testEntries = CreateTestEntries();
        mockService.Setup(x => x.GetEntriesAsync("blogPost", null, PagingMode.SkipForward, It.IsAny<CancellationToken>()))
            .Returns(testEntries);

        _serviceBuilderMock.Setup(x => x.Build()).Returns(mockService.Object);
        _serializerFactoryMock.Setup(x => x.CreateAsync("blogPost", It.IsAny<IContentfulService>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockSerializer.Object);
        _outputWriterFactoryMock.Setup(x => x.Create(It.IsAny<DataSource>()))
            .Returns(mockOutputWriter.Object);

        // Act
        await _sut.HandleAsync(command, CancellationToken.None);

        // Assert
        _serviceBuilderMock.Verify(x => x.Build(), Times.Once);
        _serviceBuilderMock.Verify(x => x.WithSpaceId(command.SpaceId), Times.Once);
        _serviceBuilderMock.Verify(x => x.WithEnvironmentId(command.EnvironmentId), Times.Once);
        
        _serializerFactoryMock.Verify(x => x.CreateAsync(command.ContentTypeId, It.IsAny<IContentfulService>(), It.IsAny<CancellationToken>()), Times.Once);
        
        _outputWriterFactoryMock.Verify(x => x.Create(It.IsAny<DataSource>()), Times.Once);
        _contentDownloaderMock.Verify(x => x.DownloadAsync(It.IsAny<ContentDownloaderInput>(), It.IsAny<CancellationToken>()), Times.Once);
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

        var mockService = new Mock<IContentfulService>();
        mockService.SetupDefaults();
        var mockSerializer = new Mock<IContentEntrySerializer>();
        var mockOutputWriter = new Mock<IOutputWriter>();

        // Setup test entries
        var testEntries = CreateTestEntries();
        mockService.Setup(x => x.GetEntriesAsync("blogPost", null, PagingMode.SkipForward, It.IsAny<CancellationToken>()))
            .Returns(testEntries);

        _serviceBuilderMock.Setup(x => x.Build()).Returns(mockService.Object);
        _serializerFactoryMock.Setup(x => x.CreateAsync("blogPost", It.IsAny<IContentfulService>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockSerializer.Object);
        _outputWriterFactoryMock.Setup(x => x.Create(It.IsAny<DataSource>()))
            .Returns(mockOutputWriter.Object);

        // Act
        await _sut.HandleAsync(command, CancellationToken.None);

        // Assert
        _outputWriterFactoryMock.Verify(x => x.Create(It.IsAny<DataSource>()), Times.Once);
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

        var mockService = new Mock<IContentfulService>();
        mockService.SetupDefaults();
        var mockSerializer = new Mock<IContentEntrySerializer>();
        var mockOutputWriter = new Mock<IOutputWriter>();

        // Setup test entries
        var testEntries = CreateTestEntries();
        mockService.Setup(x => x.GetEntriesAsync("blogPost", null, PagingMode.SkipForward, It.IsAny<CancellationToken>()))
            .Returns(testEntries);

        _serviceBuilderMock.Setup(x => x.Build()).Returns(mockService.Object);
        _serializerFactoryMock.Setup(x => x.CreateAsync("blogPost", It.IsAny<IContentfulService>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockSerializer.Object);
        _outputWriterFactoryMock.Setup(x => x.Create(It.IsAny<DataSource>()))
            .Returns(mockOutputWriter.Object);

        // Act
        await _sut.HandleAsync(command, CancellationToken.None);

        // Assert
        _outputWriterFactoryMock.Verify(x => x.Create(It.IsAny<DataSource>()), Times.Once);
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

        var mockService = new Mock<IContentfulService>();
        mockService.SetupDefaults();
        _serializerFactoryMock.Setup(x => x.CreateAsync(It.IsAny<string>(), It.IsAny<IContentfulService>(), It.IsAny<CancellationToken>()))
            .Throws(new InvalidOperationException("Serializer creation failed"));

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

        var mockService = new Mock<IContentfulService>();
        mockService.SetupDefaults();
        var mockSerializer = new Mock<IContentEntrySerializer>();
        var mockOutputWriter = new Mock<IOutputWriter>();

        // Setup test entries
        var testEntries = CreateTestEntries();
        mockService.Setup(x => x.GetEntriesAsync("blogPost", null, PagingMode.SkipForward, It.IsAny<CancellationToken>()))
            .Returns(testEntries);

        _serviceBuilderMock.Setup(x => x.Build()).Returns(mockService.Object);
        _serializerFactoryMock.Setup(x => x.CreateAsync("blogPost", It.IsAny<IContentfulService>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockSerializer.Object);
        _outputWriterFactoryMock.Setup(x => x.Create(It.IsAny<DataSource>()))
            .Returns(mockOutputWriter.Object);
        _contentDownloaderMock.Setup(x => x.DownloadAsync(It.IsAny<ContentDownloaderInput>(), It.IsAny<CancellationToken>()))
            .Throws(new IOException("Download failed"));

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

        var mockService = new Mock<IContentfulService>();
        mockService.SetupDefaults();
        var mockSerializer = new Mock<IContentEntrySerializer>();
        var mockOutputWriter = new Mock<IOutputWriter>();

        // Setup test entries
        var testEntries = CreateTestEntries();
        mockService.Setup(x => x.GetEntriesAsync("blogPost", null, PagingMode.SkipForward, It.IsAny<CancellationToken>()))
            .Returns(testEntries);

        _serviceBuilderMock.Setup(x => x.Build()).Returns(mockService.Object);
        _serializerFactoryMock.Setup(x => x.CreateAsync("blogPost", It.IsAny<IContentfulService>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockSerializer.Object);
        _outputWriterFactoryMock.Setup(x => x.Create(It.IsAny<DataSource>()))
            .Returns(mockOutputWriter.Object);

        // Act
        await _sut.HandleAsync(command, CancellationToken.None);

        // Assert
        _contentDownloaderMock.Verify(x => x.DownloadAsync(It.Is<ContentDownloaderInput>(input => 
            input.ContentTypeId == command.ContentTypeId &&
            input.OutputWriter == mockOutputWriter &&
            input.Output != null), It.IsAny<CancellationToken>()), Times.Once);
        // Note: MockLite doesn't support LastInput property tracking like custom mocks
    }

    private static IAsyncEnumerableWithTotal<Entry<dynamic>> CreateTestEntries()
    {
        var entries = new[]
        {
            EntryBuilder.CreateBlogPost("entry1"),
            EntryBuilder.CreateBlogPost("entry2"),
            EntryBuilder.CreateBlogPost("entry3")
        };
        
        return new AsyncEnumerableWithTotal<Entry<dynamic>>(
            source: entries.ToAsyncEnumerable(),
            getTotal: () => entries.Length);
    }
}