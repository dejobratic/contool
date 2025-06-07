using Contentful.Core;
using Contentful.Core.Configuration;
using Microsoft.Extensions.Options;

namespace Contool.Contentful.Services;

internal class ContentfulServiceBuilder(
    IHttpClientFactory httpClientFactory,
    IOptions<ContentfulOptions> options) : IContentfulServiceBuilder
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly ContentfulOptions _options = options.Value;

    private string? _spaceId;
    private string? _environmentId;
    private bool _usePreviewApi = false;

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
        return new ContentfulService(
            new ContentfulManagementClient(
            _httpClientFactory.CreateClient(),
            new ContentfulOptions
            {
                ManagementApiKey = _options.ManagementApiKey,
                DeliveryApiKey = _options.DeliveryApiKey,
                PreviewApiKey = _options.PreviewApiKey,
                SpaceId = _spaceId ?? _options.SpaceId,
                Environment = _environmentId ?? _options.Environment,
                UsePreviewApi = _usePreviewApi,
            }));
    }
}