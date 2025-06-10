using Contentful.Core.Configuration;
using Microsoft.Extensions.Options;

namespace Contool.Contentful.Services;

internal class ContentfulServiceBuilder(
    IContentfulManagementClientAdapterFactory adapterFactory,
    IOptions<ContentfulOptions> options) : IContentfulServiceBuilder
{
    private readonly IContentfulManagementClientAdapterFactory _adapterFactory = adapterFactory;
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
        var adapter = _adapterFactory.Create(
            _spaceId ?? _options.SpaceId,
            _environmentId ?? _options.Environment,
            _usePreviewApi);

        return new ContentfulService(adapter);
    }
}
