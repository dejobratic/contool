namespace Contool.Core.Infrastructure.Utils.Models;

public record OperationResult
{
    public bool IsSuccess { get; private set; }
    
    public Exception? Exception { get; private set; }
    
    public string EntityId { get; private set; } = string.Empty;
    
    public Operation Operation { get; private set; } = default!;

    public static OperationResult Success(string entityId, Operation operation)
        => new() { IsSuccess = true, EntityId = entityId, Operation = operation };

    public static OperationResult Failure(string entityId, Operation operation, Exception exception)
        => new() { IsSuccess = false, EntityId = entityId, Operation = operation, Exception = exception };
}