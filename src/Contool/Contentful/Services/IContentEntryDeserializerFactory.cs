namespace Contool.Contentful.Services;

internal interface IContentEntryDeserializerFactory
{
    Task<IContentEntryDeserializer> CreateAsync(string contentTypeId, CancellationToken cancellationToken = default);
}