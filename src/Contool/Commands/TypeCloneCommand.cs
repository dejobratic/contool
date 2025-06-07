using Contool.Contentful.Services;
using Contentful.Core.Models;
using Contool.Contentful.Extensions;

namespace Contool.Commands;

internal class TypeCloneCommand : CommandBase
{
    public string ContentTypeId { get; init; } = default!;

    public string TargetEnvironmentId { get; init; } = default!;

    public bool ShouldPublish { get; init; }
}

internal class TypeCloneCommandHandler(
    IContentfulServiceBuilder contentfulServiceBuilder)
{
    public async Task HandleAsync(TypeCloneCommand command, CancellationToken cancellationToken = default)
    {
        var sourceContentfulService = contentfulServiceBuilder
            .WithSpaceId(command.SpaceId)
            .WithEnvironmentId(command.EnvironmentId)
            .Build();

        var sourceContentType = await GetContentTypeAsync(
            command.ContentTypeId, sourceContentfulService, required: true, cancellationToken);

        var targetContentfulService = contentfulServiceBuilder
            .WithSpaceId(command.SpaceId)
            .WithEnvironmentId(command.TargetEnvironmentId)
            .Build();

        var targetContentType = await GetContentTypeAsync(
            command.ContentTypeId, targetContentfulService, required: false, cancellationToken);

        targetContentType ??= await CloneContentTypeAsync(
            sourceContentType!, targetContentfulService, cancellationToken);

        if (sourceContentType!.IsEquivalentTo(targetContentType))
        {
            await CloneEntriesAsync(command, sourceContentfulService, targetContentfulService, cancellationToken);
            return;
        }

        throw new InvalidOperationException($"Content types '{command.ContentTypeId}' in source and target environments are not equivalent.");
    }

    private static async Task<ContentType?> GetContentTypeAsync(string contentTypeId, IContentfulService contentfulService, bool required = false, CancellationToken cancellationToken = default)
    {
        var contentType = await contentfulService.GetContentTypeAsync(contentTypeId, cancellationToken);

        return contentType is null && required
            ? throw new ArgumentException($"Content type '{contentTypeId}' does not exist.")
            : contentType;
    }

    private static async Task<ContentType> CloneContentTypeAsync(ContentType contentType, IContentfulService contentfulService, CancellationToken cancellationToken)
    {
        return await contentfulService.CreateContentTypeAsync(contentType.Clone(), cancellationToken);
    }

    private static async Task CloneEntriesAsync(TypeCloneCommand command, IContentfulService sourceContentfulService, IContentfulService targetContentfulService, CancellationToken cancellationToken)
    {
        var buffer = new List<Entry<dynamic>>(capacity: 50);
        await foreach (var entry in sourceContentfulService.GetEntriesAsync(command.ContentTypeId, cancellationToken))
        {
            buffer.Add(entry);

            if (buffer.Count != buffer.Capacity)
                continue;

            await targetContentfulService.UpsertEntriesAsync(
                buffer,
                publish: command.ShouldPublish,
                cancellationToken);

            buffer.Clear();
        }

        if (buffer.Count > 0)
        {
            await targetContentfulService.UpsertEntriesAsync(
                buffer,
                publish: command.ShouldPublish,
                cancellationToken);
        }
    }
}

