using Microsoft.VisualStudio.TestTools.UnitTesting;
using CssUI.CSS.Formatting;

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
    public void LayoutCache_SameElementDifferentSizes_IndependentCaching()
    {
        var cache = new LayoutCache();
        
        var result100 = new LayoutCache.FlexLayoutResult { LineCount = 1 };
        var result200 = new LayoutCache.FlexLayoutResult { LineCount = 2 };
        
        cache.SetFlexLayout(1, 100f, 100f, result100);
        cache.SetFlexLayout(1, 200f, 200f, result200);
        
        // Only the latest entry should be cached
        bool found100 = cache.TryGetFlexLayout(1, 100f, 100f, out _);
        bool found200 = cache.TryGetFlexLayout(1, 200f, 200f, out var actual200);
        
        Assert.IsFalse(found100);
        Assert.IsTrue(found200);
        Assert.AreEqual(2, actual200!.LineCount);
    }

    [TestMethod]
    [TestCategory("LayoutCache")]
    public void LayoutCache_SmallSizeDifference_TreatedAsDifferent()
    {
        var cache = new LayoutCache();
        var result = new LayoutCache.FlexLayoutResult();
        
        cache.SetFlexLayout(1, 100f, 100f, result);
        
        // 0.001f difference should be treated as same
        bool foundSame = cache.TryGetFlexLayout(1, 100.0005f, 100f, out _);
        Assert.IsTrue(foundSame);
        
        // 0.01f difference should be treated as different
        bool foundDifferent = cache.TryGetFlexLayout(1, 100.01f, 100f, out _);
        Assert.IsFalse(foundDifferent);
    }

    #endregion
}
