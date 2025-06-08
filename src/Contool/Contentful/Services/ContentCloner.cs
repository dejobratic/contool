using Contentful.Core.Models;
using Contool.Contentful.Models;

namespace Contool.Contentful.Services;

internal class ContentCloner : IContentCloner
{
    public async Task CloneContentEntriesAsync(ContentEntryCloneRequest request, CancellationToken cancellationToken)
    {
        var buffer = new List<Entry<dynamic>>(capacity: 50);

        await foreach (var entry in request.SourceContentfulService.GetEntriesAsync(request.ContentTypeId, cancellationToken))
        {
            buffer.Add(entry);

            if (buffer.Count != buffer.Capacity)
                continue;

            await request.TargetContentfulService.UpsertEntriesAsync(
                buffer,
                publish: request.ShouldPublish,
                cancellationToken);

            buffer.Clear();
        }

        if (buffer.Count > 0)
        {
            await request.TargetContentfulService.UpsertEntriesAsync(
                buffer,
                publish: request.ShouldPublish,
                cancellationToken);
        }
    }
}
