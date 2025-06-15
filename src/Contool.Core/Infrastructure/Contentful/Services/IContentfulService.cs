using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Contentful.Core.Search;
using Contool.Core.Infrastructure.Utils;

namespace Contool.Core.Infrastructure.Contentful.Services;

public interface IContentfulService
{
    Task<IEnumerable<Locale>> GetLocalesAsync(CancellationToken cancellationToken);

    Task<ContentType?> GetContentTypeAsync(string contentTypeId, CancellationToken cancellationToken);

    Task<IEnumerable<ContentType>> GetContentTypesAsync(CancellationToken cancellationToken = default);

    Task<ContentType> CreateContentTypeAsync(ContentType contentType, CancellationToken cancellationToken = default);

    Task DeleteContentTypeAsync(string contentTypeId, CancellationToken cancellationToken = default);

    IAsyncEnumerableWithTotal<Entry<dynamic>> GetEntriesAsync(string? contentTypeId = null, QueryBuilder<Entry<dynamic>>? query = null, CancellationToken cancellationToken = default);

    Task CreateOrUpdateEntriesAsync(IAsyncEnumerableWithTotal<Entry<dynamic>> entries, bool publish = false, CancellationToken cancellationToken = default);

    Task PublishEntriesAsync(IAsyncEnumerableWithTotal<Entry<dynamic>> entries, CancellationToken cancellationToken = default);

    Task DeleteEntriesAsync(IAsyncEnumerableWithTotal<Entry<dynamic>> entries, CancellationToken cancellationToken = default);
}
