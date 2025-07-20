using Contentful.Core.Models;
using Contool.Core.Infrastructure.Contentful.Models;

namespace Contool.Core.Tests.Unit.Helpers;

public class BulkActionResponseBuilder
{
    private string _id = "bulk-action-1";
    private BulkActionType _action = BulkActionType.Publish;
    private BulkActionStatus _status = BulkActionStatus.Created;
    private DateTime _createdAt = DateTime.UtcNow;
    private DateTime? _completedAt;
    private List<BulkActionItem> _items = [];
    private string? _error;
    private SystemProperties? _sys;

    public BulkActionResponseBuilder WithId(string id)
    {
        _id = id;
        return this;
    }

    public BulkActionResponseBuilder WithAction(BulkActionType action)
    {
        _action = action;
        return this;
    }

    public BulkActionResponseBuilder WithStatus(BulkActionStatus status)
    {
        _status = status;
        return this;
    }

    public BulkActionResponseBuilder WithCreatedAt(DateTime createdAt)
    {
        _createdAt = createdAt;
        return this;
    }

    public BulkActionResponseBuilder WithCompletedAt(DateTime? completedAt)
    {
        _completedAt = completedAt;
        return this;
    }

    public BulkActionResponseBuilder WithItems(IEnumerable<BulkActionItem> items)
    {
        _items = items.ToList();
        return this;
    }

    public BulkActionResponseBuilder WithError(string? error)
    {
        _error = error;
        return this;
    }

    public BulkActionResponseBuilder WithSys(SystemProperties? sys)
    {
        _sys = sys;
        return this;
    }

    public BulkActionResponseBuilder AsSucceeded()
    {
        _status = BulkActionStatus.Succeeded;
        _completedAt = DateTime.UtcNow;
        return this;
    }

    public BulkActionResponseBuilder AsFailed(string error = "Operation failed")
    {
        _status = BulkActionStatus.Failed;
        _error = error;
        _completedAt = DateTime.UtcNow;
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
            Id = _id,
            Action = _action,
            Status = _status,
            CreatedAt = _createdAt,
            CompletedAt = _completedAt,
            Items = _items,
            Error = _error,
            Sys = _sys
        };
    }

    public static BulkActionResponseBuilder Create() => new();

    public static BulkActionResponse CreateSuccessful(string id = "bulk-action-1", BulkActionType action = BulkActionType.Publish) =>
        new BulkActionResponseBuilder()
            .WithId(id)
            .WithAction(action)
            .AsSucceeded()
            .Build();

    public static BulkActionResponse CreateFailed(string id = "bulk-action-1", BulkActionType action = BulkActionType.Publish, string error = "Operation failed") =>
        new BulkActionResponseBuilder()
            .WithId(id)
            .WithAction(action)
            .AsFailed(error)
            .Build();

    public static BulkActionResponse CreateInProgress(string id = "bulk-action-1", BulkActionType action = BulkActionType.Publish) =>
        new BulkActionResponseBuilder()
            .WithId(id)
            .WithAction(action)
            .AsInProgress()
            .Build();
}