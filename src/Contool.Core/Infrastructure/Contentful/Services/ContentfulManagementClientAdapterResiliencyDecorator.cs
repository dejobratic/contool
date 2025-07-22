using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Contool.Core.Infrastructure.Utils.Services;

using Locale = Contentful.Core.Models.Management.Locale;

namespace Contool.Core.Infrastructure.Contentful.Services;

public class ContentfulManagementClientAdapterResiliencyDecorator(
    IContentfulManagementClientAdapter inner,
    IResiliencyExecutor resiliencyExecutor) : IContentfulManagementClientAdapter
{
    public Task<Space> GetSpaceAsync(string spaceId, CancellationToken cancellationToken)
        => resiliencyExecutor.ExecuteAsync(ct => inner.GetSpaceAsync(spaceId, ct), cancellationToken);

    public Task<ContentfulEnvironment> GetEnvironmentAsync(string environmentId, CancellationToken cancellationToken)
        => resiliencyExecutor.ExecuteAsync(ct => inner.GetEnvironmentAsync(environmentId, ct), cancellationToken);

    public Task<User> GetCurrentUser(CancellationToken cancellationToken)
        => resiliencyExecutor.ExecuteAsync(inner.GetCurrentUser, cancellationToken);

    public Task<IEnumerable<Locale>> GetLocalesCollectionAsync(CancellationToken cancellationToken)
        => resiliencyExecutor.ExecuteAsync(inner.GetLocalesCollectionAsync, cancellationToken);

    public Task<ContentType?> GetContentTypeAsync(string contentTypeId, CancellationToken cancellationToken)
        => resiliencyExecutor.ExecuteAsync(ct => inner.GetContentTypeAsync(contentTypeId, ct), cancellationToken);

    public Task<IEnumerable<ContentType>> GetContentTypesAsync(CancellationToken cancellationToken)
        => resiliencyExecutor.ExecuteAsync(inner.GetContentTypesAsync, cancellationToken);

    public Task<ContentType> CreateOrUpdateContentTypeAsync(ContentType contentType, CancellationToken cancellationToken)
        => resiliencyExecutor.ExecuteAsync(ct => inner.CreateOrUpdateContentTypeAsync(contentType, ct), cancellationToken);

    public Task<ContentType> ActivateContentTypeAsync(string contentTypeId, int version, CancellationToken cancellationToken)
        => resiliencyExecutor.ExecuteAsync(ct => inner.ActivateContentTypeAsync(contentTypeId, version, ct), cancellationToken);

    public Task DeactivateContentTypeAsync(string contentTypeId, CancellationToken cancellationToken)
        => resiliencyExecutor.ExecuteAsync(ct => inner.DeactivateContentTypeAsync(contentTypeId, ct), cancellationToken);

    public Task DeleteContentTypeAsync(string contentTypeId, CancellationToken cancellationToken)
        => resiliencyExecutor.ExecuteAsync(ct => inner.DeleteContentTypeAsync(contentTypeId, ct), cancellationToken);

    public Task<ContentfulCollection<Entry<dynamic>>> GetEntriesCollectionAsync(string queryString, CancellationToken cancellationToken)
        => resiliencyExecutor.ExecuteAsync(ct => inner.GetEntriesCollectionAsync(queryString, ct), cancellationToken);

    public Task<Entry<dynamic>> CreateOrUpdateEntryAsync(Entry<dynamic> entry, int version, CancellationToken cancellationToken)
        => resiliencyExecutor.ExecuteAsync(ct => inner.CreateOrUpdateEntryAsync(entry, version, ct), cancellationToken);

    public Task<Entry<dynamic>> PublishEntryAsync(string entryId, int version, CancellationToken cancellationToken)
        => resiliencyExecutor.ExecuteAsync(ct => inner.PublishEntryAsync(entryId, version, ct), cancellationToken);

    public Task<Entry<dynamic>> UnpublishEntryAsync(string entryId, int version, CancellationToken cancellationToken)
        => resiliencyExecutor.ExecuteAsync(ct => inner.UnpublishEntryAsync(entryId, version, ct), cancellationToken);

    public Task<Entry<dynamic>> ArchiveEntryAsync(string entryId, int version, CancellationToken cancellationToken)
        => resiliencyExecutor.ExecuteAsync(ct => inner.ArchiveEntryAsync(entryId, version, ct), cancellationToken);

    public Task<Entry<dynamic>> UnarchiveEntryAsync(string entryId, int version, CancellationToken cancellationToken)
        => resiliencyExecutor.ExecuteAsync(ct => inner.UnarchiveEntryAsync(entryId, version, ct), cancellationToken);

    public Task DeleteEntryAsync(string entryId, int version, CancellationToken cancellationToken)
        => resiliencyExecutor.ExecuteAsync(ct => inner.DeleteEntryAsync(entryId, version, ct), cancellationToken);
}