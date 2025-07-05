namespace Contool.Core.Infrastructure.Utils.Models;

public record Operation
{
    public static readonly Operation Download = new("DOWNLOAD");
    public static readonly Operation Upload = new("UPLOAD");
    public static readonly Operation Publish = new("PUBLISH");
    public static readonly Operation Unpublish = new("UNPUBLISH");
    public static readonly Operation Archive = new("ARCHIVE");
    public static readonly Operation Unarchive = new("UNARCHIVE");
    public static readonly Operation Delete = new("DELETE");

    // Had to name it 'Clon' instead of 'Clone' to avoid conflicts with records' Clone method
    public static readonly Operation Clon = new("CLONE");

    public string Name { get; }

    private Operation(string name)
        => Name = name;
}