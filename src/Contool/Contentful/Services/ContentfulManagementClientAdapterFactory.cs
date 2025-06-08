using Contentful.Core;
using Contentful.Core.Configuration;
using Microsoft.Extensions.Options;

namespace Contool.Contentful.Services;

internal class ContentfulManagementClientAdapterFactory(
    IHttpClientFactory httpClientFactory,
    IOptions<ContentfulOptions> options,
    Func<IContentfulManagementClientAdapter, IContentfulManagementClientAdapter> decorator) : IContentfulManagementClientAdapterFactory
{
    private readonly ContentfulOptions _options = options.Value;

    public IContentfulManagementClientAdapter Create(string spaceId, string environmentId, bool usePreviewApi)
    {
        var contentfulManagementClient = new ContentfulManagementClient(
            httpClientFactory.CreateClient(),
            new ContentfulOptions
            {
                ManagementApiKey = _options.ManagementApiKey,
                DeliveryApiKey = _options.DeliveryApiKey,
                PreviewApiKey = _options.PreviewApiKey,
                SpaceId = spaceId,
                Environment = environmentId,
                UsePreviewApi = usePreviewApi
            });

        var baseAdapter = new ContentfulManagementClientAdapter(contentfulManagementClient);

        return decorator(baseAdapter);
    }
}
