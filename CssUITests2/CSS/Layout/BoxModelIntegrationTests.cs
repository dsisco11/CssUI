using CssUI;
using CssUI.CSS;
using CssUI.CSS.Enums;
using CssUI.DOM.Nodes;
using CssUITests.Fixtures;
using Xunit;

namespace CssUITests.CSS.Layout;

/// <summary>
/// Integration tests for BoxModel calculations (Phase 14.3).
/// Tests width/height resolution, margin handling, and containing block relationships.
/// Uses ForceFullLayout() to run the complete multi-pass layout pipeline.
/// </summary>
/// <remarks>
/// Note: Some tests expose pre-existing bugs documented in 14.5.12.
/// These tests fail by design to track those bugs until they're fixed.
/// </remarks>
public class BoxModelIntegrationTests
{
    #region Auto Margin Centering Tests

    /// <summary>
    /// Tests that margin-left: auto; margin-right: auto centers a block horizontally.
    /// </summary>
    /// <remarks>
    /// Per CSS 2.1 10.3.3: If both margin-left and margin-right are auto, their used values are equal.
    /// This causes horizontal centering.
    /// Spec: https://www.w3.org/TR/CSS2/visudet.html#blockwidth
    ///
    /// Known Bug (14.5.12): Auto margin centering returns 0 instead of calculated values.
    /// </remarks>
    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Integration")]
    public void AutoMargins_HorizontalCentering_MarginsAreEqual()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        // Container is 800px wide (default viewport)
        var element = fixture.CreateCenteredBlock(200, 100);

        // Act - Use full layout pipeline
        fixture.ForceFullLayout();
        var box = element.Box;

        // Assert
        Assert.NotNull(box);
        var cascaded = element.Style?.Cascaded;
        Assert.NotNull(cascaded);

        // Both margins should be equal: (800 - 200) / 2 = 300
        var marginLeft = cascaded.Margin_Left.Computed;
        var marginRight = cascaded.Margin_Right.Computed;

        // Both should be resolved to the same value
        Assert.Equal(marginLeft.AsDecimal(), marginRight.AsDecimal());

        // The total should equal the remaining space
        double totalMargin = marginLeft.AsDecimal() + marginRight.AsDecimal();
        double expectedMargin = fixture.ViewportWidth - 200; // 800 - 200 = 600
        Assert.Equal(expectedMargin, totalMargin, precision: 1);
    }

    /// <summary>
    /// Tests that a single auto margin takes all remaining space.
    /// </summary>
    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Integration")]
    public void AutoMarginLeft_TakesRemainingSpace()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        var element = fixture.CreateElement("div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Width.Set(200);
            style.Height.Set(100);
            style.Margin_Left.Assigned = CssValue.Auto;
            style.Margin_Right.Set(50);
        });

        // Act - Use full layout pipeline
        fixture.ForceFullLayout();
        var box = element.Box;

        // Assert
        Assert.NotNull(box);
        var cascaded = element.Style?.Cascaded;
        Assert.NotNull(cascaded);

        // margin-left should absorb: 800 - 200 - 50 = 550
        var marginLeft = cascaded.Margin_Left.Computed;
        Assert.Equal(550, marginLeft.AsDecimal(), precision: 1);
    }

    #endregion

    #region Percentage Width Resolution Tests

    /// <summary>
    /// Tests that percentage width resolves against containing block width.
    /// </summary>
    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Integration")]
    public void PercentageWidth_ResolvesAgainstContainingBlock()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        var container = fixture.CreateContainingBlock(400, 300);
        var element = fixture.CreateChild(container, "div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Width.Set(CssValue.From_Percent(50.0)); // 50%
            style.Height.Set(100);
        });

        // Act - Use full layout pipeline
        fixture.ForceFullLayout();

        // Assert
        var elementBox = element.Box;
        Assert.NotNull(elementBox);
        var elementCascaded = element.Style?.Cascaded;
        Assert.NotNull(elementCascaded);

        // Width should be 50% of 400 = 200
        var width = elementCascaded.Width.Computed;
        Assert.Equal(200, width.AsDecimal(), precision: 1);
    }

    /// <summary>
    /// Tests that percentage width inside percentage width container chains correctly.
    /// </summary>
    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Integration")]
    public void NestedPercentageWidths_ResolveCorrectly()
    {
        // Arrange - 800px viewport -> 50% = 400px -> 50% = 200px
        using var fixture = new LayoutTestFixture();
        var outer = fixture.CreateElement("div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Width.Set(CssValue.From_Percent(50.0)); // 50% of 800 = 400
        });
        var inner = fixture.CreateChild(outer, "div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Width.Set(CssValue.From_Percent(50.0)); // 50% of 400 = 200
        });

        // Act - Use full layout pipeline
        fixture.ForceFullLayout();

        // Assert
        var outerBox = outer.Box;
        var innerBox = inner.Box;
        Assert.NotNull(outerBox);
        Assert.NotNull(innerBox);

        var outerCascaded = outer.Style?.Cascaded;
        var innerCascaded = inner.Style?.Cascaded;
        Assert.NotNull(outerCascaded);
        Assert.NotNull(innerCascaded);

        // Outer should be 400px (50% of 800)
        Assert.Equal(400, outerCascaded.Width.Computed.AsDecimal(), precision: 1);

        // Inner should be 200px (50% of 400)
        Assert.Equal(200, innerCascaded.Width.Computed.AsDecimal(), precision: 1);
    }

    #endregion

    #region Min/Max Width Constraint Tests

    /// <summary>
    /// Tests that min-width constrains computed width.
    /// </summary>
    /// <remarks>
    /// Known Bug (14.5.12): min-width not clamping resolved width upward.
    /// </remarks>
    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Integration")]
    public void MinWidth_ConstrainsComputedWidth()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        var element = fixture.CreateElement("div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Width.Set(100);
            style.Min_Width.Assigned = CssValue.From_Dimension(200, ECssUnit.PX); // Min is larger than width
            style.Height.Set(50);
        });

        // Act - Use full layout pipeline
        fixture.ForceFullLayout();
        var box = element.Box;

        // Assert - Width should be at least min-width
        Assert.NotNull(box);
        var cascaded = element.Style?.Cascaded;
        Assert.NotNull(cascaded);

        var width = cascaded.Width.Computed.AsDecimal();
        var minWidth = cascaded.Min_Width.Computed.AsDecimal();

        // Width should be clamped to min-width
        Assert.True(width >= minWidth,
            $"Width ({width}) should be >= min-width ({minWidth})");
    }

    /// <summary>
    /// Tests that max-width constrains computed width.
    /// </summary>
    /// <remarks>
    /// Known Bug (14.5.12): max-width not clamping resolved width downward.
    /// </remarks>
    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Integration")]
    public void MaxWidth_ConstrainsComputedWidth()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        var element = fixture.CreateElement("div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Width.Set(500);
            style.Max_Width.Assigned = CssValue.From_Dimension(300, ECssUnit.PX); // Max is smaller than width
            style.Height.Set(50);
        });

        // Act - Use full layout pipeline
        fixture.ForceFullLayout();
        var box = element.Box;

        // Assert - Width should be at most max-width
        Assert.NotNull(box);
        var cascaded = element.Style?.Cascaded;
        Assert.NotNull(cascaded);

        var width = cascaded.Width.Computed.AsDecimal();
        var maxWidth = cascaded.Max_Width.Computed.AsDecimal();

        // Width should be clamped to max-width
        Assert.True(width <= maxWidth,
            $"Width ({width}) should be <= max-width ({maxWidth})");
    }

    /// <summary>
    /// Tests that min-width takes precedence over max-width when min > max.
    /// </summary>
    /// <remarks>
    /// Per CSS 2.1 10.4: If min-width is greater than max-width, max-width is ignored.
    /// </remarks>
    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Integration")]
    public void MinWidth_TakesPrecedenceOverMaxWidth()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        var element = fixture.CreateElement("div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Width.Set(200);
            style.Min_Width.Assigned = CssValue.From_Dimension(300, ECssUnit.PX); // Min > Max!
            style.Max_Width.Assigned = CssValue.From_Dimension(100, ECssUnit.PX);
            style.Height.Set(50);
        });

        // Act - Use full layout pipeline
        fixture.ForceFullLayout();
        var box = element.Box;

        // Assert - min-width should win
        Assert.NotNull(box);
        var cascaded = element.Style?.Cascaded;
        Assert.NotNull(cascaded);

        var width = cascaded.Width.Computed.AsDecimal();
        var minWidth = cascaded.Min_Width.Computed.AsDecimal();

        // Width should be at least min-width, even though max-width is smaller
        Assert.True(width >= minWidth,
            $"Width ({width}) should be >= min-width ({minWidth}) even when max-width is smaller");
    }

    #endregion

    #region Height Auto Calculation Tests

    /// <summary>
    /// Tests that height: auto uses content height.
    /// </summary>
    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Integration")]
    public void HeightAuto_UsesContentHeight()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        var container = fixture.CreateElement("div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Width.Set(200);
            // height: auto (default)
        });

        // Create children that contribute content height
        fixture.CreateChild(container, "div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Height.Set(50);
        });
        fixture.CreateChild(container, "div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Height.Set(75);
        });

        // Act - Use full layout pipeline (Flow will calculate content height)
        fixture.ForceFullLayout();

        // Assert
        var containerBox = container.Box;
        Assert.NotNull(containerBox);
        var cascaded = container.Style?.Cascaded;
        Assert.NotNull(cascaded);

        var height = cascaded.Height.Computed;

        // Height should be the sum of children: 50 + 75 = 125
        // Note: This requires proper Flow implementation to work
        Assert.Equal(125, height.AsDecimal(), precision: 1);
    }

    /// <summary>
    /// Tests that explicit height overrides content height.
    /// </summary>
    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Integration")]
    public void ExplicitHeight_OverridesContentHeight()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        var container = fixture.CreateElement("div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Width.Set(200);
            style.Height.Set(100); // Explicit height
        });

        // Create a child taller than container
        fixture.CreateChild(container, "div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Height.Set(200); // Taller than parent
        });

        // Act - Use full layout pipeline
        fixture.ForceFullLayout();

        // Assert
        var containerBox = container.Box;
        Assert.NotNull(containerBox);
        var cascaded = container.Style?.Cascaded;
        Assert.NotNull(cascaded);

        var height = cascaded.Height.Computed;

        // Height should be explicit 100, not content 200
        Assert.Equal(100, height.AsDecimal(), precision: 1);
    }

    #endregion

    #region Absolutely Positioned Element Tests

    /// <summary>
    /// Tests that absolutely positioned element uses positioned ancestor as containing block.
    /// </summary>
    /// <remarks>
    /// Known Bug (14.5.12): Absolute positioning constraint equation issues.
    /// </remarks>
    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Integration")]
    public void AbsolutePosition_UsesPositionedAncestorAsContainingBlock()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        var containingBlock = fixture.CreateContainingBlock(400, 300, EBoxPositioning.Relative);
        var absolute = fixture.CreateAbsolutelyPositioned(containingBlock, style =>
        {
            style.Width.Set(CssValue.From_Percent(50.0)); // 50% of containing block
            style.Height.Set(100);
            style.Top.Set(10);
            style.Left.Set(20);
        });

        // Act - Use full layout pipeline
        fixture.ForceFullLayout();

        // Assert
        var absoluteBox = absolute.Box;
        Assert.NotNull(absoluteBox);
        var cascaded = absolute.Style?.Cascaded;
        Assert.NotNull(cascaded);

        // Width should be 50% of 400 = 200
        Assert.Equal(200, cascaded.Width.Computed.AsDecimal(), precision: 1);
    }

    /// <summary>
    /// Tests absolutely positioned element with opposite offset constraints.
    /// </summary>
    /// <remarks>
    /// Per CSS 2.1 10.3.7: If both left and right are set, and width is auto,
    /// the width is determined by the constraint equation.
    ///
    /// Known Bug (14.5.12): Absolute positioning constraint equation issues.
    /// </remarks>
    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Integration")]
    public void AbsolutePosition_ConstraintEquation_DeterminesWidth()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        var containingBlock = fixture.CreateContainingBlock(400, 300, EBoxPositioning.Relative);
        var absolute = fixture.CreateAbsolutelyPositioned(containingBlock, style =>
        {
            // width: auto with both left and right set
            style.Left.Set(50);
            style.Right.Set(50);
            style.Height.Set(100);
            // width is auto by default
        });

        // Act - Use full layout pipeline
        fixture.ForceFullLayout();

        // Assert
        var absoluteBox = absolute.Box;
        Assert.NotNull(absoluteBox);
        var cascaded = absolute.Style?.Cascaded;
        Assert.NotNull(cascaded);

        // Width should be: 400 - 50 - 50 = 300 (minus margins if any)
        var width = cascaded.Width.Computed.AsDecimal();
        Assert.True(width > 0, "Width should be calculated from constraint equation");
    }

    #endregion

    #region Block Width Fills Container Tests

    /// <summary>
    /// Tests that block with width: auto fills available space.
    /// </summary>
    /// <remarks>
    /// Known Bug (14.5.12): AsDecimal() called on AUTO values throws exception.
    /// Block with width: auto should fill containing block minus margins.
    /// </remarks>
    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Integration")]
    public void BlockWithAutoWidth_FillsAvailableSpace()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        var container = fixture.CreateContainingBlock(400, 300);
        var block = fixture.CreateChild(container, "div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            // width: auto (default)
            style.Height.Set(100);
            style.Margin_Left.Set(20);
            style.Margin_Right.Set(30);
        });

        // Act - Use full layout pipeline
        fixture.ForceFullLayout();

        // Assert
        var blockBox = block.Box;
        Assert.NotNull(blockBox);
        var cascaded = block.Style?.Cascaded;
        Assert.NotNull(cascaded);

        // Width should be: 400 - 20 - 30 = 350
        var width = cascaded.Width.Computed.AsDecimal();
        Assert.Equal(350, width, precision: 1);
    }

    #endregion
}
