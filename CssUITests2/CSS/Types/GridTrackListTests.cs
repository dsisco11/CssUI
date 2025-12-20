using System;
using CssUI.CSS;
using CssUI.CSS.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CssUITests.CSS.Types;

/// <summary>
/// Unit tests for CSS Grid track list parsing and fr unit support.
/// Spec: https://www.w3.org/TR/css-grid-1/#track-sizing
/// </summary>
[TestClass]
public class GridTrackListTests
{
    #region GridTrackSize Tests

    [TestMethod]
    [TestCategory("Grid")]
    public void GridTrackSize_Auto_IsIntrinsic()
    {
        var size = GridTrackSize.Auto;
        Assert.AreEqual(EGridTrackSizeType.Auto, size.Type);
        Assert.IsTrue(size.IsIntrinsic);
        Assert.IsFalse(size.IsFlexible);
    }

    [TestMethod]
    [TestCategory("Grid")]
    public void GridTrackSize_MinContent_IsIntrinsic()
    {
        var size = GridTrackSize.MinContent;
        Assert.AreEqual(EGridTrackSizeType.MinContent, size.Type);
        Assert.IsTrue(size.IsIntrinsic);
    }

    [TestMethod]
    [TestCategory("Grid")]
    public void GridTrackSize_MaxContent_IsIntrinsic()
    {
        var size = GridTrackSize.MaxContent;
        Assert.AreEqual(EGridTrackSizeType.MaxContent, size.Type);
        Assert.IsTrue(size.IsIntrinsic);
    }

    [TestMethod]
    [TestCategory("Grid")]
    public void GridTrackSize_Flex_HasCorrectFactor()
    {
        var size = GridTrackSize.Flex(2);
        Assert.AreEqual(EGridTrackSizeType.Fixed, size.Type);
        Assert.AreEqual(ECssUnit.FR, size.Unit);
        Assert.IsTrue(size.IsFlexible);
        Assert.AreEqual(2, size.FlexFactor);
    }

    [TestMethod]
    [TestCategory("Grid")]
    public void GridTrackSize_Fixed_PixelValue()
    {
        var size = new GridTrackSize(100, ECssUnit.PX);
        Assert.AreEqual(EGridTrackSizeType.Fixed, size.Type);
        Assert.AreEqual(100, size.Value);
        Assert.AreEqual(ECssUnit.PX, size.Unit);
        Assert.IsFalse(size.IsFlexible);
    }

    [TestMethod]
    [TestCategory("Grid")]
    public void GridTrackSize_Percentage_HasCorrectValue()
    {
        var size = GridTrackSize.Percentage(50);
        Assert.AreEqual(EGridTrackSizeType.Percentage, size.Type);
        Assert.AreEqual(50, size.Value);
    }

    [TestMethod]
    [TestCategory("Grid")]
    public void GridTrackSize_Minmax_StoresMinAndMax()
    {
        var min = new GridTrackSize(100, ECssUnit.PX);
        var max = GridTrackSize.Flex(1);
        var size = GridTrackSize.Minmax(min, max);

        Assert.AreEqual(EGridTrackSizeType.Minmax, size.Type);
        Assert.AreEqual(100, size.MinSize.Value);
        Assert.IsTrue(size.MaxSize.IsFlexible);
    }

    [TestMethod]
    [TestCategory("Grid")]
    public void GridTrackSize_ToString_Flex()
    {
        var size = GridTrackSize.Flex(1.5);
        Assert.AreEqual("1.5fr", size.ToString());
    }

    [TestMethod]
    [TestCategory("Grid")]
    public void GridTrackSize_ToString_Auto()
    {
        var size = GridTrackSize.Auto;
        Assert.AreEqual("auto", size.ToString());
    }

    #endregion

    #region GridTrackList Parsing Tests

    [TestMethod]
    [TestCategory("Grid")]
    public void GridTrackList_Parse_None_ReturnsEmpty()
    {
        var list = GridTrackList.Parse("none");
        Assert.AreEqual(0, list.Count);
    }

    [TestMethod]
    [TestCategory("Grid")]
    public void GridTrackList_Parse_Empty_ReturnsEmpty()
    {
        var list = GridTrackList.Parse("");
        Assert.AreEqual(0, list.Count);
    }

    [TestMethod]
    [TestCategory("Grid")]
    public void GridTrackList_Parse_SinglePixel()
    {
        var list = GridTrackList.Parse("100px");
        Assert.AreEqual(1, list.Count);
        Assert.AreEqual(100, list[0].Value);
        Assert.AreEqual(ECssUnit.PX, list[0].Unit);
    }

    [TestMethod]
    [TestCategory("Grid")]
    public void GridTrackList_Parse_SingleFr()
    {
        var list = GridTrackList.Parse("1fr");
        Assert.AreEqual(1, list.Count);
        Assert.IsTrue(list[0].IsFlexible);
        Assert.AreEqual(1, list[0].FlexFactor);
    }

    [TestMethod]
    [TestCategory("Grid")]
    public void GridTrackList_Parse_MultipleFr()
    {
        var list = GridTrackList.Parse("1fr 2fr 3fr");
        Assert.AreEqual(3, list.Count);
        Assert.AreEqual(1, list[0].FlexFactor);
        Assert.AreEqual(2, list[1].FlexFactor);
        Assert.AreEqual(3, list[2].FlexFactor);
    }

    [TestMethod]
    [TestCategory("Grid")]
    public void GridTrackList_Parse_MixedUnits()
    {
        var list = GridTrackList.Parse("100px 1fr auto");
        Assert.AreEqual(3, list.Count);
        Assert.AreEqual(ECssUnit.PX, list[0].Unit);
        Assert.IsTrue(list[1].IsFlexible);
        Assert.AreEqual(EGridTrackSizeType.Auto, list[2].Type);
    }

    [TestMethod]
    [TestCategory("Grid")]
    public void GridTrackList_Parse_Percentage()
    {
        var list = GridTrackList.Parse("25%");
        Assert.AreEqual(1, list.Count);
        Assert.AreEqual(EGridTrackSizeType.Percentage, list[0].Type);
        Assert.AreEqual(25, list[0].Value);
    }

    [TestMethod]
    [TestCategory("Grid")]
    public void GridTrackList_Parse_IntrinsicKeywords()
    {
        var list = GridTrackList.Parse("min-content max-content auto");
        Assert.AreEqual(3, list.Count);
        Assert.AreEqual(EGridTrackSizeType.MinContent, list[0].Type);
        Assert.AreEqual(EGridTrackSizeType.MaxContent, list[1].Type);
        Assert.AreEqual(EGridTrackSizeType.Auto, list[2].Type);
    }

    [TestMethod]
    [TestCategory("Grid")]
    public void GridTrackList_HasFlexibleTracks_True()
    {
        var list = GridTrackList.Parse("100px 1fr");
        Assert.IsTrue(list.HasFlexibleTracks);
    }

    [TestMethod]
    [TestCategory("Grid")]
    public void GridTrackList_HasFlexibleTracks_False()
    {
        var list = GridTrackList.Parse("100px 200px");
        Assert.IsFalse(list.HasFlexibleTracks);
    }

    [TestMethod]
    [TestCategory("Grid")]
    public void GridTrackList_TotalFlexFactor_Sums()
    {
        var list = GridTrackList.Parse("1fr 2fr 100px 3fr");
        Assert.AreEqual(6, list.TotalFlexFactor);
    }

    [TestMethod]
    [TestCategory("Grid")]
    public void GridTrackList_ToString_Roundtrip()
    {
        var list = GridTrackList.Parse("1fr 2fr 100px");
        var result = list.ToString();
        Assert.IsTrue(result.Contains("1fr"));
        Assert.IsTrue(result.Contains("2fr"));
        Assert.IsTrue(result.Contains("100"));
    }

    #endregion

    #region FR Unit Resolution Tests

    [TestMethod]
    [TestCategory("Grid")]
    [TestCategory("FrUnit")]
    public void GridTrackSize_Resolve_Fr_DistributesSpace()
    {
        var size = GridTrackSize.Flex(1);
        var resolved = size.Resolve(1000, null, totalFlexFactor: 2, freeSpace: 800);

        Assert.IsNotNull(resolved);
        Assert.AreEqual(400, resolved.Value, 0.001); // 1fr out of 2fr = 50% of 800 = 400
    }

    [TestMethod]
    [TestCategory("Grid")]
    [TestCategory("FrUnit")]
    public void GridTrackSize_Resolve_Fr_ZeroFlexFactor_ReturnsZero()
    {
        var size = GridTrackSize.Flex(1);
        var resolved = size.Resolve(1000, null, totalFlexFactor: 0, freeSpace: 800);

        Assert.IsNotNull(resolved);
        Assert.AreEqual(0, resolved.Value);
    }

    [TestMethod]
    [TestCategory("Grid")]
    [TestCategory("FrUnit")]
    public void GridTrackSize_Resolve_Percentage()
    {
        var size = GridTrackSize.Percentage(25);
        var resolved = size.Resolve(400, null);

        Assert.IsNotNull(resolved);
        Assert.AreEqual(100, resolved.Value); // 25% of 400 = 100
    }

    [TestMethod]
    [TestCategory("Grid")]
    [TestCategory("FrUnit")]
    public void GridTrackSize_Resolve_Fixed_NoResolver()
    {
        var size = new GridTrackSize(100, ECssUnit.PX);
        var resolved = size.Resolve(1000, null);

        Assert.IsNotNull(resolved);
        Assert.AreEqual(100, resolved.Value);
    }

    [TestMethod]
    [TestCategory("Grid")]
    [TestCategory("FrUnit")]
    public void GridTrackSize_Resolve_Auto_ReturnsNull()
    {
        var size = GridTrackSize.Auto;
        var resolved = size.Resolve(1000, null);

        Assert.IsNull(resolved); // Auto requires intrinsic measurement
    }

    #endregion

    #region ECssUnit.FR Tests

    [TestMethod]
    [TestCategory("Grid")]
    public void ECssUnit_FR_Exists()
    {
        Assert.IsTrue(Enum.IsDefined(typeof(ECssUnit), ECssUnit.FR));
    }

    #endregion
}
