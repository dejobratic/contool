using Contool.Contentful.Extensions;
using Contool.Contentful.Services;
using Contentful.Core.Models;

namespace Contool.Commands;

internal class ContentPublishCommand
{
    public string ContentTypeId { get; set; } = default!;
}

internal class ContentPublishCommandHandler(
    IContentfulService contentfulService)
{
    public async Task HandleAsync(ContentPublishCommand command, CancellationToken cancellationToken = default)
    {
        var unpublishedEntries = new List<Entry<dynamic>>();

        await foreach (var entry in contentfulService.GetEntriesAsync(command.ContentTypeId, cancellationToken: cancellationToken))
        {
            if (entry.IsArchived() || entry.IsPublished())
            {
                continue;
            }

            unpublishedEntries.Add(entry);
        }

        await contentfulService.PublishEntriesAsync(unpublishedEntries, cancellationToken: cancellationToken);
    }
}