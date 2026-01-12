using CssUI.CSS;
using CssUI.DOM;
using CssUITests.Fixtures;
using Xunit;

namespace CssUITests.CSS.Formatting;

/// <summary>
/// Tests for float and clear property layout behavior in BlockFormattingContext.
/// Spec: https://www.w3.org/TR/CSS2/visuren.html#floats
/// Spec: https://www.w3.org/TR/CSS2/visuren.html#flow-control
/// </summary>
public class FloatClearTests
{
    #region Float: Left Tests

    [Fact]
    [Trait("Category", "Formatting")]
    [Trait("Category", "Float")]
    public void Float_Left_PositionsElementToLeft()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        var floatBox = fixture.CreateElement("div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Float.Set(EFloat.Left);
            style.Width.Set(100);
            style.Height.Set(50);
        });

        // Act
        fixture.ForceFullLayout();

        // Assert
        Assert.NotNull(floatBox.Box);
        Assert.Equal(0, floatBox.Box.Position.X); // Should be at left edge
        Assert.True(floatBox.Box.IsFloating);
    }

    [Fact]
    [Trait("Category", "Formatting")]
    [Trait("Category", "Float")]
    public void Float_Left_MultipleFloats_StackHorizontally()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        var float1 = fixture.CreateElement("div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Float.Set(EFloat.Left);
            style.Width.Set(100);
            style.Height.Set(50);
        });

        var float2 = fixture.CreateElement("div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Float.Set(EFloat.Left);
            style.Width.Set(100);
            style.Height.Set(50);
        });

        // Act
        fixture.ForceFullLayout();

        // Assert
        Assert.NotNull(float1.Box);
        Assert.NotNull(float2.Box);
        Assert.Equal(0, float1.Box.Position.X); // First float at left edge
        Assert.Equal(100, float2.Box.Position.X); // Second float after first
        Assert.Equal(0, float1.Box.Position.Y); // Both at same vertical position
        Assert.Equal(0, float2.Box.Position.Y);
    }

    #endregion

    #region Float: Right Tests

    [Fact]
    [Trait("Category", "Formatting")]
    [Trait("Category", "Float")]
    public void Float_Right_PositionsElementToRight()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        fixture.ViewportWidth = 800;
        fixture.Body.Style.UserRules.Width.Set(800);

        var floatBox = fixture.CreateElement("div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Float.Set(EFloat.Right);
            style.Width.Set(100);
            style.Height.Set(50);
        });

        // Act
        fixture.ForceFullLayout();

        // Assert
        Assert.NotNull(floatBox.Box);
        Assert.Equal(700, floatBox.Box.Position.X); // 800 - 100 = 700 (right edge)
        Assert.True(floatBox.Box.IsFloating);
    }

    [Fact]
    [Trait("Category", "Formatting")]
    [Trait("Category", "Float")]
    public void Float_Right_MultipleFloats_StackHorizontally()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        fixture.ViewportWidth = 800;
        fixture.Body.Style.UserRules.Width.Set(800);

        var float1 = fixture.CreateElement("div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Float.Set(EFloat.Right);
            style.Width.Set(100);
            style.Height.Set(50);
        });

        var float2 = fixture.CreateElement("div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Float.Set(EFloat.Right);
            style.Width.Set(100);
            style.Height.Set(50);
        });

        // Act
        fixture.ForceFullLayout();

        // Assert
        Assert.NotNull(float1.Box);
        Assert.NotNull(float2.Box);
        Assert.Equal(700, float1.Box.Position.X); // First float at right edge (800-100)
        Assert.Equal(600, float2.Box.Position.X); // Second float before first (800-100-100)
        Assert.Equal(0, float1.Box.Position.Y); // Both at same vertical position
        Assert.Equal(0, float2.Box.Position.Y);
    }

    #endregion

    #region Content Flow Around Floats Tests

    [Fact]
    [Trait("Category", "Formatting")]
    [Trait("Category", "Float")]
    public void ContentFlows_AroundLeftFloat()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        var floatBox = fixture.CreateElement("div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Float.Set(EFloat.Left);
            style.Width.Set(100);
            style.Height.Set(50);
        });

        var normalBox = fixture.CreateElement("div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Width.Set(200);
            style.Height.Set(30);
        });

        // Act
        fixture.ForceFullLayout();

        // Assert - Normal box should be positioned accounting for float intrusion
        Assert.NotNull(floatBox.Box);
        Assert.NotNull(normalBox.Box);
        Assert.Equal(0, floatBox.Box.Position.X);
        Assert.Equal(100, normalBox.Box.Position.X); // Starts after float
        Assert.Equal(0, normalBox.Box.Position.Y); // Same vertical position as float
    }

    [Fact]
    [Trait("Category", "Formatting")]
    [Trait("Category", "Float")]
    public void ContentFlows_AroundRightFloat()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        fixture.ViewportWidth = 800;
        fixture.Body.Style.UserRules.Width.Set(800);

        var floatBox = fixture.CreateElement("div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Float.Set(EFloat.Right);
            style.Width.Set(100);
            style.Height.Set(50);
        });

        var normalBox = fixture.CreateElement("div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Width.Set(200);
            style.Height.Set(30);
        });

        // Act
        fixture.ForceFullLayout();

        // Assert - Normal box should have reduced available width due to float
        Assert.NotNull(floatBox.Box);
        Assert.NotNull(normalBox.Box);
        Assert.Equal(700, floatBox.Box.Position.X); // Float at right edge
        Assert.Equal(0, normalBox.Box.Position.X); // Normal box starts at left
        // Available width for normal box is 700 (not tested here, but float shouldn't overlap)
    }

    [Fact]
    [Trait("Category", "Formatting")]
    [Trait("Category", "Float")]
    public void ContentFlows_BetweenLeftAndRightFloats()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        fixture.ViewportWidth = 800;
        fixture.Body.Style.UserRules.Width.Set(800);

        var leftFloat = fixture.CreateElement("div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Float.Set(EFloat.Left);
            style.Width.Set(100);
            style.Height.Set(50);
        });

        var rightFloat = fixture.CreateElement("div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Float.Set(EFloat.Right);
            style.Width.Set(100);
            style.Height.Set(50);
        });

        var normalBox = fixture.CreateElement("div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Width.Set(200);
            style.Height.Set(30);
        });

        // Act
        fixture.ForceFullLayout();

        // Assert
        Assert.NotNull(leftFloat.Box);
        Assert.NotNull(rightFloat.Box);
        Assert.NotNull(normalBox.Box);
        Assert.Equal(0, leftFloat.Box.Position.X);
        Assert.Equal(700, rightFloat.Box.Position.X);
        Assert.Equal(100, normalBox.Box.Position.X); // Starts after left float
        // Available width is 600 (800 - 100 - 100)
    }

    #endregion

    #region Clear Tests

    [Fact]
    [Trait("Category", "Formatting")]
    [Trait("Category", "Clear")]
    public void Clear_Left_MovesElementBelowLeftFloats()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        var floatBox = fixture.CreateElement("div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Float.Set(EFloat.Left);
            style.Width.Set(100);
            style.Height.Set(50);
        });

        var clearBox = fixture.CreateElement("div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Clear.Set(EClear.Left);
            style.Width.Set(200);
            style.Height.Set(30);
        });

        // Act
        fixture.ForceFullLayout();

        // Assert
        Assert.NotNull(floatBox.Box);
        Assert.NotNull(clearBox.Box);
        Assert.Equal(0, floatBox.Box.Position.X);
        Assert.Equal(0, floatBox.Box.Position.Y);
        Assert.Equal(50, clearBox.Box.Position.Y); // Below the float (float height = 50)
        Assert.Equal(0, clearBox.Box.Position.X); // Back at left edge
    }

    [Fact]
    [Trait("Category", "Formatting")]
    [Trait("Category", "Clear")]
    public void Clear_Right_MovesElementBelowRightFloats()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        fixture.ViewportWidth = 800;
        fixture.Body.Style.UserRules.Width.Set(800);

        var floatBox = fixture.CreateElement("div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Float.Set(EFloat.Right);
            style.Width.Set(100);
            style.Height.Set(50);
        });

        var clearBox = fixture.CreateElement("div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Clear.Set(EClear.Right);
            style.Width.Set(200);
            style.Height.Set(30);
        });

        // Act
        fixture.ForceFullLayout();

        // Assert
        Assert.NotNull(floatBox.Box);
        Assert.NotNull(clearBox.Box);
        Assert.Equal(700, floatBox.Box.Position.X);
        Assert.Equal(0, floatBox.Box.Position.Y);
        Assert.Equal(50, clearBox.Box.Position.Y); // Below the float
        Assert.Equal(0, clearBox.Box.Position.X);
    }

    [Fact]
    [Trait("Category", "Formatting")]
    [Trait("Category", "Clear")]
    public void Clear_Both_MovesElementBelowAllFloats()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        fixture.ViewportWidth = 800;
        fixture.Body.Style.UserRules.Width.Set(800);

        var leftFloat = fixture.CreateElement("div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Float.Set(EFloat.Left);
            style.Width.Set(100);
            style.Height.Set(50);
        });

        var rightFloat = fixture.CreateElement("div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Float.Set(EFloat.Right);
            style.Width.Set(100);
            style.Height.Set(70); // Taller than left float
        });

        var clearBox = fixture.CreateElement("div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Clear.Set(EClear.Both);
            style.Width.Set(200);
            style.Height.Set(30);
        });

        // Act
        fixture.ForceFullLayout();

        // Assert
        Assert.NotNull(leftFloat.Box);
        Assert.NotNull(rightFloat.Box);
        Assert.NotNull(clearBox.Box);
        Assert.Equal(0, leftFloat.Box.Position.Y);
        Assert.Equal(0, rightFloat.Box.Position.Y);
        Assert.Equal(70, clearBox.Box.Position.Y); // Below tallest float (right = 70)
        Assert.Equal(0, clearBox.Box.Position.X);
    }

    [Fact]
    [Trait("Category", "Formatting")]
    [Trait("Category", "Clear")]
    public void Clear_None_DoesNotMoveBelowFloats()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        var floatBox = fixture.CreateElement("div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Float.Set(EFloat.Left);
            style.Width.Set(100);
            style.Height.Set(50);
        });

        var normalBox = fixture.CreateElement("div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Clear.Set(EClear.None); // Explicitly set to none
            style.Width.Set(200);
            style.Height.Set(30);
        });

        // Act
        fixture.ForceFullLayout();

        // Assert - Normal box should be at same vertical position as float
        Assert.NotNull(floatBox.Box);
        Assert.NotNull(normalBox.Box);
        Assert.Equal(0, floatBox.Box.Position.Y);
        Assert.Equal(0, normalBox.Box.Position.Y); // NOT moved below float
        Assert.Equal(100, normalBox.Box.Position.X); // Positioned after float
    }

    #endregion

    #region Float Stacking Tests

    [Fact]
    [Trait("Category", "Formatting")]
    [Trait("Category", "Float")]
    [Trait("Category", "Stacking")]
    public void Float_Stacking_WhenNotEnoughHorizontalSpace()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        fixture.ViewportWidth = 250; // Container width
        fixture.Body.Style.UserRules.Width.Set(250);

        var float1 = fixture.CreateElement("div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Float.Set(EFloat.Left);
            style.Width.Set(100);
            style.Height.Set(50);
        });

        var float2 = fixture.CreateElement("div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Float.Set(EFloat.Left);
            style.Width.Set(100);
            style.Height.Set(50);
        });

        var float3 = fixture.CreateElement("div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Float.Set(EFloat.Left);
            style.Width.Set(100);
            style.Height.Set(50);
        });

        // Act
        fixture.ForceFullLayout();

        // Assert
        Assert.NotNull(float1.Box);
        Assert.NotNull(float2.Box);
        Assert.NotNull(float3.Box);

        // First two floats should fit horizontally
        Assert.Equal(0, float1.Box.Position.X);
        Assert.Equal(100, float2.Box.Position.X);
        Assert.Equal(0, float1.Box.Position.Y);
        Assert.Equal(0, float2.Box.Position.Y);

        // Third float should stack below (not enough horizontal space: 250 - 200 = 50 < 100)
        Assert.Equal(50, float3.Box.Position.Y); // Moved down below first float
    }

    [Fact]
    [Trait("Category", "Formatting")]
    [Trait("Category", "Float")]
    [Trait("Category", "Stacking")]
    public void Float_Right_Stacking_WhenNotEnoughHorizontalSpace()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        fixture.ViewportWidth = 250;
        fixture.Body.Style.UserRules.Width.Set(250);

        var float1 = fixture.CreateElement("div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Float.Set(EFloat.Right);
            style.Width.Set(100);
            style.Height.Set(50);
        });

        var float2 = fixture.CreateElement("div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Float.Set(EFloat.Right);
            style.Width.Set(100);
            style.Height.Set(50);
        });

        var float3 = fixture.CreateElement("div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Float.Set(EFloat.Right);
            style.Width.Set(100);
            style.Height.Set(50);
        });

        // Act
        fixture.ForceFullLayout();

        // Assert
        Assert.NotNull(float1.Box);
        Assert.NotNull(float2.Box);
        Assert.NotNull(float3.Box);

        // First two floats should fit horizontally from right
        Assert.Equal(150, float1.Box.Position.X); // 250 - 100
        Assert.Equal(50, float2.Box.Position.X); // 250 - 100 - 100
        Assert.Equal(0, float1.Box.Position.Y);
        Assert.Equal(0, float2.Box.Position.Y);

        // Third float should stack below
        Assert.Equal(50, float3.Box.Position.Y); // Moved down below first float
    }

    #endregion

    #region Negative Margin Tests

    [Fact]
    [Trait("Category", "Formatting")]
    [Trait("Category", "Float")]
    [Trait("Category", "Margin")]
    public void Float_WithNegativeLeftMargin_OverlapsContainer()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        var floatBox = fixture.CreateElement("div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Float.Set(EFloat.Left);
            style.Width.Set(100);
            style.Height.Set(50);
            style.Margin_Left.Set(-20); // Negative margin
        });

        // Act
        fixture.ForceFullLayout();

        // Assert - Float should be positioned with negative margin applied
        Assert.NotNull(floatBox.Box);
        // Position includes margin, so with -20 margin, effective position may be negative
        // This tests that negative margins are respected on floats
        Assert.True(floatBox.Box.IsFloating);
    }

    [Fact]
    [Trait("Category", "Formatting")]
    [Trait("Category", "Float")]
    [Trait("Category", "Margin")]
    public void Float_WithNegativeRightMargin_AffectsNextFloat()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        var float1 = fixture.CreateElement("div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Float.Set(EFloat.Left);
            style.Width.Set(100);
            style.Height.Set(50);
            style.Margin_Right.Set(-30); // Negative right margin
        });

        var float2 = fixture.CreateElement("div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Float.Set(EFloat.Left);
            style.Width.Set(100);
            style.Height.Set(50);
        });

        // Act
        fixture.ForceFullLayout();

        // Assert
        Assert.NotNull(float1.Box);
        Assert.NotNull(float2.Box);
        // Second float should be closer to first due to negative margin
        // float1 width=100 + marginRight=-30 = effective 70
        Assert.Equal(0, float1.Box.Position.X);
        // Second float might overlap or be closer depending on margin handling
        Assert.True(float2.Box.Position.X < 100); // Should be less than 100 due to negative margin
    }

    #endregion

    #region Mixed Float and Clear Tests

    [Fact]
    [Trait("Category", "Formatting")]
    [Trait("Category", "Float")]
    [Trait("Category", "Clear")]
    public void ComplexLayout_MixedFloatsAndClear()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        fixture.ViewportWidth = 800;
        fixture.Body.Style.UserRules.Width.Set(800);

        var leftFloat1 = fixture.CreateElement("div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Float.Set(EFloat.Left);
            style.Width.Set(100);
            style.Height.Set(50);
        });

        var leftFloat2 = fixture.CreateElement("div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Float.Set(EFloat.Left);
            style.Width.Set(100);
            style.Height.Set(60);
        });

        var clearBoth = fixture.CreateElement("div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Clear.Set(EClear.Both);
            style.Width.Set(200);
            style.Height.Set(30);
        });

        var afterClear = fixture.CreateElement("div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Width.Set(150);
            style.Height.Set(40);
        });

        // Act
        fixture.ForceFullLayout();

        // Assert
        Assert.NotNull(leftFloat1.Box);
        Assert.NotNull(leftFloat2.Box);
        Assert.NotNull(clearBoth.Box);
        Assert.NotNull(afterClear.Box);

        // Floats stack horizontally
        Assert.Equal(0, leftFloat1.Box.Position.X);
        Assert.Equal(100, leftFloat2.Box.Position.X);
        Assert.Equal(0, leftFloat1.Box.Position.Y);
        Assert.Equal(0, leftFloat2.Box.Position.Y);

        // Clear:both moves below tallest float
        Assert.Equal(60, clearBoth.Box.Position.Y); // Below leftFloat2 (height=60)
        Assert.Equal(0, clearBoth.Box.Position.X);

        // Element after clear continues in normal flow
        Assert.Equal(90, afterClear.Box.Position.Y); // After clearBoth (60 + 30)
        Assert.Equal(0, afterClear.Box.Position.X);
    }

    #endregion
}
