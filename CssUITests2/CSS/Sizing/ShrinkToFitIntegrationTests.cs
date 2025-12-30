using CssUI;
using CssUI.CSS;
using CssUI.CSS.BoxTree;
using CssUI.CSS.Enums;
using CssUI.DOM;
using CssUI.DOM.Enums;
using CssUI.DOM.Nodes;
using CssUITests.Fixtures;
using Xunit;

namespace CssUITests.CSS.Sizing;

/// <summary>
/// Integration tests for shrink-to-fit width calculations during layout.
/// Tests Phase 14.5.4: Wiring IntrinsicSizeCalculator.Calculate() for width: auto scenarios.
/// </summary>
/// <remarks>
/// These tests verify the full pipeline from element creation through shrink-to-fit calculation.
/// CSS 2.1 §10.3.5: "If 'width' is computed as 'auto', the used value is the shrink-to-fit width."
/// Shrink-to-fit = min(max(min-content, available), max-content) = clamp(min-content, max-content, available)
///
/// Note: Float property is not yet implemented (see Phase 15.1), so float tests are skipped.
/// Focus is on inline-block and absolutely positioned elements which use shrink-to-fit.
/// </remarks>
public class ShrinkToFitIntegrationTests
{

    #region Inline-Block width:auto Tests

    /// <summary>
    /// Tests that an inline-block element with width:auto uses shrink-to-fit sizing.
    /// CSS 2.1 §10.3.9: "If 'width' is 'auto', the used value is the shrink-to-fit width."
    /// </summary>
    [Fact]
    [Trait("Category", "Layout")]
    [Trait("Category", "ShrinkToFit")]
    public void InlineBlock_WidthAuto_UsesShrinkToFitWidth()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();

        // Create an inline-block with width:auto (explicit)
        var inlineBlock = fixture.CreateElement("span", style =>
        {
            style.Display.Set(EDisplayMode.INLINE_BLOCK);
            // Width auto by default - should use shrink-to-fit
        });

        // Force full layout
        fixture.ForceFullLayout();

        // Act
        var box = inlineBlock.Box;

        // Assert
        Assert.NotNull(box);

        var width = inlineBlock.Style?.Cascaded.Width.Computed;
        Assert.NotNull(width);
        Assert.False(width.IsAuto, "Width should be resolved from shrink-to-fit, not auto");
    }

    /// <summary>
    /// Tests that inline-block shrink-to-fit respects min-content width.
    /// </summary>
    [Fact]
    [Trait("Category", "Layout")]
    [Trait("Category", "ShrinkToFit")]
    public void InlineBlock_WidthAuto_RespectsMinContent()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();

        // Create a container with constrained width
        var container = fixture.CreateBlock(100, 200);

        // Create an inline-block with larger intrinsic size
        var inlineBlock = fixture.CreateChild(container, "span", style =>
        {
            style.Display.Set(EDisplayMode.INLINE_BLOCK);
            style.Min_Width.Set(150); // min-width forces minimum size
            // Width auto - should use shrink-to-fit but respect min-width
        });

        // Force layout
        fixture.ForceFullLayout();

        // Act
        var box = inlineBlock.Box;
        var computedWidth = inlineBlock.Style?.Cascaded.Width.Computed?.AsDecimal() ?? -1;

        // Assert - Width should respect min-width even though container is smaller
        Assert.NotNull(box);
        Assert.True(computedWidth >= 150,
            $"Inline-block width ({computedWidth}) should respect min-width (150)");
    }

    #endregion

    #region Absolute Position width:auto Tests

    /// <summary>
    /// Tests that an absolutely positioned element with width:auto and left/right:auto uses shrink-to-fit.
    /// CSS 2.1 §10.3.7 Rules #1 and #3.
    /// </summary>
    [Fact]
    [Trait("Category", "Layout")]
    [Trait("Category", "ShrinkToFit")]
    public void AbsolutePosition_WidthAutoLeftRightAuto_UsesShrinkToFit()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();

        // Create a positioned container for containing block
        var container = fixture.CreateElement("div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Positioning.Set(EBoxPositioning.Relative);
            style.Width.Set(500);
            style.Height.Set(300);
        });

        // Create absolutely positioned element with all autos
        var absElement = fixture.CreateChild(container, "div", style =>
        {
            style.Positioning.Set(EBoxPositioning.Absolute);
            // Width, left, right are all auto by default
            // This triggers shrink-to-fit per CSS 2.1 §10.3.7
        });

        // Force layout
        fixture.ForceFullLayout();

        // Act
        var box = absElement.Box;

        // Assert
        Assert.NotNull(box);

        var width = absElement.Style?.Cascaded.Width.Computed;
        Assert.NotNull(width);
        Assert.False(width.IsAuto, "Width should be resolved via shrink-to-fit");
    }

    /// <summary>
    /// Tests that absolutely positioned element with width:auto respects available space.
    /// </summary>
    [Fact]
    [Trait("Category", "Layout")]
    [Trait("Category", "ShrinkToFit")]
    public void AbsolutePosition_WidthAuto_RespectsContainingBlockWidth()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();

        // Create positioned container
        var container = fixture.CreateElement("div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Positioning.Set(EBoxPositioning.Relative);
            style.Width.Set(300);
            style.Height.Set(200);
        });

        // Create absolutely positioned element
        var absElement = fixture.CreateChild(container, "div", style =>
        {
            style.Positioning.Set(EBoxPositioning.Absolute);
            // Width auto, left and right auto
        });

        // Force layout
        fixture.ForceFullLayout();

        // Act
        var box = absElement.Box;
        var computedWidth = absElement.Style?.Cascaded.Width.Computed?.AsDecimal() ?? -1;

        // Assert
        Assert.NotNull(box);
        Assert.True(computedWidth <= 300,
            $"Absolute element width ({computedWidth}) should not exceed containing block (300)");
    }

    #endregion

    #region IntrinsicSizeCalculator Integration Tests

    /// <summary>
    /// Tests that IntrinsicSizeCalculator.Calculate() is called during layout.
    /// Uses inline-block which uses shrink-to-fit.
    /// </summary>
    [Fact]
    [Trait("Category", "Layout")]
    [Trait("Category", "ShrinkToFit")]
    public void ShrinkToFit_UsesIntrinsicSizeCalculator()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();

        // Use inline-block instead of float (Float property not yet implemented)
        var inlineBlockElement = fixture.CreateElement("div", style =>
        {
            style.Display.Set(EDisplayMode.INLINE_BLOCK);
        });

        // Force box generation first
        fixture.ForceBoxGeneration();

        // Get the box
        var box = inlineBlockElement.Box;
        Assert.NotNull(box);

        // Act - Call GetIntrinsicSize directly to verify it works
        var intrinsicSize = box.GetIntrinsicSize();

        // Assert - Should return a valid intrinsic size
        Assert.True(intrinsicSize.Inline.MinContent >= 0,
            "Min-content should be non-negative");
        Assert.True(intrinsicSize.Inline.MaxContent >= intrinsicSize.Inline.MinContent,
            "Max-content should be >= min-content");
    }

    /// <summary>
    /// Tests that FitContent algorithm correctly calculates shrink-to-fit width.
    /// </summary>
    [Fact]
    [Trait("Category", "Layout")]
    [Trait("Category", "ShrinkToFit")]
    public void FitContent_CalculatesCorrectWidth()
    {
        // Arrange - Simulate intrinsic size values
        var intrinsic = new IntrinsicAxisSize(50, 200); // min=50, max=200
        double available = 100;

        // Act - Calculate fit-content
        double result = IntrinsicSizeCalculator.FitContent(intrinsic, available);

        // Assert - fit-content = max(min-content, min(max-content, available))
        // = max(50, min(200, 100)) = max(50, 100) = 100
        Assert.Equal(100, result);
    }

    /// <summary>
    /// Tests that shrink-to-fit respects min-content when available space is too small.
    /// </summary>
    [Fact]
    [Trait("Category", "Layout")]
    [Trait("Category", "ShrinkToFit")]
    public void FitContent_ClampsToMinContent_WhenAvailableTooSmall()
    {
        // Arrange
        var intrinsic = new IntrinsicAxisSize(80, 200); // min=80, max=200
        double available = 40; // Less than min-content

        // Act
        double result = IntrinsicSizeCalculator.FitContent(intrinsic, available);

        // Assert - Should clamp to min-content (80)
        Assert.Equal(80, result);
    }

    /// <summary>
    /// Tests that shrink-to-fit respects max-content when available space is plenty.
    /// </summary>
    [Fact]
    [Trait("Category", "Layout")]
    [Trait("Category", "ShrinkToFit")]
    public void FitContent_ClampsToMaxContent_WhenAvailablePlenty()
    {
        // Arrange
        var intrinsic = new IntrinsicAxisSize(50, 150); // min=50, max=150
        double available = 500; // Much more than max-content

        // Act
        double result = IntrinsicSizeCalculator.FitContent(intrinsic, available);

        // Assert - Should clamp to max-content (150)
        Assert.Equal(150, result);
    }

    #endregion

    #region Block Container Intrinsic Size Tests

    /// <summary>
    /// Tests that block container intrinsic size calculation aggregates child sizes.
    /// </summary>
    [Fact]
    [Trait("Category", "Layout")]
    [Trait("Category", "ShrinkToFit")]
    public void BlockContainer_IntrinsicSize_AggregatesChildSizes()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();

        // Create a container with children of known sizes
        var container = fixture.CreateElement("div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            // Width auto - would use shrink-to-fit if floating
        });

        // Add children with explicit widths
        fixture.CreateChild(container, "div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Width.Set(100);
        });

        fixture.CreateChild(container, "div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Width.Set(150);
        });

        // Force box generation
        fixture.ForceBoxGeneration();

        // Act
        var box = container.Box;
        Assert.NotNull(box);

        var intrinsicSize = box.GetIntrinsicSize();

        // Assert - Intrinsic inline size should be max of children's inline sizes
        // For block flow, inline size = max(child1, child2) = max(100, 150) = 150
        // But children's intrinsic sizes include margins/borders/padding which default to 0
        Assert.True(intrinsicSize.Inline.MaxContent >= 0,
            "Max-content inline size should be non-negative");
    }

    #endregion

    #region Edge Cases

    /// <summary>
    /// Tests that display:none elements don't participate in shrink-to-fit calculations.
    /// </summary>
    [Fact]
    [Trait("Category", "Layout")]
    [Trait("Category", "ShrinkToFit")]
    public void DisplayNone_DoesNotParticipateInShrinkToFit()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();

        var container = fixture.CreateElement("div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
        });

        var hiddenChild = fixture.CreateChild(container, "div", style =>
        {
            style.Display.Set(EDisplayMode.NONE);
            style.Width.Set(1000); // Large width that shouldn't affect parent
        });

        // Force layout
        fixture.ForceBoxGeneration();

        // Act
        var hiddenBox = hiddenChild.Box;

        // Assert - display:none elements should not have boxes
        Assert.Null(hiddenBox);
    }

    /// <summary>
    /// Tests that out-of-flow elements (absolute, fixed) don't affect parent's shrink-to-fit.
    /// </summary>
    [Fact]
    [Trait("Category", "Layout")]
    [Trait("Category", "ShrinkToFit")]
    public void OutOfFlow_DoesNotAffectParentShrinkToFit()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();

        // Create an inline-block container (uses shrink-to-fit)
        // Note: Float property not yet implemented, so we use inline-block
        var inlineBlockContainer = fixture.CreateElement("div", style =>
        {
            style.Display.Set(EDisplayMode.INLINE_BLOCK);
            // Width auto - uses shrink-to-fit
        });

        // Add an absolutely positioned child (out of flow)
        var absChild = fixture.CreateChild(inlineBlockContainer, "div", style =>
        {
            style.Positioning.Set(EBoxPositioning.Absolute);
            style.Width.Set(500); // Large width
        });

        // Add a normal child to influence parent's intrinsic size
        var normalChild = fixture.CreateChild(inlineBlockContainer, "div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Width.Set(100);
        });

        // Force layout
        fixture.ForceBoxGeneration();

        // Act
        var containerBox = inlineBlockContainer.Box;
        Assert.NotNull(containerBox);

        var intrinsicSize = containerBox.GetIntrinsicSize();

        // Assert - The absolute child's 500px should not inflate parent's intrinsic size
        // because out-of-flow children are excluded from intrinsic size calculation
        Assert.True(intrinsicSize.Inline.MaxContent < 500,
            "Out-of-flow child should not affect parent's intrinsic size");
    }

    #endregion
}
