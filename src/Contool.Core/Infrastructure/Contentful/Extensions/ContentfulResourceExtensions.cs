using Contentful.Core.Models;

namespace Contool.Core.Infrastructure.Contentful.Extensions;

public static class ContentfulResourceExtensions
{
    public static string? GetId(this IContentfulResource contentfulResource)
        => contentfulResource.SystemProperties?.Id;

    public static int GetVersion(this IContentfulResource contentfulResource)
        => contentfulResource.SystemProperties.Version ?? 0;
    
    public static string? GetContentTypeId(this IContentfulResource contentfulResource)
        => contentfulResource.SystemProperties?.ContentType?.GetId();
}
