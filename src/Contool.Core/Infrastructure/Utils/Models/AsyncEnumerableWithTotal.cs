using Contool.Core.Infrastructure.Utils.Services;

namespace Contool.Core.Infrastructure.Utils.Models;

public class AsyncEnumerableWithTotal<T>(
    IAsyncEnumerable<T> source,
    Func<int> getTotal,
    IProgressReporter? progressReporter = null) : IAsyncEnumerableWithTotal<T>
{
    private bool _isTotalSet;

    public int Total { get; private set; }

    public async IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        await foreach (var item in source.WithCancellation(cancellationToken))
        {
            if (!_isTotalSet)
            {
                Total = getTotal();
                _isTotalSet = true;
            }

            progressReporter?.Increment();
            yield return item;
        }

        progressReporter?.Complete();
    }
}
