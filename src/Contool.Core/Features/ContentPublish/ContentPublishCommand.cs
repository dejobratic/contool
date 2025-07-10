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

        await PublishContentAsync(command, contentfulService, cancellationToken);
    }

    private async Task PublishContentAsync(
        ContentPublishCommand command,
        IContentfulService contentfulService,
        CancellationToken cancellationToken)
    {
        var input = CreateContentPublisherInput(
            command, contentfulService);
        
        await contentPublisher.PublishAsync(
            input, cancellationToken);
    }

    private static ContentPublisherInput CreateContentPublisherInput(
        ContentPublishCommand command,
        IContentfulService contentfulService)
    {
        return new ContentPublisherInput
        {
            ContentTypeId = command.ContentTypeId,
            ContentfulService = contentfulService,
        };
    }
}