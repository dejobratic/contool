using Contool.Core.Infrastructure.Utils;

namespace Contool.Core.Infrastructure.Contentful.Services;

public class ContentfulServiceBuilder(
    IContentfulManagementClientAdapterFactory adapterFactory,
    IProgressReporter progressReporter) : IContentfulServiceBuilder
{
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
        var adapter = adapterFactory.Create(_spaceId!, _environmentId!, _usePreviewApi);

        return new ContentfulService(adapter, progressReporter);
    }
}
