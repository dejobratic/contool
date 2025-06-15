using Contool.Core.Infrastructure.Contentful.Services;

namespace Contool.Core.Infrastructure.Contentful.Extensions;

public static class IContentfulServiceBuilderExtensions
{
    public static IContentfulService Build(
        this IContentfulServiceBuilder contentfulServiceBuilder,
        string? spaceId = null,
        string? environmentId = null)
    {
        return contentfulServiceBuilder
            .WithSpaceId(spaceId)
            .WithEnvironmentId(environmentId)
            .Build();
    }
}
