using Contentful.Core;
using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Contentful.Core.Search;
using System.Runtime.CompilerServices;

namespace Contool.Contentful.Services;

internal class ContentfulService(IContentfulManagementClient client) : IContentfulService
{
    public async Task<IEnumerable<Locale>> GetLocalesAsync(CancellationToken cancellationToken)
    {
        return await client.GetLocalesCollection(cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<ContentType>> GetContentTypesAsync(string? spaceId = null, CancellationToken cancellationToken = default)
    {
        return await client.GetContentTypes(spaceId, cancellationToken);
    }

    public async IAsyncEnumerable<Entry<dynamic>> GetEntriesAsync(string contentTypeId, string? spaceId = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
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
                .GetEntriesCollection<Entry<dynamic>>(queryString, spaceId, cancellationToken);

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

    public async Task UpsertEntriesAsync(IEnumerable<Entry<dynamic>> entries, string? spaceId = null, CancellationToken cancellationToken = default)
    {
        foreach (var entry in entries)
        {
            await client.CreateOrUpdateEntry(
                entry.Fields,
                entry.SystemProperties?.Id,
                spaceId: spaceId,
                contentTypeId: entry.SystemProperties?.ContentType?.SystemProperties?.Id,
                version: entry.SystemProperties?.Version ?? 0,
                cancellationToken: cancellationToken);
        }
    }

    public async Task PublishEntriesAsync(IEnumerable<Entry<dynamic>> entries, string? spaceId = null, CancellationToken cancellationToken = default)
    {
        foreach (var entry in entries)
        {
            await client.PublishEntry(
                entryId: entry.SystemProperties?.Id,
                version: entry.SystemProperties?.Version ?? 0,
                spaceId: spaceId,
                cancellationToken: cancellationToken);
        }
    }
}