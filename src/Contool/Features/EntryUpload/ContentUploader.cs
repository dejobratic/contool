using Contentful.Core.Models;
using Contool.Infrastructure.Utils;

namespace Contool.Features.EntryUpload;

internal class ContentUploader : IContentUploader
{
    public async Task UploadAsync(ContentUploadRequest request, CancellationToken cancellationToken)
    {
        var entries = request.Content.Rows
            .Select(row => (Entry<dynamic>)request.Deserializer
                .Deserialize(request.Content.Headings, row));

        var batchProcessor = new AsyncEnumerableBatchProcessor<Entry<dynamic>>(
            items: entries.ToAsyncEnumerable(),
            batchSize: 50,
            batchActionAsync: async (batch, ct) =>
            {
                await request.ContentfulService.CreateOrUpdateEntriesAsync(
                    batch, publish: request.Publish, cancellationToken: ct);
            });

        await batchProcessor.ProcessAsync(cancellationToken);
    }
}