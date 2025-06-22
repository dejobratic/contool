using Contool.Core.Infrastructure.Utils.Models;

namespace Contool.Core.Infrastructure.Utils.Services;

public interface IOperationTracker
{
    void IncrementSuccessCount(Operation operation);

    void IncrementErrorCount(Operation operation);

    OperationTrackResult GetResult();
}