namespace Contool.Contentful.Services;

internal interface IContentEntryDeserializerFactory
{
    Task<IContentEntryDeserializer> CreateAsync(string contentTypeId, IContentfulService contentfulService, CancellationToken cancellationToken = default);
}