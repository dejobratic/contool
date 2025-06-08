using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Contentful.Core.Search;
using System.Runtime.CompilerServices;

namespace Contool.Contentful.Services;

internal class ContentfulService(IContentfulManagementClientAdapter client) : IContentfulService
{
    public async Task<IEnumerable<Locale>> GetLocalesAsync(CancellationToken cancellationToken)
    {
        return await client.GetLocalesCollectionAsync(cancellationToken);
    }

    public async Task<ContentType?> GetContentTypeAsync(string contentTypeId, CancellationToken cancellationToken)
    {
        return await client.GetContentTypeAsync(contentTypeId, cancellationToken: cancellationToken);
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

    public async IAsyncEnumerable<Entry<dynamic>> GetEntriesAsync(
        string contentTypeId,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int skip = 0;
        int pageSize = 100;
        bool moreResults = true;

        var query = new QueryBuilder<dynamic>()
            .ContentTypeIs(contentTypeId.ToCamelCase());

        while (moreResults)
        {
            var queryString = query
                .Limit(pageSize)
                .Skip(skip)
                .Build();

            var entries = await client
                .GetEntriesCollectionAsync(queryString, cancellationToken: cancellationToken);

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

    public async Task UpsertEntriesAsync(IEnumerable<Entry<dynamic>> entries, bool publish = false, CancellationToken cancellationToken = default)
    {
        // Load existing entries into lookup for fast access
        var existingEntriesLookup = (await GetEntriesAsync(
            contentTypeId: entries.First().SystemProperties.ContentType.SystemProperties.Id,
            cancellationToken: cancellationToken).ToListAsync(cancellationToken))
            .ToDictionary(e => e.SystemProperties.Id);

        foreach (var entry in entries)
        {
            existingEntriesLookup.TryGetValue(entry.SystemProperties!.Id, out var existing);

            var upserted = await client.CreateOrUpdateEntryAsync(
                entry,
                version: existing?.SystemProperties?.Version ?? 0,
                cancellationToken: cancellationToken);

            if (publish)
            {
                await client.PublishEntryAsync(
                    entryId: upserted.SystemProperties?.Id!,
                    version: upserted?.SystemProperties?.Version ?? 0,
                    cancellationToken: cancellationToken);
            }
        }
    }

    public async Task PublishEntriesAsync(IEnumerable<Entry<dynamic>> entries, CancellationToken cancellationToken = default)
    {
        foreach (var entry in entries)
        {
            await client.PublishEntryAsync(
                entryId: entry.SystemProperties?.Id!,
                version: entry.SystemProperties?.Version ?? 0,
                cancellationToken: cancellationToken);
        }
    }
}
