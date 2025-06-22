using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Contool.Core.Infrastructure.Utils.Models;
using Contool.Core.Infrastructure.Utils.Services;

namespace Contool.Core.Infrastructure.Contentful.Services;

public class ContentfulManagementClientAdapterOperationTrackerDecorator(
    IContentfulManagementClientAdapter inner,
    IOperationTracker operationTracker) : IContentfulManagementClientAdapter
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

    // TODO: track operation
    public Task<ContentType> CreateOrUpdateContentTypeAsync(ContentType contentType, CancellationToken cancellationToken)
        => inner.CreateOrUpdateContentTypeAsync(contentType, cancellationToken);

    // TODO: track operation
    public Task<ContentType> ActivateContentTypeAsync(string contentTypeId, int version, CancellationToken cancellationToken)
        => inner.ActivateContentTypeAsync(contentTypeId, version, cancellationToken);

    // TODO: track operation
    public Task DeactivateContentTypeAsync(string contentTypeId, CancellationToken cancellationToken)
        => inner.DeactivateContentTypeAsync(contentTypeId, cancellationToken);

    // TODO: Track operation
    public Task DeleteContentTypeAsync(string contentTypeId, CancellationToken cancellationToken)
        => inner.DeleteContentTypeAsync(contentTypeId, cancellationToken);

    public async Task<ContentfulCollection<Entry<dynamic>>> GetEntriesCollectionAsync(string queryString, CancellationToken cancellationToken)
        => await inner.GetEntriesCollectionAsync(queryString, cancellationToken);

    public Task<Entry<dynamic>> CreateOrUpdateEntryAsync(Entry<dynamic> entry, int version, CancellationToken cancellationToken)
        => ExecuteAsync(ct => inner.CreateOrUpdateEntryAsync(entry, version, ct), Operation.Upload, cancellationToken);

    public Task<Entry<dynamic>> PublishEntryAsync(string entryId, int version, CancellationToken cancellationToken)
        => ExecuteAsync(ct => inner.PublishEntryAsync(entryId, version, ct), Operation.Publish, cancellationToken);

    public Task<Entry<dynamic>> UnpublishEntryAsync(string entryId, int version, CancellationToken cancellationToken)
        => ExecuteAsync(ct => inner.UnpublishEntryAsync(entryId, version, ct), Operation.Unpublish, cancellationToken);

    public Task<Entry<dynamic>> ArchiveEntryAsync(string entryId, int version, CancellationToken cancellationToken)
        => ExecuteAsync(ct => inner.ArchiveEntryAsync(entryId, version, ct), Operation.Archive, cancellationToken);

    public Task<Entry<dynamic>> UnarchiveEntryAsync(string entryId, int version, CancellationToken cancellationToken)
        => ExecuteAsync(ct => inner.UnarchiveEntryAsync(entryId, version, ct), Operation.Unarchive, cancellationToken);

    public Task DeleteEntryAsync(string entryId, int version, CancellationToken cancellationToken)
        => ExecuteAsync(ct => inner.DeleteEntryAsync(entryId, version, ct), Operation.Delete, cancellationToken);

    private Task ExecuteAsync(
        Func<CancellationToken, Task> action,
        Operation operation,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = action(cancellationToken);
            operationTracker.IncrementSuccessCount(operation);
            return result;
        }
        catch
        {
            operationTracker.IncrementErrorCount(operation);
            throw;
        }
    }

    private Task<T> ExecuteAsync<T>(
        Func<CancellationToken, Task<T>> action,
        Operation operation,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = action(cancellationToken);
            operationTracker.IncrementSuccessCount(operation);
            return result;
        }
        catch
        {
            operationTracker.IncrementErrorCount(operation);
            throw;
        }
    }
}