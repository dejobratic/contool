using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Contool.Core.Infrastructure.Utils.Services;

namespace Contool.Core.Infrastructure.Contentful.Services;

public class ContentfulManagementClientAdapterDryRunDecorator(
    IContentfulManagementClientAdapter inner,
    IEntriesOperationTracker operationTracker) : IContentfulManagementClientAdapter
{
    public Task<Space> GetSpaceAsync(string spaceId, CancellationToken cancellationToken)
        => inner.GetSpaceAsync(spaceId, cancellationToken);

    public Task<ContentfulEnvironment> GetEnvironmentAsync(string environmentId, CancellationToken cancellationToken)
        => inner.GetEnvironmentAsync(environmentId, cancellationToken);

    public Task<User> GetCurrentUser(CancellationToken cancellationToken)
        => inner.GetCurrentUser(cancellationToken);

    public Task<IEnumerable<Locale>> GetLocalesCollectionAsync(CancellationToken cancellationToken)
        => inner.GetLocalesCollectionAsync(cancellationToken);

    public Task<ContentType?> GetContentTypeAsync(string contentTypeId, CancellationToken cancellationToken)
        => inner.GetContentTypeAsync(contentTypeId, cancellationToken);

    public Task<IEnumerable<ContentType>> GetContentTypesAsync(CancellationToken cancellationToken)
        => inner.GetContentTypesAsync(cancellationToken);

    public Task<ContentType> CreateOrUpdateContentTypeAsync(ContentType contentType, CancellationToken cancellationToken)
    {
        // TODO: track operation
        return Task.FromResult(contentType);
    }

    public Task<ContentType> ActivateContentTypeAsync(string contentTypeId, int version, CancellationToken cancellationToken)
    {
        // TODO: track operation
        return CreateContentfulResourceAsync<ContentType>(contentTypeId, version);
    }

    public Task DeactivateContentTypeAsync(string contentTypeId, CancellationToken cancellationToken)
    {
        // TODO: track operation
        return Task.CompletedTask;
    }

    public Task DeleteContentTypeAsync(string contentTypeId, CancellationToken cancellationToken)
    {
        // TODO: Track operation
        return Task.CompletedTask;
    }

    public async Task<ContentfulCollection<Entry<dynamic>>> GetEntriesCollectionAsync(string queryString, CancellationToken cancellationToken)
        => await inner.GetEntriesCollectionAsync(queryString, cancellationToken);

    public Task<Entry<dynamic>> CreateOrUpdateEntryAsync(Entry<dynamic> entry, int version, CancellationToken cancellationToken)
    {
        operationTracker.IncrementCreatedOrUpdatedCount();
        return Task.FromResult(entry);
    }

    public Task<Entry<dynamic>> PublishEntryAsync(string entryId, int version, CancellationToken cancellationToken)
    {
        operationTracker.IncrementPublishedCount();
        return CreateContentfulResourceAsync<Entry<dynamic>>(entryId, version);
    }

    public Task<Entry<dynamic>> UnpublishEntryAsync(string entryId, int version, CancellationToken cancellationToken)
    {
        operationTracker.IncrementUnpublishedCount();
        return CreateContentfulResourceAsync<Entry<dynamic>>(entryId, version);
    }

    public Task<Entry<dynamic>> ArchiveEntryAsync(string entryId, int version, CancellationToken cancellationToken)
    {
        operationTracker.IncrementArchivedCount();
        return CreateContentfulResourceAsync<Entry<dynamic>>(entryId, version);
    }

    public Task<Entry<dynamic>> UnarchiveEntryAsync(string entryId, int version, CancellationToken cancellationToken)
    {
        operationTracker.IncrementUnarchivedCount();
        return CreateContentfulResourceAsync<Entry<dynamic>>(entryId, version);
    }

    public Task DeleteEntryAsync(string entryId, int version, CancellationToken cancellationToken)
    {
        operationTracker.IncrementDeletedCount();
        return Task.CompletedTask;
    }

    private static Task<T> CreateContentfulResourceAsync<T>(string entryId, int version)
        where T : IContentfulResource, new()
    {
        var entry = CreateContentfulResource<T>(entryId, version);
        return Task.FromResult(entry);
    }

    private static T CreateContentfulResource<T>(string contentTypeId, int version)
        where T : IContentfulResource, new()
    {
        return new T()
        {
            SystemProperties = new SystemProperties
            {
                Id = contentTypeId,
                Version = version
            }
        };
    }
}