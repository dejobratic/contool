namespace Contool.Core.Features.ContentPublish;

public interface IContentPublisher
{
    Task PublishAsync(ContentPublisherInput input, CancellationToken cancellationToken = default);
}