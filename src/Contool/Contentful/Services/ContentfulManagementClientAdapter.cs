using Contentful.Core;
using Contentful.Core.Errors;
using Contentful.Core.Models;
using Contentful.Core.Models.Management;

namespace Contool.Contentful.Services;

internal class ContentfulManagementClientAdapter(
    IContentfulManagementClient client) : IContentfulManagementClientAdapter
{
    public async Task<IEnumerable<Locale>> GetLocalesCollectionAsync(CancellationToken cancellationToken)
    {
        return await client.GetLocalesCollection(
            cancellationToken: cancellationToken);
    }

    public async Task<ContentType?> GetContentTypeAsync(string contentTypeId, CancellationToken cancellationToken)
    {
        try
        {
            return await client.GetContentType(
                contentTypeId: contentTypeId,
                cancellationToken: cancellationToken);
        }
        catch (ContentfulException)
        {
            return null;
        }
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

    public async Task<IEnumerable<Entry<dynamic>>> GetEntriesCollectionAsync(string queryString, CancellationToken cancellationToken)
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
}
