using Contool.Core.Infrastructure.Contentful.Models;

namespace Contool.Core.Infrastructure.Contentful.Services;

public interface IContentfulBulkClient
{
    Task<BulkActionResponse> PublishAsync(
        IReadOnlyList<BulkActionItemBase> items,
        CancellationToken cancellationToken = default);

    Task<BulkActionResponse> UnpublishAsync(
        IReadOnlyList<BulkActionItemBase> items,
        CancellationToken cancellationToken = default);
}