using Contentful.Core.Errors;
using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Contentful.Core.Search;
using Contool.Core.Infrastructure.Contentful.Extensions;
using Contool.Core.Infrastructure.Contentful.Utils;
using Contool.Core.Infrastructure.Utils;

namespace Contool.Core.Infrastructure.Contentful.Services;

public class ContentfulService(
    IContentfulManagementClientAdapter client,
    IProgressReporter progressReporter) : IContentfulService
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

    public IAsyncEnumerableWithTotal<Entry<dynamic>> GetEntriesAsync(
        string? contentTypeId = null,
        QueryBuilder<Entry<dynamic>>? query = null,
        CancellationToken cancellationToken = default)
    {
        query ??= new QueryBuilder<Entry<dynamic>>()
            .ContentTypeIs(contentTypeId?.ToCamelCase());

        var queryString = query.Build();

        async Task<ContentfulCollection<Entry<dynamic>>> GetEntriesAsync(int skip, int limit, CancellationToken ct)
            => await client.GetEntriesCollectionAsync($"{queryString}&skip={skip}&limit={limit}", cancellationToken: ct);

        return new ContentfulAsyncEnumerableWithTotal<Entry<dynamic>>(GetEntriesAsync, pageSize: DefaultBatchSize);
    }

    public async Task CreateOrUpdateEntriesAsync(IAsyncEnumerableWithTotal<Entry<dynamic>> entries, bool publish = false, CancellationToken cancellationToken = default)
    {
        async Task<Dictionary<string, Entry<dynamic>>> GetExistingEntriesLookup(IEnumerable<Entry<dynamic>> entries)
        {
            var queryString = QueryBuilder<Entry<dynamic>>.New
                .FieldIncludes("sys.id", entries.Select(e => e.SystemProperties.Id))
                .Build();

            return (await client.GetEntriesCollectionAsync(
                queryString: queryString,
                cancellationToken: cancellationToken))
                .ToDictionary(e => e.SystemProperties.Id);
        }

        async Task<Dictionary<string, HashSet<string>>> GetUnpublishedOrMissingReferencedEntriesIdsLookup(IEnumerable<Entry<dynamic>> entries)
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

        var current = 0;

        var batchProcessor = new AsyncEnumerableBatchProcessor<Entry<dynamic>>(
            source: entries,
            batchSize: DefaultBatchSize,
            batchActionAsync: async (batch, ct) =>
            {
                var existingEntriesLookupTask = GetExistingEntriesLookup(batch);
                var unpublishedReferencedEntriesIdsLookupTask = GetUnpublishedOrMissingReferencedEntriesIdsLookup(batch);
                await Task.WhenAll(existingEntriesLookupTask, unpublishedReferencedEntriesIdsLookupTask);

                var existingEntriesLookup = await existingEntriesLookupTask;
                var unpublishedReferencedEntriesIdsLookup = await unpublishedReferencedEntriesIdsLookupTask;

                var tasks = batch.Select(async entry =>
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

                    progressReporter.Report(Interlocked.Increment(ref current), entries.Total);
                });

                await Task.WhenAll(tasks);
            });

        await batchProcessor.ProcessAsync(cancellationToken);
    }


    public async Task PublishEntriesAsync(IAsyncEnumerableWithTotal<Entry<dynamic>> entries, CancellationToken cancellationToken = default)
    {
        var current = 0;

        var batchProcessor = new AsyncEnumerableBatchProcessor<Entry<dynamic>>(
            source: entries,
            batchSize: DefaultBatchSize,
            batchActionAsync: async (batch, ct) =>
            {
                var tasks = batch.Select(async entry =>
                {
                    await client.PublishEntryAsync(
                        entryId: entry.SystemProperties?.Id!,
                        version: entry.SystemProperties?.Version ?? 0,
                        cancellationToken: cancellationToken);

                    progressReporter.Report(Interlocked.Increment(ref current), entries.Total);
                });

                await Task.WhenAll(tasks);
            });

        await batchProcessor.ProcessAsync(cancellationToken);
    }

    public async Task DeleteEntriesAsync(IAsyncEnumerableWithTotal<Entry<dynamic>> entries, CancellationToken cancellationToken = default)
    {
        async Task<Dictionary<string, Entry<dynamic>>> GetExistingEntriesLookup(IEnumerable<Entry<dynamic>> entries)
        {
            var queryString = QueryBuilder<Entry<dynamic>>.New
                .FieldIncludes("sys.id", entries.Select(e => e.SystemProperties.Id))
                .Build();

            return (await client.GetEntriesCollectionAsync(
                queryString: queryString,
                cancellationToken: cancellationToken))
                .ToDictionary(e => e.SystemProperties.Id);
        }

        var current = 0;

        var batchProcessor = new AsyncEnumerableBatchProcessor<Entry<dynamic>>(
            source: entries,
            batchSize: DefaultBatchSize,
            batchActionAsync: async (batch, ct) =>
            {
                var existingEntriesLookup = await GetExistingEntriesLookup(batch);

                var tasks = batch.Select(async entry =>
                {
                    existingEntriesLookup.TryGetValue(entry.SystemProperties!.Id, out var existing);

                    var entryId = existing?.SystemProperties.Id ?? entry.SystemProperties.Id;
                    var version = existing?.SystemProperties.Version ?? entry.SystemProperties.Version ?? 0;

                    try
                    {
                        if (true || existing?.IsPublished() == true)
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
                    }
                    catch (Exception ex)
                    {
                        // swallow
                    }

                    await client.DeleteEntryAsync(
                        entryId: entryId,
                        version: version,
                        cancellationToken: cancellationToken);

                    progressReporter.Report(Interlocked.Increment(ref current), entries.Total);
                });

                await Task.WhenAll(tasks);
            });

        await batchProcessor.ProcessAsync(cancellationToken);
    }
}
