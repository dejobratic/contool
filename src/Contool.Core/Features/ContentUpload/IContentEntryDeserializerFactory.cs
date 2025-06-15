using Contool.Core.Infrastructure.Contentful.Services;

namespace Contool.Core.Features.ContentUpload;

public interface IContentEntryDeserializerFactory
{
    Task<IContentEntryDeserializer> CreateAsync(string contentTypeId, IContentfulService contentfulService, CancellationToken cancellationToken = default);
}