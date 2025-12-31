using System;
using CssUI;
using CssUI.CSS;
using CssUI.CSS.BoxTree;
using CssUI.CSS.Enums;
using CssUI.CSS.Internal;
using CssUITests.Fixtures;
using Xunit;

namespace CssUITests.CSS.BoxModel;

/// <summary>
/// Unit tests for internal BoxModel methods.
/// These tests directly invoke the internal calculation methods to verify
/// the CSS 2.2 width/height/margin resolution algorithms.
/// Spec Reference: https://www.w3.org/TR/CSS2/visudet.html
/// </summary>
/// <remarks>
/// Uses BoxModelTestFixture to create minimal DOM structures with directly
/// configured boxes and styles, avoiding the full layout pipeline.
/// </remarks>
public class BoxModelInternalTests : IDisposable
{
    private readonly BoxModelTestFixture _fixture;

    public BoxModelInternalTests()
    {
        _fixture = new BoxModelTestFixture();
    }

    public void Dispose()
    {
        _fixture.Dispose();
    }

    #region Diagnostic Test

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Diagnostic")]
    public void Diagnostic_ContainingBlock_IsSetCorrectly()
    {
        // Arrange - Create element with specific containing block dimensions
        var (element, box, cascaded) = _fixture.CreateTestElement(style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Width.Set(200);
            style.Margin_Left.Set(CssValue.From_Percent(10));
        }, containingBlockWidth: 400, containingBlockHeight: 300);

        // Act - Check the containing block
        var containingBox = box.Containing_Box;
        var cbWidth = containingBox.Width;
        var cbHeight = containingBox.Height;

        // Also check the element reference
        var elementBox = element.Box;
        var elementCbWidth = elementBox!.Containing_Box.Width;

        // And check the property owner's box
        var marginLeftProperty = cascaded.Margin_Left;
        var propertyOwner = marginLeftProperty.Owner;
        var ownerBox = propertyOwner.Box;
        var ownerCbWidth = ownerBox!.Containing_Box.Width;

        // Check if element and owner are the same
        var sameElement = ReferenceEquals(element, propertyOwner);
        var sameBox = ReferenceEquals(box, ownerBox);

        // Assert
        Assert.True(sameElement, "Property owner should be the same element");
        Assert.True(sameBox, "Property owner's box should be the same box");
        Assert.Equal(400, cbWidth); // Direct box access
        Assert.Equal(400, elementCbWidth); // Through element
        Assert.Equal(400, ownerCbWidth); // Through property owner
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Diagnostic")]
    public void Diagnostic_PercentageResolver_GetsCorrectContainingBlock()
    {
        // Arrange - Create element with specific containing block dimensions
        var (element, box, cascaded) = _fixture.CreateTestElement(style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Width.Set(200);
            style.Margin_Left.Set(CssValue.From_Percent(10));
        }, containingBlockWidth: 400, containingBlockHeight: 300);

        // Act - Call the resolver directly
        var marginLeftProperty = cascaded.Margin_Left;
        
        // Get the computed value (should be the percentage)
        var computed = marginLeftProperty.Computed;
        Assert.Equal(ECssValueTypes.PERCENT, computed!.Type);
        Assert.Equal(10, computed.AsDecimal(), 0.01); // 10%
        
        // Now call ResolvePercentageIfNeeded
        var resolved = CssUI.CSS.Internal.CssPercentageResolvers.ResolvePercentageIfNeeded(marginLeftProperty);
        
        // Should be 10% of 400 = 40
        Assert.Equal(40, resolved.AsDecimal(), 1);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Diagnostic")]
    public void Diagnostic_ResolveHorizontal_MarginResolution()
    {
        // Arrange - Containing block 400px, element with 10% margins
        var (element, box, cascaded) = _fixture.CreateTestElement(style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Width.Set(200);
            style.Margin_Left.Set(CssValue.From_Percent(10));
            style.Margin_Right.Set(CssValue.From_Percent(10));
        }, containingBlockWidth: 400, containingBlockHeight: 300);

        // First, verify the raw resolved percentage
        var rawResolvedMarginLeft = CssUI.CSS.Internal.CssPercentageResolvers.ResolvePercentageIfNeeded(cascaded.Margin_Left);
        Assert.Equal(40, rawResolvedMarginLeft.AsDecimal(), 1); // 10% of 400

        // Now call Resolve_Horizontal
        CssUI.CSS.BoxModel.Resolve_Horizontal(box, cascaded,
            out CssValue outLeft, out CssValue outMarginLeft, out CssValue outWidth,
            out CssValue outMarginRight, out CssValue outRight);

        // Log what we got
        // MarginLeft should be 40 (10% of 400), unless Calculate_Horizontal changed it
        // Calculate_Horizontal might change margins for over-constrained case
        
        // Check what the constraint equation gives us:
        // marginLeft + border + padding + width + padding + border + marginRight = containingWidth
        // 40 + 0 + 0 + 200 + 0 + 0 + 40 = 280 < 400
        // So there's remaining space: 400 - 280 = 120
        // For block elements with no auto values, this goes to margin-right
        // Since margin-right is specified as 10% = 40, the system is over-constrained
        // Per CSS 2.1, margin-right should be adjusted: 400 - (40 + 0 + 0 + 200 + 0 + 0) = 160
        
        // So we expect: marginLeft = 40, marginRight = 160
        Assert.Equal(40, outMarginLeft.AsDecimal(), 1);
        Assert.Equal(160, outMarginRight.AsDecimal(), 1); // This is the over-constrained adjustment
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Diagnostic")]
    public void Diagnostic_OverConstrained_ChecksValues()
    {
        // Arrange - Over-constrained block in LTR should adjust margin-right
        var (element, box, cascaded) = _fixture.CreateTestElement(style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Width.Set(300);
            style.Margin_Left.Set(50);
            style.Margin_Right.Set(100);
            style.Direction.Set(EDirection.LTR);
        }, containingBlockWidth: 400, containingBlockHeight: 300);

        // Verify CssValue.From() doesn't return auto values
        var testMarginLeft = CssValue.From(50);
        var testMarginRight = CssValue.From(100);
        var testWidth = CssValue.From(300);
        
        // CRITICAL: These must NOT be auto
        Assert.False(testMarginLeft.IsAuto, $"MarginLeft Type={testMarginLeft.Type} should not be auto");
        Assert.False(testMarginRight.IsAuto, $"MarginRight Type={testMarginRight.Type} should not be auto");
        Assert.False(testWidth.IsAuto, $"Width Type={testWidth.Type} should not be auto");

        // Check preconditions
        Assert.Equal(EBoxDisplayGroup.BLOCK, box.DisplayGroup);
        Assert.False(box.IsReplacedElement);
        Assert.Equal(EDirection.LTR, cascaded.Direction.Actual);
        Assert.Equal(400, box.Containing_Box.Width, 1);

        // Call the function
        CssUI.CSS.BoxModel.Calculate_Horizontal(box, cascaded,
            CssValue.Auto,
            testMarginLeft,
            testWidth,
            testMarginRight,
            CssValue.Auto,
            out CssValue outLeft,
            out CssValue outMarginLeft,
            out CssValue outWidth,
            out CssValue outMarginRight,
            out CssValue outRight);

        // If outWidth changed from 300, it means Width.IsAuto evaluated to true inside
        // If outWidth is still 300, Width.IsAuto was false (correct)
        Assert.Equal(300, outWidth.AsDecimal(), 1);
        
        // Check margins
        Assert.Equal(50, outMarginLeft.AsDecimal(), 1);
        Assert.Equal(50, outMarginRight.AsDecimal(), 1);
    }

    #endregion

    #region Resolve_Horizontal Tests

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Internal")]
    public void ResolveHorizontal_PercentageWidth_ResolvesAgainstContainingBlock()
    {
        // Arrange - Containing block 400px, element 50% width
        var (_, box, cascaded) = _fixture.CreateTestElement(style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Width.Set(CssValue.From_Percent(50));
        }, containingBlockWidth: 400, containingBlockHeight: 300);

        // Act
        CssUI.CSS.BoxModel.Resolve_Horizontal(box, cascaded,
            out CssValue outLeft, out CssValue outMarginLeft, out CssValue outWidth,
            out CssValue outMarginRight, out CssValue outRight);

        // Assert - 50% of 400 = 200
        Assert.Equal(200, outWidth.AsDecimal(), precision: 1);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Internal")]
    public void ResolveHorizontal_PercentageMargins_ResolveAgainstContainingBlockWidth()
    {
        // Arrange - Containing block 400px, element with 10% margins
        var (_, box, cascaded) = _fixture.CreateTestElement(style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Width.Set(200);
            style.Margin_Left.Set(CssValue.From_Percent(10));
            style.Margin_Right.Set(CssValue.From_Percent(10));
        }, containingBlockWidth: 400, containingBlockHeight: 300);

        // Act
        CssUI.CSS.BoxModel.Resolve_Horizontal(box, cascaded,
            out CssValue outLeft, out CssValue outMarginLeft, out CssValue outWidth,
            out CssValue outMarginRight, out CssValue outRight);

        // Assert - margin-left: 10% of 400 = 40
        Assert.Equal(40, outMarginLeft.AsDecimal(), precision: 1);
        
        // NOTE: margin-right is adjusted per CSS 2.1 §10.3.3 (over-constrained case)
        // Total specified: 40 + 0 + 0 + 200 + 0 + 0 + 40 = 280
        // Containing block: 400
        // Since no margin is auto, this is over-constrained
        // Per spec, margin-right is adjusted to: 400 - (40 + 0 + 0 + 200 + 0 + 0) = 160
        Assert.Equal(160, outMarginRight.AsDecimal(), precision: 1);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Internal")]
    public void ResolveHorizontal_MinWidthConstraint_ClampsUpward()
    {
        // Arrange - Element 100px width with min-width 200px
        var (_, box, cascaded) = _fixture.CreateTestElement(style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Width.Set(100);
            style.Min_Width.Set(CssValue.From_Dimension(200, ECssUnit.PX));
        }, containingBlockWidth: 400);

        // Act
        CssUI.CSS.BoxModel.Resolve_Horizontal(box, cascaded,
            out CssValue outLeft, out CssValue outMarginLeft, out CssValue outWidth,
            out CssValue outMarginRight, out CssValue outRight);

        // Assert - min-width clamps to 200
        Assert.Equal(200, outWidth.AsDecimal(), precision: 1);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Internal")]
    public void ResolveHorizontal_MaxWidthConstraint_ClampsDownward()
    {
        // Arrange - Element 300px width with max-width 200px
        var (_, box, cascaded) = _fixture.CreateTestElement(style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Width.Set(300);
            style.Max_Width.Set(CssValue.From_Dimension(200, ECssUnit.PX));
        }, containingBlockWidth: 400);

        // Act
        CssUI.CSS.BoxModel.Resolve_Horizontal(box, cascaded,
            out CssValue outLeft, out CssValue outMarginLeft, out CssValue outWidth,
            out CssValue outMarginRight, out CssValue outRight);

        // Assert - max-width clamps to 200
        Assert.Equal(200, outWidth.AsDecimal(), precision: 1);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Internal")]
    public void ResolveHorizontal_MinWidthGreaterThanMaxWidth_MinWins()
    {
        // Arrange - Per CSS 2.1 §10.4: if min-width > max-width, max-width is ignored
        var (_, box, cascaded) = _fixture.CreateTestElement(style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Width.Set(150);
            style.Min_Width.Set(CssValue.From_Dimension(200, ECssUnit.PX));
            style.Max_Width.Set(CssValue.From_Dimension(100, ECssUnit.PX));
        }, containingBlockWidth: 400);

        // Act
        CssUI.CSS.BoxModel.Resolve_Horizontal(box, cascaded,
            out CssValue outLeft, out CssValue outMarginLeft, out CssValue outWidth,
            out CssValue outMarginRight, out CssValue outRight);

        // Assert - min-width wins, width = 200
        Assert.Equal(200, outWidth.AsDecimal(), precision: 1);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Internal")]
    public void ResolveHorizontal_FixedWidthWithPadding_WidthExcludesPadding()
    {
        // Arrange - Element 200px width with 20px padding
        var (_, box, cascaded) = _fixture.CreateTestElement(style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Width.Set(200);
            style.Padding_Left.Set(20);
            style.Padding_Right.Set(20);
        }, containingBlockWidth: 400);

        // Act
        CssUI.CSS.BoxModel.Resolve_Horizontal(box, cascaded,
            out CssValue outLeft, out CssValue outMarginLeft, out CssValue outWidth,
            out CssValue outMarginRight, out CssValue outRight);

        // Assert - Width is content-box width (200px), not including padding
        Assert.Equal(200, outWidth.AsDecimal(), precision: 1);
    }

    #endregion

    #region Resolve_Vertical Tests

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Internal")]
    public void ResolveVertical_FixedHeight_ReturnsSpecifiedValue()
    {
        // Arrange - Element with explicit height
        var (_, box, cascaded) = _fixture.CreateTestElement(style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Height.Set(300);
        }, containingBlockWidth: 400, containingBlockHeight: 600);

        // Act
        CssUI.CSS.BoxModel.Resolve_Vertical(box, cascaded,
            out CssValue outTop, out CssValue outMarginTop, out CssValue outHeight,
            out CssValue outMarginBottom, out CssValue outBottom);

        // Assert
        Assert.Equal(300, outHeight.AsDecimal(), precision: 1);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Internal")]
    public void ResolveVertical_PercentageHeight_ResolvesAgainstContainingBlock()
    {
        // Arrange - Containing block 600px height, element 50% height
        var (_, box, cascaded) = _fixture.CreateTestElement(style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Height.Set(CssValue.From_Percent(50));
        }, containingBlockWidth: 400, containingBlockHeight: 600);

        // Act
        CssUI.CSS.BoxModel.Resolve_Vertical(box, cascaded,
            out CssValue outTop, out CssValue outMarginTop, out CssValue outHeight,
            out CssValue outMarginBottom, out CssValue outBottom);

        // Assert - 50% of 600 = 300
        Assert.Equal(300, outHeight.AsDecimal(), precision: 1);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Internal")]
    public void ResolveVertical_MinHeightConstraint_ClampsUpward()
    {
        // Arrange - Element 100px height with min-height 200px
        var (_, box, cascaded) = _fixture.CreateTestElement(style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Height.Set(100);
            style.Min_Height.Set(CssValue.From_Dimension(200, ECssUnit.PX));
        }, containingBlockHeight: 600);

        // Act
        CssUI.CSS.BoxModel.Resolve_Vertical(box, cascaded,
            out CssValue outTop, out CssValue outMarginTop, out CssValue outHeight,
            out CssValue outMarginBottom, out CssValue outBottom);

        // Assert - min-height clamps to 200
        Assert.Equal(200, outHeight.AsDecimal(), precision: 1);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Internal")]
    public void ResolveVertical_MaxHeightConstraint_ClampsDownward()
    {
        // Arrange - Element 400px height with max-height 200px
        var (_, box, cascaded) = _fixture.CreateTestElement(style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Height.Set(400);
            style.Max_Height.Set(CssValue.From_Dimension(200, ECssUnit.PX));
        }, containingBlockHeight: 600);

        // Act
        CssUI.CSS.BoxModel.Resolve_Vertical(box, cascaded,
            out CssValue outTop, out CssValue outMarginTop, out CssValue outHeight,
            out CssValue outMarginBottom, out CssValue outBottom);

        // Assert - max-height clamps to 200
        Assert.Equal(200, outHeight.AsDecimal(), precision: 1);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Internal")]
    public void ResolveVertical_MinHeightGreaterThanMaxHeight_MinWins()
    {
        // Arrange - Per CSS 2.1 §10.7: if min-height > max-height, max-height is ignored
        var (_, box, cascaded) = _fixture.CreateTestElement(style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Height.Set(150);
            style.Min_Height.Set(CssValue.From_Dimension(300, ECssUnit.PX));
            style.Max_Height.Set(CssValue.From_Dimension(100, ECssUnit.PX));
        }, containingBlockHeight: 600);

        // Act
        CssUI.CSS.BoxModel.Resolve_Vertical(box, cascaded,
            out CssValue outTop, out CssValue outMarginTop, out CssValue outHeight,
            out CssValue outMarginBottom, out CssValue outBottom);

        // Assert - min-height wins, height = 300
        Assert.Equal(300, outHeight.AsDecimal(), precision: 1);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Internal")]
    public void ResolveVertical_PercentageMargins_ResolveAgainstContainingBlockWidth()
    {
        // Arrange - Per CSS 2.1: vertical margins resolve against containing block WIDTH
        var (_, box, cascaded) = _fixture.CreateTestElement(style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Height.Set(200);
            style.Margin_Top.Set(CssValue.From_Percent(10));
            style.Margin_Bottom.Set(CssValue.From_Percent(10));
        }, containingBlockWidth: 400, containingBlockHeight: 600);

        // Act
        CssUI.CSS.BoxModel.Resolve_Vertical(box, cascaded,
            out CssValue outTop, out CssValue outMarginTop, out CssValue outHeight,
            out CssValue outMarginBottom, out CssValue outBottom);

        // Assert - 10% of containing block WIDTH (400) = 40
        Assert.Equal(40, outMarginTop.AsDecimal(), precision: 1);
        Assert.Equal(40, outMarginBottom.AsDecimal(), precision: 1);
    }

    #endregion

    #region Calculate_Horizontal Tests - Block Elements

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Internal")]
    public void CalculateHorizontal_Block_AutoWidth_FillsContainingBlock()
    {
        // Arrange - Block with auto width should fill containing block
        var (_, box, cascaded) = _fixture.CreateTestElement(style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            // Width defaults to auto
        }, containingBlockWidth: 400, containingBlockHeight: 300);

        CssValue Left = CssValue.Auto;
        CssValue MarginLeft = CssValue.From(0);
        CssValue Width = CssValue.Auto;
        CssValue MarginRight = CssValue.From(0);
        CssValue Right = CssValue.Auto;

        // Act
        CssUI.CSS.BoxModel.Calculate_Horizontal(box, cascaded,
            ref Left, ref MarginLeft, ref Width, ref MarginRight, ref Right);

        // Assert - auto width fills: 400 - 0 - 0 = 400
        Assert.Equal(400, Width.AsDecimal(), precision: 1);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Internal")]
    public void CalculateHorizontal_Block_AutoMargins_CentersElement()
    {
        // Arrange - Block with auto margins should be centered
        var (_, box, cascaded) = _fixture.CreateTestElement(style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Width.Set(200);
            style.Margin_Left.Set(CssValue.Auto);
            style.Margin_Right.Set(CssValue.Auto);
        }, containingBlockWidth: 400, containingBlockHeight: 300);

        CssValue Left = CssValue.Auto;
        CssValue MarginLeft = CssValue.Auto;
        CssValue Width = CssValue.From(200);
        CssValue MarginRight = CssValue.Auto;
        CssValue Right = CssValue.Auto;

        // Act
        CssUI.CSS.BoxModel.Calculate_Horizontal(box, cascaded,
            ref Left, ref MarginLeft, ref Width, ref MarginRight, ref Right);

        // Assert - margins split: (400 - 200) / 2 = 100 each
        Assert.Equal(100, MarginLeft.AsDecimal(), precision: 1);
        Assert.Equal(100, MarginRight.AsDecimal(), precision: 1);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Internal")]
    public void CalculateHorizontal_Block_OneAutoMargin_AbsorbsRemainder()
    {
        // Arrange - One auto margin absorbs remaining space
        var (_, box, cascaded) = _fixture.CreateTestElement(style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Width.Set(200);
            style.Margin_Left.Set(50);
            style.Margin_Right.Set(CssValue.Auto);
        }, containingBlockWidth: 400, containingBlockHeight: 300);

        CssValue Left = CssValue.Auto;
        CssValue MarginLeft = CssValue.From(50);
        CssValue Width = CssValue.From(200);
        CssValue MarginRight = CssValue.Auto;
        CssValue Right = CssValue.Auto;

        // Act
        CssUI.CSS.BoxModel.Calculate_Horizontal(box, cascaded,
            ref Left, ref MarginLeft, ref Width, ref MarginRight, ref Right);

        // Assert - margin-right absorbs: 400 - 200 - 50 = 150
        Assert.Equal(150, MarginRight.AsDecimal(), precision: 1);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Internal")]
    public void CalculateHorizontal_Block_OverConstrained_LTR_AdjustsMarginRight()
    {
        // Arrange - Over-constrained block in LTR should adjust margin-right
        var (_, box, cascaded) = _fixture.CreateTestElement(style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Width.Set(300);
            style.Margin_Left.Set(50);
            style.Margin_Right.Set(100);
            style.Direction.Set(EDirection.LTR);
        }, containingBlockWidth: 400, containingBlockHeight: 300);

        CssValue Left = CssValue.Auto;
        CssValue MarginLeft = CssValue.From(50);
        CssValue Width = CssValue.From(300);
        CssValue MarginRight = CssValue.From(100);
        CssValue Right = CssValue.Auto;

        // Act
        CssUI.CSS.BoxModel.Calculate_Horizontal(box, cascaded,
            ref Left, ref MarginLeft, ref Width, ref MarginRight, ref Right);

        // Assert - margin-right adjusted: 400 - 300 - 50 = 50 (not 100)
        Assert.Equal(50, MarginRight.AsDecimal(), precision: 1);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Internal")]
    public void CalculateHorizontal_Block_OverConstrained_RTL_AdjustsMarginLeft()
    {
        // Arrange - Over-constrained block in RTL should adjust margin-left
        var (_, box, cascaded) = _fixture.CreateTestElement(style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Width.Set(300);
            style.Margin_Left.Set(100);
            style.Margin_Right.Set(50);
            style.Direction.Set(EDirection.RTL);
        }, containingBlockWidth: 400, containingBlockHeight: 300);

        CssValue Left = CssValue.Auto;
        CssValue MarginLeft = CssValue.From(100);
        CssValue Width = CssValue.From(300);
        CssValue MarginRight = CssValue.From(50);
        CssValue Right = CssValue.Auto;

        // Act
        CssUI.CSS.BoxModel.Calculate_Horizontal(box, cascaded,
            ref Left, ref MarginLeft, ref Width, ref MarginRight, ref Right);

        // Assert - margin-left adjusted: 400 - 300 - 50 = 50 (not 100)
        Assert.Equal(50, MarginLeft.AsDecimal(), precision: 1);
    }

    #endregion

    #region Calculate_Horizontal Tests - Absolutely Positioned

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Internal")]
    public void CalculateHorizontal_Absolute_LeftRightSpecified_WidthCalculated()
    {
        // Arrange - Absolute with left and right specified, width is calculated
        var (_, box, cascaded) = _fixture.CreateAbsoluteElement(style =>
        {
            style.Left.Set(50);
            style.Right.Set(50);
            style.Margin_Left.Set(10);
            style.Margin_Right.Set(10);
        }, containingBlockWidth: 400, containingBlockHeight: 300);

        CssValue Left = CssValue.From(50);
        CssValue MarginLeft = CssValue.From(10);
        CssValue Width = CssValue.Auto;
        CssValue MarginRight = CssValue.From(10);
        CssValue Right = CssValue.From(50);

        // Act
        CssUI.CSS.BoxModel.Calculate_Horizontal(box, cascaded,
            ref Left, ref MarginLeft, ref Width, ref MarginRight, ref Right);

        // Assert - width = 400 - 50 - 10 - 10 - 50 = 280
        Assert.Equal(280, Width.AsDecimal(), precision: 1);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Internal")]
    public void CalculateHorizontal_Absolute_WidthSpecified_RightCalculated()
    {
        // Arrange - Absolute with width and left specified, right is calculated
        var (_, box, cascaded) = _fixture.CreateAbsoluteElement(style =>
        {
            style.Left.Set(50);
            style.Width.Set(200);
            style.Margin_Left.Set(10);
            style.Margin_Right.Set(10);
        }, containingBlockWidth: 400, containingBlockHeight: 300);

        CssValue Left = CssValue.From(50);
        CssValue MarginLeft = CssValue.From(10);
        CssValue Width = CssValue.From(200);
        CssValue MarginRight = CssValue.From(10);
        CssValue Right = CssValue.Auto;

        // Act
        CssUI.CSS.BoxModel.Calculate_Horizontal(box, cascaded,
            ref Left, ref MarginLeft, ref Width, ref MarginRight, ref Right);

        // Assert - right = 400 - 50 - 10 - 200 - 10 = 130
        Assert.Equal(130, Right.AsDecimal(), precision: 1);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Internal")]
    public void CalculateHorizontal_Absolute_AutoMargins_CentersElement()
    {
        // Arrange - Absolute with auto margins and left/right/width specified
        var (_, box, cascaded) = _fixture.CreateAbsoluteElement(style =>
        {
            style.Left.Set(0);
            style.Right.Set(0);
            style.Width.Set(200);
            style.Margin_Left.Set(CssValue.Auto);
            style.Margin_Right.Set(CssValue.Auto);
        }, containingBlockWidth: 400, containingBlockHeight: 300);

        CssValue Left = CssValue.From(0);
        CssValue MarginLeft = CssValue.Auto;
        CssValue Width = CssValue.From(200);
        CssValue MarginRight = CssValue.Auto;
        CssValue Right = CssValue.From(0);

        // Act
        CssUI.CSS.BoxModel.Calculate_Horizontal(box, cascaded,
            ref Left, ref MarginLeft, ref Width, ref MarginRight, ref Right);

        // Assert - margins split: (400 - 200) / 2 = 100 each
        Assert.Equal(100, MarginLeft.AsDecimal(), precision: 1);
        Assert.Equal(100, MarginRight.AsDecimal(), precision: 1);
    }

    #endregion

    #region Calculate_Vertical Tests - Block Elements

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Internal")]
    public void CalculateVertical_Block_FixedHeight_ReturnsSpecifiedValue()
    {
        // Arrange - Block with explicit height
        var (_, box, cascaded) = _fixture.CreateTestElement(style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Height.Set(200);
        }, containingBlockWidth: 400, containingBlockHeight: 600);

        CssValue Top = CssValue.Auto;
        CssValue MarginTop = CssValue.From(0);
        CssValue Height = CssValue.From(200);
        CssValue MarginBottom = CssValue.From(0);
        CssValue Bottom = CssValue.Auto;

        // Act
        CssUI.CSS.BoxModel.Calculate_Vertical(box, cascaded,
            ref Top, ref MarginTop, ref Height, ref MarginBottom, ref Bottom);

        // Assert
        Assert.Equal(200, Height.AsDecimal(), precision: 1);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Internal")]
    public void CalculateVertical_Block_AutoMargins_BecomeZero()
    {
        // Arrange - For blocks, auto vertical margins become 0 (no centering)
        var (_, box, cascaded) = _fixture.CreateTestElement(style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Height.Set(200);
            style.Margin_Top.Set(CssValue.Auto);
            style.Margin_Bottom.Set(CssValue.Auto);
        }, containingBlockWidth: 400, containingBlockHeight: 600);

        CssValue Top = CssValue.Auto;
        CssValue MarginTop = CssValue.Auto;
        CssValue Height = CssValue.From(200);
        CssValue MarginBottom = CssValue.Auto;
        CssValue Bottom = CssValue.Auto;

        // Act
        CssUI.CSS.BoxModel.Calculate_Vertical(box, cascaded,
            ref Top, ref MarginTop, ref Height, ref MarginBottom, ref Bottom);

        // Assert - auto vertical margins become 0 for non-absolute blocks
        Assert.Equal(0, MarginTop.AsDecimal(), precision: 1);
        Assert.Equal(0, MarginBottom.AsDecimal(), precision: 1);
    }

    #endregion

    #region Calculate_Vertical Tests - Absolutely Positioned

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Internal")]
    public void CalculateVertical_Absolute_TopBottomSpecified_HeightCalculated()
    {
        // Arrange - Absolute with top and bottom specified, height is calculated
        var (_, box, cascaded) = _fixture.CreateAbsoluteElement(style =>
        {
            style.Top.Set(50);
            style.Bottom.Set(50);
            style.Margin_Top.Set(10);
            style.Margin_Bottom.Set(10);
        }, containingBlockWidth: 400, containingBlockHeight: 600);

        CssValue Top = CssValue.From(50);
        CssValue MarginTop = CssValue.From(10);
        CssValue Height = CssValue.Auto;
        CssValue MarginBottom = CssValue.From(10);
        CssValue Bottom = CssValue.From(50);
        CssValue Width = CssValue.Auto;

        // Act
        CssUI.CSS.BoxModel.Calculate_Vertical(box, cascaded,
            ref Top, ref MarginTop, ref Height, ref MarginBottom, ref Bottom, Width);

        // Assert - height = 600 - 50 - 10 - 10 - 50 = 480
        Assert.Equal(480, Height.AsDecimal(), precision: 1);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Internal")]
    public void CalculateVertical_Absolute_HeightSpecified_BottomCalculated()
    {
        // Arrange - Absolute with height and top specified, bottom is calculated
        var (_, box, cascaded) = _fixture.CreateAbsoluteElement(style =>
        {
            style.Top.Set(50);
            style.Height.Set(200);
            style.Margin_Top.Set(10);
            style.Margin_Bottom.Set(10);
        }, containingBlockWidth: 400, containingBlockHeight: 600);

        CssValue Top = CssValue.From(50);
        CssValue MarginTop = CssValue.From(10);
        CssValue Height = CssValue.From(200);
        CssValue MarginBottom = CssValue.From(10);
        CssValue Bottom = CssValue.Auto;
        CssValue Width = CssValue.Auto;

        // Act
        CssUI.CSS.BoxModel.Calculate_Vertical(box, cascaded,
            ref Top, ref MarginTop, ref Height, ref MarginBottom, ref Bottom, Width);

        // Assert - bottom = 600 - 50 - 10 - 200 - 10 = 330
        Assert.Equal(330, Bottom.AsDecimal(), precision: 1);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Internal")]
    public void CalculateVertical_Absolute_AutoMargins_CentersElement()
    {
        // Arrange - Absolute with auto margins and top/bottom/height specified
        var (_, box, cascaded) = _fixture.CreateAbsoluteElement(style =>
        {
            style.Top.Set(0);
            style.Bottom.Set(0);
            style.Height.Set(200);
            style.Margin_Top.Set(CssValue.Auto);
            style.Margin_Bottom.Set(CssValue.Auto);
        }, containingBlockWidth: 400, containingBlockHeight: 600);

        CssValue Top = CssValue.From(0);
        CssValue MarginTop = CssValue.Auto;
        CssValue Height = CssValue.From(200);
        CssValue MarginBottom = CssValue.Auto;
        CssValue Bottom = CssValue.From(0);
        CssValue Width = CssValue.Auto;

        // Act
        CssUI.CSS.BoxModel.Calculate_Vertical(box, cascaded,
            ref Top, ref MarginTop, ref Height, ref MarginBottom, ref Bottom, Width);

        // Assert - margins split: (600 - 200) / 2 = 200 each
        Assert.Equal(200, MarginTop.AsDecimal(), precision: 1);
        Assert.Equal(200, MarginBottom.AsDecimal(), precision: 1);
    }

    #endregion

    #region Constrain_Width_Height Tests

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Internal")]
    public void ConstrainWidthHeight_WidthExceedsMax_ClampsToMax()
    {
        // Arrange - Width 300, max-width 200
        var (_, box, cascaded) = _fixture.CreateBlockElement(width: 300, height: 100, configureStyle: style =>
        {
            style.Min_Width.Set(CssValue.From_Dimension(0, ECssUnit.PX));
            style.Max_Width.Set(CssValue.From_Dimension(200, ECssUnit.PX));
            style.Min_Height.Set(CssValue.From_Dimension(0, ECssUnit.PX));
            style.Max_Height.Set(CssValue.From_Dimension(1000, ECssUnit.PX)); // Large max to not constrain
        });

        CssValue Width = CssValue.From(300);
        CssValue Height = CssValue.From(100);

        // Act
        bool changed = CssUI.CSS.BoxModel.Constrain_Width_Height(cascaded, ref Width, ref Height);

        // Assert
        Assert.True(changed);
        Assert.Equal(200, Width.AsDecimal(), precision: 1);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Internal")]
    public void ConstrainWidthHeight_WidthBelowMin_ClampsToMin()
    {
        // Arrange - Width 100, min-width 200
        var (_, box, cascaded) = _fixture.CreateBlockElement(width: 100, height: 100, configureStyle: style =>
        {
            style.Min_Width.Set(CssValue.From_Dimension(200, ECssUnit.PX));
            style.Max_Width.Set(CssValue.From_Dimension(1000, ECssUnit.PX)); // Large max to not constrain
            style.Min_Height.Set(CssValue.From_Dimension(0, ECssUnit.PX));
            style.Max_Height.Set(CssValue.From_Dimension(1000, ECssUnit.PX));
        });

        CssValue Width = CssValue.From(100);
        CssValue Height = CssValue.From(100);

        // Act
        bool changed = CssUI.CSS.BoxModel.Constrain_Width_Height(cascaded, ref Width, ref Height);

        // Assert
        Assert.True(changed);
        Assert.Equal(200, Width.AsDecimal(), precision: 1);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Internal")]
    public void ConstrainWidthHeight_HeightExceedsMax_ClampsToMax()
    {
        // Arrange - Height 300, max-height 200
        var (_, box, cascaded) = _fixture.CreateBlockElement(width: 100, height: 300, configureStyle: style =>
        {
            style.Min_Width.Set(CssValue.From_Dimension(0, ECssUnit.PX));
            style.Max_Width.Set(CssValue.From_Dimension(1000, ECssUnit.PX));
            style.Min_Height.Set(CssValue.From_Dimension(0, ECssUnit.PX));
            style.Max_Height.Set(CssValue.From_Dimension(200, ECssUnit.PX));
        });

        CssValue Width = CssValue.From(100);
        CssValue Height = CssValue.From(300);

        // Act
        bool changed = CssUI.CSS.BoxModel.Constrain_Width_Height(cascaded, ref Width, ref Height);

        // Assert
        Assert.True(changed);
        Assert.Equal(200, Height.AsDecimal(), precision: 1);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Internal")]
    public void ConstrainWidthHeight_HeightBelowMin_ClampsToMin()
    {
        // Arrange - Height 50, min-height 100
        var (_, box, cascaded) = _fixture.CreateBlockElement(width: 100, height: 50, configureStyle: style =>
        {
            style.Min_Width.Set(CssValue.From_Dimension(0, ECssUnit.PX));
            style.Max_Width.Set(CssValue.From_Dimension(1000, ECssUnit.PX));
            style.Min_Height.Set(CssValue.From_Dimension(100, ECssUnit.PX));
            style.Max_Height.Set(CssValue.From_Dimension(1000, ECssUnit.PX));
        });

        CssValue Width = CssValue.From(100);
        CssValue Height = CssValue.From(50);

        // Act
        bool changed = CssUI.CSS.BoxModel.Constrain_Width_Height(cascaded, ref Width, ref Height);

        // Assert
        Assert.True(changed);
        Assert.Equal(100, Height.AsDecimal(), precision: 1);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Internal")]
    public void ConstrainWidthHeight_NoConstraintViolation_ReturnsFalse()
    {
        // Arrange - Width and height within bounds
        var (_, box, cascaded) = _fixture.CreateBlockElement(width: 100, height: 100, configureStyle: style =>
        {
            style.Min_Width.Set(CssValue.From_Dimension(50, ECssUnit.PX));
            style.Max_Width.Set(CssValue.From_Dimension(200, ECssUnit.PX));
            style.Min_Height.Set(CssValue.From_Dimension(50, ECssUnit.PX));
            style.Max_Height.Set(CssValue.From_Dimension(200, ECssUnit.PX));
        });

        CssValue Width = CssValue.From(100);
        CssValue Height = CssValue.From(100);

        // Act
        bool changed = CssUI.CSS.BoxModel.Constrain_Width_Height(cascaded, ref Width, ref Height);

        // Assert
        Assert.False(changed);
        Assert.Equal(100, Width.AsDecimal(), precision: 1);
        Assert.Equal(100, Height.AsDecimal(), precision: 1);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Internal")]
    public void ConstrainWidthHeight_MinGreaterThanMax_MinWins()
    {
        // Arrange - Per CSS spec: min wins when min > max
        var (_, box, cascaded) = _fixture.CreateBlockElement(width: 150, height: 150, configureStyle: style =>
        {
            style.Min_Width.Set(CssValue.From_Dimension(200, ECssUnit.PX));
            style.Max_Width.Set(CssValue.From_Dimension(100, ECssUnit.PX));
            style.Min_Height.Set(CssValue.From_Dimension(0, ECssUnit.PX));
            style.Max_Height.Set(CssValue.From_Dimension(1000, ECssUnit.PX));
        });

        CssValue Width = CssValue.From(150);
        CssValue Height = CssValue.From(150);

        // Act
        bool changed = CssUI.CSS.BoxModel.Constrain_Width_Height(cascaded, ref Width, ref Height);

        // Assert - min-width (200) is used since min > max
        Assert.True(changed);
        Assert.Equal(200, Width.AsDecimal(), precision: 1);
    }

    #endregion

    #region Inline-Block Tests

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Internal")]
    public void CalculateHorizontal_InlineBlock_ExplicitWidth_UsesSpecifiedWidth()
    {
        // Arrange - Inline-block with explicit width
        var (_, box, cascaded) = _fixture.CreateInlineBlockElement(width: 150, height: 100);

        CssValue Left = CssValue.Auto;
        CssValue MarginLeft = CssValue.From(0);
        CssValue Width = CssValue.From(150);
        CssValue MarginRight = CssValue.From(0);
        CssValue Right = CssValue.Auto;

        // Act
        CssUI.CSS.BoxModel.Calculate_Horizontal(box, cascaded,
            ref Left, ref MarginLeft, ref Width, ref MarginRight, ref Right);

        // Assert - width stays at 150 (doesn't fill container)
        Assert.Equal(150, Width.AsDecimal(), precision: 1);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Internal")]
    public void CalculateHorizontal_InlineBlock_AutoMargins_BecomeZero()
    {
        // Arrange - Inline-block auto margins become 0 (unlike block centering)
        var (_, box, cascaded) = _fixture.CreateInlineBlockElement(width: 150, height: 100, configureStyle: style =>
        {
            style.Margin_Left.Set(CssValue.Auto);
            style.Margin_Right.Set(CssValue.Auto);
        });

        CssValue Left = CssValue.Auto;
        CssValue MarginLeft = CssValue.Auto;
        CssValue Width = CssValue.From(150);
        CssValue MarginRight = CssValue.Auto;
        CssValue Right = CssValue.Auto;

        // Act
        CssUI.CSS.BoxModel.Calculate_Horizontal(box, cascaded,
            ref Left, ref MarginLeft, ref Width, ref MarginRight, ref Right);

        // Assert - auto margins become 0 for inline-block
        Assert.Equal(0, MarginLeft.AsDecimal(), precision: 1);
        Assert.Equal(0, MarginRight.AsDecimal(), precision: 1);
    }

    #endregion

    #region Negative Margin Centering Tests (§10.3.7)

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Internal")]
    [Trait("Category", "Pending")]
    public void CalculateHorizontal_Absolute_NegativeCenteringMargins_LTR_MarginLeftBecomesZero()
    {
        // Arrange - Per CSS 2.1 §10.3.7: If centering auto margins would be negative,
        // in LTR the equation is solved by setting margin-left to zero and solving for margin-right.
        // Scenario: element wider than available space between left and right offsets
        var (_, box, cascaded) = _fixture.CreateAbsoluteElement(style =>
        {
            style.Positioning.Set(EBoxPositioning.Absolute);
            style.Direction.Set(EDirection.LTR);
            style.Left.Set(50);
            style.Right.Set(50);
            style.Width.Set(400); // Wider than 400-50-50=300 available
            style.Margin_Left.Set(CssValue.Auto);
            style.Margin_Right.Set(CssValue.Auto);
        }, containingBlockWidth: 400, containingBlockHeight: 300);

        CssValue Left = CssValue.From(50);
        CssValue MarginLeft = CssValue.Auto;
        CssValue Width = CssValue.From(400);
        CssValue MarginRight = CssValue.Auto;
        CssValue Right = CssValue.From(50);

        // Act
        CssUI.CSS.BoxModel.Calculate_Horizontal(box, cascaded,
            ref Left, ref MarginLeft, ref Width, ref MarginRight, ref Right);

        // Assert - margin-left becomes 0, margin-right absorbs the overflow (negative)
        // Per spec: "solve the equation under the extra constraint that margin-left = 0"
        // margin-right = 400 - 50 - 0 - 400 - 50 = -100
        Assert.Equal(0, MarginLeft.AsDecimal(), precision: 1);
        Assert.Equal(-100, MarginRight.AsDecimal(), precision: 1);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Internal")]
    [Trait("Category", "Pending")]
    public void CalculateHorizontal_Absolute_NegativeCenteringMargins_RTL_MarginRightBecomesZero()
    {
        // Arrange - Per CSS 2.1 §10.3.7: For RTL, if centering would be negative,
        // margin-right is set to zero and the equation is solved for margin-left.
        var (_, box, cascaded) = _fixture.CreateAbsoluteElement(style =>
        {
            style.Positioning.Set(EBoxPositioning.Absolute);
            style.Direction.Set(EDirection.RTL);
            style.Left.Set(50);
            style.Right.Set(50);
            style.Width.Set(400); // Wider than available
            style.Margin_Left.Set(CssValue.Auto);
            style.Margin_Right.Set(CssValue.Auto);
        }, containingBlockWidth: 400, containingBlockHeight: 300);

        CssValue Left = CssValue.From(50);
        CssValue MarginLeft = CssValue.Auto;
        CssValue Width = CssValue.From(400);
        CssValue MarginRight = CssValue.Auto;
        CssValue Right = CssValue.From(50);

        // Act
        CssUI.CSS.BoxModel.Calculate_Horizontal(box, cascaded,
            ref Left, ref MarginLeft, ref Width, ref MarginRight, ref Right);

        // Assert - margin-right becomes 0, margin-left absorbs the overflow (negative)
        // margin-left = 400 - 50 - 400 - 0 - 50 = -100
        Assert.Equal(-100, MarginLeft.AsDecimal(), precision: 1);
        Assert.Equal(0, MarginRight.AsDecimal(), precision: 1);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Internal")]
    [Trait("Category", "Pending")]
    public void CalculateVertical_Absolute_NegativeCenteringMargins_MarginTopBecomesZero()
    {
        // Arrange - Per CSS 2.1 §10.6.4: If centering auto margins would be negative,
        // set margin-top to zero and solve for margin-bottom.
        var (_, box, cascaded) = _fixture.CreateAbsoluteElement(style =>
        {
            style.Positioning.Set(EBoxPositioning.Absolute);
            style.Top.Set(50);
            style.Bottom.Set(50);
            style.Height.Set(600); // Taller than 600-50-50=500 available
            style.Margin_Top.Set(CssValue.Auto);
            style.Margin_Bottom.Set(CssValue.Auto);
        }, containingBlockWidth: 400, containingBlockHeight: 600);

        CssValue Top = CssValue.From(50);
        CssValue MarginTop = CssValue.Auto;
        CssValue Height = CssValue.From(600);
        CssValue MarginBottom = CssValue.Auto;
        CssValue Bottom = CssValue.From(50);
        CssValue Width = CssValue.Auto;

        // Act
        CssUI.CSS.BoxModel.Calculate_Vertical(box, cascaded,
            ref Top, ref MarginTop, ref Height, ref MarginBottom, ref Bottom, Width);

        // Assert - margin-top becomes 0, margin-bottom absorbs the overflow (negative)
        // margin-bottom = 600 - 50 - 0 - 600 - 50 = -100
        Assert.Equal(0, MarginTop.AsDecimal(), precision: 1);
        Assert.Equal(-100, MarginBottom.AsDecimal(), precision: 1);
    }

    #endregion

    #region Percentage Height with Auto Containing Block (§10.5)

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Internal")]
    [Trait("Category", "Pending")]
    public void ResolveVertical_PercentageHeight_AutoContainingBlockHeight_BecomesAuto()
    {
        // Arrange - Per CSS 2.1 §10.5: "If the height of the containing block is not 
        // specified explicitly (i.e., it depends on content height), and this element 
        // is not absolutely positioned, the percentage value is treated as 'auto'."
        // 
        // When the containing block has auto height, percentage heights cannot be resolved.
        var (_, box, cascaded) = _fixture.CreateTestElement(style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Height.Set(CssValue.From_Percent(50)); // 50% of auto = auto
        }, containingBlockWidth: 400, containingBlockHeight: 0); // 0 or auto-height containing block

        // Act
        CssUI.CSS.BoxModel.Resolve_Vertical(box, cascaded,
            out CssValue outTop, out CssValue outMarginTop, out CssValue outHeight,
            out CssValue outMarginBottom, out CssValue outBottom);

        // Assert - percentage height with auto/zero containing block height should become auto
        Assert.True(outHeight.IsAuto, "Percentage height with auto containing block height should resolve to auto");
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Internal")]
    [Trait("Category", "Pending")]
    public void ResolveVertical_PercentageHeight_AbsoluteWithAutoContainingBlock_StillResolves()
    {
        // Arrange - Per CSS 2.1 §10.5: Absolutely positioned elements CAN use percentage 
        // heights even when the containing block height depends on content, because the 
        // containing block is the padding box of the positioned ancestor.
        var (_, box, cascaded) = _fixture.CreateAbsoluteElement(style =>
        {
            style.Positioning.Set(EBoxPositioning.Absolute);
            style.Height.Set(CssValue.From_Percent(50));
            style.Top.Set(0);
            style.Bottom.Set(CssValue.Auto);
        }, containingBlockWidth: 400, containingBlockHeight: 600);

        // Act
        CssUI.CSS.BoxModel.Resolve_Vertical(box, cascaded,
            out CssValue outTop, out CssValue outMarginTop, out CssValue outHeight,
            out CssValue outMarginBottom, out CssValue outBottom);

        // Assert - absolute elements can resolve percentage heights: 50% of 600 = 300
        Assert.Equal(300, outHeight.AsDecimal(), precision: 1);
    }

    #endregion

    #region Replaced Element Intrinsic Ratio Tests (§10.4)

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Internal")]
    [Trait("Category", "Pending")]
    public void ConstrainWidthHeight_ReplacedElement_IntrinsicRatio_MaxWidthAffectsHeight()
    {
        // Arrange - Per CSS 2.1 §10.4 constraint table for replaced elements:
        // When a replaced element has an intrinsic ratio and max-width constrains the width,
        // height should be scaled proportionally to maintain the intrinsic ratio.
        // 
        // Example: Image with intrinsic 400x200 (2:1 ratio), max-width: 200
        // Result should be width: 200, height: 100 (maintaining 2:1 ratio)
        var (_, box, cascaded) = _fixture.CreateReplacedElement(
            intrinsicWidth: 400, 
            intrinsicHeight: 200, 
            configureStyle: style =>
            {
                style.Max_Width.Set(CssValue.From_Dimension(200, ECssUnit.PX));
                // Height is auto, should scale with intrinsic ratio
            });

        CssValue Width = CssValue.From(400);  // Intrinsic width
        CssValue Height = CssValue.From(200); // Intrinsic height

        // Act
        bool changed = CssUI.CSS.BoxModel.Constrain_Width_Height(cascaded, ref Width, ref Height);

        // Assert - width constrained to 200, height scaled to maintain 2:1 ratio
        Assert.True(changed);
        Assert.Equal(200, Width.AsDecimal(), precision: 1);
        Assert.Equal(100, Height.AsDecimal(), precision: 1); // Scaled proportionally
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Internal")]
    [Trait("Category", "Pending")]
    public void ConstrainWidthHeight_ReplacedElement_IntrinsicRatio_MaxHeightAffectsWidth()
    {
        // Arrange - Per CSS 2.1 §10.4: When max-height constrains height,
        // width should scale proportionally for replaced elements with intrinsic ratio.
        // 
        // Example: Image with intrinsic 400x200 (2:1 ratio), max-height: 50
        // Result should be width: 100, height: 50 (maintaining 2:1 ratio)
        var (_, box, cascaded) = _fixture.CreateReplacedElement(
            intrinsicWidth: 400, 
            intrinsicHeight: 200, 
            configureStyle: style =>
            {
                style.Max_Height.Set(CssValue.From_Dimension(50, ECssUnit.PX));
                // Width is auto, should scale with intrinsic ratio
            });

        CssValue Width = CssValue.From(400);  // Intrinsic width
        CssValue Height = CssValue.From(200); // Intrinsic height

        // Act
        bool changed = CssUI.CSS.BoxModel.Constrain_Width_Height(cascaded, ref Width, ref Height);

        // Assert - height constrained to 50, width scaled to maintain 2:1 ratio
        Assert.True(changed);
        Assert.Equal(100, Width.AsDecimal(), precision: 1); // Scaled proportionally
        Assert.Equal(50, Height.AsDecimal(), precision: 1);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Internal")]
    [Trait("Category", "Pending")]
    public void ConstrainWidthHeight_ReplacedElement_IntrinsicRatio_BothConstraintsApplied()
    {
        // Arrange - Per CSS 2.1 §10.4 constraint table:
        // When both max-width and max-height apply, use the one that results in 
        // the smaller dimensions while maintaining the intrinsic ratio.
        // 
        // Example: Image 400x200 (2:1), max-width: 300, max-height: 100
        // max-width alone → 300x150 (but 150 > max-height 100)
        // max-height alone → 200x100 (200 < max-width 300) ✓
        // Result: 200x100
        var (_, box, cascaded) = _fixture.CreateReplacedElement(
            intrinsicWidth: 400, 
            intrinsicHeight: 200, 
            configureStyle: style =>
            {
                style.Max_Width.Set(CssValue.From_Dimension(300, ECssUnit.PX));
                style.Max_Height.Set(CssValue.From_Dimension(100, ECssUnit.PX));
            });

        CssValue Width = CssValue.From(400);  // Intrinsic width
        CssValue Height = CssValue.From(200); // Intrinsic height

        // Act
        bool changed = CssUI.CSS.BoxModel.Constrain_Width_Height(cascaded, ref Width, ref Height);

        // Assert - the tighter constraint wins while maintaining ratio
        Assert.True(changed);
        Assert.Equal(200, Width.AsDecimal(), precision: 1);
        Assert.Equal(100, Height.AsDecimal(), precision: 1);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Internal")]
    [Trait("Category", "Pending")]
    public void ConstrainWidthHeight_ReplacedElement_IntrinsicRatio_MinWidthScalesHeightUp()
    {
        // Arrange - Per CSS 2.1 §10.4: When min-width causes width to increase,
        // height should scale up proportionally for replaced elements.
        // 
        // Example: Image 100x50 (2:1 ratio), min-width: 200
        // Result: 200x100
        var (_, box, cascaded) = _fixture.CreateReplacedElement(
            intrinsicWidth: 100, 
            intrinsicHeight: 50, 
            configureStyle: style =>
            {
                style.Min_Width.Set(CssValue.From_Dimension(200, ECssUnit.PX));
            });

        CssValue Width = CssValue.From(100);  // Intrinsic width
        CssValue Height = CssValue.From(50);  // Intrinsic height

        // Act
        bool changed = CssUI.CSS.BoxModel.Constrain_Width_Height(cascaded, ref Width, ref Height);

        // Assert - width increased to 200, height scaled proportionally to 100
        Assert.True(changed);
        Assert.Equal(200, Width.AsDecimal(), precision: 1);
        Assert.Equal(100, Height.AsDecimal(), precision: 1);
    }

    #endregion
}
