using Contentful.Core.Models;
using Contool.Core.Infrastructure.Contentful.Extensions;
using Contool.Core.Infrastructure.Contentful.Services;
using Contool.Core.Infrastructure.Utils;

namespace Contool.Core.Features.ContentPublish;

public class ContentPublishCommand : CommandBase
{
    public string ContentTypeId { get; init; } = default!;
}

public class ContentPublishCommandHandler(
    IContentfulServiceBuilder contentfulServiceBuilder,
    IProgressReporter progressReporter) : ICommandHandler<ContentPublishCommand>
{
    public async Task HandleAsync(ContentPublishCommand command, CancellationToken cancellationToken = default)
    {
        var contentfulService = contentfulServiceBuilder.Build(
            command.SpaceId, command.EnvironmentId);

        var entriesForPublishing = GetEntriesForPublishing(
            command.ContentTypeId, contentfulService, cancellationToken);

        await contentfulService.PublishEntriesAsync(
            entriesForPublishing, cancellationToken);
    }

    private AsyncEnumerableWithTotal<Entry<dynamic>> GetEntriesForPublishing(
        string contentTypeId,
        IContentfulService contentfulService,
        CancellationToken cancellationToken)
    {
        var entries = contentfulService.GetEntriesAsync(
            contentTypeId: contentTypeId, cancellationToken: cancellationToken);

        return new AsyncEnumerableWithTotal<Entry<dynamic>>(
            GetEntriesForPublishingAsync(entries),
            getTotal: () => entries.Total, // TODO: this is not accurate
            progressReporter);
    }

    private static async IAsyncEnumerable<Entry<dynamic>> GetEntriesForPublishingAsync(
        IAsyncEnumerable<Entry<dynamic>> entries)
    {
        await foreach (var entry in entries)
            if (!entry.IsArchived() && !entry.IsPublished())
                yield return entry;
    }
}