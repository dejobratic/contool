using Contool.Core.Infrastructure.Contentful.Extensions;
using Contool.Core.Infrastructure.Contentful.Services;
using Microsoft.Extensions.Logging;

namespace Contool.Core.Features.ContentDelete;

public class ContentDeleteCommand : WriteCommandBase
{
    public string ContentTypeId { get; init; } = default!;
}

public class ContentDeleteCommandHandler(
    IContentfulServiceBuilder contentfulServiceBuilder,
    ILogger<ContentDeleteCommandHandler> logger) : ICommandHandler<ContentDeleteCommand>
{
    public async Task HandleAsync(ContentDeleteCommand command, CancellationToken cancellationToken = default)
    {
        var contentfulService = contentfulServiceBuilder.Build(
            command.SpaceId, command.EnvironmentId);

        var entriesForDeleting = contentfulService.GetEntriesAsync(
            contentTypeId: command.ContentTypeId, cancellationToken: cancellationToken);

        await contentfulService.DeleteEntriesAsync(
            entriesForDeleting, cancellationToken);

        logger.LogInformation(
            "{Total} {ContentTypeId} entries deleted.", entriesForDeleting.Total, command.ContentTypeId);
    }
}