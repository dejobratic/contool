namespace Contool.Contentful.Services;

internal interface IContentEntrySerializerFactory
{
    Task<IContentEntrySerializer> CreateAsync(string contentTypeId, CancellationToken cancellationToken);
}