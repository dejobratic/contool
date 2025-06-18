using Contool.Core.Infrastructure.Contentful.Services;

namespace Contool.Core.Features.ContentUnpublish;

public interface IContentUnpublisher
{
    Task UnpublishAsync(string contentTypeId, IContentfulService contentfulService, CancellationToken cancellationToken = default);
}