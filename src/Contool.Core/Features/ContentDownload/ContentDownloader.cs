using Contool.Core.Infrastructure.IO.Models;
using Contool.Core.Infrastructure.IO.Services;
using Contool.Core.Infrastructure.Utils.Models;
using Contool.Core.Infrastructure.Utils.Services;

namespace Contool.Core.Features.ContentDownload;

public class ContentDownloader(
    IProgressReporter progressReporter) : IContentDownloader
{
    public async Task DownloadAsync(string contentTypeId, OutputContent output, IOutputWriter outputWriter, CancellationToken cancellationToken)
    {
        var entries = GetEntriesToDownload(output.Content);

        await outputWriter.SaveAsync(
            output.FullPath, entries, cancellationToken);
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