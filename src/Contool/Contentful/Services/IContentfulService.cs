using Contentful.Core.Models;
using Contentful.Core.Models.Management;

namespace Contool.Contentful.Services;

internal interface IContentfulService
{
    Task<IEnumerable<Locale>> GetLocalesAsync(CancellationToken cancellationToken);

    Task<ContentType?> GetContentTypeAsync(string contentTypeId, CancellationToken cancellationToken);

    Task<IEnumerable<ContentType>> GetContentTypesAsync(CancellationToken cancellationToken = default);

    Task<ContentType> CreateContentTypeAsync(ContentType contentType, CancellationToken cancellationToken = default);

    IAsyncEnumerable<Entry<dynamic>> GetEntriesAsync(string contentTypeId, CancellationToken cancellationToken = default);

    Task UpsertEntriesAsync(IEnumerable<Entry<dynamic>> entries, bool publish = false, CancellationToken cancellationToken = default);

    Task PublishEntriesAsync(IEnumerable<Entry<dynamic>> entries, CancellationToken cancellationToken = default);
}
