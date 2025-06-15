using Contool.Core.Infrastructure.Contentful.Services;

namespace Contool.Core.Features.ContentDownload;

public interface IContentEntrySerializerFactory
{
    Task<IContentEntrySerializer> CreateAsync(string contentTypeId, IContentfulService contentfulService, CancellationToken cancellationToken);
}