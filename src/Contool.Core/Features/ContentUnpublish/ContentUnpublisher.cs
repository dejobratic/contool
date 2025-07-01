using Contentful.Core.Models;
using Contool.Core.Infrastructure.Contentful.Services;
using Contool.Core.Infrastructure.Utils.Models;
using Contool.Core.Infrastructure.Utils.Services;
using Microsoft.Extensions.Logging;

namespace Contool.Core.Features.ContentUnpublish;

public class ContentUnpublisher(
    IBatchProcessor batchProcessor,
    IProgressReporter progressReporter) : IContentUnpublisher
{
    private const int DefaultBatchSize = 50;

    public async Task UnpublishAsync(string contentTypeId, IContentfulService contentfulService, CancellationToken cancellationToken = default)
    {
        var entries = contentfulService.GetEntriesAsync(
            contentTypeId, DefaultBatchSize, cancellationToken: cancellationToken);

        await UnpublishEntriesAsync(
            entries, contentfulService, cancellationToken);
    }

    private async Task UnpublishEntriesAsync(
        IAsyncEnumerableWithTotal<Entry<dynamic>> entries,
        IContentfulService contentfulService,
        CancellationToken cancellationToken)
    {
        progressReporter.Start(Operation.Unpublish, getTotal: () => entries.Total);

        await batchProcessor.ProcessAsync(
            source: entries,
            batchSize: DefaultBatchSize,
            batchActionAsync: contentfulService.UnpublishEntriesAsync,
            cancellationToken: cancellationToken);

        progressReporter.Complete();
    }
}
