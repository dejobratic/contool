using Contentful.Core.Configuration;
using Contool.Core.Infrastructure.Utils.Services;
using Microsoft.Extensions.Options;

namespace Contool.Core.Infrastructure.Contentful.Services;

public class ContentfulEntryBulkOperationServiceFactory(
    IHttpClientFactory httpClientFactory,
    IOptions<ContentfulOptions> options,
    IOperationTracker operationTracker,
    IProgressReporter progressReporter) : IContentfulEntryBulkOperationServiceFactory
{
    public IContentfulEntryBulkOperationService Create(string? spaceId, string? environmentId)
    {
        var service = new ContentfulEntryBulkOperationService(
            httpClientFactory.CreateClient(),
            CreateOptions(spaceId, environmentId));
        
        return new ContentfulEntryBulkOperationServiceProgressTrackingDecorator(
            service, operationTracker, progressReporter);
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