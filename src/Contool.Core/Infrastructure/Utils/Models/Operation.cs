namespace Contool.Core.Infrastructure.Utils.Models;

public abstract class Operation
{
    public string Action { get; }              // e.g. "DELETE"

    public string PresentParticiple { get; }   // e.g. "Deleting"

    public string PastTense { get; }           // e.g. "deleted"

    public string Target { get; }              // e.g. "entry" or "ContentTypeId"

    protected Operation(string action, string presentParticiple, string pastTense, string target)
    {
        Action = action;
        PresentParticiple = presentParticiple;
        PastTense = pastTense;
        Target = target;
    }
}

public class EntryOperation : Operation
{
    public static readonly EntryOperation DownloadEntries = new("DOWNLOAD", "downloading", "downloaded");
    public static readonly EntryOperation UploadEntries = new("UPLOAD", "uploading", "uploaded");
    public static readonly EntryOperation DeleteEntries = new("DELETE", "deleting", "deleted");
    public static readonly EntryOperation PublishEntries = new("PUBLISH", "publishing", "published");
    public static readonly EntryOperation UnpublishEntries = new("UNPUBLISH", "unpublishing", "unpublished");
    public static readonly EntryOperation ArchiveEntries = new("ARCHIVE", "archiving", "archived");
    public static readonly EntryOperation UnarchiveEntries = new("UNARCHIVE", "unarchiving", "unarchived");
    public static readonly EntryOperation CloneEntries = new("CLONE", "cloning", "cloned");

    private EntryOperation(string action, string presentParticiple, string pastTense)
        : base(action, presentParticiple, pastTense, "entries")
    {
    }
}
