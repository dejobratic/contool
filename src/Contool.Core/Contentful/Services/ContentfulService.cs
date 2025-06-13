using Contentful.Core.Errors;
using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Contentful.Core.Search;
using Contool.Core.Contentful.Extensions;
using System.Runtime.CompilerServices;

namespace Contool.Core.Contentful.Services;

public class ContentfulService(IContentfulManagementClientAdapter client) : IContentfulService
{
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
            contentType.SystemProperties.Id,
            cancellationToken);

        if (existingContentType is not null)
            throw new InvalidOperationException($"Content type with ID '{contentType.SystemProperties.Id}' already exists.");

        var created = await client.CreateOrUpdateContentTypeAsync(
            contentType,
            cancellationToken: cancellationToken);

        created = await client.ActivateContentTypeAsync(
            contentTypeId: created.SystemProperties.Id,
            version: created.SystemProperties.Version!.Value,
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

    public async IAsyncEnumerable<Entry<dynamic>> GetEntriesAsync(
        string? contentTypeId = null,
        QueryBuilder<Entry<dynamic>>? query = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int skip = 0;
        int pageSize = 100;
        bool moreResults = true;

        query ??= new QueryBuilder<Entry<dynamic>>()
            .ContentTypeIs(contentTypeId?.ToCamelCase());

        var queryString = query.Build();

        while (moreResults)
        {
            var entries = await client
                .GetEntriesCollectionAsync($"{queryString}&skip={skip}&limit={pageSize}", cancellationToken: cancellationToken);

            if (entries == null || !entries.Any())
            {
                yield break;
            }

            foreach (var entry in entries)
            {
                yield return entry;
            }

            if (entries.Count() < pageSize)
            {
                moreResults = false;
            }
            else
            {
                skip += pageSize;
            }
        }
    }

    public async Task CreateOrUpdateEntriesAsync(IEnumerable<Entry<dynamic>> entries, bool publish = false, CancellationToken cancellationToken = default)
    {
        async Task<Dictionary<string, Entry<dynamic>>> GetExistingEntriesLookup()
        {
            var queryString = QueryBuilder<Entry<dynamic>>.New
                .FieldIncludes("sys.id", entries.Select(e => e.SystemProperties.Id))
                .Build();

            return (await client.GetEntriesCollectionAsync(
                queryString: queryString,
                cancellationToken: cancellationToken))
                .ToDictionary(e => e.SystemProperties.Id);
        }

        async Task<Dictionary<string, HashSet<string>>> GetUnpublishedOrMissingReferencedEntriesIdsLookup()
        {
            var referencedEntryIdsPerEntry = entries.ToDictionary(
                entry => entry.SystemProperties.Id,
                entry => entry.GetReferencedEntryIds().ToHashSet());

            var allReferencedEntryIds = referencedEntryIdsPerEntry
                .SelectMany(kvp => kvp.Value)
                .Distinct();

            var queryString = QueryBuilder<Entry<dynamic>>.New
                .FieldIncludes("sys.id", allReferencedEntryIds)
                .Build();

            var publishedReferencedEntryIds = (await client.GetEntriesCollectionAsync(
                queryString: queryString,
                cancellationToken: cancellationToken))
                .Where(e => e.IsPublished())
                .Select(e => e.SystemProperties.Id)
                .ToHashSet();

            var unresolvedReferencedEntryIds = allReferencedEntryIds
                .Except(publishedReferencedEntryIds)
                .ToHashSet();

            return referencedEntryIdsPerEntry.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Intersect(unresolvedReferencedEntryIds).ToHashSet());
        }

        bool AllReferencedEntriesArePublished(Entry<dynamic> entry, Dictionary<string, HashSet<string>> unpublishedReferencedEntriesIds)
            => unpublishedReferencedEntriesIds.TryGetValue(entry.SystemProperties!.Id, out var unresolvedRefs)
               && unresolvedRefs.Count == 0;

        var existingEntriesLookupTask = GetExistingEntriesLookup();
        var unpublishedReferencedEntriesIdsLookupTask = GetUnpublishedOrMissingReferencedEntriesIdsLookup();
        await Task.WhenAll(existingEntriesLookupTask, unpublishedReferencedEntriesIdsLookupTask);

        var existingEntriesLookup = await existingEntriesLookupTask;
        var unpublishedReferencedEntriesIdsLookup = await unpublishedReferencedEntriesIdsLookupTask;

        var tasks = entries.Select(async entry =>
        {
            existingEntriesLookup.TryGetValue(entry.SystemProperties!.Id, out var existing);

            var upserted = await client.CreateOrUpdateEntryAsync(
                entry,
                version: existing?.SystemProperties?.Version ?? 0,
                cancellationToken: cancellationToken);

            if (publish && AllReferencedEntriesArePublished(entry, unpublishedReferencedEntriesIdsLookup))
            {
                await client.PublishEntryAsync(
                    entryId: upserted.SystemProperties?.Id!,
                    version: upserted?.SystemProperties?.Version ?? 0,
                    cancellationToken: cancellationToken);
            }
        });

        await Task.WhenAll(tasks);
    }


    public async Task PublishEntriesAsync(IEnumerable<Entry<dynamic>> entries, CancellationToken cancellationToken = default)
    {
        var tasks = entries.Select(async entry =>
        {
            await client.PublishEntryAsync(
                entryId: entry.SystemProperties?.Id!,
                version: entry.SystemProperties?.Version ?? 0,
                cancellationToken: cancellationToken);
        });

        await Task.WhenAll(tasks);
    }

    public async Task DeleteEntriesAsync(IEnumerable<Entry<dynamic>> entries, CancellationToken cancellationToken = default)
    {
        async Task<Dictionary<string, Entry<dynamic>>> GetExistingEntriesLookup()
        {
            var query = QueryBuilder<Entry<dynamic>>.New
                .FieldIncludes("sys.id", entries.Select(e => e.SystemProperties.Id));

            return (await GetEntriesAsync(
                query: query,
                cancellationToken: cancellationToken).ToListAsync(cancellationToken))
                .ToDictionary(e => e.SystemProperties.Id);
        }

        var existingEntriesLookup = await GetExistingEntriesLookup();

        var tasks = entries.Select(async entry =>
        {
            existingEntriesLookup.TryGetValue(entry.SystemProperties!.Id, out var existing);

            var entryId = existing?.SystemProperties.Id ?? entry.SystemProperties.Id;
            var version = existing?.SystemProperties.Version ?? entry.SystemProperties.Version ?? 0;

            if (existing?.IsPublished() == true)
            {
                await client.UnpublishEntryAsync(
                    entryId: entryId,
                    version: version,
                    cancellationToken: cancellationToken);
            }
            else if (existing?.IsArchived() == true)
            {
                await client.UnarchiveEntryAsync(
                    entryId: entryId,
                    version: version,
                    cancellationToken: cancellationToken);
            }

            await client.DeleteEntryAsync(
                entryId: entryId,
                version: version,
                cancellationToken: cancellationToken);
        });

        await Task.WhenAll(tasks);
    }
}
