using Contentful.Core.Models;
using Contool.Core.Contentful.Extensions;
using Contool.Core.Contentful.Services;
using Contool.Core.Infrastructure.Utils;

namespace Contool.Core.Features.EntryPublish;

public class ContentPublishCommand : CommandBase
{
    public string ContentTypeId { get; init; } = default!;
}

public class ContentPublishCommandHandler(
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