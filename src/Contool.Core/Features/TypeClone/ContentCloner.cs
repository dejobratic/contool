using Contentful.Core.Models;
using Contool.Core.Infrastructure.Contentful.Extensions;
using Contool.Core.Infrastructure.Contentful.Services;
using Contool.Core.Infrastructure.Utils.Models;
using Contool.Core.Infrastructure.Utils.Services;

namespace Contool.Core.Features.TypeClone;

public class ContentCloner(
    IBatchProcessor batchProcessor,
    IProgressReporter progressReporter) : IContentCloner
{
    private const int DefaultBatchSize = 50;

    public async Task CloneAsync(ContentClonerInput input, CancellationToken cancellationToken = default)
    {
        var entriesForCloning = GetEntriesForCloning(
            input.ContentTypeId, input.SourceContentfulService, cancellationToken);

        await CloneEntriesAsync(
            entriesForCloning, input.TargetContentfulService, input.PublishEntries, cancellationToken);
    }

    private async Task CloneEntriesAsync(AsyncEnumerableWithTotal<Entry<dynamic>> entries, IContentfulService contentfulService, bool publish, CancellationToken cancellationToken)
    {
        progressReporter.Start(Operation.Clone, getTotal: () => entries.Total);

        await batchProcessor.ProcessAsync(
            source: entries,
            batchSize: DefaultBatchSize,
            batchActionAsync: (batch, ct) => contentfulService.CreateOrUpdateEntriesAsync(batch, publish, ct),
            batchItemFilter: entry => entry.IsPublished(),
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
            entries,
            getTotal: () => entries.Total);
    }
}