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

        var targetContentType = await GetOrCreateContentTypeAsync(command.ContentTypeId, command.TargetEnvironmentId, cancellationToken);
        
        if (!AreEquivalent(sourceContentType, targetContentType))
            throw new InvalidOperationException($"Content types '{command.ContentTypeId}' in source and target environments are not equivalent.");

        var buffer = new List<Entry<dynamic>>(capacity: 50);
        await foreach (var entry in contentfulService.GetEntriesAsync(command.ContentTypeId, command.SourceEnvironmentId, cancellationToken))
        {
            buffer.Add(entry);
            
            if (buffer.Count != buffer.Capacity)
                continue;
            
            await contentfulService.UpsertEntriesAsync(command.ContentTypeId, command.TargetEnvironmentId, cancellationToken);
            buffer.Clear();
        }
    }

    private Task<ContentType?> GetContentTypeAsync(string contentTypeId, string environmentId, CancellationToken cancellationToken)
    {
        return contentfulService.GetContentTypeAsync(contentTypeId, environmentId, cancellationToken)
            ?? throw new ArgumentException($"Content type '{contentTypeId}' does not exist on environment '{environmentId}'.");
    }
    
    private Task<ContentType> GetOrCreateContentTypeAsync(string contentTypeId, string environmentId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private static bool AreEquivalent(ContentType sourceContentType, ContentType targetContentType)
    {
        // Implement logic to compare the structure and fields of the content types
        return true; // Placeholder for actual comparison logic
    }
}

