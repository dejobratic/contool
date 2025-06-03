using Contentful.Core.Models;
using Contentful.Core.Models.Management;

namespace Contool.Contentful.Services;

internal interface IContentfulService
{
    Task<IEnumerable<Locale>> GetLocalesAsync(CancellationToken cancellationToken);
    Task<ContentType?> GetContentTypeAsync(string contentTypeId, string environmentId, CancellationToken cancellationToken);
    Task<IEnumerable<ContentType>> GetContentTypesAsync(CancellationToken cancellationToken = default);
    IAsyncEnumerable<Entry<dynamic>> GetEntriesAsync(string contentTypeId, CancellationToken cancellationToken = default);
    Task UpsertEntriesAsync(IEnumerable<Entry<dynamic>> entries, CancellationToken cancellationToken = default);
    Task PublishEntriesAsync(IEnumerable<Entry<dynamic>> entries, CancellationToken cancellationToken = default);
}
