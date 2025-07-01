using Contentful.Core.Models;
using Contool.Core.Infrastructure.Contentful.Services;
using Contool.Core.Infrastructure.Utils.Models;
using Contool.Core.Infrastructure.Utils.Services;
using Microsoft.Extensions.Logging;

namespace Contool.Core.Features.ContentPublish;

public class ContentPublisher(
    IBatchProcessor batchProcessor,
    IProgressReporter progressReporter) : IContentPublisher
{
    private const int DefaultBatchSize = 50;

    public async Task PublishAsync(string contentTypeId, IContentfulService contentfulService, CancellationToken cancellationToken = default)
    {
        var entries = contentfulService.GetEntriesAsync(
            contentTypeId, DefaultBatchSize, cancellationToken: cancellationToken);

        await PublishEntriesAsync(
            entries, contentfulService, cancellationToken);
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
