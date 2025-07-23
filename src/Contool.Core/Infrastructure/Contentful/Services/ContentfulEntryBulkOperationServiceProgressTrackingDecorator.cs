using Contentful.Core.Models;
using Contool.Core.Infrastructure.Utils.Models;
using Contool.Core.Infrastructure.Utils.Services;

namespace Contool.Core.Infrastructure.Contentful.Services;

public class ContentfulEntryBulkOperationServiceProgressTrackingDecorator(
    IContentfulEntryBulkOperationService inner,
    IOperationTracker operationTracker) : IContentfulEntryBulkOperationService
{
    public async Task<IReadOnlyList<OperationResult>> PublishEntriesAsync(
        IReadOnlyList<Entry<dynamic>> entries,
        CancellationToken cancellationToken = default)
    {
        var results = await inner.PublishEntriesAsync(entries, cancellationToken);
        
        foreach (var result in results)
            TrackOperation(result);
        
        return results;
    }

    public async Task<IReadOnlyList<OperationResult>> UnpublishEntriesAsync(
        IReadOnlyList<Entry<dynamic>> entries,
        CancellationToken cancellationToken = default)
    {
        var results = await inner.UnpublishEntriesAsync(entries, cancellationToken);
     
        foreach (var result in results)
            TrackOperation(result);
        
        return results;
    }

    private void TrackOperation(OperationResult result)
    {
        if (result.IsSuccess)
        {
            operationTracker.IncrementSuccessCount(result.Operation, result.EntryId);
        }
        else
        {
            operationTracker.IncrementErrorCount(result.Operation, result.EntryId);
        }
    }
}