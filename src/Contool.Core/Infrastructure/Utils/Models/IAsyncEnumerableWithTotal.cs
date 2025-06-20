namespace Contool.Core.Infrastructure.Utils.Models;

public interface IAsyncEnumerableWithTotal<T> : IAsyncEnumerable<T>
{
    int Total { get; }
}
