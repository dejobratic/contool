using Contool.Contentful.Models;
using Contentful.Core.Models;
using Contentful.Core.Search;

namespace Contool.Contentful.Services;

internal class ContentEntrySerializerFactory(IContentfulService contentfulService) : IContentEntrySerializerFactory
{
    public async Task<IContentEntrySerializer> CreateAsync(string contentTypeId, CancellationToken cancellationToken)
    {
        var contentType = await GetContentTypeByIdAsync(contentTypeId, cancellationToken);
        var contentLocales = await GetContentLocalesAsync(cancellationToken);

        return new ContentEntrySerializer(contentType, contentLocales);
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