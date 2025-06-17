namespace Contool.Core.Infrastructure.Utils;

public class AsyncEnumerableWithTotal<T>(
    IAsyncEnumerable<T> source,
    Func<int> getTotal,
    IProgressReporter? progressReporter = null) : IAsyncEnumerableWithTotal<T>
{
    private bool isTotalSet = false;

    public int Total { get; private set; }

    public async IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        var current = 0;
        await foreach (var item in source.WithCancellation(cancellationToken))
        {
            if (!isTotalSet)
            {
                Total = getTotal();
                isTotalSet = true;
            }

            progressReporter?.Report(++current, Total);
            yield return item;
        }

        progressReporter?.Complete();
    }
}
