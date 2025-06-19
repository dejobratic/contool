namespace Contool.Core.Infrastructure.Contentful.Services;

public interface IContentfulLoginServiceBuilder
{
    IContentfulLoginServiceBuilder WithSpaceId(string? spaceId);

    IContentfulLoginServiceBuilder WithEnvironmentId(string? environmentId);

    IContentfulLoginService Build();
}
