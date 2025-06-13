using Contentful.Core.Models;
using Contool.Core.Contentful.Extensions;
using Contool.Core.Contentful.Services;
using Contool.Core.Infrastructure.Utils;

namespace Contool.Core.Features.TypeClone;

public class TypeCloneCommand : CommandBase
{
    public string ContentTypeId { get; init; } = default!;

    public string TargetEnvironmentId { get; init; } = default!;

    public bool ShouldPublish { get; init; }
}

public class TypeCloneCommandHandler(
    IContentfulServiceBuilder contentfulServiceBuilder)
{
    public async Task HandleAsync(TypeCloneCommand command, CancellationToken cancellationToken = default)
    {
        var sourceContentfulService = contentfulServiceBuilder
            .Build(command.SpaceId, command.EnvironmentId);

        var targetContentfulService = contentfulServiceBuilder
            .Build(command.SpaceId, command.TargetEnvironmentId);

        await ThrowIfLocalesDifferBetweenEnvironmentsAsync(
            sourceContentfulService, targetContentfulService, cancellationToken);

        await ThrowIfContentTypeDefinitionsDifferBetweenEnvironmentsAsync(
            command, sourceContentfulService, targetContentfulService, cancellationToken);

        await CloneEntriesAsync(
            command, sourceContentfulService, targetContentfulService, cancellationToken);
    }

    private static async Task ThrowIfLocalesDifferBetweenEnvironmentsAsync(IContentfulService sourceContentfulService, IContentfulService targetContentfulService, CancellationToken cancellationToken)
    {
        var sourceLocales = await sourceContentfulService.GetLocalesAsync(cancellationToken);

        var targetLocales = await targetContentfulService.GetLocalesAsync(cancellationToken);

        if (!sourceLocales.IsEquivalentTo(targetLocales))
        {
            throw new InvalidOperationException($"Locales in source and target environments are not equivalent.");
        }
    }

    private static async Task ThrowIfContentTypeDefinitionsDifferBetweenEnvironmentsAsync(TypeCloneCommand command, IContentfulService sourceContentfulService, IContentfulService targetContentfulService, CancellationToken cancellationToken)
    {
        var sourceContentType = await GetContentTypeAsync(
            command.ContentTypeId, sourceContentfulService, required: true, cancellationToken);

        var targetContentType = await GetContentTypeAsync(
            command.ContentTypeId, targetContentfulService, required: false, cancellationToken);

        targetContentType ??= await CloneContentTypeAsync(
            sourceContentType!, targetContentfulService, cancellationToken);

        if (!sourceContentType!.IsEquivalentTo(targetContentType))
        {
            throw new InvalidOperationException($"Content types '{command.ContentTypeId}' in source and target environments are not equivalent.");
        }
    }

    private static async Task<ContentType?> GetContentTypeAsync(string contentTypeId, IContentfulService contentfulService, bool required = false, CancellationToken cancellationToken = default)
    {
        var contentType = await contentfulService.GetContentTypeAsync(contentTypeId, cancellationToken);

        return contentType is null && required
            ? throw new InvalidOperationException($"Content type '{contentTypeId}' does not exist.")
            : contentType;
    }

    private static async Task<ContentType> CloneContentTypeAsync(ContentType contentType, IContentfulService contentfulService, CancellationToken cancellationToken)
    {
        return await contentfulService.CreateContentTypeAsync(contentType.Clone(), cancellationToken);
    }

    private static async Task CloneEntriesAsync(TypeCloneCommand command, IContentfulService sourceContentfulService, IContentfulService targetContentfulService, CancellationToken cancellationToken)
    {
        var batchProcessor = new AsyncEnumerableBatchProcessor<Entry<dynamic>>(
            items: sourceContentfulService.GetEntriesAsync(command.ContentTypeId, cancellationToken: cancellationToken),
            batchSize: 50,
            batchActionAsync: async (entries, ct) =>
            {
                await targetContentfulService.CreateOrUpdateEntriesAsync(
                    entries,
                    publish: command.ShouldPublish,
                    ct);
            },
            shouldInclude: entry => !entry.IsArchived());

        await batchProcessor.ProcessAsync(cancellationToken);
    }
}

