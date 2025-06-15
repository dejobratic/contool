namespace Contool.Core.Infrastructure.Contentful.Services;

public interface IContentfulServiceBuilder
{
    IContentfulServiceBuilder WithSpaceId(string? spaceId);

    IContentfulServiceBuilder WithEnvironmentId(string? environmentId);

    IContentfulServiceBuilder WithPreviewApi(bool usePreviewApi);

    IContentfulService Build();
}