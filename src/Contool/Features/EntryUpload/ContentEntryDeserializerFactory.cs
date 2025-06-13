using Contentful.Core.Models;
using Contentful.Core.Search;
using Contool.Contentful.Models;
using Contool.Contentful.Services;

namespace Contool.Features.EntryUpload;

internal class ContentEntryDeserializerFactory : IContentEntryDeserializerFactory
{
    public async Task<IContentEntryDeserializer> CreateAsync(string contentTypeId, IContentfulService contentfulService, CancellationToken cancellationToken = default)
    {
        var contentType = await GetContentTypeByIdAsync(contentTypeId, contentfulService, cancellationToken);
        var contentLocales = await GetContentLocalesAsync(contentfulService, cancellationToken);

        return new ContentEntryDeserializer(contentType, contentLocales);
    }

    private static async Task<ContentType> GetContentTypeByIdAsync(string contentTypeId, IContentfulService contentfulService, CancellationToken cancellationToken)
    {
        return await contentfulService.GetContentTypeAsync(contentTypeId, cancellationToken)
            ?? throw new ArgumentException($"Content type '{contentTypeId}' is not valid.");
    }

    private static async Task<ContentLocales> GetContentLocalesAsync(IContentfulService contentfulService, CancellationToken cancellationToken)
    {
        var locales = await contentfulService.GetLocalesAsync(cancellationToken);

        return new ContentLocales(locales);
    }
}