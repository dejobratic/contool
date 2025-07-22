using Contool.Core.Infrastructure.Utils.Models;

namespace Contool.Core.Infrastructure.Contentful.Services;

public class ContentfulServiceBuilder(
    IContentfulManagementClientAdapterFactory clientFactory,
    IContentfulBulkClientFactory bulkClientFactory,
    IContentfulEntryOperationServiceFactory operationServiceFactory,
    IContentfulEntryBulkOperationServiceFactory bulkOperationServiceFactory,
    IRuntimeContext runtimeContext) : IContentfulServiceBuilder
{
    private string? _spaceId;
    private string? _environmentId;
    private bool _usePreviewApi;

    public IContentfulServiceBuilder WithSpaceId(string? spaceId)
    {
        _spaceId = spaceId;
        return this;
    }

    public IContentfulServiceBuilder WithEnvironmentId(string? environmentId)
    {
        _environmentId = environmentId;
        return this;
    }

    public IContentfulServiceBuilder WithPreviewApi(bool usePreviewApi)
    {
        _usePreviewApi = usePreviewApi;
        return this;
    }

    public IContentfulService Build()
    {
        var client = clientFactory.Create(_spaceId, _environmentId, _usePreviewApi);
        var entryOperationService = operationServiceFactory.Create(client);

        var bulkClient = bulkClientFactory.Create(_spaceId, _environmentId);
        var entryBulkOperationService = bulkOperationServiceFactory.Create(bulkClient);

        var service = new ContentfulService(
            client, entryOperationService, entryBulkOperationService);

        return runtimeContext.IsDryRun
            ? new ContentfulServiceDryRunDecorator(service)
            : service;
    }
}