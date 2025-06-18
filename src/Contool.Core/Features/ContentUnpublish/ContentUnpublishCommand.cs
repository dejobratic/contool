using Contool.Core.Infrastructure.Contentful.Extensions;
using Contool.Core.Infrastructure.Contentful.Services;

namespace Contool.Core.Features.ContentUnpublish;

public class ContentUnpublishCommand : CommandBase
{
    public string ContentTypeId { get; init; } = default!;
}

public class ContentUnpublishCommandHandler(
    IContentfulServiceBuilder contentfulServiceBuilder,
    IContentUnpublisher contentPublisher) : ICommandHandler<ContentUnpublishCommand>
{
    public async Task HandleAsync(ContentUnpublishCommand command, CancellationToken cancellationToken = default)
    {
        var contentfulService = contentfulServiceBuilder.Build(
            command.SpaceId, command.EnvironmentId);

        await contentPublisher.UnpublishAsync(
            command.ContentTypeId, contentfulService, cancellationToken);
    }
}