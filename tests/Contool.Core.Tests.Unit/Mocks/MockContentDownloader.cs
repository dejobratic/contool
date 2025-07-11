using Contool.Core.Features.ContentDownload;

namespace Contool.Core.Tests.Unit.Mocks;

public class MockContentDownloader : IContentDownloader
{
    private Exception? _exceptionToThrow;

    public bool DownloadAsyncWasCalled { get; private set; }
    public ContentDownloaderInput? LastInput { get; private set; }

    public void SetupToThrow(Exception exception)
    {
        _exceptionToThrow = exception;
    }

    public async Task DownloadAsync(ContentDownloaderInput input, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        if (_exceptionToThrow != null)
            throw _exceptionToThrow;

        DownloadAsyncWasCalled = true;
        LastInput = input;

        // Simulate processing
        await Task.Delay(1, cancellationToken);
    }
}