using Contentful.Core;
using Contentful.Core.Configuration;
using Contool.Core.Infrastructure.Contentful.Options;
using Contool.Core.Infrastructure.Utils.Models;
using Contool.Core.Infrastructure.Utils.Services;
using Microsoft.Extensions.Options;

namespace Contool.Core.Infrastructure.Contentful.Services;

public class ContentfulManagementClientAdapterFactory(
    IHttpClientFactory httpClientFactory,
    IOptions<ContentfulOptions> options,
    IResiliencyExecutor resiliencyExecutor,
    IOperationTracker operationTracker,
    IRuntimeContext runtimeContext) : IContentfulManagementClientAdapterFactory
{
    private readonly ContentfulOptions _contentfulOptions = options.Value;

    public IContentfulManagementClientAdapter Create(string? spaceId, string? environmentId, bool usePreviewApi)
    {
        IContentfulManagementClientAdapter client = CreateBaseClient(spaceId, environmentId, usePreviewApi);
        client = DecorateWithResiliency(client);

        if (runtimeContext.IsDryRun)
            client = DecorateWithDryRun(client);

        return DecorateWithOperationTracking(client);
    }

    private ContentfulManagementClientAdapter CreateBaseClient(
        string? spaceId, string? environmentId, bool usePreviewApi)
    {
        var client = CreateContentfulClient(spaceId, environmentId, usePreviewApi);
        return new ContentfulManagementClientAdapter(client);
    }

    private ContentfulManagementClient CreateContentfulClient(
        string? spaceId, string? environmentId, bool usePreviewApi)
        => new(
            httpClientFactory.CreateClient(),
            new ContentfulOptions
            {
                ManagementApiKey = _contentfulOptions.ManagementApiKey,
                DeliveryApiKey = _contentfulOptions.DeliveryApiKey,
                PreviewApiKey = _contentfulOptions.PreviewApiKey,
                SpaceId = spaceId ?? _contentfulOptions.SpaceId,
                Environment = environmentId ?? _contentfulOptions.Environment,
                UsePreviewApi = usePreviewApi
            });

    private ContentfulManagementClientAdapterResiliencyDecorator DecorateWithResiliency(
        IContentfulManagementClientAdapter inner)
        => new(inner, resiliencyExecutor);

    private static ContentfulManagementClientAdapterDryRunDecorator DecorateWithDryRun(
        IContentfulManagementClientAdapter inner)
        => new(inner);

    private ContentfulManagementClientAdapterOperationTrackerDecorator DecorateWithOperationTracking(
        IContentfulManagementClientAdapter inner)
        => new(inner, operationTracker);
}