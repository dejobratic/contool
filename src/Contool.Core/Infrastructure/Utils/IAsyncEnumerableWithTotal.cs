namespace Contool.Core.Infrastructure.Utils;

public interface IAsyncEnumerableWithTotal<T> : IAsyncEnumerable<T>
{
    int Total { get; }
}
