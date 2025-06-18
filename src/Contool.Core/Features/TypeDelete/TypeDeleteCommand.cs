using Contool.Core.Features.ContentDelete;
using Contool.Core.Infrastructure.Contentful.Extensions;
using Contool.Core.Infrastructure.Contentful.Services;

namespace Contool.Core.Features.TypeDelete;

public class TypeDeleteCommand : CommandBase
{
    public string ContentTypeId { get; init; } = default!;

    public bool Force { get; init; } = false;
}

public class TypeDeleteCommandHandler(
    IContentfulServiceBuilder contentfulServiceBuilder,
    IContentDeleter contentDeleter) : ICommandHandler<TypeDeleteCommand>
{
    public async Task HandleAsync(TypeDeleteCommand command, CancellationToken cancellationToken = default)
    {
        var contentfulService = contentfulServiceBuilder.Build(
            command.SpaceId, command.EnvironmentId);

        await ThrowIfContentTypeDoesNotExistAsync(
            command.ContentTypeId, contentfulService, cancellationToken);

        await ThrowIfDeleteNotForcedWithExistingEntriesAsync(
            command.ContentTypeId, command.Force, contentfulService, cancellationToken);

        await contentDeleter.DeleteAsync(
            command.ContentTypeId, contentfulService, cancellationToken);

        await contentfulService.DeleteContentTypeAsync(
            command.ContentTypeId, cancellationToken);
    }

    private static async Task ThrowIfContentTypeDoesNotExistAsync(
        string contentTypeId,
        IContentfulService contentfulService,
        CancellationToken cancellationToken)
    {
        _ = await contentfulService.GetContentTypeAsync(contentTypeId, cancellationToken)
            ?? throw new ArgumentException($"Content type with ID '{contentTypeId}' does not exist.");
    }

    private static async Task ThrowIfDeleteNotForcedWithExistingEntriesAsync(
        string contentTypeId,
        bool force,
        IContentfulService contentfulService,
        CancellationToken cancellationToken)
    {
        if (!force && await HasContentEntriesAsync(contentTypeId, contentfulService, cancellationToken))
            throw new InvalidOperationException($"Content type with ID '{contentTypeId}' cannot be deleted because it contains entries. Use the force option to delete it anyway.");
    }

    private static async Task<bool> HasContentEntriesAsync(
        string contentTypeId,
        IContentfulService contentfulService,
        CancellationToken cancellationToken)
    {
        var hasEntries = false;

        var entries = contentfulService.GetEntriesAsync(
            contentTypeId: contentTypeId, pageSize: 1, cancellationToken: cancellationToken);

        await foreach (var entry in entries)
        {
            hasEntries = true;
            break;
        }

        return hasEntries;
    }
}
