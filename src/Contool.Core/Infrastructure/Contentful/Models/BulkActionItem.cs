namespace Contool.Core.Infrastructure.Contentful.Models;

public class BulkActionItem
{
    public string Id { get; init; } = null!;

    public int Version { get; init; }

    public string Type { get; init; } = "Link";

    public string LinkType { get; init; } = "Entry";
}