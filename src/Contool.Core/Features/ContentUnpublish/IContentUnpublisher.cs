namespace Contool.Core.Features.ContentUnpublish;

public interface IContentUnpublisher
{
    Task UnpublishAsync(ContentUnpublisherInput input, CancellationToken cancellationToken = default);
}