using Contentful.Core.Models;
using Contool.Contentful.Extensions;
using Contool.Contentful.Services;
using Contool.Contentful.Utils;

namespace Contool.Commands;

internal class TypeDeleteCommand : CommandBase
{
    public string ContentTypeId { get; init; } = default!;

    public bool Force { get; init; } = false;
}

internal class TypeDeleteCommandHandler(
    IContentfulServiceBuilder contentfulServiceBuilder)
{
    public async Task HandleAsync(TypeDeleteCommand command, CancellationToken cancellationToken = default)
    {
        var contentfulService = contentfulServiceBuilder.Build(command.SpaceId, command.EnvironmentId);

        await ThrowIfDeleteNotForcedWithExistingEntriesAsync(command, contentfulService, cancellationToken);
        
        await DeleteEntriesAsync(command, contentfulService, cancellationToken);

        await contentfulService.DeleteContentTypeAsync(command.ContentTypeId, cancellationToken);
    }

    private static async Task ThrowIfDeleteNotForcedWithExistingEntriesAsync(TypeDeleteCommand command, IContentfulService contentfulService, CancellationToken cancellationToken)
    {
        if (!command.Force && await HasContentEntriesAsync(command.ContentTypeId, contentfulService, cancellationToken))
            throw new InvalidOperationException($"Content type '{command.ContentTypeId}' cannot be deleted because it contains entries. Use the force option to delete it anyway.");
    }

    private static async Task<bool> HasContentEntriesAsync(string contentTypeId, IContentfulService contentfulService, CancellationToken cancellationToken)
    {
        var hasContent = false;

        await foreach (var entry in contentfulService.GetEntriesAsync(contentTypeId: contentTypeId, cancellationToken: cancellationToken))
        {
            hasContent = true;
            break;
        }

        return hasContent;
    }

    private static async Task DeleteEntriesAsync(TypeDeleteCommand command, IContentfulService contentfulService, CancellationToken cancellationToken)
    {
        var batchProcessor = new AsyncEnumerableBatchProcessor<Entry<dynamic>>(
            items: contentfulService.GetEntriesAsync(contentTypeId: command.ContentTypeId, cancellationToken: cancellationToken),
            batchSize: 50,
            batchActionAsync: contentfulService.DeleteEntriesAsync);

        await batchProcessor.ProcessAsync(cancellationToken);
    }
}
