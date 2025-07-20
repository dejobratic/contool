namespace Contool.Core.Infrastructure.Contentful.Models;

public class BulkActionRequest
{
    public BulkActionType Action { get; init; }
    
    public object Entities { get; init; } = null!;
}