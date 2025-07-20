using Contentful.Core.Models;

namespace Contool.Core.Infrastructure.Contentful.Models;

public class BulkActionResponse
{
    public SystemProperties? Sys { get; init; }

    public string Id { get; init; } = null!;
    
    public BulkActionType Action { get; init; }
    
    public BulkActionStatus Status { get; init; }
    
    public DateTime CreatedAt { get; init; }
    
    public DateTime? CompletedAt { get; init; }
    
    public IEnumerable<BulkActionItem> Items { get; init; } = [];
    
    public string? Error { get; init; }
}