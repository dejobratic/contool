using Contool.Core.Infrastructure.Contentful.Services;

namespace Contool.Core.Features.ContentDelete;

public interface IContentDeleter
{
    Task DeleteAsync(string contentTypeId, IContentfulService contentfulService, CancellationToken cancellationToken);
}