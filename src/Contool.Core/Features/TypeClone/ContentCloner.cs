using Contentful.Core.Models;
using Contool.Core.Infrastructure.Contentful.Extensions;
using Contool.Core.Infrastructure.Contentful.Services;
using Contool.Core.Infrastructure.Utils.Models;
using Contool.Core.Infrastructure.Utils.Services;
using Microsoft.Extensions.Logging;

namespace Contool.Core.Features.TypeClone;

public class ContentCloner(
    IBatchProcessor batchProcessor,
    IProgressReporter progressReporter,
    ILogger<ContentCloner> logger) : IContentCloner
{
    private const int DefaultBatchSize = 50;

    public async Task CloneAsync(string contentTypeId, IContentfulService sourceContentfulService, IContentfulService targetContentfulService, bool publish, CancellationToken cancellationToken = default)
    {
        var entriesForCloning = GetEntriesForCloning(
            contentTypeId, sourceContentfulService, cancellationToken);

        await CloneEntries(
            entriesForCloning, targetContentfulService, publish, cancellationToken);

        LogInfo(contentTypeId, entriesForCloning.Total);
    }

    private async Task CloneEntries(AsyncEnumerableWithTotal<Entry<dynamic>> entries, IContentfulService contentfulService, bool publish, CancellationToken cancellationToken)
    {
        progressReporter.Start(Operation.Copy, getTotal: () => entries.Total);

        await batchProcessor.ProcessAsync(
            source: entries,
            batchSize: DefaultBatchSize,
            batchActionAsync: (batch, ct) => contentfulService.CreateOrUpdateEntriesAsync(batch, publish, ct),
            cancellationToken: cancellationToken);

        progressReporter.Complete();
    }

    private static AsyncEnumerableWithTotal<Entry<dynamic>> GetEntriesForCloning(
        string contentTypeId,
        IContentfulService contentfulService,
        CancellationToken cancellationToken)
    {
        var entries = contentfulService.GetEntriesAsync(
            contentTypeId, cancellationToken: cancellationToken);

        return new AsyncEnumerableWithTotal<Entry<dynamic>>(
            FilterPublishedEntries(entries),
            getTotal: () => entries.Total);
    }

    private static async IAsyncEnumerable<Entry<dynamic>> FilterPublishedEntries(
        IAsyncEnumerable<Entry<dynamic>> entries)
    {
        await foreach (var entry in entries)
            if (entry.IsPublished())
                yield return entry;
    }

    private void LogInfo(string contentTypeId, int total)
    {
        if (total == 0)
        {
            logger.LogInformation(
                "No {ContentTypeId} entries found for cloning.", contentTypeId);
        }
        else
        {
            logger.LogInformation(
                "{Total} {ContentTypeId} entries cloned.", total, contentTypeId);
        }
    }
}