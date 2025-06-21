namespace Contool.Core.Infrastructure.Utils.Models;

public class EntriesOperationTrackResults
{
    public int CreatedOrUpdatedCount { get; set; }

    public int PublishedCount { get; set; }

    public int UnpublishedCount { get; set; }

    public int ArchivedCount { get; set; }

    public int UnarchivedCount { get; set; }

    public int DeletedCount { get; set; }
}