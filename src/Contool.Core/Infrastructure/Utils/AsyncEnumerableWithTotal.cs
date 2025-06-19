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
        var count = 0;
        await foreach (var item in source.WithCancellation(cancellationToken))
        {
            if (!isTotalSet)
            {
                Total = getTotal();
                isTotalSet = true;
            }

            count++;
            progressReporter?.Increment();
            yield return item;
        }
        
        Total = count;
        progressReporter?.Complete();
    }
}
