using Contool.Core.Infrastructure.Contentful.Extensions;
using Contool.Core.Infrastructure.Contentful.Services;

namespace Contool.Core.Features.ContentDelete;

public class ContentDeleteCommand : WriteCommandBase
{
    public string ContentTypeId { get; init; } = default!;

    public bool IncludeArchived { get; init; } // TODO: implement this feature
}

public class ContentDeleteCommandHandler(
    IContentfulServiceBuilder contentfulServiceBuilder,
    IContentDeleter contentDeleter) : ICommandHandler<ContentDeleteCommand>
{
    public async Task HandleAsync(ContentDeleteCommand command, CancellationToken cancellationToken = default)
    {
        var contentfulService = contentfulServiceBuilder.Build(
            command.SpaceId, command.EnvironmentId);

        await contentDeleter.DeleteAsync(
            command.ContentTypeId, contentfulService, cancellationToken);
    }
}