using Contool.Core.Infrastructure.Utils.Services;

namespace Contool.Core.Tests.Unit.Mocks;

public class MockBatchProcessor(int batchSize = 50) : IBatchProcessor
{
    public bool ProcessAsyncWasCalled { get; private set; }
    
    public int ProcessedItemsCount { get; private set; }
    
    public int BatchCount { get; private set; }
    
    public int LastBatchSize { get; private set; } = batchSize;
    
    public bool ShouldThrowException { get; set; }
    
    public Exception? ExceptionToThrow { get; private set; }
    
    public Func<IReadOnlyList<object>, CancellationToken, Task>? LastBatchAction { get; private set; }
    
    public async Task ProcessAsync<T>(
        IAsyncEnumerable<T> source,
        int batchSize,
        Func<IReadOnlyList<T>, CancellationToken, Task> batchActionAsync,
        Func<T, bool>? batchItemFilter = null,
        CancellationToken cancellationToken = default)
    {
        ProcessAsyncWasCalled = true;
        LastBatchSize = batchSize;
        LastBatchAction = (batch, ct) => batchActionAsync((IReadOnlyList<T>)batch, ct);

        if (ExceptionToThrow != null)
        {
            throw ExceptionToThrow;
        }
        
        if (ShouldThrowException)
        {
            throw new InvalidOperationException("Mock batch processor exception");
        }

        var batch = new List<T>();

        await foreach (var item in source.WithCancellation(cancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (batchItemFilter?.Invoke(item) == false)
                continue;

            batch.Add(item);
            ProcessedItemsCount++;

            if (batch.Count < batchSize)
                continue;

            await ExecuteBatchActionAsync(batchActionAsync, batch, cancellationToken);
        }

        if (batch.Count > 0)
            await ExecuteBatchActionAsync(batchActionAsync, batch, cancellationToken);
    }
    
    private async Task ExecuteBatchActionAsync<T>(
        Func<IReadOnlyList<T>, CancellationToken, Task> batchActionAsync,
        List<T> batch,
        CancellationToken cancellationToken)
    {
        BatchCount++;

        await batchActionAsync(batch, cancellationToken);
        batch.Clear();
    }

    public void SetupToThrow(Exception exception)
    {
        ExceptionToThrow = exception;
    }
}