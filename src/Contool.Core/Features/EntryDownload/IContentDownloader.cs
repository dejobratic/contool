using Contool.Core.Infrastructure.IO.Models;

namespace Contool.Core.Features.EntryDownload;

public interface IContentDownloader
{
    Task<OutputContent> DownloadAsync(ContentDownloadRequest request, CancellationToken cancellationToken);
}