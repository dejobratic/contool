using Contool.Core.Infrastructure.Contentful.Services;

namespace Contool.Core.Features.ContentPublish;

public interface IContentPublisher
{
    Task PublishAsync(string contentTypeId, IContentfulService contentfulService, CancellationToken cancellationToken = default);
}