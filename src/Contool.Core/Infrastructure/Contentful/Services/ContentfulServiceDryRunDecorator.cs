using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Contool.Core.Infrastructure.Contentful.Utils;
using Contool.Core.Infrastructure.Utils.Models;

namespace Contool.Core.Infrastructure.Contentful.Services;

public class ContentfulServiceDryRunDecorator(
    IContentfulService inner) : IContentfulService
{
    public Task<IEnumerable<Locale>> GetLocalesAsync(CancellationToken cancellationToken)
        => inner.GetLocalesAsync(cancellationToken);

    public Task<ContentType?> GetContentTypeAsync(string contentTypeId, CancellationToken cancellationToken)
        => inner.GetContentTypeAsync(contentTypeId, cancellationToken);

    public async Task<IEnumerable<ContentType>> GetContentTypesAsync(CancellationToken cancellationToken = default)
        => await inner.GetContentTypesAsync(cancellationToken);

    public Task<ContentType> CreateContentTypeAsync(ContentType contentType, CancellationToken cancellationToken = default)
        => inner.CreateContentTypeAsync(contentType, cancellationToken);

    public Task DeleteContentTypeAsync(string contentTypeId, CancellationToken cancellationToken = default)
        => inner.DeleteContentTypeAsync(contentTypeId, cancellationToken);

    public IAsyncEnumerableWithTotal<Entry<dynamic>> GetEntriesAsync(
        string contentTypeId,
        int? pageSize = null,
        PagingMode pagingMode = PagingMode.SkipForward,
        CancellationToken cancellationToken = default)
    {
        return inner.GetEntriesAsync(
            contentTypeId: contentTypeId,
            pageSize: pageSize,
            pagingMode: PagingMode.SkipForward, // TODO: This is the only difference. Need to think of a better approach than this decorator
            cancellationToken: cancellationToken);
    }

    public Task CreateOrUpdateEntriesAsync(IEnumerable<Entry<dynamic>> entries, bool publish = false, CancellationToken cancellationToken = default)
        => inner.CreateOrUpdateEntriesAsync(entries, publish, cancellationToken);

    public Task PublishEntriesAsync(IEnumerable<Entry<dynamic>> entries, CancellationToken cancellationToken = default)
        => inner.PublishEntriesAsync(entries, cancellationToken);

    public Task UnpublishEntriesAsync(IEnumerable<Entry<dynamic>> entries, CancellationToken cancellationToken = default)
        => inner.UnpublishEntriesAsync(entries, cancellationToken);

    public Task DeleteEntriesAsync(IEnumerable<Entry<dynamic>> entries, CancellationToken cancellationToken = default)
        => inner.DeleteEntriesAsync(entries, cancellationToken);
}
