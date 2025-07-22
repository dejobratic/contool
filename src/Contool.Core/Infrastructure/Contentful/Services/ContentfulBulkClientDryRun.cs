using Contool.Core.Infrastructure.Contentful.Models;

namespace Contool.Core.Infrastructure.Contentful.Services;

public class ContentfulBulkClientDryRun : IContentfulBulkClient
{
    public Task<BulkActionResponse> PublishAsync(
        IReadOnlyList<BulkActionItemBase> items,
        CancellationToken cancellationToken = default)
        => CreateResponse(BulkActionType.Publish, items);

    public Task<BulkActionResponse> UnpublishAsync(
        IReadOnlyList<BulkActionItemBase> items,
        CancellationToken cancellationToken = default)
        => CreateResponse(BulkActionType.Unpublish, items);

    private static Task<BulkActionResponse> CreateResponse(BulkActionType action, IReadOnlyList<BulkActionItemBase> items)
    {
        var response = new BulkActionResponse
        {
            Action = action.ToString(),
            Payload = new BulkActionPayload
            {
                Entities = new BulkActionEntities
                {
                    Items = items.Select(item => new BulkActionEntity
                    {
                        Sys = new BulkActionEntitySys
                        {
                            Id = item.Id,
                        }
                    })
                    .ToList(),
                }
            },
        };
        
        return Task.FromResult(response);
    }
}