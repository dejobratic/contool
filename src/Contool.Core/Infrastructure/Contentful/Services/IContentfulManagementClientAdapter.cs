using Contentful.Core.Models;
using Contentful.Core.Models.Management;

namespace Contool.Core.Infrastructure.Contentful.Services;

public interface IContentfulManagementClientAdapter
{
    Task<IEnumerable<Locale>> GetLocalesCollectionAsync(CancellationToken cancellationToken);

    Task<ContentType?> GetContentTypeAsync(string contentTypeId, CancellationToken cancellationToken);

    Task<IEnumerable<ContentType>> GetContentTypesAsync(CancellationToken cancellationToken);

    Task<ContentType> CreateOrUpdateContentTypeAsync(ContentType contentType, CancellationToken cancellationToken);

    Task<ContentType> ActivateContentTypeAsync(string contentTypeId, int version, CancellationToken cancellationToken);

    Task DeactivateContentTypeAsync(string contentTypeId, CancellationToken cancellationToken);

    Task DeleteContentTypeAsync(string contentTypeId, CancellationToken cancellationToken);

    Task<ContentfulCollection<Entry<dynamic>>> GetEntriesCollectionAsync(string queryString, CancellationToken cancellationToken);

    Task<Entry<dynamic>> CreateOrUpdateEntryAsync(Entry<dynamic> entry, int version, CancellationToken cancellationToken);

    Task<Entry<dynamic>> PublishEntryAsync(string entryId, int version, CancellationToken cancellationToken);

    Task<Entry<dynamic>> UnpublishEntryAsync(string entryId, int version, CancellationToken cancellationToken);

    Task<Entry<dynamic>> ArchiveEntryAsync(string entryId, int version, CancellationToken cancellationToken);

    Task<Entry<dynamic>> UnarchiveEntryAsync(string entryId, int version, CancellationToken cancellationToken);

    Task DeleteEntryAsync(string entryId, int version, CancellationToken cancellationToken);
}