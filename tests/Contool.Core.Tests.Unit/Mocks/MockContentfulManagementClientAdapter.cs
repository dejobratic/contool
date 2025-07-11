using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Contool.Core.Infrastructure.Contentful.Services;

namespace Contool.Core.Tests.Unit.Mocks;

internal class MockContentfulManagementClientAdapter : IContentfulManagementClientAdapter
{
    private ContentType? _contentType;
    private IEnumerable<ContentType>? _contentTypes;
    private IEnumerable<Locale>? _locales;
    private ContentType? _createdContentType;
    private IEnumerable<Entry<dynamic>>? _entries;

    public bool GetContentTypeAsyncWasCalled { get; private set; }
    public bool GetLocalesAsyncWasCalled { get; private set; }
    public bool GetContentTypesAsyncWasCalled { get; private set; }
    public bool CreateOrUpdateEntriesAsyncWasCalled { get; private set; }
    public bool PublishEntriesAsyncWasCalled { get; private set; }
    public bool UnpublishEntriesAsyncWasCalled { get; private set; }
    public bool DeleteEntriesAsyncWasCalled { get; private set; }
    public bool CreateContentTypeAsyncWasCalled { get; private set; }
    public bool DeleteContentTypeAsyncWasCalled { get; private set; }

    public string? LastDeletedContentTypeId { get; private set; }
    public IEnumerable<Entry<dynamic>>? LastCreatedOrUpdatedEntries { get; private set; }
    public IEnumerable<Entry<dynamic>>? LastPublishedEntries { get; private set; }
    public IEnumerable<Entry<dynamic>>? LastUnpublishedEntries { get; private set; }
    public IEnumerable<Entry<dynamic>>? LastDeletedEntries { get; private set; }
    public bool LastPublishFlag { get; private set; }

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

    public void SetupEntries(IEnumerable<Entry<dynamic>> entries)
    {
        _entries = entries;
    }

    public Task<Space> GetSpaceAsync(string spaceId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        throw new NotImplementedException();
    }

    public Task<ContentfulEnvironment> GetEnvironmentAsync(string environmentId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        throw new NotImplementedException();
    }

    public Task<User> GetCurrentUser(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Locale>> GetLocalesCollectionAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        GetLocalesAsyncWasCalled = true;
        return Task.FromResult(_locales ?? Enumerable.Empty<Locale>());
    }

    public Task<ContentType?> GetContentTypeAsync(string contentTypeId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        GetContentTypeAsyncWasCalled = true;
        return Task.FromResult(_contentType);
    }

    public Task<IEnumerable<ContentType>> GetContentTypesAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        GetContentTypesAsyncWasCalled = true;
        return Task.FromResult(_contentTypes ?? Enumerable.Empty<ContentType>());
    }

    public Task<ContentType> CreateOrUpdateContentTypeAsync(ContentType contentType, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        CreateContentTypeAsyncWasCalled = true;
        return Task.FromResult(_createdContentType ?? contentType);
    }

    public Task<ContentType> ActivateContentTypeAsync(string contentTypeId, int version, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        throw new NotImplementedException();
    }

    public Task DeactivateContentTypeAsync(string contentTypeId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        throw new NotImplementedException();
    }

    public Task DeleteContentTypeAsync(string contentTypeId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        DeleteContentTypeAsyncWasCalled = true;
        LastDeletedContentTypeId = contentTypeId;
        return Task.CompletedTask;
    }

    public Task<ContentfulCollection<Entry<dynamic>>> GetEntriesCollectionAsync(string queryString, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var entries = _entries ?? Enumerable.Empty<Entry<dynamic>>();
        var entriesArray = entries.ToArray();
        var collection = new ContentfulCollection<Entry<dynamic>>
        {
            Items = entriesArray,
            Total = entriesArray.Length
        };
        return Task.FromResult(collection);
    }

    public Task<Entry<dynamic>> CreateOrUpdateEntryAsync(Entry<dynamic> entry, int version, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        throw new NotImplementedException();
    }

    public Task<Entry<dynamic>> PublishEntryAsync(string entryId, int version, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        throw new NotImplementedException();
    }

    public Task<Entry<dynamic>> UnpublishEntryAsync(string entryId, int version, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        throw new NotImplementedException();
    }

    public Task<Entry<dynamic>> ArchiveEntryAsync(string entryId, int version, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        throw new NotImplementedException();
    }

    public Task<Entry<dynamic>> UnarchiveEntryAsync(string entryId, int version, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        throw new NotImplementedException();
    }

    public Task DeleteEntryAsync(string entryId, int version, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        throw new NotImplementedException();
    }

    public Task CreateOrUpdateEntriesAsync(IEnumerable<Entry<dynamic>> entries, bool publish, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        CreateOrUpdateEntriesAsyncWasCalled = true;
        LastCreatedOrUpdatedEntries = entries;
        LastPublishFlag = publish;
        return Task.CompletedTask;
    }

    public Task PublishEntriesAsync(IEnumerable<Entry<dynamic>> entries, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        PublishEntriesAsyncWasCalled = true;
        LastPublishedEntries = entries;
        return Task.CompletedTask;
    }

    public Task UnpublishEntriesAsync(IEnumerable<Entry<dynamic>> entries, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        UnpublishEntriesAsyncWasCalled = true;
        LastUnpublishedEntries = entries;
        return Task.CompletedTask;
    }

    public Task DeleteEntriesAsync(IEnumerable<Entry<dynamic>> entries, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        DeleteEntriesAsyncWasCalled = true;
        LastDeletedEntries = entries;
        return Task.CompletedTask;
    }
}