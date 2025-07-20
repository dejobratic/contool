using Contool.Core.Infrastructure.Utils.Models;

namespace Contool.Core.Infrastructure.Contentful.Services;

public class ContentfulServiceBuilder(
    IContentfulManagementClientAdapterFactory adapterFactory,
    IContentfulEntryOperationServiceFactory operationServiceFactory,
    IContentfulEntryBulkOperationServiceFactory entryBulkOperationServiceFactory,
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
        var clientAdapter = adapterFactory.Create(_spaceId, _environmentId, _usePreviewApi);
        var entryOperationService = operationServiceFactory.Create(clientAdapter);
        var entryBulkOperationService = entryBulkOperationServiceFactory.Create(_spaceId, _environmentId);

        var service = new ContentfulService(
            clientAdapter, entryOperationService, entryBulkOperationService);

        return runtimeContext.IsDryRun
            ? new ContentfulServiceDryRunDecorator(service)
            : service;
    }
}