using Microsoft.VisualStudio.TestTools.UnitTesting;
using CssUI.CSS.Formatting;
using System;

namespace CssUITests.CSS.Formatting;

/// <summary>
/// Unit tests for LayoutCache.
/// Tests caching of layout computation results.
/// </summary>
[TestClass]
public class LayoutCacheTests
{
    #region Initialization Tests

    [TestMethod]
    [TestCategory("LayoutCache")]
    public void LayoutCache_CanBeInstantiated()
    {
        var cache = new LayoutCache();
        Assert.IsNotNull(cache);
    }

    [TestMethod]
    [TestCategory("LayoutCache")]
    public void LayoutCache_InitialState_Empty()
    {
        var cache = new LayoutCache();
        Assert.AreEqual(0, cache.FlexCacheCount);
        Assert.AreEqual(0, cache.GridCacheCount);
    }

    #endregion

    #region Flex Layout Cache Tests

    [TestMethod]
    [TestCategory("LayoutCache")]
    public void LayoutCache_TryGetFlexLayout_MissOnEmpty()
    {
        var cache = new LayoutCache();
        bool found = cache.TryGetFlexLayout(1, 100f, 100f, out var result);
        Assert.IsFalse(found);
        Assert.IsNull(result);
    }

    [TestMethod]
    [TestCategory("LayoutCache")]
    public void LayoutCache_SetFlexLayout_IncrementsCount()
    {
        var cache = new LayoutCache();
        var result = new LayoutCache.FlexLayoutResult
        {
            ItemMainSizes = new[] { 50f, 50f },
            LineCount = 1
        };
        
        cache.SetFlexLayout(1, 100f, 100f, result);
        Assert.AreEqual(1, cache.FlexCacheCount);
    }

    [TestMethod]
    [TestCategory("LayoutCache")]
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
        
        Assert.IsTrue(found);
        Assert.IsNotNull(actual);
        Assert.AreEqual(expected.LineCount, actual.LineCount);
        CollectionAssert.AreEqual(expected.ItemMainSizes, actual.ItemMainSizes);
    }

    [TestMethod]
    [TestCategory("LayoutCache")]
    public void LayoutCache_TryGetFlexLayout_MissOnDifferentSize()
    {
        var cache = new LayoutCache();
        var result = new LayoutCache.FlexLayoutResult { LineCount = 1 };
        
        cache.SetFlexLayout(1, 100f, 100f, result);
        bool found = cache.TryGetFlexLayout(1, 200f, 100f, out _);
        
        Assert.IsFalse(found);
    }

    [TestMethod]
    [TestCategory("LayoutCache")]
    public void LayoutCache_TryGetFlexLayout_MissAfterInvalidate()
    {
        var cache = new LayoutCache();
        var result = new LayoutCache.FlexLayoutResult { LineCount = 1 };
        
        cache.SetFlexLayout(1, 100f, 100f, result);
        cache.InvalidateElement(1);
        bool found = cache.TryGetFlexLayout(1, 100f, 100f, out _);
        
        Assert.IsFalse(found);
    }

    #endregion

    #region Grid Layout Cache Tests

    [TestMethod]
    [TestCategory("LayoutCache")]
    public void LayoutCache_TryGetGridLayout_MissOnEmpty()
    {
        var cache = new LayoutCache();
        bool found = cache.TryGetGridLayout(1, 100f, 100f, out var result);
        Assert.IsFalse(found);
        Assert.IsNull(result);
    }

    [TestMethod]
    [TestCategory("LayoutCache")]
    public void LayoutCache_SetGridLayout_IncrementsCount()
    {
        var cache = new LayoutCache();
        var result = new LayoutCache.GridLayoutResult
        {
            ColumnTrackSizes = new[] { 50f, 50f },
            RowTrackSizes = new[] { 100f }
        };
        
        cache.SetGridLayout(1, 100f, 100f, result);
        Assert.AreEqual(1, cache.GridCacheCount);
    }

    [TestMethod]
    [TestCategory("LayoutCache")]
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
        
        Assert.IsTrue(found);
        Assert.IsNotNull(actual);
        CollectionAssert.AreEqual(expected.ColumnTrackSizes, actual.ColumnTrackSizes);
        CollectionAssert.AreEqual(expected.RowTrackSizes, actual.RowTrackSizes);
    }

    [TestMethod]
    [TestCategory("LayoutCache")]
    public void LayoutCache_TryGetGridLayout_MissOnDifferentSize()
    {
        var cache = new LayoutCache();
        var result = new LayoutCache.GridLayoutResult();
        
        cache.SetGridLayout(1, 100f, 100f, result);
        bool found = cache.TryGetGridLayout(1, 100f, 200f, out _);
        
        Assert.IsFalse(found);
    }

    [TestMethod]
    [TestCategory("LayoutCache")]
    public void LayoutCache_TryGetGridLayout_MissAfterInvalidate()
    {
        var cache = new LayoutCache();
        var result = new LayoutCache.GridLayoutResult();
        
        cache.SetGridLayout(1, 100f, 100f, result);
        cache.InvalidateElement(1);
        bool found = cache.TryGetGridLayout(1, 100f, 100f, out _);
        
        Assert.IsFalse(found);
    }

    #endregion

    #region Invalidation Tests

    [TestMethod]
    [TestCategory("LayoutCache")]
    public void LayoutCache_InvalidateAll_ClearsBothCaches()
    {
        var cache = new LayoutCache();
        
        cache.SetFlexLayout(1, 100f, 100f, new LayoutCache.FlexLayoutResult());
        cache.SetGridLayout(2, 100f, 100f, new LayoutCache.GridLayoutResult());
        
        Assert.AreEqual(1, cache.FlexCacheCount);
        Assert.AreEqual(1, cache.GridCacheCount);
        
        cache.InvalidateAll();
        
        Assert.AreEqual(0, cache.FlexCacheCount);
        Assert.AreEqual(0, cache.GridCacheCount);
    }

    [TestMethod]
    [TestCategory("LayoutCache")]
    public void LayoutCache_InvalidateElement_OnlyAffectsSpecificElement()
    {
        var cache = new LayoutCache();
        
        cache.SetFlexLayout(1, 100f, 100f, new LayoutCache.FlexLayoutResult());
        cache.SetFlexLayout(2, 100f, 100f, new LayoutCache.FlexLayoutResult());
        
        cache.InvalidateElement(1);
        
        // Element 1 should miss
        bool found1 = cache.TryGetFlexLayout(1, 100f, 100f, out _);
        Assert.IsFalse(found1);
        
        // Element 2 should still hit
        bool found2 = cache.TryGetFlexLayout(2, 100f, 100f, out _);
        Assert.IsTrue(found2);
    }

    [TestMethod]
    [TestCategory("LayoutCache")]
    public void LayoutCache_Clear_RemovesEverything()
    {
        var cache = new LayoutCache();
        
        cache.SetFlexLayout(1, 100f, 100f, new LayoutCache.FlexLayoutResult());
        cache.SetGridLayout(1, 100f, 100f, new LayoutCache.GridLayoutResult());
        cache.InvalidateElement(1);
        
        cache.Clear();
        
        Assert.AreEqual(0, cache.FlexCacheCount);
        Assert.AreEqual(0, cache.GridCacheCount);
    }

    #endregion

    #region Edge Cases

    [TestMethod]
    [TestCategory("LayoutCache")]
    public void LayoutCache_MultipleElementsSameSize_IndependentCaching()
    {
        var cache = new LayoutCache();
        
        var result1 = new LayoutCache.FlexLayoutResult { LineCount = 1 };
        var result2 = new LayoutCache.FlexLayoutResult { LineCount = 2 };
        
        cache.SetFlexLayout(1, 100f, 100f, result1);
        cache.SetFlexLayout(2, 100f, 100f, result2);
        
        cache.TryGetFlexLayout(1, 100f, 100f, out var actual1);
        cache.TryGetFlexLayout(2, 100f, 100f, out var actual2);
        
        Assert.AreEqual(1, actual1!.LineCount);
        Assert.AreEqual(2, actual2!.LineCount);
    }

    [TestMethod]
    [TestCategory("LayoutCache")]
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
        
        Assert.IsTrue(found200, "Latest entry should always be cached");
        Assert.AreEqual(2, actual200!.LineCount);
    }

    [TestMethod]
    [TestCategory("LayoutCache")]
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
        Assert.IsTrue(foundTiny, "Very small size differences should be tolerated");
        Assert.IsFalse(foundLarger, "Larger size differences should result in cache miss");
    }

    #endregion

    #region Additional Edge Cases and Boundary Tests

    [TestMethod]
    [TestCategory("LayoutCache")]
    public void LayoutCache_ZeroElementId_WorksCorrectly()
    {
        var cache = new LayoutCache();
        var result = new LayoutCache.FlexLayoutResult { LineCount = 1 };
        
        cache.SetFlexLayout(0, 100f, 100f, result);
        bool found = cache.TryGetFlexLayout(0, 100f, 100f, out var actual);
        
        Assert.IsTrue(found);
        Assert.AreEqual(1, actual!.LineCount);
    }

    [TestMethod]
    [TestCategory("LayoutCache")]
    public void LayoutCache_NegativeElementId_WorksCorrectly()
    {
        var cache = new LayoutCache();
        var result = new LayoutCache.FlexLayoutResult { LineCount = 2 };
        
        cache.SetFlexLayout(-1, 100f, 100f, result);
        bool found = cache.TryGetFlexLayout(-1, 100f, 100f, out var actual);
        
        Assert.IsTrue(found);
        Assert.AreEqual(2, actual!.LineCount);
    }

    [TestMethod]
    [TestCategory("LayoutCache")]
    public void LayoutCache_ZeroSize_WorksCorrectly()
    {
        var cache = new LayoutCache();
        var result = new LayoutCache.FlexLayoutResult { LineCount = 1 };
        
        cache.SetFlexLayout(1, 0f, 0f, result);
        bool found = cache.TryGetFlexLayout(1, 0f, 0f, out var actual);
        
        Assert.IsTrue(found);
        Assert.AreEqual(1, actual!.LineCount);
    }

    [TestMethod]
    [TestCategory("LayoutCache")]
    public void LayoutCache_VeryLargeSize_WorksCorrectly()
    {
        var cache = new LayoutCache();
        var result = new LayoutCache.FlexLayoutResult { LineCount = 1 };
        
        cache.SetFlexLayout(1, float.MaxValue, float.MaxValue, result);
        bool found = cache.TryGetFlexLayout(1, float.MaxValue, float.MaxValue, out var actual);
        
        Assert.IsTrue(found);
        Assert.AreEqual(1, actual!.LineCount);
    }

    [TestMethod]
    [TestCategory("LayoutCache")]
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
        
        Assert.IsTrue(found);
        Assert.AreEqual(0, actual!.LineCount);
        Assert.AreEqual(0, actual.ItemMainSizes?.Length ?? 0);
    }

    [TestMethod]
    [TestCategory("LayoutCache")]
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
        
        Assert.IsTrue(found);
        Assert.IsNotNull(actual);
        CollectionAssert.AreEqual(result.ColumnTrackSizes, actual.ColumnTrackSizes);
        CollectionAssert.AreEqual(result.RowTrackSizes, actual.RowTrackSizes);
    }

    [TestMethod]
    [TestCategory("LayoutCache")]
    public void LayoutCache_OverwriteExistingEntry_ReplacesValue()
    {
        var cache = new LayoutCache();
        
        var result1 = new LayoutCache.FlexLayoutResult { LineCount = 1 };
        var result2 = new LayoutCache.FlexLayoutResult { LineCount = 5 };
        
        cache.SetFlexLayout(1, 100f, 100f, result1);
        cache.SetFlexLayout(1, 100f, 100f, result2); // Same key, different value
        
        bool found = cache.TryGetFlexLayout(1, 100f, 100f, out var actual);
        
        Assert.IsTrue(found);
        Assert.AreEqual(5, actual!.LineCount, "Should return the most recent value");
    }

    [TestMethod]
    [TestCategory("LayoutCache")]
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
        
        Assert.AreEqual(1, cache.FlexCacheCount);
        Assert.AreEqual(1, cache.GridCacheCount);
        
        // Both should still be retrievable
        bool foundFlex = cache.TryGetFlexLayout(1, 100f, 100f, out var actualFlex);
        bool foundGrid = cache.TryGetGridLayout(1, 100f, 100f, out var actualGrid);
        
        Assert.IsTrue(foundFlex);
        Assert.IsTrue(foundGrid);
        Assert.AreEqual(1, actualFlex!.LineCount);
        Assert.AreEqual(1, actualGrid!.ColumnTrackSizes?.Length ?? 0);
    }

    [TestMethod]
    [TestCategory("LayoutCache")]
    public void LayoutCache_InvalidateElement_AffectsBothCaches()
    {
        var cache = new LayoutCache();
        
        cache.SetFlexLayout(1, 100f, 100f, new LayoutCache.FlexLayoutResult());
        cache.SetGridLayout(1, 100f, 100f, new LayoutCache.GridLayoutResult());
        
        cache.InvalidateElement(1);
        
        bool foundFlex = cache.TryGetFlexLayout(1, 100f, 100f, out _);
        bool foundGrid = cache.TryGetGridLayout(1, 100f, 100f, out _);
        
        Assert.IsFalse(foundFlex, "Flex cache should be invalidated");
        Assert.IsFalse(foundGrid, "Grid cache should be invalidated");
    }

    #endregion
}
