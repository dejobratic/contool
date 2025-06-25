using Contentful.Core.Errors;
using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Contool.Core.Infrastructure.Contentful.Extensions;
using Contool.Core.Infrastructure.Contentful.Models;
using Contool.Core.Infrastructure.Contentful.Utils;

namespace Contool.Core.Infrastructure.Contentful.Services;

public class ContentfulLoginService(
    string defaultSpaceId,
    string defaultEnvironmentId,
    IContentfulManagementClientAdapterFactory clientFactory) : IContentfulLoginService
{
    private readonly IContentfulManagementClientAdapter _client = clientFactory.Create(
        spaceId: defaultSpaceId,
        environmentId: defaultEnvironmentId,
        usePreviewApi: false);

    public Task<IEnumerable<Locale>> GetLocalesAsync(CancellationToken cancellationToken = default)
        => _client.GetLocalesCollectionAsync(cancellationToken);

    public Task<Space> GetDefaultSpaceAsync(CancellationToken cancellationToken = default)
        => _client.GetSpaceAsync(defaultSpaceId, cancellationToken);

    public Task<ContentfulEnvironment> GetDefaultEnvironmentAsync(CancellationToken cancellationToken = default)
        => _client.GetEnvironmentAsync(defaultEnvironmentId, cancellationToken);

    public Task<User> GetCurrentUserAsync(CancellationToken cancellationToken = default)
        => _client.GetCurrentUser(cancellationToken);

    public async Task<IEnumerable<ContentTypeExtended>> GetContentTypeExtendedAsync(CancellationToken cancellationToken = default)
    {
        var contentTypes = await _client.GetContentTypesAsync(cancellationToken);

        var contentTypeExtTasks = contentTypes
            .ToDictionary(type => type, type =>
            {
                var queryString = new EntryQueryBuilder()
                    .WithContentTypeId(type.GetId())
                    .Limit(0)
                    .Skip(0)
                    .Build();

                return _client.GetEntriesCollectionAsync(queryString, cancellationToken);
            });

        await Task.WhenAll(contentTypeExtTasks.Values);

        return contentTypeExtTasks.Select(kv => new ContentTypeExtended(kv.Key, kv.Value.Result.Total));
    }

    public async Task<bool> CanConnectAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _ = await GetCurrentUserAsync(cancellationToken);
            return true;
        }
        catch (ContentfulException)
        {
            return false;
        }
    }
}