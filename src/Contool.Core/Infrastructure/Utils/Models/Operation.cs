namespace Contool.Core.Infrastructure.Utils.Models;

public class Operation
{
    public static readonly Operation Read = new("READ", "Reading", "Read");
    public static readonly Operation Download = new("DOWNLOAD", "Downloading", "Downloaded");
    public static readonly Operation Upload = new("UPLOAD", "Uploading", "Uploaded");
    public static readonly Operation Publish = new("PUBLISH", "Publishing", "Published");
    public static readonly Operation Unpublish = new("UNPUBLISH", "Unpublishing", "Unpublished");
    public static readonly Operation Archive = new("ARCHIVE", "Archiving", "Archived");
    public static readonly Operation Unarchive = new("UNARCHIVE", "Unarchiving", "Unarchived");
    public static readonly Operation Delete = new("DELETE", "Deleting", "Deleted");
    public static readonly Operation Clone = new("CLONE", "Cloning", "Cloned");

    public string Name { get; }
    public string ActiveName { get; }
    public string CompletedName { get; }

    private Operation(string name, string activeName, string completedName)
    {
        Name = name;
        ActiveName = activeName;
        CompletedName = completedName;
    }
}