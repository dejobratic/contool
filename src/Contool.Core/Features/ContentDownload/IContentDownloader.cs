namespace Contool.Core.Features.ContentDownload;

public interface IContentDownloader
{
    Task DownloadAsync(ContentDownloaderInput input, CancellationToken cancellationToken);
}