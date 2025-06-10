using Contool.Contentful.Services;

namespace Contool.Features.EntryDownload;

internal interface IContentEntrySerializerFactory
{
    Task<IContentEntrySerializer> CreateAsync(string contentTypeId, IContentfulService contentfulService, CancellationToken cancellationToken);
}