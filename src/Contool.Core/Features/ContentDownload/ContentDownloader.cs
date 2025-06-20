using Contool.Core.Infrastructure.IO.Models;
using Contool.Core.Infrastructure.IO.Services;
using Contool.Core.Infrastructure.Utils.Models;
using Contool.Core.Infrastructure.Utils.Services;
using Microsoft.Extensions.Logging;

namespace Contool.Core.Features.ContentDownload;

public class ContentDownloader(
    IProgressReporter progressReporter,
    ILogger<ContentDownloadCommandHandler> logger) : IContentDownloader
{
    public async Task DownloadAsync(string contentTypeId, OutputContent output, IOutputWriter outputWriter, CancellationToken cancellationToken)
    {
        var entries = GetEntriesToDownload(output.Content);

        await outputWriter.SaveAsync(
            output.FullPath, entries, cancellationToken);

        LogInfo(contentTypeId, output.Content.Total, output.FullPath);
    }

    private AsyncEnumerableWithTotal<dynamic> GetEntriesToDownload(
        IAsyncEnumerableWithTotal<dynamic> entries)
    {
        progressReporter.Start("Downloading", getTotal: () => entries.Total);

        return new AsyncEnumerableWithTotal<dynamic>(
            source: entries,
            getTotal: () => entries.Total,
            progressReporter);
    }

    private void LogInfo(string contentTypeId, int total, string outputPath)
    {
        if (total == 0)
        {
            logger.LogInformation(
                "No {ContentTypeId} entries found for download.", contentTypeId);
        }
        else
        {
            logger.LogInformation(
                "{Total} {ContentTypeId} entries downloaded to {OutputPath}.", total, contentTypeId, outputPath);
        }
    }
}