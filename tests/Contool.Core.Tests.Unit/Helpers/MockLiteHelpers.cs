using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Contool.Core.Features.ContentDelete;
using Contool.Core.Features.ContentDownload;
using Contool.Core.Features.ContentUpload;
using Contool.Core.Features.ContentUpload.Validation;
using Contool.Core.Infrastructure.Contentful.Services;
using Contool.Core.Infrastructure.IO.Models;
using Contool.Core.Infrastructure.IO.Services;
using Contool.Core.Infrastructure.Utils.Models;
using Contool.Core.Infrastructure.Utils.Services;
using Contool.Core.Infrastructure.Validation;
using MockLite;

namespace Contool.Core.Tests.Unit.Helpers;

public static class MockLiteHelpers
{
    public static Mock<IContentfulManagementClientAdapter> CreateContentfulManagementClientAdapterMock()
    {
        var mock = new Mock<IContentfulManagementClientAdapter>();
        
        // Set up default return values to prevent null reference exceptions
        mock.Setup(x => x.GetLocalesCollectionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Locale>());
        
        mock.Setup(x => x.GetContentTypeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ContentType?)null);
        
        mock.Setup(x => x.GetContentTypesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ContentType>());
        
        mock.Setup(x => x.GetEntriesCollectionAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ContentfulCollection<Entry<dynamic>>
            {
                Items = new List<Entry<dynamic>>(),
                Total = 0
            });
        
        mock.Setup(x => x.DeactivateContentTypeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        mock.Setup(x => x.DeleteContentTypeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        return mock;
    }
    
    public static Mock<IContentfulEntryOperationService> CreateContentfulEntryOperationServiceMock()
    {
        var mock = new Mock<IContentfulEntryOperationService>();
        
        // Set up default return values
        mock.Setup(x => x.CreateOrUpdateEntryAsync(
                It.IsAny<Entry<dynamic>>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(OperationResult.Success("test-id", Operation.Upload));
        
        mock.Setup(x => x.PublishEntryAsync(It.IsAny<Entry<dynamic>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(OperationResult.Success("test-id", Operation.Publish));
        
        mock.Setup(x => x.UnpublishEntryAsync(It.IsAny<Entry<dynamic>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(OperationResult.Success("test-id", Operation.Unpublish));
        
        mock.Setup(x => x.DeleteEntryAsync(It.IsAny<Entry<dynamic>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(OperationResult.Success("test-id", Operation.Delete));
        
        return mock;
    }
    
    public static void SetupDefaults(this Mock<IContentfulService> mock)
    {
        // Set up default return values
        mock.Setup(x => x.GetLocalesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Locale>());
        
        mock.Setup(x => x.GetContentTypeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ContentType?)null);
        
        mock.Setup(x => x.GetContentTypesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ContentType>());
        
        mock.Setup(x => x.CreateOrUpdateEntriesAsync(
                It.IsAny<IReadOnlyList<Entry<dynamic>>>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        mock.Setup(x => x.PublishEntriesAsync(
                It.IsAny<IReadOnlyList<Entry<dynamic>>>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        mock.Setup(x => x.UnpublishEntriesAsync(
                It.IsAny<IReadOnlyList<Entry<dynamic>>>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        mock.Setup(x => x.DeleteEntriesAsync(
                It.IsAny<IReadOnlyList<Entry<dynamic>>>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        mock.Setup(x => x.DeleteContentTypeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
    }
    
    public static void SetupDefaults(this Mock<IBatchProcessor> mock)
    {
        // Set up default behavior - note the generic method will be set up per test
    }
    
    public static void SetupDefaults(this Mock<IProgressReporter> mock)
    {
        // Set up default behavior - all methods are void, no setup needed
    }
    
    public static Mock<IInputReaderFactory> SetupDefaults(this Mock<IInputReaderFactory> mock)
    {
        // Default behavior can be set up per test
        return mock;
    }
    
    public static void SetupDefaults(
        this Mock<IContentfulServiceBuilder> mock,
        Mock<IContentfulService> contentfulService)
    {
        // Set up builder pattern
        mock.Setup(x => x.WithSpaceId(It.IsAny<string>()))
            .Returns(mock.Object);
        
        mock.Setup(x => x.WithEnvironmentId(It.IsAny<string>()))
            .Returns(mock.Object);
        
        mock.Setup(x => x.WithPreviewApi(It.IsAny<bool>()))
            .Returns(mock.Object);
        
        mock.Setup(x => x.Build())
            .Returns(contentfulService.Object);
    }
    
    public static Mock<IContentEntryDeserializerFactory> CreateContentEntryDeserializerFactoryMock()
    {
        var mock = new Mock<IContentEntryDeserializerFactory>();
        
        // Default behavior can be set up per test
        return mock;
    }
    
    public static Mock<IContentUploader> CreateContentUploaderMock()
    {
        var mock = new Mock<IContentUploader>();
        
        // Set up default behavior
        mock.Setup(x => x.UploadAsync(It.IsAny<ContentUploaderInput>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        return mock;
    }
    
    public static Mock<IContentDeleter> SetupDefaults(this Mock<IContentDeleter> mock)
    {
        mock.Setup(x => x.DeleteAsync(It.IsAny<ContentDeleterInput>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        return mock;
    }
    
    public static Mock<IContentDownloader> CreateContentDownloaderMock()
    {
        var mock = new Mock<IContentDownloader>();
        
        mock.Setup(x => x.DownloadAsync(It.IsAny<ContentDownloaderInput>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        return mock;
    }
    
    public static Mock<IContentEntryDeserializer> CreateContentEntryDeserializerMock()
    {
        var mock = new Mock<IContentEntryDeserializer>();
        
        // Set up default behavior to return a basic entry
        mock.Setup(x => x.Deserialize(It.IsAny<object>()))
            .Returns(new Entry<dynamic> { Fields = new Dictionary<string, dynamic>() });
        
        return mock;
    }
    
    public static Mock<IContentEntrySerializer> CreateContentEntrySerializerMock()
    {
        var mock = new Mock<IContentEntrySerializer>();
        
        // Set up default behavior
        mock.Setup(x => x.FieldNames)
            .Returns(new[] { "id", "title" });
        mock.Setup(x => x.Serialize(It.IsAny<Entry<dynamic>>()))
            .Returns(new { id = "test", title = "Test" });
        
        return mock;
    }
    
    public static Mock<IContentEntrySerializerFactory> CreateContentEntrySerializerFactoryMock()
    {
        var mock = new Mock<IContentEntrySerializerFactory>();
        
        var serializerMock = CreateContentEntrySerializerMock();
        mock.Setup(x => x.CreateAsync(It.IsAny<string>(), It.IsAny<IContentfulService>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(serializerMock.Object);
        
        return mock;
    }
    
    public static Mock<IContentUploadEntryValidator> CreateContentUploadEntryValidatorMock()
    {
        var mock = new Mock<IContentUploadEntryValidator>();
        
        // Set up default behavior to return valid result
        mock.Setup(x => x.ValidateAsync(It.IsAny<ContentUploaderInput>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new EntryValidationSummary());
        
        return mock;
    }
    
    public static Mock<IContentfulManagementClientAdapterFactory> CreateContentfulManagementClientAdapterFactoryMock()
    {
        var mock = new Mock<IContentfulManagementClientAdapterFactory>();
        
        var adapterMock = CreateContentfulManagementClientAdapterMock();
        mock.Setup(x => x.Create(It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<bool>()))
            .Returns(adapterMock.Object);
        
        return mock;
    }
    
    public static Mock<IInputReader> CreateInputReaderMock()
    {
        var mock = new Mock<IInputReader>();
        
        // Set up default behavior
        mock.Setup(x => x.DataSource)
            .Returns(new FileDataSource("test", ".csv"));
        
        return mock;
    }
    
    public static Mock<IOutputWriter> CreateOutputWriterMock()
    {
        var mock = new Mock<IOutputWriter>();
        
        // Set up default behavior
        mock.Setup(x => x.DataSource)
            .Returns(new FileDataSource("test", ".csv"));
        mock.Setup(x => x.SaveAsync(It.IsAny<string>(), It.IsAny<IAsyncEnumerable<dynamic>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        return mock;
    }
    
    public static Mock<IOutputWriterFactory> CreateOutputWriterFactoryMock()
    {
        var mock = new Mock<IOutputWriterFactory>();
        
        var writerMock = CreateOutputWriterMock();
        mock.Setup(x => x.Create(It.IsAny<DataSource>()))
            .Returns(writerMock.Object);
        
        return mock;
    }
    
    public static Mock<IOperationTracker> CreateOperationTrackerMock()
    {
        var mock = new Mock<IOperationTracker>();
        
        // Set up default behavior
        mock.Setup(x => x.GetResult())
            .Returns(new OperationTrackResult
            {
                TotalEntries = 0,
                SuccessfulEntries = 0,
                Operations = new Dictionary<Operation, (int SuccessCount, int ErrorCount)>()
            });
        
        return mock;
    }
    
    public static Mock<IRuntimeContext> CreateRuntimeContextMock()
    {
        var mock = new Mock<IRuntimeContext>();
        
        // Set up default behavior
        mock.Setup(x => x.IsDryRun)
            .Returns(false);
        
        return mock;
    }
    
    public static Mock<IContentfulEntryOperationServiceFactory> CreateContentfulEntryOperationServiceFactoryMock()
    {
        var mock = new Mock<IContentfulEntryOperationServiceFactory>();
        
        var entryOperationServiceMock = CreateContentfulEntryOperationServiceMock();
        mock.Setup(x => x.Create(It.IsAny<IContentfulManagementClientAdapter>()))
            .Returns(entryOperationServiceMock.Object);
        
        return mock;
    }
}