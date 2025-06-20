namespace Contool.Core.Infrastructure.Utils.Services;

public interface IBatchProcessor
{
    Task ProcessAsync<T>(
        IAsyncEnumerable<T> source,
        int batchSize,
        Func<IReadOnlyList<T>, CancellationToken, Task> batchActionAsync,
        Func<T, bool>? batchItemFilter = null,
        CancellationToken cancellationToken = default);
}
