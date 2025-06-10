using Contentful.Core.Models;
using Contool.Contentful.Extensions;
using Contool.Contentful.Services;
using Contool.Contentful.Utils;

namespace Contool.Commands;

internal class ContentDeleteCommand : CommandBase
{
    public string ContentTypeId { get; init; } = default!;
}

internal class ContentDeleteCommandHandler(
    IContentfulServiceBuilder contentfulServiceBuilder)
{
    public async Task HandleAsync(ContentDeleteCommand command, CancellationToken cancellationToken = default)
    {
        var contentfulService = contentfulServiceBuilder.Build(
            command.SpaceId, command.EnvironmentId);

        var batchProcessor = new AsyncEnumerableBatchProcessor<Entry<dynamic>>(
            items: contentfulService.GetEntriesAsync(contentTypeId: command.ContentTypeId, cancellationToken: cancellationToken),
            batchSize: 50,
            batchActionAsync: contentfulService.DeleteEntriesAsync);

        await batchProcessor.ProcessAsync(cancellationToken);
    }
}
