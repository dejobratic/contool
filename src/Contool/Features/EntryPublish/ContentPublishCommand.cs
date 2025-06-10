using Contentful.Core.Models;
using Contool.Contentful.Extensions;
using Contool.Contentful.Services;
using Contool.Infrastructure.Utils;

namespace Contool.Features.EntryPublish;

internal class ContentPublishCommand : CommandBase
{
    public string ContentTypeId { get; init; } = default!;
}

internal class ContentPublishCommandHandler(
    IContentfulServiceBuilder contentfulServiceBuilder)
{
    public async Task HandleAsync(ContentPublishCommand command, CancellationToken cancellationToken = default)
    {
        var contentfulService = contentfulServiceBuilder.Build(command.SpaceId, command.EnvironmentId);

        var batchProcessor = new AsyncEnumerableBatchProcessor<Entry<dynamic>>(
            items: contentfulService.GetEntriesAsync(contentTypeId: command.ContentTypeId, cancellationToken: cancellationToken),
            batchSize: 50,
            batchActionAsync: contentfulService.PublishEntriesAsync,
            shouldInclude: entry => !entry.IsArchived() && !entry.IsPublished());

        await batchProcessor.ProcessAsync(cancellationToken);
    }
}