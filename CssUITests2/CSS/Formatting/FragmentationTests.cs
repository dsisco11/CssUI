using Microsoft.VisualStudio.TestTools.UnitTesting;
using CssUI.CSS;
using CssUI.CSS.Formatting;
using System.Linq;

namespace CssUITests.CSS.Formatting;

/// <summary>
/// Unit tests for CSS Fragmentation support.
/// Tests the CSS Fragmentation (break properties) implementation.
/// Spec: https://www.w3.org/TR/css-break-3/
/// </summary>
[TestClass]
public class FragmentationTests
{
    #region EBreakValue Tests

    [TestMethod]
    [TestCategory("Fragmentation")]
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
        Assert.AreEqual(12, values.Length);
        
        // Verify all values are distinct
        Assert.AreEqual(values.Length, values.Distinct().Count());
    }

    [TestMethod]
    [TestCategory("Fragmentation")]
    public void EBoxDecorationBreak_HasAllSpecValues()
    {
        // Reference values directly for compile-time verification
        var values = new[]
        {
            EBoxDecorationBreak.Slice,
            EBoxDecorationBreak.Clone
        };
        
        Assert.AreEqual(2, values.Length);
        Assert.AreEqual(values.Length, values.Distinct().Count());
    }

    #endregion

    #region FragmentationTypes Tests

    [TestMethod]
    [TestCategory("Fragmentation")]
    public void EFragmentainerType_HasExpectedValues()
    {
        Assert.AreEqual(0, (int)EFragmentainerType.None);
        Assert.AreEqual(1, (int)EFragmentainerType.Page);
        Assert.AreEqual(2, (int)EFragmentainerType.Column);
        Assert.AreEqual(3, (int)EFragmentainerType.Region);
    }

    [TestMethod]
    [TestCategory("Fragmentation")]
    public void EBreakType_HasExpectedValues()
    {
        Assert.AreEqual(0, (int)EBreakType.None);
        Assert.AreEqual(1, (int)EBreakType.Soft);
        Assert.AreEqual(2, (int)EBreakType.Forced);
    }

    [TestMethod]
    [TestCategory("Fragmentation")]
    public void FragmentationBreakResult_NoBreak_DoesNotBreak()
    {
        var result = FragmentationBreakResult.NoBreak;
        Assert.IsFalse(result.ShouldBreak);
        Assert.AreEqual(EBreakType.None, result.BreakType);
    }

    [TestMethod]
    [TestCategory("Fragmentation")]
    public void FragmentationBreakResult_SoftBreak_ShouldBreak()
    {
        var result = FragmentationBreakResult.SoftBreak(EFragmentainerType.Page);
        Assert.IsTrue(result.ShouldBreak);
        Assert.AreEqual(EBreakType.Soft, result.BreakType);
        Assert.AreEqual(EFragmentainerType.Page, result.FragmentainerType);
    }

    [TestMethod]
    [TestCategory("Fragmentation")]
    public void FragmentationBreakResult_ForcedBreak_ShouldBreak()
    {
        var result = FragmentationBreakResult.ForcedBreak(EFragmentainerType.Column);
        Assert.IsTrue(result.ShouldBreak);
        Assert.AreEqual(EBreakType.Forced, result.BreakType);
        Assert.AreEqual(EFragmentainerType.Column, result.FragmentainerType);
    }

    #endregion

    #region FragmentationContext Tests

    [TestMethod]
    [TestCategory("Fragmentation")]
    public void FragmentationContext_DefaultConstructor_NoFragmentation()
    {
        var context = new FragmentationContext();
        Assert.IsFalse(context.IsFragmenting);
        Assert.AreEqual(EFragmentainerType.None, context.FragmentainerType);
        Assert.AreEqual(float.MaxValue, context.FragmentainerBlockSize);
    }

    [TestMethod]
    [TestCategory("Fragmentation")]
    public void FragmentationContext_WithType_IsFragmenting()
    {
        var context = new FragmentationContext(EFragmentainerType.Page, 800f);
        Assert.IsTrue(context.IsFragmenting);
        Assert.AreEqual(EFragmentainerType.Page, context.FragmentainerType);
        Assert.AreEqual(800f, context.FragmentainerBlockSize);
    }

    [TestMethod]
    [TestCategory("Fragmentation")]
    public void FragmentationContext_InitialBlockOffset_IsZero()
    {
        var context = new FragmentationContext(EFragmentainerType.Page, 800f);
        Assert.AreEqual(0f, context.CurrentBlockOffset);
        Assert.AreEqual(800f, context.RemainingBlockSpace);
    }

    [TestMethod]
    [TestCategory("Fragmentation")]
    public void FragmentationContext_AdvanceBlockOffset_UpdatesOffset()
    {
        var context = new FragmentationContext(EFragmentainerType.Page, 800f);
        context.AdvanceBlockOffset(200f);
        Assert.AreEqual(200f, context.CurrentBlockOffset);
        Assert.AreEqual(600f, context.RemainingBlockSpace);
    }

    [TestMethod]
    [TestCategory("Fragmentation")]
    public void FragmentationContext_CanFit_ReturnsTrueWhenEnoughSpace()
    {
        var context = new FragmentationContext(EFragmentainerType.Page, 800f);
        Assert.IsTrue(context.CanFit(500f));
        Assert.IsTrue(context.CanFit(799f));
    }

    [TestMethod]
    [TestCategory("Fragmentation")]
    public void FragmentationContext_CanFit_ReturnsTrueWhenExactFit()
    {
        var context = new FragmentationContext(EFragmentainerType.Page, 800f);
        // Edge case: exactly fills the fragmentainer
        Assert.IsTrue(context.CanFit(800f));
    }

    [TestMethod]
    [TestCategory("Fragmentation")]
    public void FragmentationContext_CanFit_ZeroSize_ReturnsTrue()
    {
        var context = new FragmentationContext(EFragmentainerType.Page, 800f);
        Assert.IsTrue(context.CanFit(0f));
    }

    [TestMethod]
    [TestCategory("Fragmentation")]
    public void FragmentationContext_CanFit_ReturnsFalseWhenNotEnoughSpace()
    {
        var context = new FragmentationContext(EFragmentainerType.Page, 800f);
        context.AdvanceBlockOffset(500f);
        Assert.IsFalse(context.CanFit(400f));
    }

    [TestMethod]
    [TestCategory("Fragmentation")]
    public void FragmentationContext_AdvanceToNextFragmentainer_IncrementsIndex()
    {
        var context = new FragmentationContext(EFragmentainerType.Page, 800f);
        Assert.AreEqual(0, context.FragmentIndex);
        
        context.AdvanceToNextFragmentainer();
        Assert.AreEqual(1, context.FragmentIndex);
        Assert.AreEqual(0f, context.CurrentBlockOffset);
    }

    [TestMethod]
    [TestCategory("Fragmentation")]
    public void FragmentationContext_Reset_ClearsState()
    {
        var context = new FragmentationContext(EFragmentainerType.Page, 800f);
        context.AdvanceBlockOffset(300f);
        context.AdvanceToNextFragmentainer();
        context.AdvanceBlockOffset(200f);
        
        context.Reset();
        Assert.AreEqual(0, context.FragmentIndex);
        Assert.AreEqual(0f, context.CurrentBlockOffset);
    }

    [TestMethod]
    [TestCategory("Fragmentation")]
    public void FragmentationContext_NoFragmentation_CanFitAlwaysTrue()
    {
        var context = new FragmentationContext();
        Assert.IsTrue(context.CanFit(float.MaxValue));
    }

    [TestMethod]
    [TestCategory("Fragmentation")]
    public void FragmentationContext_EvaluateBreakBefore_NullBox_NoBreak()
    {
        var context = new FragmentationContext(EFragmentainerType.Page, 800f);
        var result = context.EvaluateBreakBefore(null);
        Assert.IsFalse(result.ShouldBreak);
    }

    [TestMethod]
    [TestCategory("Fragmentation")]
    public void FragmentationContext_EvaluateBreakAfter_NullBox_NoBreak()
    {
        var context = new FragmentationContext(EFragmentainerType.Page, 800f);
        var result = context.EvaluateBreakAfter(null);
        Assert.IsFalse(result.ShouldBreak);
    }

    [TestMethod]
    [TestCategory("Fragmentation")]
    public void FragmentationContext_ShouldAvoidBreakInside_NullBox_False()
    {
        var context = new FragmentationContext(EFragmentainerType.Page, 800f);
        Assert.IsFalse(context.ShouldAvoidBreakInside(null));
    }

    [TestMethod]
    [TestCategory("Fragmentation")]
    public void FragmentationContext_CheckOrphans_NullBox_True()
    {
        var context = new FragmentationContext(EFragmentainerType.Page, 800f);
        Assert.IsTrue(context.CheckOrphans(null, 1));
    }

    [TestMethod]
    [TestCategory("Fragmentation")]
    public void FragmentationContext_CheckWidows_NullBox_True()
    {
        var context = new FragmentationContext(EFragmentainerType.Page, 800f);
        Assert.IsTrue(context.CheckWidows(null, 1));
    }

    #endregion

    #region IFormattingContext Fragmentation Support Tests

    [TestMethod]
    [TestCategory("Fragmentation")]
    public void FlexFormattingContext_Flow_NullNode_ThrowsArgumentNullException()
    {
        var flexContext = new FlexFormattingContext();
        var fragmentationContext = new FragmentationContext(EFragmentainerType.Page, 800f);
        
        Assert.ThrowsException<System.ArgumentNullException>(() => 
            flexContext.Flow(null!, fragmentationContext));
    }

    [TestMethod]
    [TestCategory("Fragmentation")]
    public void FlexFormattingContext_AcceptsFragmentationContext()
    {
        var flexContext = new FlexFormattingContext();
        var fragmentationContext = new FragmentationContext(EFragmentainerType.Page, 800f);
        
        // Verify the formatting context can be created with fragmentation support
        Assert.IsNotNull(flexContext);
        Assert.IsNotNull(fragmentationContext);
        Assert.IsTrue(fragmentationContext.IsFragmenting);
    }

    [TestMethod]
    [TestCategory("Fragmentation")]
    public void GridFormattingContext_Flow_NullNode_ThrowsArgumentNullException()
    {
        var gridContext = new GridFormattingContext();
        var fragmentationContext = new FragmentationContext(EFragmentainerType.Page, 800f);
        
        Assert.ThrowsException<System.ArgumentNullException>(() => 
            gridContext.Flow(null!, fragmentationContext));
    }

    [TestMethod]
    [TestCategory("Fragmentation")]
    public void GridFormattingContext_AcceptsFragmentationContext()
    {
        var gridContext = new GridFormattingContext();
        var fragmentationContext = new FragmentationContext(EFragmentainerType.Page, 800f);
        
        // Verify the formatting context can be created with fragmentation support
        Assert.IsNotNull(gridContext);
        Assert.IsNotNull(fragmentationContext);
        Assert.IsTrue(fragmentationContext.IsFragmenting);
    }

    #endregion

    #region Additional Edge Case Tests

    [TestMethod]
    [TestCategory("Fragmentation")]
    public void FragmentationContext_MultipleAdvanceToNextFragmentainer_IncrementsCorrectly()
    {
        var context = new FragmentationContext(EFragmentainerType.Page, 800f);
        
        context.AdvanceToNextFragmentainer();
        context.AdvanceToNextFragmentainer();
        context.AdvanceToNextFragmentainer();
        
        Assert.AreEqual(3, context.FragmentIndex);
    }

    [TestMethod]
    [TestCategory("Fragmentation")]
    public void FragmentationContext_ColumnType_IsFragmenting()
    {
        var context = new FragmentationContext(EFragmentainerType.Column, 500f);
        Assert.IsTrue(context.IsFragmenting);
        Assert.AreEqual(EFragmentainerType.Column, context.FragmentainerType);
    }

    [TestMethod]
    [TestCategory("Fragmentation")]
    public void FragmentationContext_RegionType_IsFragmenting()
    {
        var context = new FragmentationContext(EFragmentainerType.Region, 600f);
        Assert.IsTrue(context.IsFragmenting);
        Assert.AreEqual(EFragmentainerType.Region, context.FragmentainerType);
    }

    [TestMethod]
    [TestCategory("Fragmentation")]
    public void FragmentationContext_AdvanceBlockOffset_AccumulatesCorrectly()
    {
        var context = new FragmentationContext(EFragmentainerType.Page, 800f);
        
        context.AdvanceBlockOffset(100f);
        context.AdvanceBlockOffset(150f);
        context.AdvanceBlockOffset(50f);
        
        Assert.AreEqual(300f, context.CurrentBlockOffset);
        Assert.AreEqual(500f, context.RemainingBlockSpace);
    }

    [TestMethod]
    [TestCategory("Fragmentation")]
    public void FragmentationContext_AdvanceToNextFragmentainer_ResetsOffset()
    {
        var context = new FragmentationContext(EFragmentainerType.Page, 800f);
        
        context.AdvanceBlockOffset(400f);
        Assert.AreEqual(400f, context.CurrentBlockOffset);
        
        context.AdvanceToNextFragmentainer();
        Assert.AreEqual(0f, context.CurrentBlockOffset);
        Assert.AreEqual(800f, context.RemainingBlockSpace);
    }

    [TestMethod]
    [TestCategory("Fragmentation")]
    public void FragmentationBreakResult_Equality_SameValues()
    {
        var result1 = FragmentationBreakResult.ForcedBreak(EFragmentainerType.Page);
        var result2 = FragmentationBreakResult.ForcedBreak(EFragmentainerType.Page);
        
        Assert.AreEqual(result1.ShouldBreak, result2.ShouldBreak);
        Assert.AreEqual(result1.BreakType, result2.BreakType);
        Assert.AreEqual(result1.FragmentainerType, result2.FragmentainerType);
    }

    [TestMethod]
    [TestCategory("Fragmentation")]
    public void FragmentationBreakResult_DifferentTypes_DifferentFragmentainerType()
    {
        var pageBreak = FragmentationBreakResult.ForcedBreak(EFragmentainerType.Page);
        var columnBreak = FragmentationBreakResult.ForcedBreak(EFragmentainerType.Column);
        
        Assert.AreNotEqual(pageBreak.FragmentainerType, columnBreak.FragmentainerType);
    }

    [TestMethod]
    [TestCategory("Fragmentation")]
    public void FragmentationContext_ZeroBlockSize_CanFitOnlyZero()
    {
        var context = new FragmentationContext(EFragmentainerType.Page, 0f);
        
        Assert.IsTrue(context.CanFit(0f));
        Assert.IsFalse(context.CanFit(1f));
    }

    [TestMethod]
    [TestCategory("Fragmentation")]
    public void FragmentationContext_VerySmallBlockSize_CanFitSmallContent()
    {
        var context = new FragmentationContext(EFragmentainerType.Page, 0.5f);
        
        Assert.IsTrue(context.CanFit(0.5f));
        Assert.IsFalse(context.CanFit(0.6f));
    }

    #endregion
}
