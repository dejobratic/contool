namespace Contool.Contentful.Models;

internal class Link(string? linkType, string id)
{
    public static string Type = "Link";

    public string LinkType { get; } = linkType ?? "Entry";

    public string Id { get; } = id;
}
