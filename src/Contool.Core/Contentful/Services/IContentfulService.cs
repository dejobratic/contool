using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Contentful.Core.Search;

namespace Contool.Core.Contentful.Services;

public interface IContentfulService
{
    Task<IEnumerable<Locale>> GetLocalesAsync(CancellationToken cancellationToken);

    Task<ContentType?> GetContentTypeAsync(string contentTypeId, CancellationToken cancellationToken);

    Task<IEnumerable<ContentType>> GetContentTypesAsync(CancellationToken cancellationToken = default);

    Task<ContentType> CreateContentTypeAsync(ContentType contentType, CancellationToken cancellationToken = default);

    Task DeleteContentTypeAsync(string contentTypeId, CancellationToken cancellationToken = default);

    IAsyncEnumerable<Entry<dynamic>> GetEntriesAsync(string? contentTypeId = null, QueryBuilder<Entry<dynamic>>? query = null, CancellationToken cancellationToken = default);

    Task CreateOrUpdateEntriesAsync(IEnumerable<Entry<dynamic>> entries, bool publish = false, CancellationToken cancellationToken = default);

    Task PublishEntriesAsync(IEnumerable<Entry<dynamic>> entries, CancellationToken cancellationToken = default);

    Task DeleteEntriesAsync(IEnumerable<Entry<dynamic>> entries, CancellationToken cancellationToken = default);
}
