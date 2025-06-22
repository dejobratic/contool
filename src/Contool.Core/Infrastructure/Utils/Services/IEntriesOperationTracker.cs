using Contool.Core.Infrastructure.Utils.Models;

namespace Contool.Core.Infrastructure.Utils.Services;

public interface IEntriesOperationTracker
{
    void IncrementSuccessCount(Operation operation);

    void IncrementErrorCount(Operation operation);

    EntriesOperationTrackResults GetResults();
}