using Contentful.Core;
using Contentful.Core.Configuration;
using Contool.Core.Infrastructure.Utils.Models;
using Contool.Core.Infrastructure.Utils.Services;
using Microsoft.Extensions.Options;

namespace Contool.Core.Infrastructure.Contentful.Services;

public class ContentfulManagementClientAdapterFactory(
    IHttpClientFactory httpClientFactory,
    IOptions<ContentfulOptions> options,
    Func<IContentfulManagementClientAdapter, IContentfulManagementClientAdapter> decorator,
    IEntriesOperationTracker operationTracker,
    IRuntimeContext runtimeContext) : IContentfulManagementClientAdapterFactory
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
                SpaceId = spaceId ?? _options.SpaceId,
                Environment = environmentId ?? _options.Environment,
                UsePreviewApi = usePreviewApi
            });

        var baseAdapter = new ContentfulManagementClientAdapter(contentfulManagementClient);

        var client = decorator(baseAdapter);

        return runtimeContext.IsDryRun
            ? new ContentfulManagementClientAdapterDryRunDecorator(client, operationTracker)
            : client;
    }
}
