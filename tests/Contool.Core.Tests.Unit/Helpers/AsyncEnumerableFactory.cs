namespace Contool.Core.Tests.Unit.Helpers;

public static class AsyncEnumerableFactory
{
    public static async IAsyncEnumerable<T> From<T>(IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            await Task.Delay(10); // Simulate async operation
            yield return item;
        }
    }
}
