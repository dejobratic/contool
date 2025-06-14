using Contentful.Core.Models;
using Contool.Core.Infrastructure.IO.Models;
using Contool.Core.Infrastructure.Utils;

namespace Contool.Core.Features.EntryDownload;

public class ContentDownloader(IProgressReporter progressReporter) : IContentDownloader
{
    public async Task<OutputContent> DownloadAsync(ContentDownloadRequest request, CancellationToken cancellationToken)
    {
        await Task.CompletedTask; // TODO: remove ??

        return new OutputContent(
            path: request.OutputPath,
            name: request.ContentTypeId,
            type: request.OutputFormat,
            content: GetEntriesToDownload(request, cancellationToken));
    }

    private AsyncEnumerableWithTotal<dynamic> GetEntriesToDownload(
        ContentDownloadRequest request,
        CancellationToken cancellationToken)
    {
        var entries = request.ContentfulService.GetEntriesAsync(
            request.ContentTypeId, cancellationToken: cancellationToken);

        return new AsyncEnumerableWithTotal<dynamic>(
            source: GetEntriesToSerialize(entries, request.Serializer),
            getTotal: () => entries.Total,
            progressReporter);
    }

    public static async IAsyncEnumerable<dynamic> GetEntriesToSerialize(
        IAsyncEnumerable<Entry<dynamic>> entries,
        IContentEntrySerializer serializer)
    {
        await foreach (var entry in entries)
            yield return serializer.Serialize(entry);
    }
}
