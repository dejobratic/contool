namespace Contool.Core.Infrastructure.Contentful.Models;

public class BulkActionSys
{
    public string Id { get; init; } = null!;

    public string Type { get; init; } = null!;

    public string Status { get; init; } = null!;

    public string SchemaVersion { get; init; } = null!;

    public DateTime CreatedAt { get; init; } = default!;

    public DateTime UpdatedAt { get; init; } = default!;
}