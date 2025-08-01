﻿namespace Contool.Core.Infrastructure.Utils.Models;

public class OperationTrackResult
{
    public IReadOnlyDictionary<Operation, (int SuccessCount, int ErrorCount)> Operations { get; init; }
        = new Dictionary<Operation, (int SuccessCount, int ErrorCount)>();
    
    public int TotalEntries { get; init; }
    public int SuccessfulEntries { get; init; }
}