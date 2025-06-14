namespace Contool.Core.Infrastructure.Utils;

public class AsyncEnumerableBatchProcessor<T>(
    IAsyncEnumerable<T> source,
    int batchSize,
    Func<IReadOnlyList<T>, CancellationToken, Task> batchActionAsync,
    Func<T, bool>? shouldInclude = null)
{
    private readonly IAsyncEnumerable<T> _source = source
        ?? throw new ArgumentNullException(nameof(source));

    private readonly int _batchSize = batchSize > 0 && batchSize <= 100
        ? batchSize
        : throw new ArgumentOutOfRangeException(nameof(batchSize), "Batch size must be between 1 and 100.");

    private readonly Func<IReadOnlyList<T>, CancellationToken, Task> _batchActionAsync = batchActionAsync
        ?? throw new ArgumentNullException(nameof(batchActionAsync));

    private readonly Func<T, bool>? _shouldInclude = shouldInclude;

    public async Task ProcessAsync(CancellationToken cancellationToken = default)
    {
        var buffer = new List<T>(_batchSize);

        await foreach (var item in _source.WithCancellation(cancellationToken))
        {
            if (_shouldInclude is not null && !_shouldInclude(item))
                continue;

            buffer.Add(item);

            if (buffer.Count != _batchSize)
                continue;

            await _batchActionAsync(buffer, cancellationToken);

            buffer.Clear();
        }

        if (buffer.Count > 0)
        {
            await _batchActionAsync(buffer, cancellationToken);
            buffer.Clear();
        }
    }
}