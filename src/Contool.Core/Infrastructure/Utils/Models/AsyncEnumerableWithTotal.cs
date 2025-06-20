using Contool.Core.Infrastructure.Utils.Services;

namespace Contool.Core.Infrastructure.Utils.Models;

public class AsyncEnumerableWithTotal<T>(
    IAsyncEnumerable<T> source,
    Func<int> getTotal,
    IProgressReporter? progressReporter = null) : IAsyncEnumerableWithTotal<T>
{
    private bool isTotalSet = false;

    public int Total { get; private set; }

    public async IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        await foreach (var item in source.WithCancellation(cancellationToken))
        {
            if (!isTotalSet)
            {
                Total = getTotal();
                isTotalSet = true;
            }

            progressReporter?.Increment();
            yield return item;
        }
        
        progressReporter?.Complete();
    }
}
