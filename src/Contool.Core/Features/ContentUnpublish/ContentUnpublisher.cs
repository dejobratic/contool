using Contentful.Core.Models;
using Contool.Core.Infrastructure.Contentful.Services;
using Contool.Core.Infrastructure.Utils;
using Microsoft.Extensions.Logging;

namespace Contool.Core.Features.ContentUnpublish;

public class ContentUnpublisher(
    IBatchProcessor batchProcessor,
    IProgressReporter progressReporter,
    ILogger<ContentUnpublisher> logger) : IContentUnpublisher
{
    private const int DefaultBatchSize = 50;

    public async Task UnpublishAsync(string contentTypeId, IContentfulService contentfulService, CancellationToken cancellationToken = default)
    {
        var entries = contentfulService.GetEntriesAsync(
            contentTypeId: contentTypeId, cancellationToken: cancellationToken);

        await UnpublishEntriesAsync(
            entries, contentfulService, cancellationToken);

        LogInfo(contentTypeId, entries.Total);
    }

    private async Task UnpublishEntriesAsync(
        IAsyncEnumerableWithTotal<Entry<dynamic>> entries,
        IContentfulService contentfulService,
        CancellationToken cancellationToken)
    {
        progressReporter.Start("Unpublishing", getTotal: () => entries.Total);

        await batchProcessor.ProcessAsync(
            source: entries,
            batchSize: DefaultBatchSize,
            batchActionAsync: contentfulService.UnpublishEntriesAsync,
            cancellationToken: cancellationToken);

        progressReporter.Complete();
    }

    private void LogInfo(string contentTypeId, int total)
    {
        if (total == 0)
        {
            logger.LogInformation(
                "No {ContentTypeId} entries found for unpublishing.", contentTypeId);
        }
        else
        {
            logger.LogInformation(
                "{Total} {ContentTypeId} entries unpublished.", total, contentTypeId);
        }
    }
}
