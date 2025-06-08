using Contentful.Core.Models;
using Contentful.Core.Models.Management;

namespace Contool.Contentful.Services;

internal interface IContentfulManagementClientAdapter
{
    Task<IEnumerable<Locale>> GetLocalesCollectionAsync(CancellationToken cancellationToken);
    
    Task<ContentType?> GetContentTypeAsync(string contentTypeId, CancellationToken cancellationToken);
    
    Task<IEnumerable<ContentType>> GetContentTypesAsync(CancellationToken cancellationToken);
    
    Task<ContentType> CreateOrUpdateContentTypeAsync(ContentType contentType, CancellationToken cancellationToken);
    
    Task<ContentType> ActivateContentTypeAsync(string contentTypeId, int version, CancellationToken cancellationToken);
    
    Task<IEnumerable<Entry<dynamic>>> GetEntriesCollectionAsync(string queryString, CancellationToken cancellationToken);
    
    Task<Entry<dynamic>> CreateOrUpdateEntryAsync(Entry<dynamic> entry, int version, CancellationToken cancellationToken);
    
    Task<Entry<dynamic>> PublishEntryAsync(string entryId, int version, CancellationToken cancellationToken);
}