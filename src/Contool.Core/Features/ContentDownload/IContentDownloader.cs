using Contool.Core.Infrastructure.IO.Models;
using Contool.Core.Infrastructure.IO.Services;

namespace Contool.Core.Features.ContentDownload;

public interface IContentDownloader
{
    Task DownloadAsync(string contentTypeId, OutputContent output, IOutputWriter outputWriter, CancellationToken cancellationToken);
}