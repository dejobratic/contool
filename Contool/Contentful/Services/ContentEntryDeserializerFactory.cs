using Contool.Contentful.Models;
using Contentful.Core.Models;
using Contentful.Core.Search;

namespace Contool.Contentful.Services;

internal class ContentEntryDeserializerFactory(IContentfulService contentfulService) : IContentEntryDeserializerFactory
{
    public async Task<IContentEntryDeserializer> CreateAsync(string contentTypeId, CancellationToken cancellationToken = default)
    {
        var contentType = await GetContentTypeByIdAsync(contentTypeId, cancellationToken);
        var contentLocales = await GetContentLocalesAsync(cancellationToken);

        return new ContentEntryDeserializer(contentType, contentLocales);
    }

    private async Task<ContentType> GetContentTypeByIdAsync(string contentTypeId, CancellationToken cancellationToken)
    {
        var contentTypes = await contentfulService.GetContentTypesAsync(cancellationToken: cancellationToken);

        return contentTypes.FirstOrDefault(ct => ct.Name.ToCamelCase() == contentTypeId.ToCamelCase())
            ?? throw new ArgumentException($"Content type '{contentTypeId}' is not valid.");
    }

    private async Task<ContentLocales> GetContentLocalesAsync(CancellationToken cancellationToken)
    {
        var locales = await contentfulService.GetLocalesAsync(cancellationToken);

        return new ContentLocales(locales);
    }
}