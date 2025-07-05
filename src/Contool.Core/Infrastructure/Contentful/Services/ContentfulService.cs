using Contentful.Core.Errors;
using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Contool.Core.Infrastructure.Contentful.Extensions;
using Contool.Core.Infrastructure.Contentful.Utils;
using Contool.Core.Infrastructure.Utils.Models;

namespace Contool.Core.Infrastructure.Contentful.Services;

public class ContentfulService(
    IContentfulManagementClientAdapter client,
    IContentfulEntryOperationService entryOperationService) : IContentfulService
{
    private const int DefaultBatchSize = 50;

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
            contentType.GetId(),
            cancellationToken);

        if (existingContentType is not null)
            throw new InvalidOperationException($"Content type with ID '{contentType.GetId()}' already exists.");

        var created = await client.CreateOrUpdateContentTypeAsync(
            contentType,
            cancellationToken: cancellationToken);

        created = await client.ActivateContentTypeAsync(
            contentTypeId: created.GetId(),
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
        async Task<ContentfulCollection<Entry<dynamic>>> GetEntriesAsync(string queryString, CancellationToken ct)
            => await client.GetEntriesCollectionAsync(queryString, cancellationToken: ct);

        var query = new EntryQueryBuilder()
            .WithContentTypeId(contentTypeId)
            .Limit(pageSize ?? DefaultBatchSize);

        return new EntryAsyncEnumerableWithTotal<dynamic>(GetEntriesAsync, query, pagingMode);
    }

    public async Task CreateOrUpdateEntriesAsync(IEnumerable<Entry<dynamic>> entries, bool publish = false, CancellationToken cancellationToken = default)
    {
        async Task<(Dictionary<string, Entry<dynamic>>, Dictionary<string, HashSet<string>>)> GetLookupsAsync(
            IEnumerable<Entry<dynamic>> entries, CancellationToken cancellationToken)
        {
            var existingEntriesLookupTask = client.GetExistingEntriesLookupByIdAsync(entries.Select(e => e.GetId()), cancellationToken);
            var unpublishedReferencedEntriesLookupTask = client.GetUnpublishedOrMissingReferencedEntriesIdsLookup(entries, cancellationToken);
            await Task.WhenAll(existingEntriesLookupTask, unpublishedReferencedEntriesLookupTask);

            return (await existingEntriesLookupTask, await unpublishedReferencedEntriesLookupTask);
        }

        var (existingEntriesLookup, unpublishedReferencedEntriesLookup) =
            await GetLookupsAsync(entries, cancellationToken);

        await ExecuteAsync(
            entries,
            entryAction: async (entry, ct) =>
            {
                var version = existingEntriesLookup.TryGetValue(entry.GetId(), out var existing)
                    ? existing.GetVersion()
                    : entry.GetVersion();

                var archived = existing?.IsArchived() == true;

                // all referenced entries are published
                var canPublish =
                    unpublishedReferencedEntriesLookup.TryGetValue(entry.GetId(), out var unpublishedReferencedEntries)
                    && unpublishedReferencedEntries.Count == 0;

                await entryOperationService.CreateOrUpdateEntryAsync(entry, version, archived, publish && canPublish,
                    cancellationToken);
            },
            cancellationToken);
    }

    public async Task PublishEntriesAsync(IEnumerable<Entry<dynamic>> entries, CancellationToken cancellationToken = default)
        => await ExecuteAsync(
            entries,
            entryAction: async (entry, ct) => await entryOperationService.PublishEntryAsync(entry, ct),
            cancellationToken);

    public async Task UnpublishEntriesAsync(IEnumerable<Entry<dynamic>> entries, CancellationToken cancellationToken = default)
        => await ExecuteAsync(
            entries,
            entryAction: async (entry, ct) => await entryOperationService.UnpublishEntryAsync(entry, ct),
            cancellationToken);

    public async Task DeleteEntriesAsync(IEnumerable<Entry<dynamic>> entries, CancellationToken cancellationToken = default)
        => await ExecuteAsync(
            entries,
            entryAction: async (entry, ct) => await entryOperationService.DeleteEntryAsync(entry, ct),
            cancellationToken);

    private static async Task ExecuteAsync(
        IEnumerable<Entry<dynamic>> entries,
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