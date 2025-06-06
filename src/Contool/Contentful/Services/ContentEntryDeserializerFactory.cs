﻿using Contool.Contentful.Models;
using Contentful.Core.Models;
using Contentful.Core.Search;

namespace Contool.Contentful.Services;

internal class ContentEntryDeserializerFactory : IContentEntryDeserializerFactory
{
    public async Task<IContentEntryDeserializer> CreateAsync(string contentTypeId, IContentfulService contentfulService, CancellationToken cancellationToken = default)
    {
        var contentType = await GetContentTypeByIdAsync(contentTypeId, contentfulService, cancellationToken);
        var contentLocales = await GetContentLocalesAsync(contentfulService, cancellationToken);

        return new ContentEntryDeserializer(contentType, contentLocales);
    }

    private async Task<ContentType> GetContentTypeByIdAsync(string contentTypeId, IContentfulService contentfulService, CancellationToken cancellationToken)
    {
        var contentTypes = await contentfulService.GetContentTypesAsync(cancellationToken: cancellationToken);

        return contentTypes.FirstOrDefault(ct => ct.Name.ToCamelCase() == contentTypeId.ToCamelCase())
            ?? throw new ArgumentException($"Content type '{contentTypeId}' is not valid.");
    }

    private async Task<ContentLocales> GetContentLocalesAsync(IContentfulService contentfulService, CancellationToken cancellationToken)
    {
        var locales = await contentfulService.GetLocalesAsync(cancellationToken);

        return new ContentLocales(locales);
    }
}