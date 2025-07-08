using Contool.Core.Infrastructure.Contentful.Extensions;
using Contool.Core.Infrastructure.Contentful.Services;

namespace Contool.Core.Features.ContentPublish;

public class ContentPublishCommand : CommandBase
{
    public string ContentTypeId { get; init; } = null!;
}

public class ContentPublishCommandHandler(
    IContentfulServiceBuilder contentfulServiceBuilder,
    IContentPublisher contentPublisher) : ICommandHandler<ContentPublishCommand>
{
    public async Task HandleAsync(ContentPublishCommand command, CancellationToken cancellationToken = default)
    {
        var contentfulService = contentfulServiceBuilder.Build(
            command.SpaceId, command.EnvironmentId);

        await contentPublisher.PublishAsync(
            command.ContentTypeId, contentfulService, cancellationToken);
    }
}