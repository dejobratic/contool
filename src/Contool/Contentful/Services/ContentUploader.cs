using Contool.Contentful.Models;
using Contentful.Core.Models;

namespace Contool.Contentful.Services;

internal class ContentUploader(IContentfulService contentfulService) : IContentUploader
{
    public async Task UploadAsync(ContentUploadRequest request, CancellationToken cancellationToken)
    {
       var entries = request.Content.Rows
            .Select(row => (Entry<dynamic>)request.Deserializer.Deserialize(request.Content.Headings, row))
            .ToList();

        await contentfulService.UpsertEntriesAsync(entries, cancellationToken: cancellationToken);

        if (request.ShouldPublishContent)
        {
            await contentfulService.PublishEntriesAsync(entries, cancellationToken: cancellationToken);
        }
    }
}