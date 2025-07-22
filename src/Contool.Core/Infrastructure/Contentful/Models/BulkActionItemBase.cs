namespace Contool.Core.Infrastructure.Contentful.Models;

public abstract class BulkActionItemBase
{
    public string Id { get; init; } = null!;

    public string Type { get; init; } = "Link";

    
    public string LinkType { get; init; } = "Entry";
}

public class PublishBulkActionItem : BulkActionItemBase
{
    public int Version { get; init; }
}

public class UnpublishBulkActionItem : BulkActionItemBase
{
}