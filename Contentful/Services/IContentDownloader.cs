using Contool.Commands;
using Contool.Contentful.Models;
using Contool.Models;

namespace Contool.Contentful.Services;

internal interface IContentDownloader
{
    Task<OutputContent> DownloadAsync(ContentDownloadRequest request, CancellationToken cancellationToken);
}