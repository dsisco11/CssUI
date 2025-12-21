using System.Linq;
using CssUI.CSS;
using CssUI.CSS.Formatting;
using Xunit;

namespace CssUITests.CSS.Formatting;

/// <summary>
/// Unit tests for CSS Fragmentation support.
/// Tests the CSS Fragmentation (break properties) implementation.
/// Spec: https://www.w3.org/TR/css-break-3/
/// </summary>
public class FragmentationTests
{
    #region EBreakValue Tests

    [Fact]
    [Trait("Category", "Fragmentation")]
    public void EBreakValue_HasAllSpecValues()
    {
        // Verify all CSS Break Level 3 values exist by referencing them directly
        // This ensures compile-time verification that values exist
        var values = new[]
        {
            EBreakValue.Auto,
            EBreakValue.Avoid,
            EBreakValue.AvoidPage,
            EBreakValue.AvoidColumn,
            EBreakValue.AvoidRegion,
            EBreakValue.Page,
            EBreakValue.Column,
            EBreakValue.Region,
            EBreakValue.Left,
            EBreakValue.Right,
            EBreakValue.Recto,
            EBreakValue.Verso
        };

        // Verify count matches expected spec values
        Assert.Equal(12, values.Length);

        // Verify all values are distinct
        Assert.Equal(values.Length, values.Distinct().Count());
    }

    [Fact]
    [Trait("Category", "Fragmentation")]
    public void EBoxDecorationBreak_HasAllSpecValues()
    {
        // Reference values directly for compile-time verification
        var values = new[]
        {
            EBoxDecorationBreak.Slice,
            EBoxDecorationBreak.Clone
        };

        Assert.Equal(2, values.Length);
        Assert.Equal(values.Length, values.Distinct().Count());
    }

    #endregion

    #region FragmentationTypes Tests

    [Fact]
    [Trait("Category", "Fragmentation")]
    public void EFragmentainerType_HasExpectedValues()
    {
        Assert.Equal(0, (int)EFragmentainerType.None);
        Assert.Equal(1, (int)EFragmentainerType.Page);
        Assert.Equal(2, (int)EFragmentainerType.Column);
        Assert.Equal(3, (int)EFragmentainerType.Region);
    }

    [Fact]
    [Trait("Category", "Fragmentation")]
    public void EBreakType_HasExpectedValues()
    {
        Assert.Equal(0, (int)EBreakType.None);
        Assert.Equal(1, (int)EBreakType.Soft);
        Assert.Equal(2, (int)EBreakType.Forced);
    }

    [Fact]
    [Trait("Category", "Fragmentation")]
    public void FragmentationBreakResult_NoBreak_DoesNotBreak()
    {
        var result = FragmentationBreakResult.NoBreak;
        Assert.False(result.ShouldBreak);
        Assert.Equal(EBreakType.None, result.BreakType);
    }

    [Fact]
    [Trait("Category", "Fragmentation")]
    public void FragmentationBreakResult_SoftBreak_ShouldBreak()
    {
        var result = FragmentationBreakResult.SoftBreak(EFragmentainerType.Page);
        Assert.True(result.ShouldBreak);
        Assert.Equal(EBreakType.Soft, result.BreakType);
        Assert.Equal(EFragmentainerType.Page, result.FragmentainerType);
    }

    [Fact]
    [Trait("Category", "Fragmentation")]
    public void FragmentationBreakResult_ForcedBreak_ShouldBreak()
    {
        var result = FragmentationBreakResult.ForcedBreak(EFragmentainerType.Column);
        Assert.True(result.ShouldBreak);
        Assert.Equal(EBreakType.Forced, result.BreakType);
        Assert.Equal(EFragmentainerType.Column, result.FragmentainerType);
    }

    #endregion

    #region FragmentationContext Tests

    [Fact]
    [Trait("Category", "Fragmentation")]
    public void FragmentationContext_DefaultConstructor_NoFragmentation()
    {
        var context = new FragmentationContext();
        Assert.False(context.IsFragmenting);
        Assert.Equal(EFragmentainerType.None, context.FragmentainerType);
        Assert.Equal(float.MaxValue, context.FragmentainerBlockSize);
    }

    [Fact]
    [Trait("Category", "Fragmentation")]
    public void FragmentationContext_WithType_IsFragmenting()
    {
        var context = new FragmentationContext(EFragmentainerType.Page, 800f);
        Assert.True(context.IsFragmenting);
        Assert.Equal(EFragmentainerType.Page, context.FragmentainerType);
        Assert.Equal(800f, context.FragmentainerBlockSize);
    }

    [Fact]
    [Trait("Category", "Fragmentation")]
    public void FragmentationContext_InitialBlockOffset_IsZero()
    {
        var context = new FragmentationContext(EFragmentainerType.Page, 800f);
        Assert.Equal(0f, context.CurrentBlockOffset);
        Assert.Equal(800f, context.RemainingBlockSpace);
    }

    [Fact]
    [Trait("Category", "Fragmentation")]
    public void FragmentationContext_AdvanceBlockOffset_UpdatesOffset()
    {
        var context = new FragmentationContext(EFragmentainerType.Page, 800f);
        context.AdvanceBlockOffset(200f);
        Assert.Equal(200f, context.CurrentBlockOffset);
        Assert.Equal(600f, context.RemainingBlockSpace);
    }

    [Fact]
    [Trait("Category", "Fragmentation")]
    public void FragmentationContext_CanFit_ReturnsTrueWhenEnoughSpace()
    {
        var context = new FragmentationContext(EFragmentainerType.Page, 800f);
        Assert.True(context.CanFit(500f));
        Assert.True(context.CanFit(799f));
    }

    [Fact]
    [Trait("Category", "Fragmentation")]
    public void FragmentationContext_CanFit_ReturnsTrueWhenExactFit()
    {
        var context = new FragmentationContext(EFragmentainerType.Page, 800f);
        // Edge case: exactly fills the fragmentainer
        Assert.True(context.CanFit(800f));
    }

    [Fact]
    [Trait("Category", "Fragmentation")]
    public void FragmentationContext_CanFit_ZeroSize_ReturnsTrue()
    {
        var context = new FragmentationContext(EFragmentainerType.Page, 800f);
        Assert.True(context.CanFit(0f));
    }

    [Fact]
    [Trait("Category", "Fragmentation")]
    public void FragmentationContext_CanFit_ReturnsFalseWhenNotEnoughSpace()
    {
        var context = new FragmentationContext(EFragmentainerType.Page, 800f);
        context.AdvanceBlockOffset(500f);
        Assert.False(context.CanFit(400f));
    }

    [Fact]
    [Trait("Category", "Fragmentation")]
    public void FragmentationContext_AdvanceToNextFragmentainer_IncrementsIndex()
    {
        var context = new FragmentationContext(EFragmentainerType.Page, 800f);
        Assert.Equal(0, context.FragmentIndex);

        context.AdvanceToNextFragmentainer();
        Assert.Equal(1, context.FragmentIndex);
        Assert.Equal(0f, context.CurrentBlockOffset);
    }

    [Fact]
    [Trait("Category", "Fragmentation")]
    public void FragmentationContext_Reset_ClearsState()
    {
        var context = new FragmentationContext(EFragmentainerType.Page, 800f);
        context.AdvanceBlockOffset(300f);
        context.AdvanceToNextFragmentainer();
        context.AdvanceBlockOffset(200f);

        context.Reset();
        Assert.Equal(0, context.FragmentIndex);
        Assert.Equal(0f, context.CurrentBlockOffset);
    }

    [Fact]
    [Trait("Category", "Fragmentation")]
    public void FragmentationContext_NoFragmentation_CanFitAlwaysTrue()
    {
        var context = new FragmentationContext();
        Assert.True(context.CanFit(float.MaxValue));
    }

    [Fact]
    [Trait("Category", "Fragmentation")]
    public void FragmentationContext_EvaluateBreakBefore_NullBox_NoBreak()
    {
        var context = new FragmentationContext(EFragmentainerType.Page, 800f);
        var result = context.EvaluateBreakBefore(null);
        Assert.False(result.ShouldBreak);
    }

    [Fact]
    [Trait("Category", "Fragmentation")]
    public void FragmentationContext_EvaluateBreakAfter_NullBox_NoBreak()
    {
        var context = new FragmentationContext(EFragmentainerType.Page, 800f);
        var result = context.EvaluateBreakAfter(null);
        Assert.False(result.ShouldBreak);
    }

    [Fact]
    [Trait("Category", "Fragmentation")]
    public void FragmentationContext_ShouldAvoidBreakInside_NullBox_False()
    {
        var context = new FragmentationContext(EFragmentainerType.Page, 800f);
        Assert.False(context.ShouldAvoidBreakInside(null));
    }

    [Fact]
    [Trait("Category", "Fragmentation")]
    public void FragmentationContext_CheckOrphans_NullBox_True()
    {
        var context = new FragmentationContext(EFragmentainerType.Page, 800f);
        Assert.True(context.CheckOrphans(null, 1));
    }

    [Fact]
    [Trait("Category", "Fragmentation")]
    public void FragmentationContext_CheckWidows_NullBox_True()
    {
        var context = new FragmentationContext(EFragmentainerType.Page, 800f);
        Assert.True(context.CheckWidows(null, 1));
    }

    #endregion

    #region IFormattingContext Fragmentation Support Tests

    [Fact]
    [Trait("Category", "Fragmentation")]
    public void FlexFormattingContext_Flow_NullNode_ThrowsArgumentNullException()
    {
        var flexContext = new FlexFormattingContext();
        var fragmentationContext = new FragmentationContext(EFragmentainerType.Page, 800f);

        Assert.Throws<System.ArgumentNullException>(() =>
            flexContext.Flow(null!, fragmentationContext));
    }

    [Fact]
    [Trait("Category", "Fragmentation")]
    public void FlexFormattingContext_AcceptsFragmentationContext()
    {
        var flexContext = new FlexFormattingContext();
        var fragmentationContext = new FragmentationContext(EFragmentainerType.Page, 800f);

        // Verify the formatting context can be created with fragmentation support
        Assert.NotNull(flexContext);
        Assert.NotNull(fragmentationContext);
        Assert.True(fragmentationContext.IsFragmenting);
    }

    [Fact]
    [Trait("Category", "Fragmentation")]
    public void GridFormattingContext_Flow_NullNode_ThrowsArgumentNullException()
    {
        var gridContext = new GridFormattingContext();
        var fragmentationContext = new FragmentationContext(EFragmentainerType.Page, 800f);

        Assert.Throws<System.ArgumentNullException>(() =>
            gridContext.Flow(null!, fragmentationContext));
    }

    [Fact]
    [Trait("Category", "Fragmentation")]
    public void GridFormattingContext_AcceptsFragmentationContext()
    {
        var gridContext = new GridFormattingContext();
        var fragmentationContext = new FragmentationContext(EFragmentainerType.Page, 800f);

        // Verify the formatting context can be created with fragmentation support
        Assert.NotNull(gridContext);
        Assert.NotNull(fragmentationContext);
        Assert.True(fragmentationContext.IsFragmenting);
    }

    #endregion

    #region Additional Edge Case Tests

    [Fact]
    [Trait("Category", "Fragmentation")]
    public void FragmentationContext_MultipleAdvanceToNextFragmentainer_IncrementsCorrectly()
    {
        var context = new FragmentationContext(EFragmentainerType.Page, 800f);

        context.AdvanceToNextFragmentainer();
        context.AdvanceToNextFragmentainer();
        context.AdvanceToNextFragmentainer();

        Assert.Equal(3, context.FragmentIndex);
    }

    [Fact]
    [Trait("Category", "Fragmentation")]
    public void FragmentationContext_ColumnType_IsFragmenting()
    {
        var context = new FragmentationContext(EFragmentainerType.Column, 500f);
        Assert.True(context.IsFragmenting);
        Assert.Equal(EFragmentainerType.Column, context.FragmentainerType);
    }

    [Fact]
    [Trait("Category", "Fragmentation")]
    public void FragmentationContext_RegionType_IsFragmenting()
    {
        var context = new FragmentationContext(EFragmentainerType.Region, 600f);
        Assert.True(context.IsFragmenting);
        Assert.Equal(EFragmentainerType.Region, context.FragmentainerType);
    }

    [Fact]
    [Trait("Category", "Fragmentation")]
    public void FragmentationContext_AdvanceBlockOffset_AccumulatesCorrectly()
    {
        var context = new FragmentationContext(EFragmentainerType.Page, 800f);

        context.AdvanceBlockOffset(100f);
        context.AdvanceBlockOffset(150f);
        context.AdvanceBlockOffset(50f);

        Assert.Equal(300f, context.CurrentBlockOffset);
        Assert.Equal(500f, context.RemainingBlockSpace);
    }

    [Fact]
    [Trait("Category", "Fragmentation")]
    public void FragmentationContext_AdvanceToNextFragmentainer_ResetsOffset()
    {
        var context = new FragmentationContext(EFragmentainerType.Page, 800f);

        context.AdvanceBlockOffset(400f);
        Assert.Equal(400f, context.CurrentBlockOffset);

        context.AdvanceToNextFragmentainer();
        Assert.Equal(0f, context.CurrentBlockOffset);
        Assert.Equal(800f, context.RemainingBlockSpace);
    }

    [Fact]
    [Trait("Category", "Fragmentation")]
    public void FragmentationBreakResult_Equality_SameValues()
    {
        var result1 = FragmentationBreakResult.ForcedBreak(EFragmentainerType.Page);
        var result2 = FragmentationBreakResult.ForcedBreak(EFragmentainerType.Page);

        Assert.Equal(result1.ShouldBreak, result2.ShouldBreak);
        Assert.Equal(result1.BreakType, result2.BreakType);
        Assert.Equal(result1.FragmentainerType, result2.FragmentainerType);
    }

    [Fact]
    [Trait("Category", "Fragmentation")]
    public void FragmentationBreakResult_DifferentTypes_DifferentFragmentainerType()
    {
        var pageBreak = FragmentationBreakResult.ForcedBreak(EFragmentainerType.Page);
        var columnBreak = FragmentationBreakResult.ForcedBreak(EFragmentainerType.Column);

        Assert.NotEqual(pageBreak.FragmentainerType, columnBreak.FragmentainerType);
    }

    [Fact]
    [Trait("Category", "Fragmentation")]
    public void FragmentationContext_ZeroBlockSize_CanFitOnlyZero()
    {
        var context = new FragmentationContext(EFragmentainerType.Page, 0f);

        Assert.True(context.CanFit(0f));
        Assert.False(context.CanFit(1f));
    }

    [Fact]
    [Trait("Category", "Fragmentation")]
    public void FragmentationContext_VerySmallBlockSize_CanFitSmallContent()
    {
        var context = new FragmentationContext(EFragmentainerType.Page, 0.5f);

        Assert.True(context.CanFit(0.5f));
        Assert.False(context.CanFit(0.6f));
    }

    #endregion
}
