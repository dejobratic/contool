using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Contool.Core.Infrastructure.Contentful.Services;
using Contool.Core.Infrastructure.Contentful.Utils;
using Contool.Core.Infrastructure.Utils.Models;
using Contool.Core.Tests.Unit.Helpers;

namespace Contool.Core.Tests.Unit.Mocks;

public class MockContentfulService : IContentfulService
{
    private ContentType? _contentType;
    private List<Locale> _locales = [];

    public void SetupContentType(ContentType contentType)
    {
        _contentType = contentType;
    }

    public void SetupLocales(params Locale[] locales)
    {
        _locales = locales.ToList();
    }

    public Task<ContentType?> GetContentTypeAsync(string contentTypeId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_contentType);
    }

    public Task<IEnumerable<Locale>> GetLocalesAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<Locale>>(_locales);
    }

    // Tracking properties for upload operations
    public bool CreateOrUpdateEntriesAsyncWasCalled { get; private set; }
    public bool PublishEntriesAsyncWasCalled { get; private set; }
    public bool UnpublishEntriesAsyncWasCalled { get; private set; }
    public bool DeleteEntriesAsyncWasCalled { get; private set; }
    public IEnumerable<Entry<dynamic>>? LastUploadedEntries { get; private set; }

    // Not needed for validator tests, but required by interface
    public Task<IEnumerable<ContentType>> GetContentTypesAsync(CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();

    public Task<ContentType> CreateContentTypeAsync(ContentType contentType, CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();

    public Task DeleteContentTypeAsync(string contentTypeId, CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();

    public IAsyncEnumerableWithTotal<Entry<dynamic>> GetEntriesAsync(string contentTypeId, int? pageSize = null, PagingMode pagingMode = PagingMode.SkipForward, CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();

    public Task CreateOrUpdateEntriesAsync(IEnumerable<Entry<dynamic>> entries, bool publish = false, CancellationToken cancellationToken = default)
    {
        CreateOrUpdateEntriesAsyncWasCalled = true;
        LastUploadedEntries = entries;
        return Task.CompletedTask;
    }

    public Task UnpublishEntriesAsync(IEnumerable<Entry<dynamic>> entries, CancellationToken cancellationToken = default)
    {
        UnpublishEntriesAsyncWasCalled = true;
        return Task.CompletedTask;
    }

    public Task PublishEntriesAsync(IEnumerable<Entry<dynamic>> entries, CancellationToken cancellationToken = default)
    {
        PublishEntriesAsyncWasCalled = true;
        return Task.CompletedTask;
    }

    public Task DeleteEntriesAsync(IEnumerable<Entry<dynamic>> entries, CancellationToken cancellationToken = default)
    {
        DeleteEntriesAsyncWasCalled = true;
        return Task.CompletedTask;
    }
}