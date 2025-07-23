using Contentful.Core.Models;
using Contool.Core.Infrastructure.Contentful.Extensions;

namespace Contool.Core.Infrastructure.Contentful.Models;

public abstract class BulkActionItemBase(Entry<dynamic> entry)
{
    public string Id { get; init; } = entry.GetId()!;

    public string Type { get; init; } = "Link";
    
    public string LinkType { get; init; } = "Entry";
}

public class PublishBulkActionItem(Entry<dynamic> entry) : BulkActionItemBase(entry)
{
    public int Version { get; init; } = entry.GetVersionForPublishing();
}

public class UnpublishBulkActionItem(Entry<dynamic> entry) : BulkActionItemBase(entry)
{
}