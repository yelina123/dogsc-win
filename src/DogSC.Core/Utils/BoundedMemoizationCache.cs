using System;
using System.Collections.Generic;

namespace DogSC.Core.Utils;

/// <summary>
/// 有限大小的备忘录缓存
/// </summary>
public class BoundedMemoizationCache<TKey, TValue> where TKey : notnull
{
    private readonly int _maxSize;
    private readonly Dictionary<TKey, TValue> _cache = new();
    private readonly LinkedList<TKey> _lru = new();

    public BoundedMemoizationCache(int maxSize = 1000)
    {
        _maxSize = maxSize;
    }

    public TValue GetOrAdd(TKey key, Func<TKey, TValue> factory)
    {
        if (_cache.TryGetValue(key, out var value))
        {
            _lru.Remove(key);
            _lru.AddLast(key);
            return value;
        }

        value = factory(key);
        Add(key, value);
        return value;
    }

    public void Add(TKey key, TValue value)
    {
        if (_cache.Count >= _maxSize)
        {
            var oldest = _lru.First!.Value;
            _lru.RemoveFirst();
            _cache.Remove(oldest);
        }

        _cache[key] = value;
        _lru.AddLast(key);
    }

    public bool TryGetValue(TKey key, out TValue value) => _cache.TryGetValue(key, out value!);

    public void Clear()
    {
        _cache.Clear();
        _lru.Clear();
    }

    public int Count => _cache.Count;
}
