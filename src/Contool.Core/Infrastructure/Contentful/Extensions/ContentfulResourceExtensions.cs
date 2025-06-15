using Contentful.Core.Models;

namespace Contool.Core.Infrastructure.Contentful.Extensions;

public static class ContentfulResourceExtensions
{
    public static string GetId(this IContentfulResource contentfulResource)
        => contentfulResource.SystemProperties.Id;

    public static string GetType(this IContentfulResource contentfulResource)
        => contentfulResource.SystemProperties.Type;

    public static int GetVersion(this IContentfulResource contentfulResource)
        => contentfulResource.SystemProperties.Version ?? 0;
}
