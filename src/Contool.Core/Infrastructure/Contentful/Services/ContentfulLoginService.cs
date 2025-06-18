using Contentful.Core.Configuration;
using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Contool.Core.Infrastructure.Contentful.Extensions;
using Contool.Core.Infrastructure.Contentful.Models;
using Contool.Core.Infrastructure.Contentful.Utils;
using Microsoft.Extensions.Options;

namespace Contool.Core.Infrastructure.Contentful.Services;

public class ContentfulLoginService : IContentfulLoginService
{
    private readonly string _defaultSpaceId;
    private readonly string _defaultEnvironmentId;

    private readonly IContentfulManagementClientAdapter _contentfulClient;

    public ContentfulLoginService(
        IOptions<ContentfulOptions> contentfulOptions,
        IContentfulManagementClientAdapterFactory contentfulClientFactory)
    {
        _defaultSpaceId = contentfulOptions.Value.SpaceId;
        _defaultEnvironmentId = contentfulOptions.Value.Environment;

        _contentfulClient = contentfulClientFactory.Create(
            _defaultSpaceId,
            _defaultEnvironmentId,
            false);
    }

    public Task<IEnumerable<Locale>> GetLocalesAsync(CancellationToken cancellationToken = default)
        => _contentfulClient.GetLocalesCollectionAsync(cancellationToken);

    public Task<Space> GetDefaultSpaceAsync(CancellationToken cancellationToken = default)
        => _contentfulClient.GetSpaceAsync(_defaultSpaceId, cancellationToken);

    public Task<ContentfulEnvironment> GetDefaultEnvironmentAsync(CancellationToken cancellationToken = default)
        => _contentfulClient.GetEnvironmentAsync(_defaultEnvironmentId, cancellationToken);

    public Task<User> GetCurrentUserAsync(CancellationToken cancellationToken = default)
        => _contentfulClient.GetCurrentUser(cancellationToken);

    public async Task<IEnumerable<ContentTypeExtended>> GetContentTypeExtendedAsync(CancellationToken cancellationToken = default)
    {
        var contentTypes = await _contentfulClient.GetContentTypesAsync(cancellationToken);

        var contentTypeExtTasks = contentTypes
            .ToDictionary(type => type, type =>
            {
                var queryString = new EntryQueryBuilder()
                    .WithContentTypeId(type.GetId())
                    .Limit(0)
                    .Skip(0)
                    .Build();

                return _contentfulClient.GetEntriesCollectionAsync(queryString, cancellationToken);
            });

        await Task.WhenAll(contentTypeExtTasks.Values);

        return contentTypeExtTasks.Select(kv => new ContentTypeExtended(kv.Key, kv.Value.Result.Total));
    }
}