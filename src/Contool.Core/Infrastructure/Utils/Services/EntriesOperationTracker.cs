using Contool.Core.Infrastructure.Utils.Models;

namespace Contool.Core.Infrastructure.Utils.Services;

public class EntriesOperationTracker : IEntriesOperationTracker
{
    private readonly Dictionary<Operation, (int SuccessCount, int ErrorCount)> _operations = [];

    public void IncrementSuccessCount(Operation operation)
    {
        _operations.TryGetValue(operation, out (int SuccessCount, int ErrorCount) counts);

        Increment(ref counts.SuccessCount);
        _operations[operation] = counts;
    }

    public void IncrementErrorCount(Operation operation)
    {
        _operations.TryGetValue(operation, out (int SuccessCount, int ErrorCount) counts);

        Increment(ref counts.ErrorCount);
        _operations[operation] = counts;
    }

    private static void Increment(ref int count)
        => Interlocked.Increment(ref count);

    public EntriesOperationTrackResults GetResults() 
        => new()
        {
            Operations = _operations,
        };
}