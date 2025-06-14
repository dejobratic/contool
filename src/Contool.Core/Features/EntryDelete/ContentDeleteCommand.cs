using Contool.Core.Contentful.Extensions;
using Contool.Core.Contentful.Services;

namespace Contool.Core.Features.EntryDelete;

public class ContentDeleteCommand : WriteCommandBase
{
    public string ContentTypeId { get; init; } = default!;
}

public class ContentDeleteCommandHandler(
    IContentfulServiceBuilder contentfulServiceBuilder) : ICommandHandler<ContentDeleteCommand>
{
    public async Task HandleAsync(ContentDeleteCommand command, CancellationToken cancellationToken = default)
    {
        var contentfulService = contentfulServiceBuilder.Build(
            command.SpaceId, command.EnvironmentId);

        var entriesForDeleteing = contentfulService.GetEntriesAsync(
            contentTypeId: command.ContentTypeId, cancellationToken: cancellationToken);

        await contentfulService.DeleteEntriesAsync(entriesForDeleteing, cancellationToken);
    }
}