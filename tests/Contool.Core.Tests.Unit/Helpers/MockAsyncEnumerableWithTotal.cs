using Contool.Core.Infrastructure.Utils.Models;

namespace Contool.Core.Tests.Unit.Helpers;

public class MockAsyncEnumerableWithTotal<T> : IAsyncEnumerableWithTotal<T>
{
    private readonly IEnumerable<T> _items;

    public MockAsyncEnumerableWithTotal(IEnumerable<T> items)
    {
        _items = items.ToList();
        Total = _items.Count();
    }

    public int Total { get; }

    public async IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        foreach (var item in _items)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            await Task.Delay(1, cancellationToken); // Simulate async operation
            
            yield return item;
        }
    }
}