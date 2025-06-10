using Contool.Contentful.Services;

namespace Contool.Features.EntryUpload;

internal interface IContentEntryDeserializerFactory
{
    Task<IContentEntryDeserializer> CreateAsync(string contentTypeId, IContentfulService contentfulService, CancellationToken cancellationToken = default);
}