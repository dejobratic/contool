using Contool.Core.Features.ContentDownload;
using Contool.Core.Infrastructure.IO.Models;
using Contool.Core.Infrastructure.IO.Services;
using Contool.Core.Infrastructure.Utils.Models;
using Contool.Core.Tests.Unit.Helpers;
using Contool.Core.Tests.Unit.Mocks;

namespace Contool.Core.Tests.Unit.Features.ContentDownload;

public class ContentDownloaderTests
{
    private readonly ContentDownloader _sut;
    
    private readonly MockProgressReporter _progressReporterMock = new();

    public ContentDownloaderTests()
    {
        _sut = new ContentDownloader(_progressReporterMock);
    }

    [Fact]
    public async Task GivenValidInput_WhenDownloadAsync_ThenSavesContentToOutput()
    {
        // Arrange
        var entries = CreateTestEntries();
        var outputWriterMock = new MockOutputWriter();
        var input = CreateDownloaderInput(entries, outputWriterMock);
        
        // Act
        await _sut.DownloadAsync(input, CancellationToken.None);
        
        // Assert
        Assert.True(outputWriterMock.SaveAsyncWasCalled);
        Assert.Equal(input.Output.FullPath, outputWriterMock.LastSavedPath);
        Assert.NotNull(outputWriterMock.LastSavedContent);
    }

    [Fact]
    public async Task GivenValidInput_WhenDownloadAsync_ThenReportsProgressCorrectly()
    {
        // Arrange
        var entries = CreateTestEntries();
        var outputWriterMock = new MockOutputWriter();
        var input = CreateDownloaderInput(entries, outputWriterMock);
        
        // Act
        await _sut.DownloadAsync(input, CancellationToken.None);
        
        // Assert
        Assert.True(_progressReporterMock.StartWasCalled);
        Assert.Equal(Operation.Download, _progressReporterMock.LastOperation);
        Assert.Equal(entries.Total, _progressReporterMock.LastTotal);
    }

    [Fact]
    public async Task GivenEmptyEntries_WhenDownloadAsync_ThenHandlesEmptyCollection()
    {
        // Arrange
        var entries = CreateEmptyEntries();
        var outputWriterMock = new MockOutputWriter();
        var input = CreateDownloaderInput(entries, outputWriterMock);
        
        // Act
        await _sut.DownloadAsync(input, CancellationToken.None);
        
        // Assert
        Assert.True(outputWriterMock.SaveAsyncWasCalled);
        Assert.Equal(0, _progressReporterMock.LastTotal);
    }

    [Fact]
    public async Task GivenCancellationToken_WhenDownloadAsync_ThenRespectsCancellation()
    {
        // Arrange
        var entries = CreateTestEntries();
        var outputWriterMock = new MockOutputWriter();
        var input = CreateDownloaderInput(entries, outputWriterMock);
        
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        
        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            _sut.DownloadAsync(input, cts.Token));
    }

    [Fact]
    public async Task GivenLargeDataSet_WhenDownloadAsync_ThenStreamsEfficiently()
    {
        // Arrange
        const int totalEntries = 1000;
        var entries = CreateLargeTestEntries(totalEntries);
        var outputWriterMock = new MockOutputWriter();
        var input = CreateDownloaderInput(entries, outputWriterMock);
        
        // Act
        await _sut.DownloadAsync(input, CancellationToken.None);
        
        // Assert
        Assert.True(outputWriterMock.SaveAsyncWasCalled);
        Assert.Equal(totalEntries, _progressReporterMock.LastTotal);
    }

    [Fact]
    public async Task GivenOutputWriterThrowsException_WhenDownloadAsync_ThenBubblesException()
    {
        // Arrange
        var entries = CreateTestEntries();
        var outputWriterMock = new MockOutputWriter();
        outputWriterMock.SetupToThrow(new IOException("Write failed"));
        var input = CreateDownloaderInput(entries, outputWriterMock);
        
        // Act & Assert
        var exception = await Assert.ThrowsAsync<IOException>(() =>
            _sut.DownloadAsync(input, CancellationToken.None));
        
        Assert.Equal("Write failed", exception.Message);
    }

    private static ContentDownloaderInput CreateDownloaderInput(
        IAsyncEnumerableWithTotal<dynamic> entries,
        IOutputWriter outputWriter)
    {
        var output = new OutputContent(
            path: "/test/path",
            name: "test-content",
            type: "csv",
            content: entries);

        return new ContentDownloaderInput
        {
            ContentTypeId = "test-content-type",
            Output = output,
            OutputWriter = outputWriter
        };
    }

    private static IAsyncEnumerableWithTotal<dynamic> CreateTestEntries()
    {
        var entries = new[]
        {
            new Dictionary<string, object> { ["id"] = "1", ["name"] = "Test 1" },
            new Dictionary<string, object> { ["id"] = "2", ["name"] = "Test 2" },
            new Dictionary<string, object> { ["id"] = "3", ["name"] = "Test 3" }
        };

        var asyncEnumerable = new AsyncEnumerableWithTotal<dynamic>(
            source: AsyncEnumerableFactory.From(entries.Cast<dynamic>()),
            getTotal: () => entries.Length);

        // Force the total to be set by reading the first item and resetting
        return new MockAsyncEnumerableWithTotal<dynamic>(entries.Cast<dynamic>(), entries.Length);
    }

    private static IAsyncEnumerableWithTotal<dynamic> CreateEmptyEntries()
    {
        return new MockAsyncEnumerableWithTotal<dynamic>(Array.Empty<dynamic>(), 0);
    }

    private static IAsyncEnumerableWithTotal<dynamic> CreateLargeTestEntries(int count)
    {
        var entries = Enumerable.Range(1, count)
            .Select(i => new Dictionary<string, object> { ["id"] = i.ToString(), ["name"] = $"Test {i}" })
            .Cast<dynamic>()
            .ToArray();

        return new MockAsyncEnumerableWithTotal<dynamic>(entries, count);
    }
}