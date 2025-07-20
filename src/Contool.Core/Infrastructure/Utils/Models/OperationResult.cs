namespace Contool.Core.Infrastructure.Utils.Models;

public record OperationResult
{
    public string EntryId { get; private set; } = null!;
    
    public Operation Operation { get; private set; } = null!;
    
    public bool IsSuccess { get; private set; }
    
    public Exception? Exception { get; private set; }
    
    public static OperationResult Success(string entityId, Operation operation)
        => new() { IsSuccess = true, EntryId = entityId, Operation = operation };

    public static OperationResult Failure(string entityId, Operation operation, Exception exception)
        => new() { IsSuccess = false, EntryId = entityId, Operation = operation, Exception = exception };
}