using Contool.Core.Infrastructure.Utils.Models;

namespace Contool.Core.Infrastructure.Utils.Services;

public class EntriesOperationTracker : IEntriesOperationTracker
{
    private bool _tracked;

    private int _createdOrUpdatedCount;
    private int _publishedCount;
    private int _unpublishedCount;
    private int _archivedCount;
    private int _unarchivedCount;
    private int _deletedCount;

    public void IncrementCreatedOrUpdatedCount()
    {
        Increment(ref _createdOrUpdatedCount);
    }

    public void IncrementPublishedCount()
    {
        Increment(ref _publishedCount);
    }

    public void IncrementUnpublishedCount()
    {
        Increment(ref _unpublishedCount);
    }

    public void IncrementArchivedCount()
    {
        Increment(ref _archivedCount);
    }

    public void IncrementUnarchivedCount()
    {
        Increment(ref _unarchivedCount);
    }

    public void IncrementDeletedCount()
    {
        Increment(ref _deletedCount);
    }

    private void Increment(ref int count)
    {
        _tracked = true;
        Interlocked.Increment(ref count);
    }

    public EntriesOperationTrackResults? GetResults()
    {
        return _tracked ? CreateResults() : null;
    }

    private EntriesOperationTrackResults CreateResults()
    {
        return new EntriesOperationTrackResults
        {
            CreatedOrUpdatedCount = _createdOrUpdatedCount,
            PublishedCount = _publishedCount,
            UnpublishedCount = _unpublishedCount,
            ArchivedCount = _archivedCount,
            UnarchivedCount = _unarchivedCount,
            DeletedCount = _deletedCount
        };
    }
}