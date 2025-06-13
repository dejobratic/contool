using Contool.Core.Contentful.Services;

namespace Contool.Core.Features.EntryUpload;

public interface IContentEntryDeserializerFactory
{
    Task<IContentEntryDeserializer> CreateAsync(string contentTypeId, IContentfulService contentfulService, CancellationToken cancellationToken = default);
}