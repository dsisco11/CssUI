using System;
using CssUI;
using CssUI.CSS;
using CssUI.CSS.BoxTree;
using CssUI.CSS.Enums;
using CssUI.CSS.Internal;
using CssUI.DOM;
using CssUITests.Fixtures;
using Xunit;

namespace CssUITests.CSS.BoxModel;

/// <summary>
/// Tests for <see cref="CssUI.CSS.BoxModel"/> static class.
/// Covers width/height resolution for various display types per CSS 2.1 §10.
/// </summary>
public class BoxModelTests : IDisposable
{
    private readonly LayoutTestFixture _fixture;

    public BoxModelTests()
    {
        _fixture = new LayoutTestFixture();
    }

    public void Dispose()
    {
        _fixture.Dispose();
    }

    #region ResolveWidth Tests

    #region Block-level, non-replaced elements (CSS 2.1 §10.3.3)

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Block")]
    public void ResolveWidth_Block_ExplicitWidth_UsesSpecifiedValue()
    {
        // Arrange
        var element = _fixture.CreateBlock(200, 100);
        _fixture.ForceLayoutUpdate();

        var box = element.Box;
        var cascaded = element.Style?.Cascaded;

        Assert.NotNull(box);
        Assert.NotNull(cascaded);

        // Act
        CssUI.CSS.BoxModel.ResolveWidth(box, cascaded);

        // Assert
        Assert.Equal(200, cascaded.Width.Computed.AsDecimal(), precision: 1);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Block")]
    public void ResolveWidth_Block_AutoWidth_FillsContainingBlock()
    {
        // Arrange - Container 400px, child auto width
        var container = _fixture.CreateContainingBlock(400, 300);
        var child = _fixture.CreateChild(container, "div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            // Width defaults to auto
        });

        _fixture.ForceLayoutUpdate();

        var containerBox = container.Box;
        var childBox = child.Box;
        var containerCascaded = container.Style?.Cascaded;
        var childCascaded = child.Style?.Cascaded;

        Assert.NotNull(containerBox);
        Assert.NotNull(childBox);
        Assert.NotNull(containerCascaded);
        Assert.NotNull(childCascaded);

        // Act - resolve container first, then child
        CssUI.CSS.BoxModel.ResolveWidth(containerBox, containerCascaded);
        CssUI.CSS.BoxModel.ResolveWidth(childBox, childCascaded);

        // Assert - auto width should fill the 400px container
        Assert.Equal(400, childCascaded.Width.Computed.AsDecimal(), precision: 1);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Block")]
    public void ResolveWidth_Block_AutoMargins_CentersElement()
    {
        // Arrange - Container 400px, child 200px with auto margins
        var container = _fixture.CreateContainingBlock(400, 300);
        var child = _fixture.CreateChild(container, "div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Width.Set(200);
            style.Margin_Left.Set(CssValue.Auto);
            style.Margin_Right.Set(CssValue.Auto);
        });

        _fixture.ForceLayoutUpdate();

        var containerBox = container.Box;
        var childBox = child.Box;
        var containerCascaded = container.Style?.Cascaded;
        var childCascaded = child.Style?.Cascaded;

        Assert.NotNull(containerBox);
        Assert.NotNull(childBox);
        Assert.NotNull(containerCascaded);
        Assert.NotNull(childCascaded);

        // Act
        CssUI.CSS.BoxModel.ResolveWidth(containerBox, containerCascaded);
        CssUI.CSS.BoxModel.ResolveWidth(childBox, childCascaded);

        // Assert - margins should be 100px each ((400 - 200) / 2)
        Assert.Equal(100, childCascaded.Margin_Left.Computed.AsDecimal(), precision: 1);
        Assert.Equal(100, childCascaded.Margin_Right.Computed.AsDecimal(), precision: 1);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Block")]
    public void ResolveWidth_Block_SingleAutoMarginLeft_FillsRemainingSpace()
    {
        // Arrange - Container 400px, child 200px with auto left margin
        var container = _fixture.CreateContainingBlock(400, 300);
        var child = _fixture.CreateChild(container, "div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Width.Set(200);
            style.Margin_Left.Set(CssValue.Auto);
            style.Margin_Right.Set(50);
        });

        _fixture.ForceLayoutUpdate();

        var containerBox = container.Box;
        var childBox = child.Box;
        var containerCascaded = container.Style?.Cascaded;
        var childCascaded = child.Style?.Cascaded;

        Assert.NotNull(containerBox);
        Assert.NotNull(childBox);
        Assert.NotNull(containerCascaded);
        Assert.NotNull(childCascaded);

        // Act
        CssUI.CSS.BoxModel.ResolveWidth(containerBox, containerCascaded);
        CssUI.CSS.BoxModel.ResolveWidth(childBox, childCascaded);

        // Assert - left margin fills remaining: 400 - 200 - 50 = 150
        Assert.Equal(150, childCascaded.Margin_Left.Computed.AsDecimal(), precision: 1);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Block")]
    public void ResolveWidth_Block_OverConstrained_AdjustsMarginRight()
    {
        // Arrange - Container 400px, child 300px with fixed margins that exceed container
        var container = _fixture.CreateContainingBlock(400, 300);
        var child = _fixture.CreateChild(container, "div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Direction.Set(EDirection.LTR);
            style.Width.Set(300);
            style.Margin_Left.Set(50);
            style.Margin_Right.Set(100); // Total: 300 + 50 + 100 = 450 > 400
        });

        _fixture.ForceLayoutUpdate();

        var containerBox = container.Box;
        var childBox = child.Box;
        var containerCascaded = container.Style?.Cascaded;
        var childCascaded = child.Style?.Cascaded;

        Assert.NotNull(containerBox);
        Assert.NotNull(childBox);
        Assert.NotNull(containerCascaded);
        Assert.NotNull(childCascaded);

        // Act
        CssUI.CSS.BoxModel.ResolveWidth(containerBox, containerCascaded);
        CssUI.CSS.BoxModel.ResolveWidth(childBox, childCascaded);

        // Assert - For LTR, margin-right is adjusted: 400 - 300 - 50 = 50
        Assert.Equal(50, childCascaded.Margin_Right.Computed.AsDecimal(), precision: 1);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Block")]
    public void ResolveWidth_Block_WithPaddingAndBorder_ReducesContentWidth()
    {
        // Arrange - Container 400px, child auto width with padding/border
        var container = _fixture.CreateContainingBlock(400, 300);
        var child = _fixture.CreateChild(container, "div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Padding_Left.Set(20);
            style.Padding_Right.Set(20);
            style.Border_Left_Width.Set(5);
            style.Border_Right_Width.Set(5);
            // Width defaults to auto
        });

        _fixture.ForceLayoutUpdate();

        var containerBox = container.Box;
        var childBox = child.Box;
        var containerCascaded = container.Style?.Cascaded;
        var childCascaded = child.Style?.Cascaded;

        Assert.NotNull(containerBox);
        Assert.NotNull(childBox);
        Assert.NotNull(containerCascaded);
        Assert.NotNull(childCascaded);

        // Act
        CssUI.CSS.BoxModel.ResolveWidth(containerBox, containerCascaded);
        CssUI.CSS.BoxModel.ResolveWidth(childBox, childCascaded);

        // Assert - Content width: 400 - 20 - 20 - 5 - 5 = 350
        Assert.Equal(350, childCascaded.Width.Computed.AsDecimal(), precision: 1);
    }

    #endregion

    #region Inline-block elements (CSS 2.1 §10.3.9)

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "InlineBlock")]
    public void ResolveWidth_InlineBlock_ExplicitWidth_UsesSpecifiedValue()
    {
        // Arrange
        var element = _fixture.CreateInlineBlock(150, 80);
        _fixture.ForceLayoutUpdate();

        var box = element.Box;
        var cascaded = element.Style?.Cascaded;

        Assert.NotNull(box);
        Assert.NotNull(cascaded);

        // Act
        CssUI.CSS.BoxModel.ResolveWidth(box, cascaded);

        // Assert
        Assert.Equal(150, cascaded.Width.Computed.AsDecimal(), precision: 1);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "InlineBlock")]
    public void ResolveWidth_InlineBlock_AutoMargins_BecomesZero()
    {
        // Arrange
        var element = _fixture.CreateElement("span", style =>
        {
            style.Display.Set(EDisplayMode.INLINE_BLOCK);
            style.Width.Set(100);
            style.Height.Set(50);
            style.Margin_Left.Set(CssValue.Auto);
            style.Margin_Right.Set(CssValue.Auto);
        });
        _fixture.ForceLayoutUpdate();

        var box = element.Box;
        var cascaded = element.Style?.Cascaded;

        Assert.NotNull(box);
        Assert.NotNull(cascaded);

        // Act
        CssUI.CSS.BoxModel.ResolveWidth(box, cascaded);

        // Assert - Auto margins on inline-block become 0
        Assert.Equal(0, cascaded.Margin_Left.Computed.AsDecimal(), precision: 1);
        Assert.Equal(0, cascaded.Margin_Right.Computed.AsDecimal(), precision: 1);
    }

    #endregion

    #region Absolutely positioned elements (CSS 2.1 §10.3.7)

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "AbsolutelyPositioned")]
    public void ResolveWidth_Absolute_ExplicitWidth_UsesSpecifiedValue()
    {
        // Arrange
        var container = _fixture.CreateContainingBlock(400, 300, EBoxPositioning.Relative);
        var absolute = _fixture.CreateAbsolutelyPositioned(container, style =>
        {
            style.Width.Set(200);
            style.Height.Set(100);
        });

        _fixture.ForceLayoutUpdate();

        var containerBox = container.Box;
        var absoluteBox = absolute.Box;
        var containerCascaded = container.Style?.Cascaded;
        var absoluteCascaded = absolute.Style?.Cascaded;

        Assert.NotNull(containerBox);
        Assert.NotNull(absoluteBox);
        Assert.NotNull(containerCascaded);
        Assert.NotNull(absoluteCascaded);

        // Act
        CssUI.CSS.BoxModel.ResolveWidth(containerBox, containerCascaded);
        CssUI.CSS.BoxModel.ResolveWidth(absoluteBox, absoluteCascaded);

        // Assert
        Assert.Equal(200, absoluteCascaded.Width.Computed.AsDecimal(), precision: 1);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "AbsolutelyPositioned")]
    public void ResolveWidth_Absolute_LeftRightNotAuto_CalculatesWidth()
    {
        // Arrange - Container 400px, absolute with left:50 right:50 -> width should be 300
        var container = _fixture.CreateContainingBlock(400, 300, EBoxPositioning.Relative);
        var absolute = _fixture.CreateAbsolutelyPositioned(container, style =>
        {
            style.Left.Set(50);
            style.Right.Set(50);
            // Width is auto
        });

        _fixture.ForceLayoutUpdate();

        var containerBox = container.Box;
        var absoluteBox = absolute.Box;
        var containerCascaded = container.Style?.Cascaded;
        var absoluteCascaded = absolute.Style?.Cascaded;

        Assert.NotNull(containerBox);
        Assert.NotNull(absoluteBox);
        Assert.NotNull(containerCascaded);
        Assert.NotNull(absoluteCascaded);

        // Act
        CssUI.CSS.BoxModel.ResolveWidth(containerBox, containerCascaded);
        CssUI.CSS.BoxModel.ResolveWidth(absoluteBox, absoluteCascaded);

        // Assert - Width should be: 400 - 50 - 50 = 300
        Assert.Equal(300, absoluteCascaded.Width.Computed.AsDecimal(), precision: 1);
    }

    #endregion

    #region Min/Max Width Constraints

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "MinMax")]
    public void ResolveWidth_MaxWidth_ClampsToMaximum()
    {
        // Arrange - Container 400px, child auto but max-width 200
        var container = _fixture.CreateContainingBlock(400, 300);
        var child = _fixture.CreateChild(container, "div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Max_Width.Set(CssValue.From(200, ECssUnit.PX));
            // Width defaults to auto (would fill 400)
        });

        _fixture.ForceLayoutUpdate();

        var containerBox = container.Box;
        var childBox = child.Box;
        var containerCascaded = container.Style?.Cascaded;
        var childCascaded = child.Style?.Cascaded;

        Assert.NotNull(containerBox);
        Assert.NotNull(childBox);
        Assert.NotNull(containerCascaded);
        Assert.NotNull(childCascaded);

        // Act
        CssUI.CSS.BoxModel.ResolveWidth(containerBox, containerCascaded);
        CssUI.CSS.BoxModel.ResolveWidth(childBox, childCascaded);

        // Assert - Should be clamped to max-width 200
        Assert.Equal(200, childCascaded.Width.Computed.AsDecimal(), precision: 1);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "MinMax")]
    public void ResolveWidth_MinWidth_ClampsToMinimum()
    {
        // Arrange - Container 400px, child 50px width but min-width 100
        var container = _fixture.CreateContainingBlock(400, 300);
        var child = _fixture.CreateChild(container, "div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Width.Set(50);
            style.Min_Width.Set(100);
        });

        _fixture.ForceLayoutUpdate();

        var containerBox = container.Box;
        var childBox = child.Box;
        var containerCascaded = container.Style?.Cascaded;
        var childCascaded = child.Style?.Cascaded;

        Assert.NotNull(containerBox);
        Assert.NotNull(childBox);
        Assert.NotNull(containerCascaded);
        Assert.NotNull(childCascaded);

        // Act
        CssUI.CSS.BoxModel.ResolveWidth(containerBox, containerCascaded);
        CssUI.CSS.BoxModel.ResolveWidth(childBox, childCascaded);

        // Assert - Should be clamped to min-width 100
        Assert.Equal(100, childCascaded.Width.Computed.AsDecimal(), precision: 1);
    }

    #endregion

    #endregion

    #region ResolveHeight Tests

    #region Block-level, non-replaced elements (CSS 2.1 §10.6.3)

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Block")]
    public void ResolveHeight_Block_ExplicitHeight_UsesSpecifiedValue()
    {
        // Arrange
        var element = _fixture.CreateBlock(200, 150);
        _fixture.ForceLayoutUpdate();

        var box = element.Box;
        var cascaded = element.Style?.Cascaded;

        Assert.NotNull(box);
        Assert.NotNull(cascaded);

        // Act
        CssUI.CSS.BoxModel.ResolveHeight(box, cascaded);

        // Assert
        Assert.Equal(150, cascaded.Height.Computed.AsDecimal(), precision: 1);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Block")]
    public void ResolveHeight_Block_AutoHeight_WithContentHeight_UsesContentHeight()
    {
        // Arrange
        var element = _fixture.CreateElement("div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Width.Set(200);
            // Height is auto
        });
        _fixture.ForceLayoutUpdate();

        var box = element.Box;
        var cascaded = element.Style?.Cascaded;

        Assert.NotNull(box);
        Assert.NotNull(cascaded);

        // Act - Pass content height from Flow()
        CssUI.CSS.BoxModel.ResolveHeight(box, cascaded, contentHeight: 250.0);

        // Assert - Should use the provided content height
        Assert.Equal(250, cascaded.Height.Computed.AsDecimal(), precision: 1);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Block")]
    public void ResolveHeight_Block_AutoMargins_BecomesZero()
    {
        // Arrange - Auto vertical margins on blocks become 0
        var element = _fixture.CreateElement("div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Width.Set(200);
            style.Height.Set(100);
            style.Margin_Top.Set(CssValue.Auto);
            style.Margin_Bottom.Set(CssValue.Auto);
        });
        _fixture.ForceLayoutUpdate();

        var box = element.Box;
        var cascaded = element.Style?.Cascaded;

        Assert.NotNull(box);
        Assert.NotNull(cascaded);

        // Act
        CssUI.CSS.BoxModel.ResolveHeight(box, cascaded);

        // Assert - Auto vertical margins become 0 for blocks
        Assert.Equal(0, cascaded.Margin_Top.Computed.AsDecimal(), precision: 1);
        Assert.Equal(0, cascaded.Margin_Bottom.Computed.AsDecimal(), precision: 1);
    }

    #endregion

    #region Absolutely positioned elements (CSS 2.1 §10.6.4)

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "AbsolutelyPositioned")]
    public void ResolveHeight_Absolute_TopBottomNotAuto_CalculatesHeight()
    {
        // Arrange - Container 300px, absolute with top:50 bottom:50 -> height should be 200
        var container = _fixture.CreateContainingBlock(400, 300, EBoxPositioning.Relative);
        var absolute = _fixture.CreateAbsolutelyPositioned(container, style =>
        {
            style.Top.Set(50);
            style.Bottom.Set(50);
            style.Width.Set(100);
            // Height is auto
        });

        _fixture.ForceLayoutUpdate();

        var containerBox = container.Box;
        var absoluteBox = absolute.Box;
        var containerCascaded = container.Style?.Cascaded;
        var absoluteCascaded = absolute.Style?.Cascaded;

        Assert.NotNull(containerBox);
        Assert.NotNull(absoluteBox);
        Assert.NotNull(containerCascaded);
        Assert.NotNull(absoluteCascaded);

        // Need to resolve width first, then height
        CssUI.CSS.BoxModel.ResolveWidth(containerBox, containerCascaded);
        CssUI.CSS.BoxModel.ResolveWidth(absoluteBox, absoluteCascaded);

        // Act
        CssUI.CSS.BoxModel.ResolveHeight(containerBox, containerCascaded);
        CssUI.CSS.BoxModel.ResolveHeight(absoluteBox, absoluteCascaded);

        // Assert - Height should be: 300 - 50 - 50 = 200
        Assert.Equal(200, absoluteCascaded.Height.Computed.AsDecimal(), precision: 1);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "AbsolutelyPositioned")]
    public void ResolveHeight_Absolute_AutoMarginsWithConstraints_CentersVertically()
    {
        // Arrange - Absolute positioned with top/bottom set and auto margins
        var container = _fixture.CreateContainingBlock(400, 300, EBoxPositioning.Relative);
        var absolute = _fixture.CreateAbsolutelyPositioned(container, style =>
        {
            style.Top.Set(0);
            style.Bottom.Set(0);
            style.Height.Set(100);
            style.Margin_Top.Set(CssValue.Auto);
            style.Margin_Bottom.Set(CssValue.Auto);
        });

        _fixture.ForceLayoutUpdate();

        var containerBox = container.Box;
        var absoluteBox = absolute.Box;
        var containerCascaded = container.Style?.Cascaded;
        var absoluteCascaded = absolute.Style?.Cascaded;

        Assert.NotNull(containerBox);
        Assert.NotNull(absoluteBox);
        Assert.NotNull(containerCascaded);
        Assert.NotNull(absoluteCascaded);

        CssUI.CSS.BoxModel.ResolveWidth(containerBox, containerCascaded);
        CssUI.CSS.BoxModel.ResolveWidth(absoluteBox, absoluteCascaded);

        // Act
        CssUI.CSS.BoxModel.ResolveHeight(containerBox, containerCascaded);
        CssUI.CSS.BoxModel.ResolveHeight(absoluteBox, absoluteCascaded);

        // Assert - Margins split remaining space: (300 - 100) / 2 = 100 each
        Assert.Equal(100, absoluteCascaded.Margin_Top.Computed.AsDecimal(), precision: 1);
        Assert.Equal(100, absoluteCascaded.Margin_Bottom.Computed.AsDecimal(), precision: 1);
    }

    #endregion

    #region Min/Max Height Constraints

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "MinMax")]
    public void ResolveHeight_MaxHeight_ClampsToMaximum()
    {
        // Arrange
        var element = _fixture.CreateElement("div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Width.Set(200);
            style.Height.Set(300);
            style.Max_Height.Set(CssValue.From(150, ECssUnit.PX));
        });
        _fixture.ForceLayoutUpdate();

        var box = element.Box;
        var cascaded = element.Style?.Cascaded;

        Assert.NotNull(box);
        Assert.NotNull(cascaded);

        // Act
        CssUI.CSS.BoxModel.ResolveHeight(box, cascaded);

        // Assert - Should be clamped to max-height 150
        Assert.Equal(150, cascaded.Height.Computed.AsDecimal(), precision: 1);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "MinMax")]
    public void ResolveHeight_MinHeight_ClampsToMinimum()
    {
        // Arrange
        var element = _fixture.CreateElement("div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Width.Set(200);
            style.Height.Set(50);
            style.Min_Height.Set(100);
        });
        _fixture.ForceLayoutUpdate();

        var box = element.Box;
        var cascaded = element.Style?.Cascaded;

        Assert.NotNull(box);
        Assert.NotNull(cascaded);

        // Act
        CssUI.CSS.BoxModel.ResolveHeight(box, cascaded);

        // Assert - Should be clamped to min-height 100
        Assert.Equal(100, cascaded.Height.Computed.AsDecimal(), precision: 1);
    }

    #endregion

    #endregion

    #region Resolve Tests (Combined Width/Height)

    [Fact]
    [Trait("Category", "BoxModel")]
    public void Resolve_Block_BothDimensions_ResolvesCorrectly()
    {
        // Arrange
        var element = _fixture.CreateBlock(250, 180);
        _fixture.ForceLayoutUpdate();

        var box = element.Box;
        var cascaded = element.Style?.Cascaded;

        Assert.NotNull(box);
        Assert.NotNull(cascaded);

        // Act
        CssUI.CSS.BoxModel.Resolve(box, cascaded);

        // Assert
        Assert.Equal(250, cascaded.Width.Computed.AsDecimal(), precision: 1);
        Assert.Equal(180, cascaded.Height.Computed.AsDecimal(), precision: 1);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    public void Resolve_Block_WithAllMargins_SetsAllValues()
    {
        // Arrange
        var element = _fixture.CreateElement("div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Width.Set(200);
            style.Height.Set(100);
            style.Margin_Top.Set(10);
            style.Margin_Right.Set(20);
            style.Margin_Bottom.Set(30);
            style.Margin_Left.Set(40);
        });
        _fixture.ForceLayoutUpdate();

        var box = element.Box;
        var cascaded = element.Style?.Cascaded;

        Assert.NotNull(box);
        Assert.NotNull(cascaded);

        // Act
        CssUI.CSS.BoxModel.Resolve(box, cascaded);

        // Assert - All margins should be set
        Assert.Equal(10, cascaded.Margin_Top.Computed.AsDecimal(), precision: 1);
        Assert.Equal(20, cascaded.Margin_Right.Computed.AsDecimal(), precision: 1);
        Assert.Equal(30, cascaded.Margin_Bottom.Computed.AsDecimal(), precision: 1);
        Assert.Equal(40, cascaded.Margin_Left.Computed.AsDecimal(), precision: 1);
    }

    #endregion

    #region Null Parameter Tests

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Validation")]
    public void ResolveWidth_NullBox_ThrowsArgumentNullException()
    {
        // Arrange
        var element = _fixture.CreateBlock(100, 100);
        _fixture.ForceLayoutUpdate();
        var cascaded = element.Style?.Cascaded;
        Assert.NotNull(cascaded);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => CssUI.CSS.BoxModel.ResolveWidth(null!, cascaded));
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Validation")]
    public void ResolveWidth_NullCascaded_ThrowsArgumentNullException()
    {
        // Arrange
        var element = _fixture.CreateBlock(100, 100);
        _fixture.ForceLayoutUpdate();
        var box = element.Box;
        Assert.NotNull(box);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => CssUI.CSS.BoxModel.ResolveWidth(box, null!));
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Validation")]
    public void ResolveHeight_NullBox_ThrowsArgumentNullException()
    {
        // Arrange
        var element = _fixture.CreateBlock(100, 100);
        _fixture.ForceLayoutUpdate();
        var cascaded = element.Style?.Cascaded;
        Assert.NotNull(cascaded);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => CssUI.CSS.BoxModel.ResolveHeight(null!, cascaded));
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Validation")]
    public void ResolveHeight_NullCascaded_ThrowsArgumentNullException()
    {
        // Arrange
        var element = _fixture.CreateBlock(100, 100);
        _fixture.ForceLayoutUpdate();
        var box = element.Box;
        Assert.NotNull(box);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => CssUI.CSS.BoxModel.ResolveHeight(box, null!));
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Validation")]
    public void Resolve_NullBox_ThrowsArgumentNullException()
    {
        // Arrange
        var element = _fixture.CreateBlock(100, 100);
        _fixture.ForceLayoutUpdate();
        var cascaded = element.Style?.Cascaded;
        Assert.NotNull(cascaded);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => CssUI.CSS.BoxModel.Resolve(null!, cascaded));
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Validation")]
    public void Resolve_NullCascaded_ThrowsArgumentNullException()
    {
        // Arrange
        var element = _fixture.CreateBlock(100, 100);
        _fixture.ForceLayoutUpdate();
        var box = element.Box;
        Assert.NotNull(box);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => CssUI.CSS.BoxModel.Resolve(box, null!));
    }

    #endregion

    #region Direction Tests (RTL/LTR)

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Direction")]
    public void ResolveWidth_RTL_OverConstrained_AdjustsMarginLeft()
    {
        // Arrange - Container 400px, child 300px with fixed margins in RTL
        var container = _fixture.CreateContainingBlock(400, 300);
        var child = _fixture.CreateChild(container, "div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Direction.Set(EDirection.RTL);
            style.Width.Set(300);
            style.Margin_Left.Set(50);
            style.Margin_Right.Set(100); // Total: 300 + 50 + 100 = 450 > 400
        });

        _fixture.ForceLayoutUpdate();

        var containerBox = container.Box;
        var childBox = child.Box;
        var containerCascaded = container.Style?.Cascaded;
        var childCascaded = child.Style?.Cascaded;

        Assert.NotNull(containerBox);
        Assert.NotNull(childBox);
        Assert.NotNull(containerCascaded);
        Assert.NotNull(childCascaded);

        // Act
        CssUI.CSS.BoxModel.ResolveWidth(containerBox, containerCascaded);
        CssUI.CSS.BoxModel.ResolveWidth(childBox, childCascaded);

        // Assert - For RTL, margin-left is adjusted: 400 - 300 - 100 = 0
        Assert.Equal(0, childCascaded.Margin_Left.Computed.AsDecimal(), precision: 1);
    }

    #endregion

    #region Inline Elements

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Inline")]
    public void ResolveWidth_Inline_AutoMargins_BecomesZero()
    {
        // Arrange - Inline non-replaced: width doesn't apply, auto margins = 0
        var element = _fixture.CreateElement("span", style =>
        {
            style.Display.Set(EDisplayMode.INLINE);
            style.Margin_Left.Set(CssValue.Auto);
            style.Margin_Right.Set(CssValue.Auto);
        });
        _fixture.ForceLayoutUpdate();

        var box = element.Box;
        var cascaded = element.Style?.Cascaded;

        Assert.NotNull(box);
        Assert.NotNull(cascaded);

        // Act
        CssUI.CSS.BoxModel.ResolveWidth(box, cascaded);

        // Assert - Auto margins become 0 for inline
        Assert.Equal(0, cascaded.Margin_Left.Computed.AsDecimal(), precision: 1);
        Assert.Equal(0, cascaded.Margin_Right.Computed.AsDecimal(), precision: 1);
    }

    #endregion
}
