using Contentful.Core.Models;
using Contool.Core.Infrastructure.Contentful.Services;
using Contool.Core.Infrastructure.Utils.Models;
using Contool.Core.Infrastructure.Utils.Services;

namespace Contool.Core.Features.ContentPublish;

public class ContentPublisher(
    IBatchProcessor batchProcessor,
    IProgressReporter progressReporter) : IContentPublisher
{
    private const int DefaultBatchSize = 100;

    public async Task PublishAsync(ContentPublisherInput input, CancellationToken cancellationToken = default)
    {
        var entries = input.ContentfulService.GetEntriesAsync(
            input.ContentTypeId, DefaultBatchSize, cancellationToken: cancellationToken);

        await PublishEntriesAsync(
            entries, input.ContentfulService, cancellationToken);
    }

    private async Task PublishEntriesAsync(
        IAsyncEnumerableWithTotal<Entry<dynamic>> entries,
        IContentfulService contentfulService,
        CancellationToken cancellationToken)
    {
        progressReporter.Start(Operation.Publish, getTotal: () => entries.Total);

        await batchProcessor.ProcessAsync(
            source: entries,
            batchSize: DefaultBatchSize,
            batchActionAsync: contentfulService.PublishEntriesAsync,
            cancellationToken: cancellationToken);

        progressReporter.Complete();
    }
}
