using Contool.Core.Infrastructure.Utils.Models;

namespace Contool.Core.Infrastructure.Utils.Services;

public interface IEntriesOperationTracker
{
    void IncrementCreatedOrUpdatedCount();

    void IncrementPublishedCount();

    void IncrementUnpublishedCount();

    void IncrementArchivedCount();

    void IncrementUnarchivedCount();

    void IncrementDeletedCount();

    EntriesOperationTrackResults? GetResults();
}