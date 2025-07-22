using Contentful.Core.Configuration;
using Contool.Core.Infrastructure.Utils.Models;
using Microsoft.Extensions.Options;

namespace Contool.Core.Infrastructure.Contentful.Services;

public class ContentfulBulkClientFactory(
    IHttpClientFactory httpClientFactory,
    IOptions<ContentfulOptions> options,
    IRuntimeContext runtimeContext) : IContentfulBulkClientFactory
{
    public IContentfulBulkClient Create(string? spaceId, string? environmentId)
    {
        if (runtimeContext.IsDryRun)
            return new ContentfulBulkClientDryRun();

        return new ContentfulBulkClient(
            httpClientFactory.CreateClient(),
            CreateOptions(spaceId, environmentId));
    }

    private ContentfulOptions CreateOptions(string? spaceId, string? environmentId)
        => new()
        {
            ManagementApiKey = options.Value.ManagementApiKey,
            DeliveryApiKey = options.Value.DeliveryApiKey,
            PreviewApiKey = options.Value.PreviewApiKey,
            SpaceId = spaceId ?? options.Value.SpaceId,
            Environment = environmentId ?? options.Value.Environment,
            UsePreviewApi = options.Value.UsePreviewApi
        };
}