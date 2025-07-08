using System.Collections.Concurrent;

namespace Contool.Core.Infrastructure.Utils.Models;

public class ConcurrentHashSet<T> : IDisposable where T : notnull
{
    private readonly ConcurrentDictionary<T, byte> _dictionary = new();
    
    public bool Add(T item) => _dictionary.TryAdd(item, 0);
    
    public bool Contains(T item) => _dictionary.ContainsKey(item);

    public void Dispose()
    {
        _dictionary.Clear();
        GC.SuppressFinalize(this);
    }
}