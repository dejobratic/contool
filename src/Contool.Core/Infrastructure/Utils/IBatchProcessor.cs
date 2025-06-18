namespace Contool.Core.Infrastructure.Utils;

public interface IBatchProcessor
{
    Task ProcessAsync<T>(
        IAsyncEnumerable<T> source,
        int batchSize,
        Func<IReadOnlyList<T>, CancellationToken, Task> batchActionAsync,
        CancellationToken cancellationToken);
}
