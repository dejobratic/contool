using Contentful.Core.Models;
using Contool.Infrastructure.Utils;

namespace Contool.Features.TypeClone;

internal class ContentCloner : IContentCloner
{
    public async Task CloneContentEntriesAsync(ContentEntryCloneRequest request, CancellationToken cancellationToken)
    {
        var batchProcessor = new AsyncEnumerableBatchProcessor<Entry<dynamic>>(
            items: request.SourceContentfulService.GetEntriesAsync(request.ContentTypeId, cancellationToken: cancellationToken),
            batchSize: 50,
            batchActionAsync: async (entries, ct) =>
            {
                await request.TargetContentfulService.UpsertEntriesAsync(
                    entries,
                    publish: request.ShouldPublish,
                    ct);
            });

        await batchProcessor.ProcessAsync(cancellationToken);
    }
}
