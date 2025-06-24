using Contentful.Core.Errors;
using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Contool.Core.Infrastructure.Contentful.Extensions;
using Contool.Core.Infrastructure.Contentful.Utils;
using Contool.Core.Infrastructure.Utils.Models;
using Contool.Core.Infrastructure.Utils.Services;

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
        async Task CreateOrUpdateEntry(
            Entry<dynamic> entry, bool publish, Dictionary<string, Entry<dynamic>> existingEntriesLookup, Dictionary<string, HashSet<string>> unpublishedReferencedEntriesLookup, CancellationToken cancellationToken)
        {
            var version = existingEntriesLookup.TryGetValue(entry.GetId(), out var existing)
                ? existing.GetVersion()
                : entry.GetVersion();

            var archived = existing?.IsArchived() == true;

            // all referenced entries are published
            var canPublish = unpublishedReferencedEntriesLookup.TryGetValue(entry.GetId(), out var unpublishedReferencedEntries)
                && unpublishedReferencedEntries.Count == 0;

            await ExecuteCreateOrUpdateEntry(entry, version, archived, publish && canPublish, cancellationToken);
        }

        async Task ExecuteCreateOrUpdateEntry(
            Entry<dynamic> entry, int version, bool archived, bool publish, CancellationToken cancellationToken)
        {
            if (archived)
            {
                var unarchived = await client.UnarchiveEntryAsync(
                    entryId: entry.GetId(),
                    version: version,
                    cancellationToken: cancellationToken);

                version = unarchived.GetVersion();
            }

            var upserted = await client.CreateOrUpdateEntryAsync(
                entry,
                version: version,
                cancellationToken: cancellationToken);

            if (publish)
            {
                await client.PublishEntryAsync(
                    entryId: upserted.GetId(),
                    version: upserted.GetVersion(),
                    cancellationToken: cancellationToken);
            }
        }

        async Task<(Dictionary<string, Entry<dynamic>>, Dictionary<string, HashSet<string>>)> GetLookupsAsync(
            IEnumerable<Entry<dynamic>> entries, CancellationToken cancellationToken)
        {
            var existingEntriesLookupTask = client.GetExistingEntriesLookupByIdAsync(entries.Select(e => e.GetId()), cancellationToken);
            var unpublishedReferencedEntriesLookupTask = client.GetUnpublishedOrMissingReferencedEntriesIdsLookup(entries, cancellationToken);
            await Task.WhenAll(existingEntriesLookupTask, unpublishedReferencedEntriesLookupTask);

            return (await existingEntriesLookupTask, await unpublishedReferencedEntriesLookupTask);
        }

        var (existingEntriesLookup, unpublishedReferencedEntriesLookup) = await GetLookupsAsync(entries, cancellationToken);

        var tasks = entries.Select(async entry =>
        {
            try
            {
                await CreateOrUpdateEntry(entry, publish, existingEntriesLookup, unpublishedReferencedEntriesLookup, cancellationToken);
                progressReporter.Increment();
            }
            catch
            {
                // will be tracked by operation tracker
            }
        });

        await Task.WhenAll(tasks);
    }

    public async Task PublishEntriesAsync(IEnumerable<Entry<dynamic>> entries, CancellationToken cancellationToken = default)
    {
        async Task PublishEntry(Entry<dynamic> entry, CancellationToken cancellationToken)
        {
            if (entry.IsPublished() || entry.IsArchived())
                return;

            await client.PublishEntryAsync(
                entryId: entry.GetId(),
                version: entry.GetVersion(),
                cancellationToken: cancellationToken);
        }

        var tasks = entries.Select(async entry =>
        {
            try
            {
                await PublishEntry(entry, cancellationToken);
                progressReporter.Increment();
            }
            catch
            {
                // will be tracked by operation tracker
            }
        });

        await Task.WhenAll(tasks);
    }

    public async Task UnpublishEntriesAsync(IEnumerable<Entry<dynamic>> entries, CancellationToken cancellationToken = default)
    {
        async Task UnpublishEntry(Entry<dynamic> entry, CancellationToken cancellationToken)
        {
            if (!entry.IsPublished())
                return;

            await client.UnpublishEntryAsync(
                entryId: entry.GetId(),
                version: entry.GetVersion(),
                cancellationToken: cancellationToken);
        }

        var tasks = entries.Select(async entry =>
        {
            try
            {
                await UnpublishEntry(entry, cancellationToken);
                progressReporter.Increment();
            }
            catch
            {
                // will be tracked by operation tracker
            }
        });

        await Task.WhenAll(tasks);
    }

    public async Task DeleteEntriesAsync(IEnumerable<Entry<dynamic>> entries, CancellationToken cancellationToken = default)
    {
        async Task DeleteEntryAsync(Entry<dynamic> entry, CancellationToken cancellationToken)
        {
            var entryId = entry.GetId();

            if (entry.IsPublished())
            {
                entry = await client.UnpublishEntryAsync(
                    entryId: entryId,
                    version: entry.GetVersion(),
                    cancellationToken: cancellationToken);
            }
            else if (entry.IsArchived())
            {
                entry = await client.UnarchiveEntryAsync(
                    entryId: entryId,
                    version: entry.GetVersion(),
                    cancellationToken: cancellationToken);
            }

            await client.DeleteEntryAsync(
                entryId: entryId,
                version: entry.GetVersion(),
                cancellationToken: cancellationToken);
        }

        var tasks = entries.Select(async entry =>
        {
            try
            {
                await DeleteEntryAsync(entry, cancellationToken);
                progressReporter.Increment();
            }
            catch
            {
                // will be tracked by operation tracker
            }
        });

        await Task.WhenAll(tasks);
    }
}
