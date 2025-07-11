using Contentful.Core.Models;
using Contentful.Core.Models.Management;

namespace Contool.Core.Tests.Unit.Mocks;

public class MockContentfulManagementClient
{
    // ContentType operations
    public bool GetContentTypeAsyncWasCalled { get; private set; }
    public bool GetContentTypesAsyncWasCalled { get; private set; }
    public bool CreateContentTypeAsyncWasCalled { get; private set; }
    public bool DeleteContentTypeAsyncWasCalled { get; private set; }
    public string? LastDeletedContentTypeId { get; private set; }
    
    // Locale operations
    public bool GetLocalesAsyncWasCalled { get; private set; }
    
    // Entry operations
    public bool CreateOrUpdateEntriesAsyncWasCalled { get; private set; }
    public bool PublishEntriesAsyncWasCalled { get; private set; }
    public bool UnpublishEntriesAsyncWasCalled { get; private set; }
    public bool DeleteEntriesAsyncWasCalled { get; private set; }
    public IEnumerable<Entry<dynamic>>? LastCreatedOrUpdatedEntries { get; private set; }
    public IEnumerable<Entry<dynamic>>? LastPublishedEntries { get; private set; }
    public IEnumerable<Entry<dynamic>>? LastUnpublishedEntries { get; private set; }
    public IEnumerable<Entry<dynamic>>? LastDeletedEntries { get; private set; }
    public bool LastPublishFlag { get; private set; }

    private ContentType? _contentType;
    private IEnumerable<ContentType>? _contentTypes;
    private IEnumerable<Locale>? _locales;
    private ContentType? _createdContentType;

    public void SetupContentType(ContentType? contentType)
    {
        _contentType = contentType;
    }

    public void SetupContentTypes(IEnumerable<ContentType> contentTypes)
    {
        _contentTypes = contentTypes;
    }

    public void SetupLocales(IEnumerable<Locale> locales)
    {
        _locales = locales;
    }

    public void SetupCreatedContentType(ContentType contentType)
    {
        _createdContentType = contentType;
    }

    public Task<ContentType?> GetContentTypeAsync(string contentTypeId, CancellationToken cancellationToken = default)
    {
        GetContentTypeAsyncWasCalled = true;
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_contentType);
    }

    public Task<IEnumerable<ContentType>> GetContentTypesAsync(CancellationToken cancellationToken = default)
    {
        GetContentTypesAsyncWasCalled = true;
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_contentTypes ?? Enumerable.Empty<ContentType>());
    }

    public Task<ContentType> CreateContentTypeAsync(ContentType contentType, CancellationToken cancellationToken = default)
    {
        CreateContentTypeAsyncWasCalled = true;
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_createdContentType ?? contentType);
    }

    public Task DeleteContentTypeAsync(string contentTypeId, CancellationToken cancellationToken = default)
    {
        DeleteContentTypeAsyncWasCalled = true;
        LastDeletedContentTypeId = contentTypeId;
        cancellationToken.ThrowIfCancellationRequested();
        return Task.CompletedTask;
    }

    public Task<IEnumerable<Locale>> GetLocalesAsync(CancellationToken cancellationToken = default)
    {
        GetLocalesAsyncWasCalled = true;
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_locales ?? Enumerable.Empty<Locale>());
    }

    public Task CreateOrUpdateEntriesAsync(IEnumerable<Entry<dynamic>> entries, bool publish = false, CancellationToken cancellationToken = default)
    {
        CreateOrUpdateEntriesAsyncWasCalled = true;
        LastCreatedOrUpdatedEntries = entries;
        LastPublishFlag = publish;
        cancellationToken.ThrowIfCancellationRequested();
        return Task.CompletedTask;
    }

    public Task PublishEntriesAsync(IEnumerable<Entry<dynamic>> entries, CancellationToken cancellationToken = default)
    {
        PublishEntriesAsyncWasCalled = true;
        LastPublishedEntries = entries;
        cancellationToken.ThrowIfCancellationRequested();
        return Task.CompletedTask;
    }

    public Task UnpublishEntriesAsync(IEnumerable<Entry<dynamic>> entries, CancellationToken cancellationToken = default)
    {
        UnpublishEntriesAsyncWasCalled = true;
        LastUnpublishedEntries = entries;
        cancellationToken.ThrowIfCancellationRequested();
        return Task.CompletedTask;
    }

    public Task DeleteEntriesAsync(IEnumerable<Entry<dynamic>> entries, CancellationToken cancellationToken = default)
    {
        DeleteEntriesAsyncWasCalled = true;
        LastDeletedEntries = entries;
        cancellationToken.ThrowIfCancellationRequested();
        return Task.CompletedTask;
    }
}