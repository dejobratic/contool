using Contentful.Core;
using Contentful.Core.Models;
using Contentful.Core.Models.Management;

namespace Contool.Core.Infrastructure.Contentful.Services;

public class ContentfulManagementClientAdapter(
    IContentfulManagementClient client) : IContentfulManagementClientAdapter
{
    public Task<Space> GetSpaceAsync(string spaceId, CancellationToken cancellationToken)
    {
        return client.GetSpace(
            spaceId,
            cancellationToken);
    }

    public Task<ContentfulEnvironment> GetEnvironmentAsync(string environmentId, CancellationToken cancellationToken)
    {
        return client.GetEnvironment(
            environmentId,
            cancellationToken: cancellationToken);
    }

    public Task<User> GetCurrentUser(CancellationToken cancellationToken)
    {
        return client.GetCurrentUser(
            cancellationToken);
    }

    public async Task<IEnumerable<Locale>> GetLocalesCollectionAsync(CancellationToken cancellationToken)
    {
        return await client.GetLocalesCollection(
            cancellationToken: cancellationToken);
    }

    public Task<ContentType?> GetContentTypeAsync(string contentTypeId, CancellationToken cancellationToken)
    {
        return client.GetContentType(
            contentTypeId: contentTypeId,
            cancellationToken: cancellationToken);
    }

    public Task<IEnumerable<ContentType>> GetContentTypesAsync(CancellationToken cancellationToken)
    {
        return client.GetContentTypes(
            cancellationToken: cancellationToken);
    }

    public Task<ContentType> CreateOrUpdateContentTypeAsync(ContentType contentType, CancellationToken cancellationToken)
    {
        return client.CreateOrUpdateContentType(
            contentType: contentType,
            cancellationToken: cancellationToken);
    }

    public Task<ContentType> ActivateContentTypeAsync(string contentTypeId, int version, CancellationToken cancellationToken)
    {
        return client.ActivateContentType(
            contentTypeId: contentTypeId,
            version: version,
            cancellationToken: cancellationToken);
    }

    public Task DeactivateContentTypeAsync(string contentTypeId, CancellationToken cancellationToken)
    {
        return client.DeactivateContentType(
            contentTypeId: contentTypeId,
            cancellationToken: cancellationToken);
    }

    public Task DeleteContentTypeAsync(string contentTypeId, CancellationToken cancellationToken)
    {
        return client.DeleteContentType(
            contentTypeId: contentTypeId,
            cancellationToken: cancellationToken);
    }

    public Task<ContentfulCollection<Entry<dynamic>>> GetEntriesCollectionAsync(string queryString, CancellationToken cancellationToken)
    {
        return client.GetEntriesCollection<Entry<dynamic>>(
            queryString: queryString,
            cancellationToken: cancellationToken);
    }

    public Task<Entry<dynamic>> CreateOrUpdateEntryAsync(Entry<dynamic> entry, int version, CancellationToken cancellationToken)
    {
        return client.CreateOrUpdateEntry(
            entry,
            contentTypeId: entry.SystemProperties?.ContentType?.SystemProperties?.Id,
            version: version,
            cancellationToken: cancellationToken);
    }

    public Task<Entry<dynamic>> PublishEntryAsync(string entryId, int version, CancellationToken cancellationToken)
    {
        return client.PublishEntry(
            entryId: entryId,
            version: version,
            cancellationToken: cancellationToken);
    }

    public Task<Entry<dynamic>> UnpublishEntryAsync(string entryId, int version, CancellationToken cancellationToken)
    {
        return client.UnpublishEntry(
            entryId: entryId,
            version: version,
            cancellationToken: cancellationToken);
    }

    public Task<Entry<dynamic>> ArchiveEntryAsync(string entryId, int version, CancellationToken cancellationToken)
    {
        return client.ArchiveEntry(
            entryId: entryId,
            version: version,
            cancellationToken: cancellationToken);
    }

    public Task<Entry<dynamic>> UnarchiveEntryAsync(string entryId, int version, CancellationToken cancellationToken)
    {
        return client.UnarchiveEntry(
            entryId: entryId,
            version: version,
            cancellationToken: cancellationToken);
    }

    public Task DeleteEntryAsync(string entryId, int version, CancellationToken cancellationToken)
    {
        return client.DeleteEntry(
            entryId: entryId,
            version: version,
            cancellationToken: cancellationToken);
    }
}
