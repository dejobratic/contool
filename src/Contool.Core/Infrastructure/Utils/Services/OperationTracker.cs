using Contool.Core.Infrastructure.Utils.Models;
using System.Collections.Concurrent;

namespace Contool.Core.Infrastructure.Utils.Services;

public class OperationTracker : IOperationTracker
{
    private readonly ConcurrentDictionary<Operation, (int SuccessCount, int ErrorCount)> _operations = [];
    private readonly ConcurrentBag<string> _entryIds = [];
    private readonly ConcurrentBag<string> _successfulEntryIds = [];

    public void IncrementSuccessCount(Operation operation, string entryId)
    {
        _entryIds.Add(entryId);
        _successfulEntryIds.Add(entryId);
        _operations.TryGetValue(operation, out var counts);

        Increment(ref counts.SuccessCount);
        _operations[operation] = counts;
    }

    public void IncrementErrorCount(Operation operation, string entryId)
    {
        _entryIds.Add(entryId);
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
            TotalEntries = _entryIds.Distinct().Count(),
            SuccessfulEntries = _successfulEntryIds.Distinct().Count()
        };
}