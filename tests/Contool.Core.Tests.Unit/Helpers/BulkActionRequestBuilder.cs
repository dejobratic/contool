using Contool.Core.Infrastructure.Contentful.Models;

namespace Contool.Core.Tests.Unit.Helpers;

public class BulkActionRequestBuilder
{
    private BulkActionType _action = BulkActionType.Publish;
    private readonly List<BulkActionItemBase> _items = [];

    public BulkActionRequestBuilder WithAction(BulkActionType action)
    {
        _action = action;
        return this;
    }

    public BulkActionRequestBuilder WithItems(IEnumerable<BulkActionItemBase> items)
    {
        _items.AddRange(items);
        return this;
    }

    public BulkActionRequestBuilder WithEntry(string id, int version = 1)
    {
        var entry = EntryBuilder.Create().WithId(id).WithVersion(version).Build();
        _items.Add(new PublishBulkActionItem(entry));
        return this;
    }

    public BulkActionRequest Build()
    {
        return new BulkActionRequest
        {
            Action = _action,
            Entities = new
            {
                Items = _items.Select(item => new
                {
                    Sys = item,
                }),
            },
        };
    }

    public static BulkActionRequestBuilder Create() => new();

    public static BulkActionRequest CreatePublishRequest(params string[] entryIds) =>
        new BulkActionRequestBuilder()
            .WithAction(BulkActionType.Publish)
            .WithItems(entryIds.Select(id => 
            {
                var entry = EntryBuilder.Create().WithId(id).WithVersion(1).Build();
                return new PublishBulkActionItem(entry);
            }))
            .Build();

    public static BulkActionRequest CreateUnpublishRequest(params string[] entryIds) =>
        new BulkActionRequestBuilder()
            .WithAction(BulkActionType.Unpublish)
            .WithItems(entryIds.Select(id => 
            {
                var entry = EntryBuilder.Create().WithId(id).Build();
                return new UnpublishBulkActionItem(entry);
            }))
            .Build();

}