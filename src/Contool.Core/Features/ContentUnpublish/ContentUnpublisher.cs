using Contentful.Core.Models;
using Contool.Core.Infrastructure.Contentful.Services;
using Contool.Core.Infrastructure.Utils.Models;
using Contool.Core.Infrastructure.Utils.Services;

namespace Contool.Core.Features.ContentUnpublish;

public class ContentUnpublisher(
    IBatchProcessor batchProcessor,
    IProgressReporter progressReporter) : IContentUnpublisher
{
    private const int DefaultBatchSize = 50;

    public async Task UnpublishAsync(ContentUnpublisherInput input, CancellationToken cancellationToken = default)
    {
        var entries = input.ContentfulService.GetEntriesAsync(
            input.ContentTypeId, DefaultBatchSize, cancellationToken: cancellationToken);

        await UnpublishEntriesAsync(
            entries, input.ContentfulService, cancellationToken);
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
