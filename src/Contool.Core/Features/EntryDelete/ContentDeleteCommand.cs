using Contentful.Core.Models;
using Contool.Core.Contentful.Extensions;
using Contool.Core.Contentful.Services;
using Contool.Core.Infrastructure.Utils;

namespace Contool.Core.Features.EntryDelete;

public class ContentDeleteCommand : CommandBase
{
    public string ContentTypeId { get; init; } = default!;
}

public class ContentDeleteCommandHandler(
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
