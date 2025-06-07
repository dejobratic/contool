using Contool.Contentful.Models;
using Contentful.Core.Models;
using Contentful.Core.Search;

namespace Contool.Contentful.Services;

internal class ContentEntrySerializerFactory : IContentEntrySerializerFactory
{
    public async Task<IContentEntrySerializer> CreateAsync(string contentTypeId, IContentfulService contentfulService, CancellationToken cancellationToken)
    {
        var contentType = await GetContentTypeByIdAsync(contentTypeId, contentfulService, cancellationToken);
        var contentLocales = await GetContentLocalesAsync(contentfulService, cancellationToken);

        return new ContentEntrySerializer(contentType, contentLocales);
    }

    private static async Task<ContentType> GetContentTypeByIdAsync(string contentTypeId, IContentfulService contentfulService, CancellationToken cancellationToken)
    {
        var contentTypes = await contentfulService.GetContentTypesAsync(cancellationToken: cancellationToken);

        return contentTypes.FirstOrDefault(ct => ct.Name.ToCamelCase() == contentTypeId.ToCamelCase())
            ?? throw new ArgumentException($"Content type '{contentTypeId}' is not valid.");
    }

    private static async Task<ContentLocales> GetContentLocalesAsync(IContentfulService contentfulService, CancellationToken cancellationToken)
    {
        var locales = await contentfulService.GetLocalesAsync(cancellationToken);

        return new ContentLocales(locales);
    }
}