using Contool.Contentful.Services;
using Contentful.Core.Models;

namespace Contool.Commands;

internal class TypeCloneCommand
{
    public string ContentTypeId { get; init; } = default!;
    public string SourceEnvironmentId { get; init; } = default!;
    public string TargetEnvironmentId { get; init; } = default!;
}

internal class TypeCloneCommandHandler(
    IContentfulService contentfulService)
{
    public async Task HandleAsync(TypeCloneCommand command, CancellationToken cancellationToken = default)
    {
        var sourceContentType = await GetContentTypeAsync(command.ContentTypeId, command.SourceEnvironmentId, cancellationToken);
        
        var targetContentType = await GetContentTypeAsync(command.ContentTypeId, command.TargetEnvironmentId, cancellationToken);

        // throw exception if their definitions are different
        if (AreEquivalent(sourceContentType, targetContentType))
        {
            await CloneContentTypesAsync(sourceContentType, targetContentType, cancellationToken);
        }

        throw new InvalidOperationException($"Content types '{command.ContentTypeId}' in source and target environments are not equivalent.");
    }

    private Task<ContentType> GetContentTypeAsync(string contentTypeId, string environmentId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private static bool AreEquivalent(ContentType sourceContentType, ContentType targetContentType)
    {
        // Implement logic to compare the structure and fields of the content types
        return true; // Placeholder for actual comparison logic
    }

    private Task CloneContentTypesAsync(ContentType sourceContentType, ContentType targetContentType, CancellationToken cancellationToken)
    {
        // Implement logic to clone the content type from source to target environment
        throw new NotImplementedException("Cloning content types is not implemented yet.");
    }

}

