using System;
using System.Collections.Generic;

namespace CssUI.CSS.Formatting;

/// <summary>
/// Caches layout computation results to avoid redundant calculations.
/// Invalidated when style or structure changes.
/// </summary>
public class LayoutCache
{
    #region Internal Types

    /// <summary>
    /// Cached result of a flex layout computation.
    /// </summary>
    public class FlexLayoutResult
    {
        public float[] ItemMainSizes { get; set; } = Array.Empty<float>();
        public float[] ItemCrossSizes { get; set; } = Array.Empty<float>();
        public float[] ItemMainPositions { get; set; } = Array.Empty<float>();
        public float[] ItemCrossPositions { get; set; } = Array.Empty<float>();
        public int LineCount { get; set; }
        public float[] LineCrossSizes { get; set; } = Array.Empty<float>();
    }

    /// <summary>
    /// Cached result of a grid layout computation.
    /// </summary>
    public class GridLayoutResult
    {
        public float[] ColumnTrackSizes { get; set; } = Array.Empty<float>();
        public float[] RowTrackSizes { get; set; } = Array.Empty<float>();
        public float[] ColumnTrackPositions { get; set; } = Array.Empty<float>();
        public float[] RowTrackPositions { get; set; } = Array.Empty<float>();
        public (float X, float Y, float Width, float Height)[] ItemBounds { get; set; } =
            Array.Empty<(float, float, float, float)>();
    }

    /// <summary>
    /// Cache entry with version tracking.
    /// </summary>
    private class CacheEntry<T>
    {
        public T Value { get; set; }
        public long Version { get; set; }
        public float ContainerWidth { get; set; }
        public float ContainerHeight { get; set; }

        public CacheEntry(T value, long version, float containerWidth, float containerHeight)
        {
            Value = value;
            Version = version;
            ContainerWidth = containerWidth;
            ContainerHeight = containerHeight;
        }
    }

    #endregion

    #region Fields

    private readonly Dictionary<int, CacheEntry<FlexLayoutResult>> _flexCache = new();
    private readonly Dictionary<int, CacheEntry<GridLayoutResult>> _gridCache = new();
    private readonly Dictionary<int, long> _styleVersions = new();
    private long _globalVersion;

    #endregion

    #region Version Management

    /// <summary>
    /// Invalidates the cache for a specific element by incrementing its version.
    /// </summary>
    public void InvalidateElement(int elementId)
    {
        if (_styleVersions.TryGetValue(elementId, out var version))
        {
            _styleVersions[elementId] = version + 1;
        }
        else
        {
            _styleVersions[elementId] = 1;
        }
    }

    /// <summary>
    /// Invalidates the entire cache (e.g., after structural changes).
    /// </summary>
    public void InvalidateAll()
    {
        _globalVersion++;
        _flexCache.Clear();
        _gridCache.Clear();
    }

    /// <summary>
    /// Gets the current version for an element.
    /// </summary>
    private long GetElementVersion(int elementId)
    {
        return _styleVersions.TryGetValue(elementId, out var version)
            ? version + _globalVersion
            : _globalVersion;
    }

    #endregion

    #region Flex Layout Cache

    /// <summary>
    /// Tries to get a cached flex layout result.
    /// </summary>
    public bool TryGetFlexLayout(
        int containerId,
        float containerWidth,
        float containerHeight,
        out FlexLayoutResult? result)
    {
        if (_flexCache.TryGetValue(containerId, out var entry))
        {
            var currentVersion = GetElementVersion(containerId);

            // Check if cache is still valid (same version and container size)
            if (entry.Version == currentVersion &&
                Math.Abs(entry.ContainerWidth - containerWidth) < 0.001f &&
                Math.Abs(entry.ContainerHeight - containerHeight) < 0.001f)
            {
                result = entry.Value;
                return true;
            }
        }

        result = null;
        return false;
    }

    /// <summary>
    /// Caches a flex layout result.
    /// </summary>
    public void SetFlexLayout(
        int containerId,
        float containerWidth,
        float containerHeight,
        FlexLayoutResult result)
    {
        var version = GetElementVersion(containerId);
        _flexCache[containerId] = new CacheEntry<FlexLayoutResult>(
            result, version, containerWidth, containerHeight);
    }

    #endregion

    #region Grid Layout Cache

    /// <summary>
    /// Tries to get a cached grid layout result.
    /// </summary>
    public bool TryGetGridLayout(
        int containerId,
        float containerWidth,
        float containerHeight,
        out GridLayoutResult? result)
    {
        if (_gridCache.TryGetValue(containerId, out var entry))
        {
            var currentVersion = GetElementVersion(containerId);

            // Check if cache is still valid (same version and container size)
            if (entry.Version == currentVersion &&
                Math.Abs(entry.ContainerWidth - containerWidth) < 0.001f &&
                Math.Abs(entry.ContainerHeight - containerHeight) < 0.001f)
            {
                result = entry.Value;
                return true;
            }
        }

        result = null;
        return false;
    }

    /// <summary>
    /// Caches a grid layout result.
    /// </summary>
    public void SetGridLayout(
        int containerId,
        float containerWidth,
        float containerHeight,
        GridLayoutResult result)
    {
        var version = GetElementVersion(containerId);
        _gridCache[containerId] = new CacheEntry<GridLayoutResult>(
            result, version, containerWidth, containerHeight);
    }

    #endregion

    #region Statistics

    /// <summary>
    /// Gets the number of cached flex layouts.
    /// </summary>
    public int FlexCacheCount => _flexCache.Count;

    /// <summary>
    /// Gets the number of cached grid layouts.
    /// </summary>
    public int GridCacheCount => _gridCache.Count;

    /// <summary>
    /// Clears all cached data.
    /// </summary>
    public void Clear()
    {
        _flexCache.Clear();
        _gridCache.Clear();
        _styleVersions.Clear();
        _globalVersion = 0;
    }

    #endregion
}
