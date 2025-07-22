namespace Contool.Core.Infrastructure.Contentful.Models;

public class BulkActionResponse
{
    public BulkActionSys Sys { get; init; } = null!;
    
    public string Action { get; init; } = null!;

    public BulkActionPayload Payload { get; init; } = null!;

    public BulkActionError Error { get; init; } = null!;
}

public class BulkActionPayload
{
    public BulkActionEntities Entities { get; init; } = null!;
}

public class BulkActionEntities
{
    public string Type { get; init; } = null!;

    public IReadOnlyList<BulkActionEntity> Items { get; init; } = null!;
}

public class BulkActionEntity
{
    public BulkActionEntitySys Sys { get; init; } = null!;
}

public class BulkActionEntitySys
{
    public string Id { get; init; } = null!;
    
    public int Version { get; init; }

    public string Type { get; init; } = "Link";

    public string LinkType { get; init; } = "Entry";
}

public class BulkActionError
{
    public BulkActionErrorSys Sys { get; init; } = null!;
}

public class BulkActionErrorSys
{
    public string Id { get; init; } = null!;

    public string Type { get; init; } = null!;
}