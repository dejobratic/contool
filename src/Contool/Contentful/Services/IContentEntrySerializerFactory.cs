namespace Contool.Contentful.Services;

internal interface IContentEntrySerializerFactory
{
    Task<IContentEntrySerializer> CreateAsync(string contentTypeId, IContentfulService contentfulService, CancellationToken cancellationToken);
}