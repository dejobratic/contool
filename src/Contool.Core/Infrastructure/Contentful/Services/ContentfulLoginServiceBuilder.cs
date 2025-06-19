using Contentful.Core.Configuration;
using Microsoft.Extensions.Options;

namespace Contool.Core.Infrastructure.Contentful.Services;

public class ContentfulLoginServiceBuilder(
    IOptions<ContentfulOptions> contentfulOptions,
    IContentfulManagementClientAdapterFactory adapterFactory) : IContentfulLoginServiceBuilder
{
    private string? _spaceId;
    private string? _environmentId;

    public IContentfulLoginServiceBuilder WithSpaceId(string? spaceId)
    {
        _spaceId = spaceId;
        return this;
    }

    public IContentfulLoginServiceBuilder WithEnvironmentId(string? environmentId)
    {
        _environmentId = environmentId;
        return this;
    }

    public IContentfulLoginService Build()
    {
        _spaceId ??= contentfulOptions.Value.SpaceId;
        _environmentId ??= contentfulOptions.Value.Environment;

        return new ContentfulLoginService(_spaceId, _environmentId, adapterFactory);
    }
}
