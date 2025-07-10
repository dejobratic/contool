using Contool.Core.Infrastructure.Utils.Models;
using Contool.Core.Infrastructure.Utils.Services;

namespace Contool.Core.Features.ContentDownload;

public class ContentDownloader(
    IProgressReporter progressReporter) : IContentDownloader
{
    public async Task DownloadAsync(ContentDownloaderInput input, CancellationToken cancellationToken)
    {
        var entries = GetEntriesToDownload(input.Output.Content);

        await input.OutputWriter.SaveAsync(
            input.Output.FullPath, entries, cancellationToken);
    }

    private AsyncEnumerableWithTotal<dynamic> GetEntriesToDownload(
        IAsyncEnumerableWithTotal<dynamic> entries)
    {
        progressReporter.Start(Operation.Download, getTotal: () => entries.Total);

        return new AsyncEnumerableWithTotal<dynamic>(
            source: entries,
            getTotal: () => entries.Total,
            progressReporter);
    }
}