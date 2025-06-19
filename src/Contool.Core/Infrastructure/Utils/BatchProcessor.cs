namespace Contool.Core.Infrastructure.Utils;

public class BatchProcessor : IBatchProcessor
{
    public async Task ProcessAsync<T>(
        IAsyncEnumerable<T> source,
        int batchSize,
        Func<IReadOnlyList<T>, CancellationToken, Task> batchActionAsync,
        Func<T, bool>? batchItemFilter = null,
        CancellationToken cancellationToken = default)
    {
        var batch = new List<T>(batchSize);

        await foreach (var item in source.WithCancellation(cancellationToken))
        {
            if(batchItemFilter is not null && !batchItemFilter(item))
                continue;

            batch.Add(item);

            if (batch.Count != batchSize)
                continue;

            await ExecuteBatchAction(batch, batchActionAsync, cancellationToken);
        }

        if (batch.Count > 0)
            await ExecuteBatchAction(batch, batchActionAsync, cancellationToken);
    }

    private static async Task ExecuteBatchAction<T>(
        List<T> batch,
        Func<IReadOnlyList<T>, CancellationToken, Task> batchActionAsync,
        CancellationToken cancellationToken)
    {
        await batchActionAsync(batch, cancellationToken);
        batch.Clear();
    }
}
