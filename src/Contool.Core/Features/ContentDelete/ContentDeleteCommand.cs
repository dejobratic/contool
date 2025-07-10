using Contool.Core.Infrastructure.Contentful.Extensions;
using Contool.Core.Infrastructure.Contentful.Services;

namespace Contool.Core.Features.ContentDelete;

public class ContentDeleteCommand : CommandBase
{
    public string ContentTypeId { get; init; } = null!;

    public bool IncludeArchived { get; init; }
}

public class ContentDeleteCommandHandler(
    IContentfulServiceBuilder contentfulServiceBuilder,
    IContentDeleter contentDeleter) : ICommandHandler<ContentDeleteCommand>
{
    public async Task HandleAsync(ContentDeleteCommand command, CancellationToken cancellationToken = default)
    {
        var contentfulService = contentfulServiceBuilder.Build(
            command.SpaceId, command.EnvironmentId);

        await DeleteContentAsync(
            command, contentfulService, cancellationToken);
    }
    
    private async Task DeleteContentAsync(
        ContentDeleteCommand command,
        IContentfulService contentfulService,
        CancellationToken cancellationToken)
    {
        var input = CreateContentDeleterInput(
            command, contentfulService);
        
        await contentDeleter.DeleteAsync(
            input, cancellationToken);
    }

    private static ContentDeleterInput CreateContentDeleterInput(
        ContentDeleteCommand command,
        IContentfulService contentfulService)
    {
        return new ContentDeleterInput
        {
            ContentTypeId = command.ContentTypeId,
            ContentfulService = contentfulService,
            IncludeArchived = command.IncludeArchived,
        };
    }
}