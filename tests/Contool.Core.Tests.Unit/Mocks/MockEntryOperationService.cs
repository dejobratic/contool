using Contentful.Core.Models;
using Contool.Core.Infrastructure.Contentful.Services;
using Contool.Core.Infrastructure.Utils.Models;

namespace Contool.Core.Tests.Unit.Mocks;

internal class MockEntryOperationService : IContentfulEntryOperationService
{
    private IEnumerable<Entry<dynamic>>? _entries;

    public bool GetEntriesAsyncWasCalled { get; private set; }

    public void SetupEntries(IEnumerable<Entry<dynamic>> entries)
    {
        _entries = entries;
        GetEntriesAsyncWasCalled = true;
    }

    public Task<OperationResult> CreateOrUpdateEntryAsync(Entry<dynamic> entry, int version, bool archived, bool publish,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(OperationResult.Success(entry.SystemProperties.Id, Operation.Upload));
    }

    public Task<OperationResult> PublishEntryAsync(Entry<dynamic> entry, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(OperationResult.Success(entry.SystemProperties.Id, Operation.Publish));
    }

    public Task<OperationResult> UnpublishEntryAsync(Entry<dynamic> entry, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(OperationResult.Success(entry.SystemProperties.Id, Operation.Unpublish));
    }

    public Task<OperationResult> DeleteEntryAsync(Entry<dynamic> entry, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(OperationResult.Success(entry.SystemProperties.Id, Operation.Delete));
    }
}