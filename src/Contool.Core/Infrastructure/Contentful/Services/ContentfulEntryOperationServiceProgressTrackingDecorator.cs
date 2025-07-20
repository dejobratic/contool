using Contentful.Core.Models;
using Contool.Core.Infrastructure.Utils.Models;
using Contool.Core.Infrastructure.Utils.Services;

namespace Contool.Core.Infrastructure.Contentful.Services;

public class ContentfulEntryOperationServiceProgressTrackingDecorator(
    IContentfulEntryOperationService inner,
    IOperationTracker operationTracker,
    IProgressReporter progressReporter) : IContentfulEntryOperationService
{
    public async Task<OperationResult> CreateOrUpdateEntryAsync(
        Entry<dynamic> entry,
        int version,
        CancellationToken cancellationToken = default)
    {
        var result = await inner.CreateOrUpdateEntryAsync(entry, version, cancellationToken);
        
        progressReporter.Increment();
        
        if (!result.IsSuccess)
            operationTracker.IncrementErrorCount(result.Operation, result.EntryId);
        
        return result;
    }

    public async Task<OperationResult> PublishEntryAsync(
        Entry<dynamic> entry,
        CancellationToken cancellationToken = default)
    {
        var result = await inner.PublishEntryAsync(entry, cancellationToken);
        
        progressReporter.Increment();
        
        if (!result.IsSuccess)
            operationTracker.IncrementErrorCount(result.Operation, result.EntryId);
        
        return result;
    }

    public async Task<OperationResult> UnpublishEntryAsync(
        Entry<dynamic> entry,
        CancellationToken cancellationToken = default)
    {
        var result = await inner.UnpublishEntryAsync(entry, cancellationToken);
        
        progressReporter.Increment();
        
        if (!result.IsSuccess)
            operationTracker.IncrementErrorCount(result.Operation, result.EntryId);
        
        return result;
    }

    public async Task<OperationResult> DeleteEntryAsync(
        Entry<dynamic> entry,
        CancellationToken cancellationToken = default)
    {
        var result = await inner.DeleteEntryAsync(entry, cancellationToken);
        
        progressReporter.Increment();
        
        if (!result.IsSuccess)
            operationTracker.IncrementErrorCount(result.Operation, result.EntryId);
        
        return result;
    }
}