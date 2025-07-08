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
        _operations.AddOrUpdate(
            operation,
            addValue: (1, 0),
            updateValueFactory: (key, oldValue) => (oldValue.SuccessCount + 1, oldValue.ErrorCount)
        );
    }

    public void IncrementErrorCount(Operation operation, string entryId)
    {
        _entryIds.Add(entryId);
        _operations.AddOrUpdate(
            operation,
            addValue: (0, 1),
            updateValueFactory: (key, oldValue) => (oldValue.SuccessCount, oldValue.ErrorCount + 1)
        );
    }

    public OperationTrackResult GetResult()
        => new()
        {
            Operations = _operations,
            TotalEntries = _entryIds.Distinct().Count(),
            SuccessfulEntries = _successfulEntryIds.Distinct().Count()
        };
}