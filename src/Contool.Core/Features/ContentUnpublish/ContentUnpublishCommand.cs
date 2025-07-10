using Contool.Core.Infrastructure.Contentful.Extensions;
using Contool.Core.Infrastructure.Contentful.Services;

namespace Contool.Core.Features.ContentUnpublish;

public class ContentUnpublishCommand : CommandBase
{
    public string ContentTypeId { get; init; } =  null!;
}

public class ContentUnpublishCommandHandler(
    IContentfulServiceBuilder contentfulServiceBuilder,
    IContentUnpublisher contentPublisher) : ICommandHandler<ContentUnpublishCommand>
{
    public async Task HandleAsync(ContentUnpublishCommand command, CancellationToken cancellationToken = default)
    {
        var contentfulService = contentfulServiceBuilder.Build(
            command.SpaceId, command.EnvironmentId);

        await UnpublishContentAsync(
            command, contentfulService, cancellationToken);
    }

    private async Task UnpublishContentAsync(
        ContentUnpublishCommand command,
        IContentfulService contentfulService,
        CancellationToken cancellationToken)
    {
        var input = CreateContentUnpublisherInput(
            command, contentfulService);
        
        await contentPublisher.UnpublishAsync(
            input, cancellationToken);
    }

    private static ContentUnpublisherInput CreateContentUnpublisherInput(
        ContentUnpublishCommand command,
        IContentfulService contentfulService)
    {
        return new ContentUnpublisherInput
        {
            ContentTypeId = command.ContentTypeId,
            ContentfulService = contentfulService,
        };
    }
}