using Contentful.Core.Models;
using Contool.Core.Infrastructure.Utils.Models;
using Contool.Core.Infrastructure.Utils.Services;

namespace Contool.Core.Infrastructure.Contentful.Services;

public class ContentfulEntryBulkOperationServiceProgressTrackingDecorator(
    IContentfulEntryBulkOperationService inner,
    IOperationTracker operationTracker,
    IProgressReporter progressReporter) : IContentfulEntryBulkOperationService
{
    public async Task<IReadOnlyList<OperationResult>> PublishEntriesAsync(
        IReadOnlyList<Entry<dynamic>> entries,
        CancellationToken cancellationToken = default)
    {
        var results = await inner.PublishEntriesAsync(entries, cancellationToken);
        
        // Track progress for each entry operation
        foreach (var result in results)
        {
            progressReporter.Increment();
            
            if (!result.IsSuccess)
            {
                operationTracker.IncrementErrorCount(result.Operation, result.EntryId);
            }
        }
        
        return results;
    }

    public async Task<IReadOnlyList<OperationResult>> UnpublishEntriesAsync(
        IReadOnlyList<Entry<dynamic>> entries,
        CancellationToken cancellationToken = default)
    {
        var results = await inner.UnpublishEntriesAsync(entries, cancellationToken);
        
        // Track progress for each entry operation
        foreach (var result in results)
        {
            progressReporter.Increment();
            
            if (!result.IsSuccess)
            {
                operationTracker.IncrementErrorCount(result.Operation, result.EntryId);
            }
        }
        
        return results;
    }
}