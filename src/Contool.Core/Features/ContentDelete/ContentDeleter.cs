using Contentful.Core.Models;
using Contool.Core.Infrastructure.Contentful.Services;
using Contool.Core.Infrastructure.Utils;
using Microsoft.Extensions.Logging;

namespace Contool.Core.Features.ContentDelete;

public class ContentDeleter(
    IBatchProcessor batchProcessor,
    IProgressReporter progressReporter,
    ILogger<ContentDeleter> logger) : IContentDeleter
{
    private const int DefaultBatchSize = 50;

    public async Task DeleteAsync(string contentTypeId, IContentfulService contentfulService, CancellationToken cancellationToken = default)
    {
        var entries = contentfulService.GetEntriesAsync(
            contentTypeId: contentTypeId, cancellationToken: cancellationToken);

        await DeleteEntriesAsync(
            entries, contentfulService, cancellationToken);

        LogInfo(contentTypeId, entries.Total);
    }

    private async Task DeleteEntriesAsync(
        IAsyncEnumerableWithTotal<Entry<dynamic>> entries,
        IContentfulService contentfulService,
        CancellationToken cancellationToken)
    {
        progressReporter.Start("Deleting", getTotal: () => entries.Total);

        await batchProcessor.ProcessAsync(
            source: entries,
            batchSize: DefaultBatchSize,
            batchActionAsync: contentfulService.DeleteEntriesAsync,
            cancellationToken);

        progressReporter.Complete();
    }

    private void LogInfo(string contentTypeId, int total)
    {
        if (total == 0)
        {
            logger.LogInformation(
                "No {ContentTypeId} entries found for deleting.", contentTypeId);
        }
        else
        {
            logger.LogInformation(
                "{Total} {ContentTypeId} entries deleted.", total, contentTypeId);
        }
    }
}
