using Contentful.Core.Models;
using Contentful.Core.Search;
using Contool.Core.Infrastructure.Contentful.Services;

namespace Contool.Core.Infrastructure.Contentful.Extensions;

public static class ContentfulManagementClientAdapterExtensions
{
    public static async Task<Dictionary<string, Entry<dynamic>>> GetExistingEntriesLookupByIdAsync(
        this IContentfulManagementClientAdapter client,
        IEnumerable<string> entryIds,
        CancellationToken cancellationToken)
    {
        var queryString = QueryBuilder<Entry<dynamic>>.New
            .FieldIncludes("sys.id", entryIds)
            .Build();

        return (await client.GetEntriesCollectionAsync(
            queryString: queryString,
            cancellationToken: cancellationToken))
            .ToDictionary(e => e.GetId());
    }

    public static async Task<Dictionary<string, HashSet<string>>> GetUnpublishedOrMissingReferencedEntriesIdsLookup(
        this IContentfulManagementClientAdapter client,
        IEnumerable<Entry<dynamic>> entries,
        CancellationToken cancellationToken)
    {
        var referencedEntryIdsPerEntry = entries.ToDictionary(
            entry => entry.GetId(),
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
            .Select(e => e.GetId())
            .ToHashSet();

        var unresolvedReferencedEntryIds = allReferencedEntryIds
            .Except(publishedReferencedEntryIds)
            .ToHashSet();

        return referencedEntryIdsPerEntry.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.Intersect(unresolvedReferencedEntryIds).ToHashSet());
    }
}
