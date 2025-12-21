using System;
using CssUI.CSS;
using CssUI.CSS.Types;
using Xunit;

namespace CssUITests.CSS.Types;

/// <summary>
/// Unit tests for CSS Grid track list parsing and fr unit support.
/// Spec: https://www.w3.org/TR/css-grid-1/#track-sizing
/// </summary>
public class GridTrackListTests
{
    #region GridTrackSize Tests

    [Fact]
    [Trait("Category", "Grid")]
    public void GridTrackSize_Auto_IsIntrinsic()
    {
        var size = GridTrackSize.Auto;
        Assert.Equal(EGridTrackSizeType.Auto, size.Type);
        Assert.True(size.IsIntrinsic);
        Assert.False(size.IsFlexible);
    }

    [Fact]
    [Trait("Category", "Grid")]
    public void GridTrackSize_MinContent_IsIntrinsic()
    {
        var size = GridTrackSize.MinContent;
        Assert.Equal(EGridTrackSizeType.MinContent, size.Type);
        Assert.True(size.IsIntrinsic);
    }

    [Fact]
    [Trait("Category", "Grid")]
    public void GridTrackSize_MaxContent_IsIntrinsic()
    {
        var size = GridTrackSize.MaxContent;
        Assert.Equal(EGridTrackSizeType.MaxContent, size.Type);
        Assert.True(size.IsIntrinsic);
    }

    [Fact]
    [Trait("Category", "Grid")]
    public void GridTrackSize_Flex_HasCorrectFactor()
    {
        var size = GridTrackSize.Flex(2);
        Assert.Equal(EGridTrackSizeType.Fixed, size.Type);
        Assert.Equal(ECssUnit.FR, size.Unit);
        Assert.True(size.IsFlexible);
        Assert.Equal(2, size.FlexFactor);
    }

    [Fact]
    [Trait("Category", "Grid")]
    public void GridTrackSize_Fixed_PixelValue()
    {
        var size = new GridTrackSize(100, ECssUnit.PX);
        Assert.Equal(EGridTrackSizeType.Fixed, size.Type);
        Assert.Equal(100, size.Value);
        Assert.Equal(ECssUnit.PX, size.Unit);
        Assert.False(size.IsFlexible);
    }

    [Fact]
    [Trait("Category", "Grid")]
    public void GridTrackSize_Percentage_HasCorrectValue()
    {
        var size = GridTrackSize.Percentage(50);
        Assert.Equal(EGridTrackSizeType.Percentage, size.Type);
        Assert.Equal(50, size.Value);
    }

    [Fact]
    [Trait("Category", "Grid")]
    public void GridTrackSize_Minmax_StoresMinAndMax()
    {
        var min = new GridTrackSize(100, ECssUnit.PX);
        var max = GridTrackSize.Flex(1);
        var size = GridTrackSize.Minmax(min, max);

        Assert.Equal(EGridTrackSizeType.Minmax, size.Type);
        Assert.Equal(100, size.MinSize.Value);
        Assert.True(size.MaxSize.IsFlexible);
    }

    [Fact]
    [Trait("Category", "Grid")]
    public void GridTrackSize_ToString_Flex()
    {
        var size = GridTrackSize.Flex(1.5);
        Assert.Equal("1.5fr", size.ToString());
    }

    [Fact]
    [Trait("Category", "Grid")]
    public void GridTrackSize_ToString_Auto()
    {
        var size = GridTrackSize.Auto;
        Assert.Equal("auto", size.ToString());
    }

    #endregion

    #region GridTrackList Parsing Tests

    [Fact]
    [Trait("Category", "Grid")]
    public void GridTrackList_Parse_None_ReturnsEmpty()
    {
        var list = GridTrackList.Parse("none");
        Assert.Equal(0, list.Count);
    }

    [Fact]
    [Trait("Category", "Grid")]
    public void GridTrackList_Parse_Empty_ReturnsEmpty()
    {
        var list = GridTrackList.Parse("");
        Assert.Equal(0, list.Count);
    }

    [Fact]
    [Trait("Category", "Grid")]
    public void GridTrackList_Parse_SinglePixel()
    {
        var list = GridTrackList.Parse("100px");
        Assert.Equal(1, list.Count);
        Assert.Equal(100, list[0].Value);
        Assert.Equal(ECssUnit.PX, list[0].Unit);
    }

    [Fact]
    [Trait("Category", "Grid")]
    public void GridTrackList_Parse_SingleFr()
    {
        var list = GridTrackList.Parse("1fr");
        Assert.Equal(1, list.Count);
        Assert.True(list[0].IsFlexible);
        Assert.Equal(1, list[0].FlexFactor);
    }

    [Fact]
    [Trait("Category", "Grid")]
    public void GridTrackList_Parse_MultipleFr()
    {
        var list = GridTrackList.Parse("1fr 2fr 3fr");
        Assert.Equal(3, list.Count);
        Assert.Equal(1, list[0].FlexFactor);
        Assert.Equal(2, list[1].FlexFactor);
        Assert.Equal(3, list[2].FlexFactor);
    }

    [Fact]
    [Trait("Category", "Grid")]
    public void GridTrackList_Parse_MixedUnits()
    {
        var list = GridTrackList.Parse("100px 1fr auto");
        Assert.Equal(3, list.Count);
        Assert.Equal(ECssUnit.PX, list[0].Unit);
        Assert.True(list[1].IsFlexible);
        Assert.Equal(EGridTrackSizeType.Auto, list[2].Type);
    }

    [Fact]
    [Trait("Category", "Grid")]
    public void GridTrackList_Parse_Percentage()
    {
        var list = GridTrackList.Parse("25%");
        Assert.Equal(1, list.Count);
        Assert.Equal(EGridTrackSizeType.Percentage, list[0].Type);
        Assert.Equal(25, list[0].Value);
    }

    [Fact]
    [Trait("Category", "Grid")]
    public void GridTrackList_Parse_IntrinsicKeywords()
    {
        var list = GridTrackList.Parse("min-content max-content auto");
        Assert.Equal(3, list.Count);
        Assert.Equal(EGridTrackSizeType.MinContent, list[0].Type);
        Assert.Equal(EGridTrackSizeType.MaxContent, list[1].Type);
        Assert.Equal(EGridTrackSizeType.Auto, list[2].Type);
    }

    [Fact]
    [Trait("Category", "Grid")]
    public void GridTrackList_HasFlexibleTracks_True()
    {
        var list = GridTrackList.Parse("100px 1fr");
        Assert.True(list.HasFlexibleTracks);
    }

    [Fact]
    [Trait("Category", "Grid")]
    public void GridTrackList_HasFlexibleTracks_False()
    {
        var list = GridTrackList.Parse("100px 200px");
        Assert.False(list.HasFlexibleTracks);
    }

    [Fact]
    [Trait("Category", "Grid")]
    public void GridTrackList_TotalFlexFactor_Sums()
    {
        var list = GridTrackList.Parse("1fr 2fr 100px 3fr");
        Assert.Equal(6, list.TotalFlexFactor);
    }

    [Fact]
    [Trait("Category", "Grid")]
    public void GridTrackList_ToString_Roundtrip()
    {
        var list = GridTrackList.Parse("1fr 2fr 100px");
        var result = list.ToString();
        Assert.Contains("1fr", result);
        Assert.Contains("2fr", result);
        Assert.Contains("100", result);
    }

    #endregion

    #region FR Unit Resolution Tests

    [Fact]
    [Trait("Category", "Grid")]
    [Trait("Category", "FrUnit")]
    public void GridTrackSize_Resolve_Fr_DistributesSpace()
    {
        var size = GridTrackSize.Flex(1);
        var resolved = size.Resolve(1000, null, totalFlexFactor: 2, freeSpace: 800);

        Assert.NotNull(resolved);
        Assert.Equal(400, resolved.Value, 3); // 1fr out of 2fr = 50% of 800 = 400
    }

    [Fact]
    [Trait("Category", "Grid")]
    [Trait("Category", "FrUnit")]
    public void GridTrackSize_Resolve_Fr_ZeroFlexFactor_ReturnsZero()
    {
        var size = GridTrackSize.Flex(1);
        var resolved = size.Resolve(1000, null, totalFlexFactor: 0, freeSpace: 800);

        Assert.NotNull(resolved);
        Assert.Equal(0, resolved.Value);
    }

    [Fact]
    [Trait("Category", "Grid")]
    [Trait("Category", "FrUnit")]
    public void GridTrackSize_Resolve_Percentage()
    {
        var size = GridTrackSize.Percentage(25);
        var resolved = size.Resolve(400, null);

        Assert.NotNull(resolved);
        Assert.Equal(100, resolved.Value); // 25% of 400 = 100
    }

    [Fact]
    [Trait("Category", "Grid")]
    [Trait("Category", "FrUnit")]
    public void GridTrackSize_Resolve_Fixed_NoResolver()
    {
        var size = new GridTrackSize(100, ECssUnit.PX);
        var resolved = size.Resolve(1000, null);

        Assert.NotNull(resolved);
        Assert.Equal(100, resolved.Value);
    }

    [Fact]
    [Trait("Category", "Grid")]
    [Trait("Category", "FrUnit")]
    public void GridTrackSize_Resolve_Auto_ReturnsNull()
    {
        var size = GridTrackSize.Auto;
        var resolved = size.Resolve(1000, null);

        Assert.Null(resolved); // Auto requires intrinsic measurement
    }

    #endregion

    #region ECssUnit.FR Tests

    [Fact]
    [Trait("Category", "Grid")]
    public void ECssUnit_FR_Exists()
    {
        Assert.True(Enum.IsDefined(typeof(ECssUnit), ECssUnit.FR));
    }

    #endregion
}
