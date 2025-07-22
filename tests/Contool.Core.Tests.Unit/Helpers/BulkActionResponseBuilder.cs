using Contool.Core.Infrastructure.Contentful.Models;

namespace Contool.Core.Tests.Unit.Helpers;

public class BulkActionResponseBuilder
{
    private string _id = "bulk-action-1";
    private string _action = "publish";
    private string _status = BulkActionStatus.Create;
    private DateTime _createdAt = DateTime.UtcNow;
    private DateTime _updatedAt = DateTime.UtcNow;
    private List<BulkActionEntity> _items = [];
    private string? _errorId;
    private BulkActionSys? _sys;

    public BulkActionResponseBuilder WithId(string id)
    {
        _id = id;
        return this;
    }

    public BulkActionResponseBuilder WithAction(string action)
    {
        _action = action;
        return this;
    }

    public BulkActionResponseBuilder WithStatus(string status)
    {
        _status = status;
        return this;
    }

    public BulkActionResponseBuilder WithCreatedAt(DateTime createdAt)
    {
        _createdAt = createdAt;
        return this;
    }

    public BulkActionResponseBuilder WithUpdatedAt(DateTime updatedAt)
    {
        _updatedAt = updatedAt;
        return this;
    }

    public BulkActionResponseBuilder WithItems(IEnumerable<BulkActionEntity> items)
    {
        _items = items.ToList();
        return this;
    }

    public BulkActionResponseBuilder WithErrorId(string? errorId)
    {
        _errorId = errorId;
        return this;
    }

    public BulkActionResponseBuilder WithSys(BulkActionSys? sys)
    {
        _sys = sys;
        return this;
    }

    public BulkActionResponseBuilder AsSucceeded()
    {
        _status = BulkActionStatus.Succeeded;
        _updatedAt = DateTime.UtcNow;
        return this;
    }

    public BulkActionResponseBuilder AsFailed(string? errorId = "error-1")
    {
        _status = BulkActionStatus.Failed;
        _errorId = errorId;
        _updatedAt = DateTime.UtcNow;
        return this;
    }

    public BulkActionResponseBuilder AsInProgress()
    {
        _status = BulkActionStatus.InProgress;
        return this;
    }

    public BulkActionResponse Build()
    {
        return new BulkActionResponse
        {
            Sys = _sys ?? new BulkActionSys 
            { 
                Id = _id, 
                Type = "BulkAction", 
                Status = _status, 
                SchemaVersion = "1", 
                CreatedAt = _createdAt, 
                UpdatedAt = _updatedAt 
            },
            Action = _action,
            Payload = new BulkActionPayload
            {
                Entities = new BulkActionEntities
                {
                    Type = "Array",
                    Items = _items
                }
            },
            Error = _errorId != null ? new BulkActionError 
            { 
                Sys = new BulkActionErrorSys 
                { 
                    Id = _errorId, 
                    Type = "Error" 
                } 
            } : null!
        };
    }

    public static BulkActionResponseBuilder Create() => new();

    public static BulkActionResponse CreateSuccessful(string id = "bulk-action-1", string action = "publish") =>
        new BulkActionResponseBuilder()
            .WithId(id)
            .WithAction(action)
            .AsSucceeded()
            .Build();

    public static BulkActionResponse CreateFailed(string id = "bulk-action-1", string action = "publish", string? errorId = "error-1") =>
        new BulkActionResponseBuilder()
            .WithId(id)
            .WithAction(action)
            .AsFailed(errorId)
            .Build();

    public static BulkActionResponse CreateInProgress(string id = "bulk-action-1", string action = "publish") =>
        new BulkActionResponseBuilder()
            .WithId(id)
            .WithAction(action)
            .AsInProgress()
            .Build();
}