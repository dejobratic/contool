using Contentful.Core.Models;
using Contool.Core.Infrastructure.Contentful.Models;
using Contool.Core.Infrastructure.Utils.Models;

namespace Contool.Core.Infrastructure.Contentful.Services;

public class ContentfulEntryBulkOperationService(
    IContentfulBulkClient bulkClient) : IContentfulEntryBulkOperationService
{
    public Task<IReadOnlyList<OperationResult>> PublishEntriesAsync(
        IReadOnlyList<Entry<dynamic>> entries,
        CancellationToken cancellationToken = default)
    {
        var items = entries
            .Select(entry => new PublishBulkActionItem(entry))
            .ToList();
        
        return ExecuteAsync(Operation.Publish, items, bulkClient.PublishAsync, cancellationToken);
    }

    public Task<IReadOnlyList<OperationResult>> UnpublishEntriesAsync(
        IReadOnlyList<Entry<dynamic>> entries,
        CancellationToken cancellationToken = default)
    {
        var items = entries
            .Select(entry => new UnpublishBulkActionItem(entry))
            .ToList();
        
        return ExecuteAsync(Operation.Unpublish, items, bulkClient.UnpublishAsync, cancellationToken);
    }

    private static async Task<IReadOnlyList<OperationResult>> ExecuteAsync(
        Operation operation,
        IReadOnlyList<BulkActionItemBase> items,
        Func<IReadOnlyList<BulkActionItemBase>, CancellationToken, Task<BulkActionResponse>> operationFunc,
        CancellationToken cancellationToken)
    {
        if (items.Count == 0)
            return [];

        try
        {
            var response = await operationFunc(items, cancellationToken);
            return CreateOperationResults(response, operation);
        }
        catch (Exception ex)
        {
            return items.Select(item => OperationResult.Failure(item.Id, operation, ex)).ToList();
        }
    }

    private static List<OperationResult> CreateOperationResults(
        BulkActionResponse response,
        Operation operation)
    {
        if (response.Sys.Status == BulkActionStatus.Succeeded)
        {
            return response.Payload.Entities.Items
                .Select(item => OperationResult.Success(item.Sys.Id, operation))
                .ToList();
        }

        var ex = new InvalidOperationException($"{response.Error.Sys.Id} - {response.Error.Sys.Type}");
        
        return response.Payload.Entities.Items
            .Select(item => OperationResult.Failure(item.Sys.Id, operation, ex))
            .ToList();
    }
}