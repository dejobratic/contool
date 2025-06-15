using Contool.Core.Infrastructure.IO.Models;

namespace Contool.Core.Features.ContentDownload;

public interface IContentDownloader
{
    Task<OutputContent> DownloadAsync(ContentDownloadRequest request, CancellationToken cancellationToken);
}