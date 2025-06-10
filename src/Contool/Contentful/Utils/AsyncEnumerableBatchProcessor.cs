namespace Contool.Contentful.Utils;

internal class AsyncEnumerableBatchProcessor<T>
{
    private readonly IAsyncEnumerable<T> _items;
    private readonly int _batchSize;
    private readonly Func<IReadOnlyList<T>, CancellationToken, Task> _batchActionAsync;
    private readonly Func<T, bool>? _shouldInclude;

    public AsyncEnumerableBatchProcessor(
        IAsyncEnumerable<T> items,
        int batchSize,
        Func<IReadOnlyList<T>, CancellationToken, Task> batchActionAsync,
        Func<T, bool>? shouldInclude = null)
    {
        if (batchSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(batchSize), "Batch size must be greater than zero.");

        _items = items ?? throw new ArgumentNullException(nameof(items));
        _batchSize = batchSize;
        _batchActionAsync = batchActionAsync ?? throw new ArgumentNullException(nameof(batchActionAsync));
        _shouldInclude = shouldInclude;
    }

    public async Task ProcessAsync(CancellationToken cancellationToken = default)
    {
        var buffer = new List<T>(_batchSize);

        await foreach (var item in _items.WithCancellation(cancellationToken))
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
        }
    }
}

