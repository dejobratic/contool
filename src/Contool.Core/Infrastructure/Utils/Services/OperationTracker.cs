using Contool.Core.Infrastructure.Utils.Models;
using System.Collections.Concurrent;

namespace Contool.Core.Infrastructure.Utils.Services;

public class OperationTracker : IOperationTracker
{
    private readonly ConcurrentDictionary<Operation, (int SuccessCount, int ErrorCount)> _operations = [];

    public void IncrementSuccessCount(Operation operation)
    {
        _operations.TryGetValue(operation, out var counts);

        Increment(ref counts.SuccessCount);
        _operations[operation] = counts;
    }

    public void IncrementErrorCount(Operation operation)
    {
        _operations.TryGetValue(operation, out var counts);

        Increment(ref counts.ErrorCount);
        _operations[operation] = counts;
    }

    private static void Increment(ref int count)
        => Interlocked.Increment(ref count);

    public OperationTrackResult GetResult()
        => new()
        {
            Operations = _operations,
        };
}