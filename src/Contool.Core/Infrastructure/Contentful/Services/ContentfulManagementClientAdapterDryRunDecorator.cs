﻿using Contentful.Core.Models;
using Contentful.Core.Models.Management;

namespace Contool.Core.Infrastructure.Contentful.Services;

public class ContentfulManagementClientAdapterDryRunDecorator(
    IContentfulManagementClientAdapter inner) : IContentfulManagementClientAdapter
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
        => Task.FromResult(contentType);

    public Task<ContentType> ActivateContentTypeAsync(string contentTypeId, int version, CancellationToken cancellationToken)
        => CreateContentfulResourceAsync<ContentType>(contentTypeId, version);

    public Task DeactivateContentTypeAsync(string contentTypeId, CancellationToken cancellationToken)
        => Task.CompletedTask;

    public Task DeleteContentTypeAsync(string contentTypeId, CancellationToken cancellationToken)
        => Task.CompletedTask;

    public async Task<ContentfulCollection<Entry<dynamic>>> GetEntriesCollectionAsync(string queryString, CancellationToken cancellationToken)
        => await inner.GetEntriesCollectionAsync(queryString, cancellationToken);

    public Task<Entry<dynamic>> CreateOrUpdateEntryAsync(Entry<dynamic> entry, int version, CancellationToken cancellationToken)
        => Task.FromResult(entry);

    public Task<Entry<dynamic>> PublishEntryAsync(string entryId, int version, CancellationToken cancellationToken)
        => CreateContentfulResourceAsync<Entry<dynamic>>(entryId, version);

    public Task<Entry<dynamic>> UnpublishEntryAsync(string entryId, int version, CancellationToken cancellationToken)
        => CreateContentfulResourceAsync<Entry<dynamic>>(entryId, version);

    public Task<Entry<dynamic>> ArchiveEntryAsync(string entryId, int version, CancellationToken cancellationToken)
        => CreateContentfulResourceAsync<Entry<dynamic>>(entryId, version);

    public Task<Entry<dynamic>> UnarchiveEntryAsync(string entryId, int version, CancellationToken cancellationToken)
        => CreateContentfulResourceAsync<Entry<dynamic>>(entryId, version);

    public Task DeleteEntryAsync(string entryId, int version, CancellationToken cancellationToken)
        => Task.CompletedTask;

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