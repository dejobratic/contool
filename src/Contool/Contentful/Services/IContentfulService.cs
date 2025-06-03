using Contentful.Core.Models;
using Contentful.Core.Models.Management;

namespace Contool.Contentful.Services;

internal interface IContentfulService
{
    Task<IEnumerable<Locale>> GetLocalesAsync(CancellationToken cancellationToken);
    Task<IEnumerable<ContentType>> GetContentTypesAsync(string? spaceId = null, CancellationToken cancellationToken = default);
    IAsyncEnumerable<Entry<dynamic>> GetEntriesAsync(string contentTypeId, string? spaceId = null, CancellationToken cancellationToken = default);
    Task UpsertEntriesAsync(IEnumerable<Entry<dynamic>> entries, string? spaceId = null, CancellationToken cancellationToken = default);
    Task PublishEntriesAsync(IEnumerable<Entry<dynamic>> entries, string? spaceId = null, CancellationToken cancellationToken = default);
}
