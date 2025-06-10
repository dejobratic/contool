using Contool.Contentful.Services;

namespace Contool.Contentful.Extensions;

internal static class IContentfulServiceBuilderExtensions
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
