using Contool.Core.Contentful.Services;

namespace Contool.Core.Features.EntryDownload;

public interface IContentEntrySerializerFactory
{
    Task<IContentEntrySerializer> CreateAsync(string contentTypeId, IContentfulService contentfulService, CancellationToken cancellationToken);
}