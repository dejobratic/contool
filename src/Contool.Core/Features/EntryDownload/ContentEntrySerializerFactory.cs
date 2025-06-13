using Contentful.Core.Models;
using Contool.Core.Contentful.Models;
using Contool.Core.Contentful.Services;

namespace Contool.Core.Features.EntryDownload;

public class ContentEntrySerializerFactory : IContentEntrySerializerFactory
{
    public async Task<IContentEntrySerializer> CreateAsync(string contentTypeId, IContentfulService contentfulService, CancellationToken cancellationToken)
    {
        var contentType = await GetContentTypeByIdAsync(contentTypeId, contentfulService, cancellationToken);
        var contentLocales = await GetContentLocalesAsync(contentfulService, cancellationToken);

        return new ContentEntrySerializer(contentType, contentLocales);
    }

    private static async Task<ContentType> GetContentTypeByIdAsync(string contentTypeId, IContentfulService contentfulService, CancellationToken cancellationToken)
    {
        return await contentfulService.GetContentTypeAsync(contentTypeId, cancellationToken)
            ?? throw new ArgumentException($"Content type with ID '{contentTypeId}' could not be found.");
    }

    private static async Task<ContentLocales> GetContentLocalesAsync(IContentfulService contentfulService, CancellationToken cancellationToken)
    {
        var locales = await contentfulService.GetLocalesAsync(cancellationToken);

        return new ContentLocales(locales);
    }
}