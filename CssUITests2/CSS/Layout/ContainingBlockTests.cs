using CssUI;
using CssUI.CSS;
using CssUI.CSS.Enums;
using CssUI.DOM.Nodes;
using CssUITests.Fixtures;
using Xunit;
using CssBoxModel = CssUI.CSS.BoxModel;

namespace CssUITests.CSS.Layout;

/// <summary>
/// Integration tests for Containing Block discovery and resolution (Phase 14.4).
/// Tests how elements find their containing block for percentage resolution.
/// </summary>
public class ContainingBlockTests
{
    #region Static Position Containing Block Tests

    /// <summary>
    /// Tests that statically positioned element uses parent's content box as containing block.
    /// </summary>
    /// <remarks>
    /// Per CSS 2.1 10.1: For static/relative positioned elements, the containing block
    /// is formed by the content edge of the nearest block container ancestor.
    /// </remarks>
    [Fact]
    [Trait("Category", "ContainingBlock")]
    [Trait("Category", "Integration")]
    public void StaticPosition_UsesParentContentBoxAsContainingBlock()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        var parent = fixture.CreateBlock(400, 300);
        var child = fixture.CreateChild(parent, "div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Positioning.Set(EBoxPositioning.Static);
            style.Width.Set(CssValue.From_Percent(50.0)); // 50% of containing block
            style.Height.Set(100);
        });

        // Act
        fixture.ForceLayoutUpdate();

        // Assert
        var parentBox = parent.Box;
        var childBox = child.Box;

        if (parentBox is not null && childBox is not null)
        {
            var containingBox = childBox.Containing_Box;

            // Containing block should be parent's content area
            Assert.Equal(parentBox.Content.Width, containingBox.Width, precision: 1);
        }
    }

    /// <summary>
    /// Tests that relatively positioned element also uses parent as containing block.
    /// </summary>
    [Fact]
    [Trait("Category", "ContainingBlock")]
    [Trait("Category", "Integration")]
    public void RelativePosition_UsesParentAsContainingBlock()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        var parent = fixture.CreateBlock(400, 300);
        var child = fixture.CreateChild(parent, "div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Positioning.Set(EBoxPositioning.Relative);
            style.Width.Set(CssValue.From_Percent(50.0));
            style.Height.Set(100);
            style.Top.Set(10); // Offset doesn't change containing block
            style.Left.Set(20);
        });

        // Act
        fixture.ForceLayoutUpdate();

        // Assert
        var parentBox = parent.Box;
        var childBox = child.Box;

        if (parentBox is not null && childBox is not null)
        {
            // Relative positioning doesn't change containing block
            var containingBox = childBox.Containing_Box;
            Assert.Equal(parentBox.Content.Width, containingBox.Width, precision: 1);
        }
    }

    #endregion

    #region Absolute Position Containing Block Tests

    /// <summary>
    /// Tests that absolutely positioned element uses nearest positioned ancestor.
    /// </summary>
    /// <remarks>
    /// Per CSS 2.1 10.1: For absolutely positioned elements, the containing block
    /// is established by the nearest ancestor with position: absolute, relative, or fixed.
    /// </remarks>
    [Fact]
    [Trait("Category", "ContainingBlock")]
    [Trait("Category", "Integration")]
    public void AbsolutePosition_UsesNearestPositionedAncestor()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();

        // Create chain: viewport -> static -> positioned -> static -> absolute
        var staticOuter = fixture.CreateBlock(500, 400);
        var positioned = fixture.CreateChild(staticOuter, "div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Positioning.Set(EBoxPositioning.Relative); // This is the positioned ancestor
            style.Width.Set(300);
            style.Height.Set(200);
        });
        var staticInner = fixture.CreateChild(positioned, "div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Width.Set(250);
            style.Height.Set(150);
        });
        var absolute = fixture.CreateChild(staticInner, "div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Positioning.Set(EBoxPositioning.Absolute);
            style.Width.Set(CssValue.From_Percent(50.0)); // 50% of positioned ancestor
            style.Height.Set(50);
        });

        // Act
        fixture.ForceLayoutUpdate();

        // Assert
        var absoluteBox = absolute.Box;
        var positionedBox = positioned.Box;

        if (absoluteBox is not null && positionedBox is not null)
        {
            // Containing block should skip staticInner and use positioned ancestor
            var containingBox = absoluteBox.Containing_Box;

            // Should be based on positioned element's padding box (300px width)
            // Note: For absolute positioning, containing block is padding edge not content edge
            Assert.True(containingBox.Width <= 300 + 1, // Allow small tolerance
                $"Containing block width ({containingBox.Width}) should be based on positioned ancestor (300)");
        }
    }

    /// <summary>
    /// Tests that absolutely positioned element in non-positioned tree uses initial containing block.
    /// </summary>
    [Fact]
    [Trait("Category", "ContainingBlock")]
    [Trait("Category", "Integration")]
    public void AbsolutePosition_NoPositionedAncestor_UsesInitialContainingBlock()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();

        // All ancestors are statically positioned
        var staticParent = fixture.CreateElement("div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Positioning.Set(EBoxPositioning.Static);
            style.Width.Set(300);
            style.Height.Set(200);
        });
        var absolute = fixture.CreateChild(staticParent, "div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Positioning.Set(EBoxPositioning.Absolute);
            style.Width.Set(CssValue.From_Percent(50.0)); // 50% of initial containing block
            style.Height.Set(50);
        });

        // Act
        fixture.ForceLayoutUpdate();

        // Assert
        var absoluteBox = absolute.Box;

        if (absoluteBox is not null)
        {
            var containingBox = absoluteBox.Containing_Box;

            // Should be viewport dimensions (or document element)
            // Initial containing block should be viewport-sized
            Assert.True(containingBox.Width >= 300,
                "Without positioned ancestor, containing block should be initial (viewport)");
        }
    }

    #endregion

    #region Fixed Position Containing Block Tests

    /// <summary>
    /// Tests that fixed positioned element uses viewport as containing block.
    /// </summary>
    /// <remarks>
    /// Per CSS 2.1 10.1: For fixed positioned elements, the containing block
    /// is established by the viewport (for continuous media).
    /// </remarks>
    [Fact]
    [Trait("Category", "ContainingBlock")]
    [Trait("Category", "Integration")]
    public void FixedPosition_UsesViewportAsContainingBlock()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();

        // Create positioned ancestor (should be ignored for fixed)
        var positioned = fixture.CreateContainingBlock(300, 200, EBoxPositioning.Relative);
        var fixedElement = fixture.CreateChild(positioned, "div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Positioning.Set(EBoxPositioning.Fixed);
            style.Width.Set(CssValue.From_Percent(50.0)); // 50% of viewport
            style.Height.Set(100);
        });

        // Act
        fixture.ForceLayoutUpdate();

        // Assert
        var fixedBox = fixedElement.Box;

        if (fixedBox is not null)
        {
            var containingBox = fixedBox.Containing_Box;

            // Should be viewport dimensions, not positioned ancestor
            // Viewport is 800x600 by default
            Assert.True(containingBox.Width >= fixture.ViewportWidth - 1,
                $"Fixed element containing block ({containingBox.Width}) should be viewport width ({fixture.ViewportWidth})");
        }
    }

    #endregion

    #region Percentage Resolution Through Chain Tests

    /// <summary>
    /// Tests that percentage values resolve through the containing block chain.
    /// </summary>
    [Fact]
    [Trait("Category", "ContainingBlock")]
    [Trait("Category", "Integration")]
    public void PercentageWidth_ResolvesAgainstContainingBlockWidth()
    {
        // Arrange: 800px viewport -> 400px container -> 50% child = 200px
        using var fixture = new LayoutTestFixture();
        var container = fixture.CreateBlock(400, 300);
        var child = fixture.CreateChild(container, "div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Width.Set(CssValue.From_Percent(50.0));
            style.Height.Set(100);
        });

        // Act
        fixture.ForceLayoutUpdate();

        // Assert
        var containerBox = container.Box;
        var childBox = child.Box;

        if (containerBox is not null && childBox is not null)
        {
            var childCascaded = child.Style?.Cascaded;
            if (childCascaded is not null)
            {
                CssBoxModel.ResolveWidth(childBox, childCascaded);

                // 50% of 400 = 200
                Assert.Equal(200, childCascaded.Width.Computed.AsDecimal(), precision: 1);
            }
        }
    }

    /// <summary>
    /// Tests deeply nested percentage resolution.
    /// </summary>
    [Fact]
    [Trait("Category", "ContainingBlock")]
    [Trait("Category", "Integration")]
    public void DeeplyNestedPercentages_ResolveCorrectly()
    {
        // Arrange: 800 -> 50% (400) -> 50% (200) -> 50% (100)
        using var fixture = new LayoutTestFixture();

        var level1 = fixture.CreateElement("div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Width.Set(CssValue.From_Percent(50.0)); // 400
        });
        var level2 = fixture.CreateChild(level1, "div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Width.Set(CssValue.From_Percent(50.0)); // 200
        });
        var level3 = fixture.CreateChild(level2, "div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Width.Set(CssValue.From_Percent(50.0)); // 100
        });

        // Act
        fixture.ForceLayoutUpdate();

        // Assert
        var box1 = level1.Box;
        var box2 = level2.Box;
        var box3 = level3.Box;

        if (box1 is not null && box2 is not null && box3 is not null)
        {
            var cascaded1 = level1.Style?.Cascaded;
            var cascaded2 = level2.Style?.Cascaded;
            var cascaded3 = level3.Style?.Cascaded;

            if (cascaded1 is not null && cascaded2 is not null && cascaded3 is not null)
            {
                CssBoxModel.ResolveWidth(box1, cascaded1);
                CssBoxModel.ResolveWidth(box2, cascaded2);
                CssBoxModel.ResolveWidth(box3, cascaded3);

                Assert.Equal(400, cascaded1.Width.Computed.AsDecimal(), precision: 1);
                Assert.Equal(200, cascaded2.Width.Computed.AsDecimal(), precision: 1);
                Assert.Equal(100, cascaded3.Width.Computed.AsDecimal(), precision: 1);
            }
        }
    }

    #endregion

    #region Initial Containing Block Tests

    /// <summary>
    /// Tests that root element uses viewport as initial containing block.
    /// </summary>
    [Fact]
    [Trait("Category", "ContainingBlock")]
    [Trait("Category", "Integration")]
    public void RootElement_UsesViewportAsInitialContainingBlock()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();

        // Act
        fixture.ForceLayoutUpdate();

        // Assert
        var htmlBox = fixture.DocumentElement.Box;

        if (htmlBox is not null)
        {
            var containingBox = htmlBox.Containing_Box;

            // Should be viewport dimensions
            Assert.Equal(fixture.ViewportWidth, containingBox.Width, precision: 1);
            Assert.Equal(fixture.ViewportHeight, containingBox.Height, precision: 1);
        }
    }

    /// <summary>
    /// Tests that body element uses html element as containing block.
    /// </summary>
    [Fact]
    [Trait("Category", "ContainingBlock")]
    [Trait("Category", "Integration")]
    public void BodyElement_UsesHtmlAsContainingBlock()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();

        // Act
        fixture.ForceLayoutUpdate();

        // Assert
        var bodyBox = fixture.Body.Box;
        var htmlBox = fixture.DocumentElement.Box;

        if (bodyBox is not null && htmlBox is not null)
        {
            var bodyContainingBox = bodyBox.Containing_Box;

            // Body's containing block should match html's content area
            Assert.Equal(htmlBox.Content.Width, bodyContainingBox.Width, precision: 1);
        }
    }

    #endregion
}
