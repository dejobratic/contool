using Contool.Core.Infrastructure.Contentful.Services;

namespace Contool.Core.Features.TypeClone;

public interface IContentCloner
{
    Task CloneAsync(string contentTypeId, IContentfulService sourceContentfulService, IContentfulService targetContentfulService, bool publish, CancellationToken cancellationToken = default);
}