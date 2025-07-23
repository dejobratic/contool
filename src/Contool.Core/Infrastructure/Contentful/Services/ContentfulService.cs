using Contentful.Core.Errors;
using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Contool.Core.Infrastructure.Contentful.Extensions;
using Contool.Core.Infrastructure.Contentful.Utils;
using Contool.Core.Infrastructure.Utils.Models;

namespace Contool.Core.Infrastructure.Contentful.Services;

public class ContentfulService(
    IContentfulManagementClientAdapter client,
    IContentfulEntryOperationService entryOperationService,
    IContentfulEntryBulkOperationService entryBulkOperationService) : IContentfulService
{
    private const int DefaultBatchSize = 100;

    public async Task<IEnumerable<Locale>> GetLocalesAsync(CancellationToken cancellationToken)
        => await client.GetLocalesCollectionAsync(cancellationToken);

    public async Task<ContentType?> GetContentTypeAsync(
        string contentTypeId,
        CancellationToken cancellationToken)
    {
        try
        {
            return await client.GetContentTypeAsync(contentTypeId, cancellationToken);
        }
        catch (ContentfulException)
        {
            return null;
        }
    }

    public async Task<IEnumerable<ContentType>> GetContentTypesAsync(CancellationToken cancellationToken = default)
        => await client.GetContentTypesAsync(cancellationToken);

    public async Task<ContentType> CreateContentTypeAsync(
        ContentType contentType,
        CancellationToken cancellationToken = default)
    {
        var id = contentType.GetId()!;
        if (await GetContentTypeAsync(id, cancellationToken) is not null)
            throw new InvalidOperationException($"Content type with ID '{id}' already exists.");

        var created = await client.CreateOrUpdateContentTypeAsync(contentType, cancellationToken);
        return await client.ActivateContentTypeAsync(created.GetId()!, created.GetVersion(), cancellationToken);
    }

    public async Task DeleteContentTypeAsync(
        string contentTypeId,
        CancellationToken cancellationToken = default)
    {
        await client.DeactivateContentTypeAsync(contentTypeId, cancellationToken);
        await client.DeleteContentTypeAsync(contentTypeId, cancellationToken);
    }

    public IAsyncEnumerableWithTotal<Entry<dynamic>> GetEntriesAsync(
        string contentTypeId,
        int? pageSize = null,
        PagingMode pagingMode = PagingMode.SkipForward,
        CancellationToken cancellationToken = default)
    {
        async Task<ContentfulCollection<Entry<dynamic>>> GetEntriesByQueryAsync(string queryString,
            CancellationToken ct)
            => await client.GetEntriesCollectionAsync(queryString, ct);

        var query = new EntryQueryBuilder()
            .WithContentTypeId(contentTypeId)
            .Limit(pageSize ?? DefaultBatchSize);

        return new EntryAsyncEnumerableWithTotal<dynamic>(GetEntriesByQueryAsync, query, pagingMode);
    }

    public async Task CreateOrUpdateEntriesAsync(
        IReadOnlyList<Entry<dynamic>> entries,
        bool publish = false,
        CancellationToken cancellationToken = default)
    {
        await CreateOrUpdateEntriesAsync(entries, cancellationToken);

        if (publish)
            await PublishEntriesWithValidReferencesAsync(entries, cancellationToken);
    }

    private async Task CreateOrUpdateEntriesAsync(
        IReadOnlyList<Entry<dynamic>> entries,
        CancellationToken cancellationToken)
    {
        var existingEntries = await client.GetExistingEntriesLookupByIdAsync(
            entries.Select(e => e.GetId()!), cancellationToken);

        await ExecuteEntryActionsAsync(
            entries,
            async (entry, ct) =>
            {
                var version = existingEntries.TryGetValue(entry.GetId()!, out var existing)
                    ? existing.GetVersion()
                    : entry.GetVersion();

                await entryOperationService.CreateOrUpdateEntryAsync(entry, version, ct);
            },
            cancellationToken);
    }

    private async Task PublishEntriesWithValidReferencesAsync(
        IReadOnlyList<Entry<dynamic>> entries,
        CancellationToken cancellationToken)
    {
        var unpublishedRefs = await client
            .GetUnpublishedOrMissingReferencedEntriesIdsLookup(entries, cancellationToken);

        var entriesToPublish = GetEntriesToPublish(entries, unpublishedRefs);
        await PublishEntriesAsync(entriesToPublish, cancellationToken);
    }

    private static List<Entry<dynamic>> GetEntriesToPublish(
        IEnumerable<Entry<dynamic>> entries,
        Dictionary<string, HashSet<string>> unpublishedReferencedEntriesLookup)
    {
        return entries
            .Where(entry =>
                unpublishedReferencedEntriesLookup.TryGetValue(entry.GetId()!, out var unpublished)
                && unpublished.Count == 0
                && !entry.IsArchived())
            .ToList();
    }

    public async Task PublishEntriesAsync(
        IReadOnlyList<Entry<dynamic>> entries,
        CancellationToken cancellationToken = default)
    {
        var toPublish = entries.Where(e => !e.IsPublished() && !e.IsArchived()).ToList();
        await entryBulkOperationService.PublishEntriesAsync(toPublish, cancellationToken);
    }

    public async Task UnpublishEntriesAsync(
        IReadOnlyList<Entry<dynamic>> entries,
        CancellationToken cancellationToken = default)
    {
        var toUnpublish = entries.Where(e => e.IsPublished()).ToList();
        await entryBulkOperationService.UnpublishEntriesAsync(toUnpublish, cancellationToken);
    }

    public async Task DeleteEntriesAsync(
        IReadOnlyList<Entry<dynamic>> entries,
        CancellationToken cancellationToken = default)
    {
        await UnpublishEntriesAsync(entries, cancellationToken);

        await ExecuteEntryActionsAsync(
            entries,
            (entry, ct) => entryOperationService.DeleteEntryAsync(entry, ct), cancellationToken);
    }

    private static async Task ExecuteEntryActionsAsync(
        IReadOnlyList<Entry<dynamic>> entries,
        Func<Entry<dynamic>, CancellationToken, Task> entryAction,
        CancellationToken cancellationToken = default)
    {
        var tasks = entries.Select(async entry =>
        {
            try
            {
                await entryAction(entry, cancellationToken);
            }
            catch
            {
                // Errors are silently ignored, assumed to be tracked externally
            }
        });

        await Task.WhenAll(tasks);
    }
}