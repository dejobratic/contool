using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Contool.Core.Features;
using Contool.Core.Features.ContentUpload;
using Contool.Core.Infrastructure.Contentful.Models;
using Contool.Core.Infrastructure.Contentful.Services;
using Contool.Core.Infrastructure.IO.Models;
using Contool.Core.Infrastructure.IO.Services;
using Contool.Core.Infrastructure.Utils.Models;
using Contool.Core.Tests.Unit.Helpers;
using Contool.Core.Tests.Unit.Mocks;

namespace Contool.Core.Tests.Unit.Features.ContentUpload;

public class ContentUploadCommandHandlerTests
{
    private readonly ContentUploadCommandHandler _handler;
    private readonly MockInputReaderFactory _mockInputReaderFactory;
    private readonly MockContentfulServiceBuilder _mockContentfulServiceBuilder;
    private readonly MockContentEntryDeserializerFactory _mockDeserializerFactory;
    private readonly MockContentUploader _mockContentUploader;
    private readonly MockContentfulService _mockContentfulService;

    public ContentUploadCommandHandlerTests()
    {
        _mockInputReaderFactory = new MockInputReaderFactory();
        _mockContentfulServiceBuilder = new MockContentfulServiceBuilder();
        _mockDeserializerFactory = new MockContentEntryDeserializerFactory();
        _mockContentUploader = new MockContentUploader();
        _mockContentfulService = new MockContentfulService();
        
        _handler = new ContentUploadCommandHandler(
            _mockInputReaderFactory,
            _mockContentfulServiceBuilder,
            _mockDeserializerFactory,
            _mockContentUploader);
    }

    [Fact]
    public async Task GivenValidCsvCommand_WhenHandle_ThenProcessesCompleteWorkflow()
    {
        // Arrange
        var command = new ContentUploadCommand
        {
            ContentTypeId = "blogPost",
            InputPath = "test.csv",
            UploadOnlyValid = false,
            PublishUploaded = false,
            SpaceId = "spaceId",
            EnvironmentId = "environmentId"
        };

        var csvData = new Dictionary<string, object?>[]
        {
            new() { ["title"] = "Test Post 1", ["content"] = "Test content 1" },
            new() { ["title"] = "Test Post 2", ["content"] = "Test content 2" }
        };

        _mockInputReaderFactory.SetupReader(new MockInputReader(csvData));
        _mockContentfulServiceBuilder.SetupService(_mockContentfulService);
        _mockDeserializerFactory.SetupDeserializer(new MockContentEntryDeserializer());

        // Act
        await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(_mockInputReaderFactory.CreateWasCalled);
        Assert.True(_mockContentfulServiceBuilder.BuildWasCalled);
        Assert.True(_mockDeserializerFactory.CreateAsyncWasCalled);
        Assert.True(_mockContentUploader.UploadAsyncWasCalled);
        
        Assert.Equal("blogPost", _mockDeserializerFactory.LastContentTypeId);
        Assert.Equal("spaceId", _mockContentfulServiceBuilder.LastSpaceId);
        Assert.Equal("environmentId", _mockContentfulServiceBuilder.LastEnvironmentId);
        
        var uploaderInput = _mockContentUploader.LastInput;
        Assert.NotNull(uploaderInput);
        Assert.Equal("blogPost", uploaderInput.ContentTypeId);
        Assert.False(uploaderInput.UploadOnlyValidEntries);
        Assert.False(uploaderInput.PublishEntries);
        Assert.Same(_mockContentfulService, uploaderInput.ContentfulService);
    }

    [Fact]
    public async Task GivenExcelCommand_WhenHandle_ThenCreatesCorrectDataSource()
    {
        // Arrange
        var command = new ContentUploadCommand
        {
            ContentTypeId = "blogPost",
            InputPath = "test.xlsx",
            UploadOnlyValid = false,
            PublishUploaded = false,
            SpaceId = "spaceId",
            EnvironmentId = "environmentId"
        };

        var excelData = new Dictionary<string, object?>[]
        {
            new() { ["title"] = "Excel Post", ["content"] = "Excel content" }
        };

        _mockInputReaderFactory.SetupReader(new MockInputReader(excelData));
        _mockContentfulServiceBuilder.SetupService(_mockContentfulService);
        _mockDeserializerFactory.SetupDeserializer(new MockContentEntryDeserializer());

        // Act
        await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(_mockInputReaderFactory.CreateWasCalled);
        Assert.Equal(".xlsx", ((FileDataSource)_mockInputReaderFactory.LastDataSource!).Extension);
    }

    [Fact]
    public async Task GivenJsonCommand_WhenHandle_ThenCreatesCorrectDataSource()
    {
        // Arrange
        var command = new ContentUploadCommand
        {
            ContentTypeId = "blogPost",
            InputPath = "test.json",
            UploadOnlyValid = false,
            PublishUploaded = false,
            SpaceId = "spaceId",
            EnvironmentId = "environmentId"
        };

        var jsonData = new Dictionary<string, object?>[]
        {
            new() { ["title"] = "JSON Post", ["content"] = "JSON content" }
        };

        _mockInputReaderFactory.SetupReader(new MockInputReader(jsonData));
        _mockContentfulServiceBuilder.SetupService(_mockContentfulService);
        _mockDeserializerFactory.SetupDeserializer(new MockContentEntryDeserializer());

        // Act
        await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(_mockInputReaderFactory.CreateWasCalled);
        Assert.Equal(".json", ((FileDataSource)_mockInputReaderFactory.LastDataSource!).Extension);
    }

    [Fact]
    public async Task GivenCommandWithUploadOnlyValid_WhenHandle_ThenPassesCorrectFlag()
    {
        // Arrange
        var command = new ContentUploadCommand
        {
            ContentTypeId = "blogPost",
            InputPath = "test.csv",
            UploadOnlyValid = true,
            PublishUploaded = false,
            SpaceId = "spaceId",
            EnvironmentId = "environmentId"
        };

        var csvData = new Dictionary<string, object?>[]
        {
            new() { ["title"] = "Test Post", ["content"] = "Test content" }
        };

        _mockInputReaderFactory.SetupReader(new MockInputReader(csvData));
        _mockContentfulServiceBuilder.SetupService(_mockContentfulService);
        _mockDeserializerFactory.SetupDeserializer(new MockContentEntryDeserializer());

        // Act
        await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        var uploaderInput = _mockContentUploader.LastInput;
        Assert.NotNull(uploaderInput);
        Assert.True(uploaderInput.UploadOnlyValidEntries);
    }

    [Fact]
    public async Task GivenCommandWithPublishUploaded_WhenHandle_ThenPassesCorrectFlag()
    {
        // Arrange
        var command = new ContentUploadCommand
        {
            ContentTypeId = "blogPost",
            InputPath = "test.csv",
            UploadOnlyValid = false,
            PublishUploaded = true,
            SpaceId = "spaceId",
            EnvironmentId = "environmentId"
        };

        var csvData = new Dictionary<string, object?>[]
        {
            new() { ["title"] = "Test Post", ["content"] = "Test content" }
        };

        _mockInputReaderFactory.SetupReader(new MockInputReader(csvData));
        _mockContentfulServiceBuilder.SetupService(_mockContentfulService);
        _mockDeserializerFactory.SetupDeserializer(new MockContentEntryDeserializer());

        // Act
        await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        var uploaderInput = _mockContentUploader.LastInput;
        Assert.NotNull(uploaderInput);
        Assert.True(uploaderInput.PublishEntries);
    }

    [Fact]
    public async Task GivenLargeDataSet_WhenHandle_ThenPreservesTotalCount()
    {
        // Arrange
        var command = new ContentUploadCommand
        {
            ContentTypeId = "blogPost",
            InputPath = "test.csv",
            UploadOnlyValid = false,
            PublishUploaded = false,
            SpaceId = "spaceId",
            EnvironmentId = "environmentId"
        };

        const int itemCount = 1000;
        var csvData = Enumerable.Range(1, itemCount)
            .Select(i => new Dictionary<string, object?>
            {
                ["title"] = $"Test Post {i}",
                ["content"] = $"Test content {i}"
            })
            .ToArray();

        _mockInputReaderFactory.SetupReader(new MockInputReader(csvData));
        _mockContentfulServiceBuilder.SetupService(_mockContentfulService);
        _mockDeserializerFactory.SetupDeserializer(new MockContentEntryDeserializer());

        // Act
        await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        var uploaderInput = _mockContentUploader.LastInput;
        Assert.NotNull(uploaderInput);
        Assert.Equal(itemCount, uploaderInput.Entries.Total);
    }

    [Fact]
    public async Task GivenCancellationToken_WhenCancelled_ThenPropagatesCorrectly()
    {
        // Arrange
        var command = new ContentUploadCommand
        {
            ContentTypeId = "blogPost",
            InputPath = "test.csv",
            UploadOnlyValid = false,
            PublishUploaded = false,
            SpaceId = "spaceId",
            EnvironmentId = "environmentId"
        };

        var csvData = new Dictionary<string, object?>[]
        {
            new() { ["title"] = "Test Post", ["content"] = "Test content" }
        };

        _mockInputReaderFactory.SetupReader(new MockInputReader(csvData));
        _mockContentfulServiceBuilder.SetupService(_mockContentfulService);
        _mockDeserializerFactory.SetupDeserializer(new MockContentEntryDeserializer());

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            _handler.HandleAsync(command, cts.Token));
    }

    [Fact]
    public async Task GivenEmptyDataSet_WhenHandle_ThenHandlesGracefully()
    {
        // Arrange
        var command = new ContentUploadCommand
        {
            ContentTypeId = "blogPost",
            InputPath = "test.csv",
            UploadOnlyValid = false,
            PublishUploaded = false,
            SpaceId = "spaceId",
            EnvironmentId = "environmentId"
        };

        var emptyData = Array.Empty<Dictionary<string, object?>>();

        _mockInputReaderFactory.SetupReader(new MockInputReader(emptyData));
        _mockContentfulServiceBuilder.SetupService(_mockContentfulService);
        _mockDeserializerFactory.SetupDeserializer(new MockContentEntryDeserializer());

        // Act
        await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(_mockContentUploader.UploadAsyncWasCalled);
        var uploaderInput = _mockContentUploader.LastInput;
        Assert.NotNull(uploaderInput);
        Assert.Equal(0, uploaderInput.Entries.Total);
    }

    [Fact]
    public async Task GivenValidCommand_WhenHandle_ThenDeserializesEntriesCorrectly()
    {
        // Arrange
        var command = new ContentUploadCommand
        {
            ContentTypeId = "blogPost",
            InputPath = "test.csv",
            UploadOnlyValid = false,
            PublishUploaded = false,
            SpaceId = "spaceId",
            EnvironmentId = "environmentId"
        };

        var csvData = new Dictionary<string, object?>[]
        {
            new() { ["title"] = "Test Post 1", ["content"] = "Test content 1" },
            new() { ["title"] = "Test Post 2", ["content"] = "Test content 2" }
        };

        var mockDeserializer = new MockContentEntryDeserializer();
        _mockInputReaderFactory.SetupReader(new MockInputReader(csvData));
        _mockContentfulServiceBuilder.SetupService(_mockContentfulService);
        _mockDeserializerFactory.SetupDeserializer(mockDeserializer);

        // Act
        await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.Equal(2, mockDeserializer.DeserializeCallCount);
        Assert.Contains(csvData[0], mockDeserializer.DeserializedRows);
        Assert.Contains(csvData[1], mockDeserializer.DeserializedRows);
    }

    [Fact]
    public void GivenHandler_WhenCheckingInterface_ThenImplementsICommandHandler()
    {
        // Arrange & Act
        var implementsInterface = _handler is ICommandHandler<ContentUploadCommand>;

        // Assert
        Assert.True(implementsInterface);
    }
}