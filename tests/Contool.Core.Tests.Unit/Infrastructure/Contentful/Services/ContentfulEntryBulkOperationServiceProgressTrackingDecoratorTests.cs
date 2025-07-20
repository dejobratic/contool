using Contentful.Core.Models;
using Contool.Core.Infrastructure.Contentful.Services;
using Contool.Core.Infrastructure.Utils.Models;
using Contool.Core.Infrastructure.Utils.Services;
using Contool.Core.Tests.Unit.Helpers;
using MockLite;

namespace Contool.Core.Tests.Unit.Infrastructure.Contentful.Services;

public class ContentfulEntryBulkOperationServiceProgressTrackingDecoratorTests
{
    private readonly ContentfulEntryBulkOperationServiceProgressTrackingDecorator _sut;
    
    private readonly Mock<IContentfulEntryBulkOperationService> _innerMock = new Mock<IContentfulEntryBulkOperationService>();
    private readonly Mock<IOperationTracker> _operationTrackerMock = new Mock<IOperationTracker>();
    private readonly Mock<IProgressReporter> _progressReporterMock = new Mock<IProgressReporter>();

    public ContentfulEntryBulkOperationServiceProgressTrackingDecoratorTests()
    {
        _sut = new ContentfulEntryBulkOperationServiceProgressTrackingDecorator(
            _innerMock.Object, 
            _operationTrackerMock.Object, 
            _progressReporterMock.Object);
    }

    [Fact]
    public async Task GivenValidEntries_WhenPublishEntriesAsync_ThenReportsProgressForEachResult()
    {
        // Arrange
        var entries = new List<Entry<dynamic>>
        {
            EntryBuilder.CreateBlogPost("entry1"),
            EntryBuilder.CreateBlogPost("entry2")
        };
        
        var expectedResults = new List<OperationResult>
        {
            OperationResult.Success("entry1", Operation.Publish),
            OperationResult.Success("entry2", Operation.Publish)
        };
        
        _innerMock.Setup(x => x.PublishEntriesAsync(entries, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResults);

        // Act
        var actual = await _sut.PublishEntriesAsync(entries, CancellationToken.None);

        // Assert
        _innerMock.Verify(x => x.PublishEntriesAsync(entries, It.IsAny<CancellationToken>()), Times.Once);
        _progressReporterMock.Verify(x => x.Increment(), Times.Exactly(expectedResults.Count));
        _operationTrackerMock.Verify(x => x.IncrementErrorCount(It.IsAny<Operation>(), It.IsAny<string>()), Times.Never);
        Assert.Equal(expectedResults, actual);
    }

    [Fact]
    public async Task GivenValidEntries_WhenUnpublishEntriesAsync_ThenReportsProgressForEachResult()
    {
        // Arrange
        var entries = new List<Entry<dynamic>>
        {
            EntryBuilder.CreateBlogPost("entry1"),
            EntryBuilder.CreateBlogPost("entry2")
        };
        
        var expectedResults = new List<OperationResult>
        {
            OperationResult.Success("entry1", Operation.Unpublish),
            OperationResult.Success("entry2", Operation.Unpublish)
        };
        
        _innerMock.Setup(x => x.UnpublishEntriesAsync(entries, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResults);

        // Act
        var actual = await _sut.UnpublishEntriesAsync(entries, CancellationToken.None);

        // Assert
        _innerMock.Verify(x => x.UnpublishEntriesAsync(entries, It.IsAny<CancellationToken>()), Times.Once);
        _progressReporterMock.Verify(x => x.Increment(), Times.Exactly(expectedResults.Count));
        _operationTrackerMock.Verify(x => x.IncrementErrorCount(It.IsAny<Operation>(), It.IsAny<string>()), Times.Never);
        Assert.Equal(expectedResults, actual);
    }

    [Fact]
    public async Task GivenEntriesWithFailures_WhenPublishEntriesAsync_ThenTracksErrorsAndReportsProgress()
    {
        // Arrange
        var entries = new List<Entry<dynamic>>
        {
            EntryBuilder.CreateBlogPost("entry1"),
            EntryBuilder.CreateBlogPost("entry2")
        };
        
        var exception = new InvalidOperationException("Test exception");
        var expectedResults = new List<OperationResult>
        {
            OperationResult.Success("entry1", Operation.Publish),
            OperationResult.Failure("entry2", Operation.Publish, exception)
        };
        
        _innerMock.Setup(x => x.PublishEntriesAsync(entries, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResults);

        // Act
        var actual = await _sut.PublishEntriesAsync(entries, CancellationToken.None);

        // Assert
        _innerMock.Verify(x => x.PublishEntriesAsync(entries, It.IsAny<CancellationToken>()), Times.Once);
        _progressReporterMock.Verify(x => x.Increment(), Times.Exactly(expectedResults.Count));
        _operationTrackerMock.Verify(x => x.IncrementErrorCount(Operation.Publish, "entry2"), Times.Once);
        Assert.Equal(expectedResults, actual);
    }

    [Fact]
    public async Task GivenEntriesWithFailures_WhenUnpublishEntriesAsync_ThenTracksErrorsAndReportsProgress()
    {
        // Arrange
        var entries = new List<Entry<dynamic>>
        {
            EntryBuilder.CreateBlogPost("entry1"),
            EntryBuilder.CreateBlogPost("entry2")
        };
        
        var exception = new InvalidOperationException("Test exception");
        var expectedResults = new List<OperationResult>
        {
            OperationResult.Success("entry1", Operation.Unpublish),
            OperationResult.Failure("entry2", Operation.Unpublish, exception)
        };
        
        _innerMock.Setup(x => x.UnpublishEntriesAsync(entries, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResults);

        // Act
        var actual = await _sut.UnpublishEntriesAsync(entries, CancellationToken.None);

        // Assert
        _innerMock.Verify(x => x.UnpublishEntriesAsync(entries, It.IsAny<CancellationToken>()), Times.Once);
        _progressReporterMock.Verify(x => x.Increment(), Times.Exactly(expectedResults.Count));
        _operationTrackerMock.Verify(x => x.IncrementErrorCount(Operation.Unpublish, "entry2"), Times.Once);
        Assert.Equal(expectedResults, actual);
    }

    [Fact]
    public async Task GivenEmptyEntries_WhenPublishEntriesAsync_ThenDoesNotReportProgress()
    {
        // Arrange
        var entries = Array.Empty<Entry<dynamic>>();
        var expectedResults = Array.Empty<OperationResult>();
        
        _innerMock.Setup(x => x.PublishEntriesAsync(entries, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResults);

        // Act
        var actual = await _sut.PublishEntriesAsync(entries, CancellationToken.None);

        // Assert
        _innerMock.Verify(x => x.PublishEntriesAsync(entries, It.IsAny<CancellationToken>()), Times.Once);
        _progressReporterMock.Verify(x => x.Increment(), Times.Never);
        _operationTrackerMock.Verify(x => x.IncrementErrorCount(It.IsAny<Operation>(), It.IsAny<string>()), Times.Never);
        Assert.Equal(expectedResults, actual);
    }

    [Fact]
    public async Task GivenEmptyEntries_WhenUnpublishEntriesAsync_ThenDoesNotReportProgress()
    {
        // Arrange
        var entries = Array.Empty<Entry<dynamic>>();
        var expectedResults = Array.Empty<OperationResult>();
        
        _innerMock.Setup(x => x.UnpublishEntriesAsync(entries, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResults);

        // Act
        var actual = await _sut.UnpublishEntriesAsync(entries, CancellationToken.None);

        // Assert
        _innerMock.Verify(x => x.UnpublishEntriesAsync(entries, It.IsAny<CancellationToken>()), Times.Once);
        _progressReporterMock.Verify(x => x.Increment(), Times.Never);
        _operationTrackerMock.Verify(x => x.IncrementErrorCount(It.IsAny<Operation>(), It.IsAny<string>()), Times.Never);
        Assert.Equal(expectedResults, actual);
    }
}