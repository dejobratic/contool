using Contentful.Core;
using Contentful.Core.Models;
using Contentful.Core.Models.Management;

namespace Contool.Core.Infrastructure.Contentful.Services;

public class ContentfulManagementClientAdapter(
    IContentfulManagementClient client) : IContentfulManagementClientAdapter
{
    public async Task<IEnumerable<Locale>> GetLocalesCollectionAsync(CancellationToken cancellationToken)
    {
        return await client.GetLocalesCollection(
            cancellationToken: cancellationToken);
    }

    public async Task<ContentType?> GetContentTypeAsync(string contentTypeId, CancellationToken cancellationToken)
    {
        return await client.GetContentType(
            contentTypeId: contentTypeId,
            cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<ContentType>> GetContentTypesAsync(CancellationToken cancellationToken)
    {
        return await client.GetContentTypes(
            cancellationToken: cancellationToken);
    }

    public async Task<ContentType> CreateOrUpdateContentTypeAsync(ContentType contentType, CancellationToken cancellationToken)
    {
        return await client.CreateOrUpdateContentType(
            contentType: contentType,
            cancellationToken: cancellationToken);
    }

    public async Task<ContentType> ActivateContentTypeAsync(string contentTypeId, int version, CancellationToken cancellationToken)
    {
        return await client.ActivateContentType(
            contentTypeId: contentTypeId,
            version: version,
            cancellationToken: cancellationToken);
    }

    public async Task DeactivateContentTypeAsync(string contentTypeId, CancellationToken cancellationToken)
    {
        await client.DeactivateContentType(
            contentTypeId: contentTypeId,
            cancellationToken: cancellationToken);
    }

    public async Task DeleteContentTypeAsync(string contentTypeId, CancellationToken cancellationToken)
    {
        await client.DeleteContentType(
            contentTypeId: contentTypeId,
            cancellationToken: cancellationToken);
    }

    public async Task<ContentfulCollection<Entry<dynamic>>> GetEntriesCollectionAsync(string queryString, CancellationToken cancellationToken)
    {
        return await client.GetEntriesCollection<Entry<dynamic>>(
            queryString: queryString,
            cancellationToken: cancellationToken);
    }

    public async Task<Entry<dynamic>> CreateOrUpdateEntryAsync(Entry<dynamic> entry, int version, CancellationToken cancellationToken)
    {
        return await client.CreateOrUpdateEntry(
            entry,
            contentTypeId: entry.SystemProperties?.ContentType?.SystemProperties?.Id,
            version: version,
            cancellationToken: cancellationToken);
    }

    public async Task<Entry<dynamic>> PublishEntryAsync(string entryId, int version, CancellationToken cancellationToken)
    {
        return await client.PublishEntry(
            entryId: entryId,
            version: version,
            cancellationToken: cancellationToken);
    }

    public async Task<Entry<dynamic>> UnpublishEntryAsync(string entryId, int version, CancellationToken cancellationToken)
    {
        return await client.UnpublishEntry(
            entryId: entryId,
            version: version,
            cancellationToken: cancellationToken);
    }

    public async Task<Entry<dynamic>> ArchiveEntryAsync(string entryId, int version, CancellationToken cancellationToken)
    {
        return await client.ArchiveEntry(
            entryId: entryId,
            version: version,
            cancellationToken: cancellationToken);
    }

    public async Task<Entry<dynamic>> UnarchiveEntryAsync(string entryId, int version, CancellationToken cancellationToken)
    {
        return await client.UnarchiveEntry(
            entryId: entryId,
            version: version,
            cancellationToken: cancellationToken);
    }

    public async Task DeleteEntryAsync(string entryId, int version, CancellationToken cancellationToken)
    {
        await client.DeleteEntry(
            entryId: entryId,
            version: version,
            cancellationToken: cancellationToken);
    }
}
