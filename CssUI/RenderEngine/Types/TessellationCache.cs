using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace CssUI;

/// <summary>
/// Caches tessellation results keyed by source object and version.
/// </summary>
internal sealed class TessellationCache
{
    private readonly Dictionary<int, CacheEntry> _cache = new();

    /// <summary>
    /// Attempts to retrieve a cached tessellation result for the given source.
    /// </summary>
    /// <param name="source">The tessellatable source object.</param>
    /// <param name="result">The cached result, if found and valid.</param>
    /// <returns>True if a valid cached result was found; otherwise false.</returns>
    public bool TryGet(ITessellatable source, out TessellationResult result)
    {
        var key = GetKey(source);
        if (_cache.TryGetValue(key, out var entry))
        {
            if (entry.Version == source.TessellationVersion)
            {
                result = entry.Result;
                return true;
            }

            // Version mismatch - remove stale entry
            _cache.Remove(key);
        }

        result = default;
        return false;
    }

    /// <summary>
    /// Stores a tessellation result in the cache.
    /// </summary>
    /// <param name="source">The tessellatable source object.</param>
    /// <param name="result">The tessellation result to cache.</param>
    public void Set(ITessellatable source, TessellationResult result)
    {
        var key = GetKey(source);
        _cache[key] = new CacheEntry(source.TessellationVersion, result);
    }

    /// <summary>
    /// Invalidates the cached result for the given source.
    /// </summary>
    /// <param name="source">The tessellatable source object.</param>
    public void Invalidate(ITessellatable source)
    {
        var key = GetKey(source);
        _cache.Remove(key);
    }

    /// <summary>
    /// Clears all cached results.
    /// </summary>
    public void Clear()
    {
        _cache.Clear();
    }

    /// <summary>
    /// Gets the number of entries in the cache.
    /// </summary>
    public int Count => _cache.Count;

    /// <summary>
    /// Gets or creates a tessellation result for the given source.
    /// If a valid cached result exists, returns it; otherwise tessellates and caches the result.
    /// </summary>
    /// <param name="source">The tessellatable source object.</param>
    /// <param name="tessellator">The tessellator to use if cache miss.</param>
    /// <returns>The tessellation result.</returns>
    public TessellationResult GetOrCreate(ITessellatable source, ITessellator tessellator)
    {
        if (TryGet(source, out var result))
        {
            return result;
        }

        result = tessellator.Tessellate(source);
        Set(source, result);
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GetKey(ITessellatable source) => RuntimeHelpers.GetHashCode(source);

    private readonly record struct CacheEntry(int Version, TessellationResult Result);
}
