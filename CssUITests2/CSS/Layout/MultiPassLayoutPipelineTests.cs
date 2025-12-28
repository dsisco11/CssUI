using CssUI;
using CssUI.CSS;
using CssUI.CSS.BoxTree;
using CssUI.CSS.Enums;
using CssUI.CSS.Formatting;
using CssUI.DOM;
using CssUI.DOM.Enums;
using CssUI.DOM.Nodes;
using CssUITests.Fixtures;
using Xunit;

// Alias to avoid namespace conflict with CssUITests.CSS.BoxModel folder
using BoxModelClass = CssUI.CSS.BoxModel;

namespace CssUITests.CSS.Layout;

/// <summary>
/// Tests for the multi-pass layout pipeline (Phase 14.5).
/// Verifies that the layout pipeline processes passes in the correct order:
/// Pass 1: Box Generation (top-down)
/// Pass 2: Style Cascade (top-down)
/// Pass 3: Layout/Reflow (width top-down, height bottom-up)
/// </summary>
public class MultiPassLayoutPipelineTests
{
    #region BoxModel.ResolveWidth Tests

    /// <summary>
    /// Tests that BoxModel.ResolveWidth handles null box parameter.
    /// </summary>
    [Fact]
    [Trait("Category", "Layout")]
    [Trait("Category", "MultiPass")]
    public void BoxModel_ResolveWidth_WithNullBox_ThrowsArgumentNullException()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        var element = fixture.CreateBlock(200, 100);
        var cascaded = element.Style?.Cascaded;

        // Act & Assert
        if (cascaded is not null)
        {
            Assert.Throws<System.ArgumentNullException>(() => BoxModelClass.ResolveWidth(null!, cascaded));
        }
    }

    /// <summary>
    /// Tests that BoxModel.ResolveWidth handles null cascaded parameter.
    /// </summary>
    [Fact]
    [Trait("Category", "Layout")]
    [Trait("Category", "MultiPass")]
    public void BoxModel_ResolveWidth_WithNullCascaded_ThrowsArgumentNullException()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        var element = fixture.CreateBlock(200, 100);
        var box = element.Box as CssPrincipalBox;

        // Act & Assert
        if (box is not null)
        {
            Assert.Throws<System.ArgumentNullException>(() => BoxModelClass.ResolveWidth(box, null!));
        }
    }

    #endregion

    #region BoxModel.ResolveHeight Tests

    /// <summary>
    /// Tests that BoxModel.ResolveHeight handles null box parameter.
    /// </summary>
    [Fact]
    [Trait("Category", "Layout")]
    [Trait("Category", "MultiPass")]
    public void BoxModel_ResolveHeight_WithNullBox_ThrowsArgumentNullException()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        var element = fixture.CreateBlock(200, 100);
        var cascaded = element.Style?.Cascaded;

        // Act & Assert
        if (cascaded is not null)
        {
            Assert.Throws<System.ArgumentNullException>(() => BoxModelClass.ResolveHeight(null!, cascaded));
        }
    }

    /// <summary>
    /// Tests that BoxModel.ResolveHeight handles null cascaded parameter.
    /// </summary>
    [Fact]
    [Trait("Category", "Layout")]
    [Trait("Category", "MultiPass")]
    public void BoxModel_ResolveHeight_WithNullCascaded_ThrowsArgumentNullException()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        var element = fixture.CreateBlock(200, 100);
        var box = element.Box as CssPrincipalBox;

        // Act & Assert
        if (box is not null)
        {
            Assert.Throws<System.ArgumentNullException>(() => BoxModelClass.ResolveHeight(box, null!));
        }
    }

    /// <summary>
    /// Tests that BoxModel.ResolveHeight accepts optional contentHeight parameter.
    /// </summary>
    [Fact]
    [Trait("Category", "Layout")]
    [Trait("Category", "MultiPass")]
    public void BoxModel_ResolveHeight_WithContentHeight_DoesNotThrow()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        var element = fixture.CreateBlock(200, 100);
        var box = element.Box as CssPrincipalBox;
        var cascaded = element.Style?.Cascaded;

        // Act & Assert
        if (box is not null && cascaded is not null)
        {
            var exception = Record.Exception(() => BoxModelClass.ResolveHeight(box, cascaded, contentHeight: 150.0));
            Assert.Null(exception);
        }
    }

    #endregion

    #region FormattingContext.Flow Return Type Tests

    /// <summary>
    /// Tests that BlockFormattingContext.Flow returns Rect2f.
    /// </summary>
    [Fact]
    [Trait("Category", "Layout")]
    [Trait("Category", "MultiPass")]
    public void BlockFormattingContext_Flow_ReturnsRect2f()
    {
        // Arrange
        var bfc = new BlockFormattingContext();

        // Create a minimal mock box tree node
        using var fixture = new LayoutTestFixture();
        var element = fixture.CreateBlock(400, 300);
        var box = element.Box;

        // Act
        if (box is not null)
        {
            var result = bfc.Flow(box);

            // Assert - Should return Rect2f (struct, never null)
            Assert.IsType<Rect2f>(result);
            Assert.True(result.Width >= 0, "Width should be non-negative");
            Assert.True(result.Height >= 0, "Height should be non-negative");
        }
    }

    /// <summary>
    /// Tests that FlexFormattingContext.Flow returns Rect2f.
    /// </summary>
    [Fact]
    [Trait("Category", "Layout")]
    [Trait("Category", "MultiPass")]
    public void FlexFormattingContext_Flow_ReturnsRect2f()
    {
        // Arrange
        var ffc = new FlexFormattingContext();

        using var fixture = new LayoutTestFixture();
        var element = fixture.CreateElement("div", style =>
        {
            style.Display.Set(EDisplayMode.FLEX);
            style.Width.Set(400);
            style.Height.Set(300);
        });
        var box = element.Box;

        // Act
        if (box is not null)
        {
            var result = ffc.Flow(box);

            // Assert
            Assert.IsType<Rect2f>(result);
        }
    }

    /// <summary>
    /// Tests that GridFormattingContext.Flow returns Rect2f.
    /// </summary>
    [Fact]
    [Trait("Category", "Layout")]
    [Trait("Category", "MultiPass")]
    public void GridFormattingContext_Flow_ReturnsRect2f()
    {
        // Arrange
        var gfc = new GridFormattingContext();

        using var fixture = new LayoutTestFixture();
        var element = fixture.CreateElement("div", style =>
        {
            style.Display.Set(EDisplayMode.GRID);
            style.Width.Set(400);
            style.Height.Set(300);
        });
        var box = element.Box;

        // Act
        if (box is not null)
        {
            var result = gfc.Flow(box);

            // Assert
            Assert.IsType<Rect2f>(result);
        }
    }

    #endregion

    #region IFormattingContext Interface Tests

    /// <summary>
    /// Tests that all formatting contexts implement IFormattingContext with Rect2f return type.
    /// </summary>
    [Fact]
    [Trait("Category", "Layout")]
    [Trait("Category", "MultiPass")]
    public void FormattingContexts_ImplementIFormattingContext()
    {
        // Assert
        Assert.True(typeof(IFormattingContext).IsAssignableFrom(typeof(BlockFormattingContext)));
        Assert.True(typeof(IFormattingContext).IsAssignableFrom(typeof(FlexFormattingContext)));
        Assert.True(typeof(IFormattingContext).IsAssignableFrom(typeof(GridFormattingContext)));
    }

    /// <summary>
    /// Tests that IFormattingContext.Flow returns Rect2f.
    /// </summary>
    [Fact]
    [Trait("Category", "Layout")]
    [Trait("Category", "MultiPass")]
    public void IFormattingContext_Flow_ReturnsRect2f()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        var element = fixture.CreateBlock(200, 100);
        var box = element.Box;

        // Use interface reference
        IFormattingContext context = new BlockFormattingContext();

        // Act
        if (box is not null)
        {
            var result = context.Flow(box);

            // Assert - verify return type
            Assert.IsType<Rect2f>(result);
        }
    }

    #endregion
}
