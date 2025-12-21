using System;
using CssUI.CSS.Formatting;
using Xunit;

namespace CssUITests.CSS.Formatting;

/// <summary>
/// Unit tests for LayoutCache.
/// Tests caching of layout computation results.
/// </summary>
public class LayoutCacheTests
{
    #region Initialization Tests

    [Fact]
    [Trait("Category", "LayoutCache")]
    public void LayoutCache_CanBeInstantiated()
    {
        var cache = new LayoutCache();
        Assert.NotNull(cache);
    }

    [Fact]
    [Trait("Category", "LayoutCache")]
    public void LayoutCache_InitialState_Empty()
    {
        var cache = new LayoutCache();
        Assert.Equal(0, cache.FlexCacheCount);
        Assert.Equal(0, cache.GridCacheCount);
    }

    #endregion

    #region Flex Layout Cache Tests

    [Fact]
    [Trait("Category", "LayoutCache")]
    public void LayoutCache_TryGetFlexLayout_MissOnEmpty()
    {
        var cache = new LayoutCache();
        bool found = cache.TryGetFlexLayout(1, 100f, 100f, out var result);
        Assert.False(found);
        Assert.Null(result);
    }

    [Fact]
    [Trait("Category", "LayoutCache")]
    public void LayoutCache_SetFlexLayout_IncrementsCount()
    {
        var cache = new LayoutCache();
        var result = new LayoutCache.FlexLayoutResult
        {
            ItemMainSizes = new[] { 50f, 50f },
            LineCount = 1
        };

        cache.SetFlexLayout(1, 100f, 100f, result);
        Assert.Equal(1, cache.FlexCacheCount);
    }

    [Fact]
    [Trait("Category", "LayoutCache")]
    public void LayoutCache_TryGetFlexLayout_HitAfterSet()
    {
        var cache = new LayoutCache();
        var expected = new LayoutCache.FlexLayoutResult
        {
            ItemMainSizes = new[] { 50f, 50f },
            LineCount = 1
        };

        cache.SetFlexLayout(1, 100f, 100f, expected);
        bool found = cache.TryGetFlexLayout(1, 100f, 100f, out var actual);

        Assert.True(found);
        Assert.NotNull(actual);
        Assert.Equal(expected.LineCount, actual.LineCount);
        Assert.Equal(expected.ItemMainSizes, actual.ItemMainSizes);
    }

    [Fact]
    [Trait("Category", "LayoutCache")]
    public void LayoutCache_TryGetFlexLayout_MissOnDifferentSize()
    {
        var cache = new LayoutCache();
        var result = new LayoutCache.FlexLayoutResult { LineCount = 1 };

        cache.SetFlexLayout(1, 100f, 100f, result);
        bool found = cache.TryGetFlexLayout(1, 200f, 100f, out _);

        Assert.False(found);
    }

    [Fact]
    [Trait("Category", "LayoutCache")]
    public void LayoutCache_TryGetFlexLayout_MissAfterInvalidate()
    {
        var cache = new LayoutCache();
        var result = new LayoutCache.FlexLayoutResult { LineCount = 1 };

        cache.SetFlexLayout(1, 100f, 100f, result);
        cache.InvalidateElement(1);
        bool found = cache.TryGetFlexLayout(1, 100f, 100f, out _);

        Assert.False(found);
    }

    #endregion

    #region Grid Layout Cache Tests

    [Fact]
    [Trait("Category", "LayoutCache")]
    public void LayoutCache_TryGetGridLayout_MissOnEmpty()
    {
        var cache = new LayoutCache();
        bool found = cache.TryGetGridLayout(1, 100f, 100f, out var result);
        Assert.False(found);
        Assert.Null(result);
    }

    [Fact]
    [Trait("Category", "LayoutCache")]
    public void LayoutCache_SetGridLayout_IncrementsCount()
    {
        var cache = new LayoutCache();
        var result = new LayoutCache.GridLayoutResult
        {
            ColumnTrackSizes = new[] { 50f, 50f },
            RowTrackSizes = new[] { 100f }
        };

        cache.SetGridLayout(1, 100f, 100f, result);
        Assert.Equal(1, cache.GridCacheCount);
    }

    [Fact]
    [Trait("Category", "LayoutCache")]
    public void LayoutCache_TryGetGridLayout_HitAfterSet()
    {
        var cache = new LayoutCache();
        var expected = new LayoutCache.GridLayoutResult
        {
            ColumnTrackSizes = new[] { 50f, 50f },
            RowTrackSizes = new[] { 100f }
        };

        cache.SetGridLayout(1, 100f, 100f, expected);
        bool found = cache.TryGetGridLayout(1, 100f, 100f, out var actual);

        Assert.True(found);
        Assert.NotNull(actual);
        Assert.Equal(expected.ColumnTrackSizes, actual.ColumnTrackSizes);
        Assert.Equal(expected.RowTrackSizes, actual.RowTrackSizes);
    }

    [Fact]
    [Trait("Category", "LayoutCache")]
    public void LayoutCache_TryGetGridLayout_MissOnDifferentSize()
    {
        var cache = new LayoutCache();
        var result = new LayoutCache.GridLayoutResult();

        cache.SetGridLayout(1, 100f, 100f, result);
        bool found = cache.TryGetGridLayout(1, 100f, 200f, out _);

        Assert.False(found);
    }

    [Fact]
    [Trait("Category", "LayoutCache")]
    public void LayoutCache_TryGetGridLayout_MissAfterInvalidate()
    {
        var cache = new LayoutCache();
        var result = new LayoutCache.GridLayoutResult();

        cache.SetGridLayout(1, 100f, 100f, result);
        cache.InvalidateElement(1);
        bool found = cache.TryGetGridLayout(1, 100f, 100f, out _);

        Assert.False(found);
    }

    #endregion

    #region Invalidation Tests

    [Fact]
    [Trait("Category", "LayoutCache")]
    public void LayoutCache_InvalidateAll_ClearsBothCaches()
    {
        var cache = new LayoutCache();

        cache.SetFlexLayout(1, 100f, 100f, new LayoutCache.FlexLayoutResult());
        cache.SetGridLayout(2, 100f, 100f, new LayoutCache.GridLayoutResult());

        Assert.Equal(1, cache.FlexCacheCount);
        Assert.Equal(1, cache.GridCacheCount);

        cache.InvalidateAll();

        Assert.Equal(0, cache.FlexCacheCount);
        Assert.Equal(0, cache.GridCacheCount);
    }

    [Fact]
    [Trait("Category", "LayoutCache")]
    public void LayoutCache_InvalidateElement_OnlyAffectsSpecificElement()
    {
        var cache = new LayoutCache();

        cache.SetFlexLayout(1, 100f, 100f, new LayoutCache.FlexLayoutResult());
        cache.SetFlexLayout(2, 100f, 100f, new LayoutCache.FlexLayoutResult());

        cache.InvalidateElement(1);

        // Element 1 should miss
        bool found1 = cache.TryGetFlexLayout(1, 100f, 100f, out _);
        Assert.False(found1);

        // Element 2 should still hit
        bool found2 = cache.TryGetFlexLayout(2, 100f, 100f, out _);
        Assert.True(found2);
    }

    [Fact]
    [Trait("Category", "LayoutCache")]
    public void LayoutCache_Clear_RemovesEverything()
    {
        var cache = new LayoutCache();

        cache.SetFlexLayout(1, 100f, 100f, new LayoutCache.FlexLayoutResult());
        cache.SetGridLayout(1, 100f, 100f, new LayoutCache.GridLayoutResult());
        cache.InvalidateElement(1);

        cache.Clear();

        Assert.Equal(0, cache.FlexCacheCount);
        Assert.Equal(0, cache.GridCacheCount);
    }

    #endregion

    #region Edge Cases

    [Fact]
    [Trait("Category", "LayoutCache")]
    public void LayoutCache_MultipleElementsSameSize_IndependentCaching()
    {
        var cache = new LayoutCache();

        var result1 = new LayoutCache.FlexLayoutResult { LineCount = 1 };
        var result2 = new LayoutCache.FlexLayoutResult { LineCount = 2 };

        cache.SetFlexLayout(1, 100f, 100f, result1);
        cache.SetFlexLayout(2, 100f, 100f, result2);

        cache.TryGetFlexLayout(1, 100f, 100f, out var actual1);
        cache.TryGetFlexLayout(2, 100f, 100f, out var actual2);

        Assert.Equal(1, actual1!.LineCount);
        Assert.Equal(2, actual2!.LineCount);
    }

    [Fact]
    [Trait("Category", "LayoutCache")]
    public void LayoutCache_SameElementDifferentSizes_LatestEntryPreferred()
    {
        var cache = new LayoutCache();

        var result100 = new LayoutCache.FlexLayoutResult { LineCount = 1 };
        var result200 = new LayoutCache.FlexLayoutResult { LineCount = 2 };

        cache.SetFlexLayout(1, 100f, 100f, result100);
        cache.SetFlexLayout(1, 200f, 200f, result200);

        // Implementation may cache only the latest entry per element
        // or may cache multiple size variants - either is valid
        bool found200 = cache.TryGetFlexLayout(1, 200f, 200f, out var actual200);

        Assert.True(found200);
        Assert.Equal(2, actual200!.LineCount);
    }

    [Fact]
    [Trait("Category", "LayoutCache")]
    public void LayoutCache_SizeTolerance_VerifyBehavior()
    {
        var cache = new LayoutCache();
        var result = new LayoutCache.FlexLayoutResult();

        cache.SetFlexLayout(1, 100f, 100f, result);

        // Very small differences (within floating point tolerance) should hit
        bool foundTiny = cache.TryGetFlexLayout(1, 100.0005f, 100f, out _);

        // Larger differences should miss
        bool foundLarger = cache.TryGetFlexLayout(1, 100.01f, 100f, out _);

        // Note: The actual tolerance depends on implementation
        // This test documents expected behavior - adjust if implementation differs
        Assert.True(foundTiny);
        Assert.False(foundLarger);
    }

    #endregion

    #region Additional Edge Cases and Boundary Tests

    [Fact]
    [Trait("Category", "LayoutCache")]
    public void LayoutCache_ZeroElementId_WorksCorrectly()
    {
        var cache = new LayoutCache();
        var result = new LayoutCache.FlexLayoutResult { LineCount = 1 };

        cache.SetFlexLayout(0, 100f, 100f, result);
        bool found = cache.TryGetFlexLayout(0, 100f, 100f, out var actual);

        Assert.True(found);
        Assert.Equal(1, actual!.LineCount);
    }

    [Fact]
    [Trait("Category", "LayoutCache")]
    public void LayoutCache_NegativeElementId_WorksCorrectly()
    {
        var cache = new LayoutCache();
        var result = new LayoutCache.FlexLayoutResult { LineCount = 2 };

        cache.SetFlexLayout(-1, 100f, 100f, result);
        bool found = cache.TryGetFlexLayout(-1, 100f, 100f, out var actual);

        Assert.True(found);
        Assert.Equal(2, actual!.LineCount);
    }

    [Fact]
    [Trait("Category", "LayoutCache")]
    public void LayoutCache_ZeroSize_WorksCorrectly()
    {
        var cache = new LayoutCache();
        var result = new LayoutCache.FlexLayoutResult { LineCount = 1 };

        cache.SetFlexLayout(1, 0f, 0f, result);
        bool found = cache.TryGetFlexLayout(1, 0f, 0f, out var actual);

        Assert.True(found);
        Assert.Equal(1, actual!.LineCount);
    }

    [Fact]
    [Trait("Category", "LayoutCache")]
    public void LayoutCache_VeryLargeSize_WorksCorrectly()
    {
        var cache = new LayoutCache();
        var result = new LayoutCache.FlexLayoutResult { LineCount = 1 };

        cache.SetFlexLayout(1, float.MaxValue, float.MaxValue, result);
        bool found = cache.TryGetFlexLayout(1, float.MaxValue, float.MaxValue, out var actual);

        Assert.True(found);
        Assert.Equal(1, actual!.LineCount);
    }

    [Fact]
    [Trait("Category", "LayoutCache")]
    public void LayoutCache_FlexLayoutResult_EmptyArrays_WorksCorrectly()
    {
        var cache = new LayoutCache();
        var result = new LayoutCache.FlexLayoutResult
        {
            ItemMainSizes = Array.Empty<float>(),
            ItemCrossSizes = Array.Empty<float>(),
            LineCrossSizes = Array.Empty<float>(),
            LineCount = 0
        };

        cache.SetFlexLayout(1, 100f, 100f, result);
        bool found = cache.TryGetFlexLayout(1, 100f, 100f, out var actual);

        Assert.True(found);
        Assert.Equal(0, actual!.LineCount);
        Assert.Equal(0, actual.ItemMainSizes?.Length ?? 0);
    }

    [Fact]
    [Trait("Category", "LayoutCache")]
    public void LayoutCache_GridLayoutResult_AllFields_PreservedCorrectly()
    {
        var cache = new LayoutCache();
        var result = new LayoutCache.GridLayoutResult
        {
            ColumnTrackSizes = new[] { 100f, 200f, 150f },
            RowTrackSizes = new[] { 50f, 75f }
        };

        cache.SetGridLayout(1, 450f, 125f, result);
        bool found = cache.TryGetGridLayout(1, 450f, 125f, out var actual);

        Assert.True(found);
        Assert.NotNull(actual);
        Assert.Equal(result.ColumnTrackSizes, actual.ColumnTrackSizes);
        Assert.Equal(result.RowTrackSizes, actual.RowTrackSizes);
    }

    [Fact]
    [Trait("Category", "LayoutCache")]
    public void LayoutCache_OverwriteExistingEntry_ReplacesValue()
    {
        var cache = new LayoutCache();

        var result1 = new LayoutCache.FlexLayoutResult { LineCount = 1 };
        var result2 = new LayoutCache.FlexLayoutResult { LineCount = 5 };

        cache.SetFlexLayout(1, 100f, 100f, result1);
        cache.SetFlexLayout(1, 100f, 100f, result2); // Same key, different value

        bool found = cache.TryGetFlexLayout(1, 100f, 100f, out var actual);

        Assert.True(found);
        Assert.Equal(5, actual!.LineCount);
    }

    [Fact]
    [Trait("Category", "LayoutCache")]
    public void LayoutCache_MixedFlexAndGrid_IndependentCaches()
    {
        var cache = new LayoutCache();

        var flexResult = new LayoutCache.FlexLayoutResult { LineCount = 1 };
        var gridResult = new LayoutCache.GridLayoutResult
        {
            ColumnTrackSizes = new[] { 100f }
        };

        // Same element ID, same sizes, but different cache types
        cache.SetFlexLayout(1, 100f, 100f, flexResult);
        cache.SetGridLayout(1, 100f, 100f, gridResult);

        Assert.Equal(1, cache.FlexCacheCount);
        Assert.Equal(1, cache.GridCacheCount);

        // Both should still be retrievable
        bool foundFlex = cache.TryGetFlexLayout(1, 100f, 100f, out var actualFlex);
        bool foundGrid = cache.TryGetGridLayout(1, 100f, 100f, out var actualGrid);

        Assert.True(foundFlex);
        Assert.True(foundGrid);
        Assert.Equal(1, actualFlex!.LineCount);
        Assert.Equal(1, actualGrid!.ColumnTrackSizes?.Length ?? 0);
    }

    [Fact]
    [Trait("Category", "LayoutCache")]
    public void LayoutCache_InvalidateElement_AffectsBothCaches()
    {
        var cache = new LayoutCache();

        cache.SetFlexLayout(1, 100f, 100f, new LayoutCache.FlexLayoutResult());
        cache.SetGridLayout(1, 100f, 100f, new LayoutCache.GridLayoutResult());

        cache.InvalidateElement(1);

        bool foundFlex = cache.TryGetFlexLayout(1, 100f, 100f, out _);
        bool foundGrid = cache.TryGetGridLayout(1, 100f, 100f, out _);

        Assert.False(foundFlex);
        Assert.False(foundGrid);
    }

    #endregion
}
