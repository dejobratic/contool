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
    {
        return await client.GetLocalesCollectionAsync(cancellationToken);
    }

    public async Task<ContentType?> GetContentTypeAsync(string contentTypeId, CancellationToken cancellationToken)
    {
        try
        {
            return await client.GetContentTypeAsync(
                contentTypeId: contentTypeId,
                cancellationToken: cancellationToken);
        }
        catch (ContentfulException)
        {
            return null;
        }
    }

    public async Task<IEnumerable<ContentType>> GetContentTypesAsync(CancellationToken cancellationToken = default)
    {
        return await client.GetContentTypesAsync(cancellationToken);
    }

    public async Task<ContentType> CreateContentTypeAsync(ContentType contentType, CancellationToken cancellationToken = default)
    {
        var existingContentType = await GetContentTypeAsync(
            contentType.GetId()!,
            cancellationToken);

        if (existingContentType is not null)
            throw new InvalidOperationException($"Content type with ID '{contentType.GetId()}' already exists.");

        var created = await client.CreateOrUpdateContentTypeAsync(
            contentType,
            cancellationToken: cancellationToken);

        created = await client.ActivateContentTypeAsync(
            contentTypeId: created.GetId()!,
            version: created.GetVersion(),
            cancellationToken: cancellationToken);

        return created;
    }

    public async Task DeleteContentTypeAsync(string contentTypeId, CancellationToken cancellationToken = default)
    {
        await client.DeactivateContentTypeAsync(
            contentTypeId: contentTypeId,
            cancellationToken: cancellationToken);

        await client.DeleteContentTypeAsync(
            contentTypeId: contentTypeId,
            cancellationToken: cancellationToken);
    }

    public IAsyncEnumerableWithTotal<Entry<dynamic>> GetEntriesAsync(
        string contentTypeId,
        int? pageSize = null,
        PagingMode pagingMode = PagingMode.SkipForward,
        CancellationToken cancellationToken = default)
    {
        async Task<ContentfulCollection<Entry<dynamic>>> GetEntriesByQueryAsync(string queryString, CancellationToken ct)
            => await client.GetEntriesCollectionAsync(queryString, cancellationToken: ct);

        var query = new EntryQueryBuilder()
            .WithContentTypeId(contentTypeId)
            .Limit(pageSize ?? DefaultBatchSize);

        return new EntryAsyncEnumerableWithTotal<dynamic>(GetEntriesByQueryAsync, query, pagingMode);
    }

    public async Task CreateOrUpdateEntriesAsync(IReadOnlyList<Entry<dynamic>> entries, bool publish = false, CancellationToken cancellationToken = default)
    {
        var existingEntriesLookup = await client.GetExistingEntriesLookupByIdAsync(
            entries.Select(e => e.GetId()!), cancellationToken);

        // First, create or update all entries without publishing
        await ExecuteAsync(
            entries,
            entryAction: async (entry, ct) =>
            {
                var version = existingEntriesLookup.TryGetValue(entry.GetId()!, out var existing)
                    ? existing.GetVersion()
                    : entry.GetVersion();

                await entryOperationService.CreateOrUpdateEntryAsync(entry, version, ct);
            },
            cancellationToken);

        // Then, if publish is requested, bulk publish all entries that can be published
        if (publish)
        {
            var unpublishedReferencedEntriesLookup = await client.GetUnpublishedOrMissingReferencedEntriesIdsLookup(
                entries, cancellationToken);
            
            var entriesToPublish = entries.Where(entry =>
            {
                // all referenced entries are published
                var allReferencedEntriesArePublished = unpublishedReferencedEntriesLookup.TryGetValue(entry.GetId()!, out var unpublishedReferencedEntries)
                    && unpublishedReferencedEntries.Count == 0;
                
                return allReferencedEntriesArePublished && !entry.IsArchived();
            })
            .ToList();

            await entryBulkOperationService.PublishEntriesAsync(entriesToPublish, cancellationToken);
        }
    }

    public async Task PublishEntriesAsync(IReadOnlyList<Entry<dynamic>> entries, CancellationToken cancellationToken = default)
    {
        var entriesToPublish = entries
            .Where(e => !e.IsPublished() && !e.IsArchived())
            .ToList();
        
        await entryBulkOperationService.PublishEntriesAsync(entriesToPublish, cancellationToken);
    }

    public async Task UnpublishEntriesAsync(IReadOnlyList<Entry<dynamic>> entries, CancellationToken cancellationToken = default)
    {
        var entriesToUnpublish = entries
            .Where(e => e.IsPublished())
            .ToList();
        
        await entryBulkOperationService.UnpublishEntriesAsync(entriesToUnpublish, cancellationToken);
    }

    public async Task DeleteEntriesAsync(IReadOnlyList<Entry<dynamic>> entries, CancellationToken cancellationToken = default)
    {
        await UnpublishEntriesAsync(
            entries,
            cancellationToken);
        
        await ExecuteAsync(
            entries,
            entryAction: (entry, ct) => entryOperationService.DeleteEntryAsync(entry, ct),
            cancellationToken);
    }

    private static async Task ExecuteAsync(
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
                // will be tracked by operation tracker
            }
        });

        await Task.WhenAll(tasks);
    }
}