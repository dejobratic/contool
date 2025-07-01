using Contool.Core.Infrastructure.Utils.Models;

namespace Contool.Core.Infrastructure.Utils.Services;

public interface IOperationTracker
{
    void IncrementSuccessCount(Operation operation, string entryId);

    void IncrementErrorCount(Operation operation, string entryId);

    OperationTrackResult GetResult();
}