using Contool.Infrastructure.IO.Models;

namespace Contool.Features.EntryDownload;

internal interface IContentDownloader
{
    Task<OutputContent> DownloadAsync(ContentDownloadRequest request, CancellationToken cancellationToken);
}