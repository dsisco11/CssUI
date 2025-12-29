using CssUI.CSS;
using CssUI.CSS.Enums;
using CssUITests.Fixtures;
using Xunit;

namespace CssUITests.CSS.Layout;

/// <summary>
/// Tests for LayoutTestFixture itself, ensuring the test infrastructure works.
/// </summary>
public class LayoutTestFixtureTests
{

    #region Fixture Creation Tests

    [Fact]
    [Trait("Category", "Layout")]
    [Trait("Category", "Fixture")]
    public void LayoutTestFixture_CreatesDocument()
    {
        // Arrange & Act
        using var fixture = new LayoutTestFixture();

        // Assert
        Assert.NotNull(fixture.Document);
    }

    [Fact]
    [Trait("Category", "Layout")]
    [Trait("Category", "Fixture")]
    public void LayoutTestFixture_CreatesDocumentElement()
    {
        // Arrange & Act
        using var fixture = new LayoutTestFixture();

        // Assert
        Assert.NotNull(fixture.DocumentElement);
        Assert.Equal("html", fixture.DocumentElement.localName);
    }

    [Fact]
    [Trait("Category", "Layout")]
    [Trait("Category", "Fixture")]
    public void LayoutTestFixture_CreatesBody()
    {
        // Arrange & Act
        using var fixture = new LayoutTestFixture();

        // Assert
        Assert.NotNull(fixture.Body);
        Assert.Equal("body", fixture.Body.localName);
    }

    [Fact]
    [Trait("Category", "Layout")]
    [Trait("Category", "Fixture")]
    public void LayoutTestFixture_DocumentElementHasBlockDisplaySet()
    {
        // Arrange & Act
        using var fixture = new LayoutTestFixture();

        // Assert - UserRules has BLOCK set
        Assert.Equal(EDisplayMode.BLOCK, fixture.DocumentElement.Style.UserRules.Display.Assigned.AsEnum<EDisplayMode>());
    }

    [Fact]
    [Trait("Category", "Layout")]
    [Trait("Category", "Fixture")]
    public void LayoutTestFixture_BodyHasBlockDisplaySet()
    {
        // Arrange & Act
        using var fixture = new LayoutTestFixture();

        // Assert - UserRules has BLOCK set
        Assert.Equal(EDisplayMode.BLOCK, fixture.Body.Style.UserRules.Display.Assigned.AsEnum<EDisplayMode>());
    }

    #endregion

    #region Element Creation Tests

    [Fact]
    [Trait("Category", "Layout")]
    [Trait("Category", "Fixture")]
    public void CreateElement_CreatesElementWithTagName()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();

        // Act
        var element = fixture.CreateElement("div");

        // Assert
        Assert.NotNull(element);
        Assert.Equal("div", element.localName);
    }

    [Fact]
    [Trait("Category", "Layout")]
    [Trait("Category", "Fixture")]
    public void CreateElement_AppendsToBody()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();

        // Act
        var element = fixture.CreateElement("div");

        // Assert
        Assert.Same(fixture.Body, element.parentElement);
    }

    [Fact]
    [Trait("Category", "Layout")]
    [Trait("Category", "Fixture")]
    public void CreateElement_AppliesStyleConfiguration()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();

        // Act
        var element = fixture.CreateElement("div", style =>
        {
            style.Width.Set(200);
            style.Height.Set(100);
        });

        // Assert - Check that UserRules has the values set
        Assert.Equal(200, element.Style.UserRules.Width.Assigned.AsInteger());
        Assert.Equal(100, element.Style.UserRules.Height.Assigned.AsInteger());
    }

    [Fact]
    [Trait("Category", "Layout")]
    [Trait("Category", "Fixture")]
    public void CreateBlock_CreatesBlockElement()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();

        // Act
        var element = fixture.CreateBlock(200, 100);

        // Assert
        Assert.Equal(EDisplayMode.BLOCK, element.Style.UserRules.Display.Assigned.AsEnum<EDisplayMode>());
    }

    [Fact]
    [Trait("Category", "Layout")]
    [Trait("Category", "Fixture")]
    public void CreateInlineBlock_CreatesInlineBlockElement()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();

        // Act
        var element = fixture.CreateInlineBlock(200, 100);

        // Assert
        Assert.Equal(EDisplayMode.INLINE_BLOCK, element.Style.UserRules.Display.Assigned.AsEnum<EDisplayMode>());
    }

    [Fact]
    [Trait("Category", "Layout")]
    [Trait("Category", "Fixture")]
    public void CreateFlexContainer_CreatesFlexContainer()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();

        // Act
        var element = fixture.CreateFlexContainer();

        // Assert
        Assert.Equal(EDisplayMode.FLEX, element.Style.UserRules.Display.Assigned.AsEnum<EDisplayMode>());
    }

    [Fact]
    [Trait("Category", "Layout")]
    [Trait("Category", "Fixture")]
    public void CreateGridContainer_CreatesGridContainer()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();

        // Act
        var element = fixture.CreateGridContainer();

        // Assert
        Assert.Equal(EDisplayMode.GRID, element.Style.UserRules.Display.Assigned.AsEnum<EDisplayMode>());
    }

    [Fact]
    [Trait("Category", "Layout")]
    [Trait("Category", "Fixture")]
    public void CreateChild_AppendsToParent()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        var parent = fixture.CreateElement("div");

        // Act
        var child = fixture.CreateChild(parent, "span");

        // Assert
        Assert.Same(parent, child.parentElement);
    }

    #endregion

    #region Cascade Tests

    [Fact]
    [Trait("Category", "Layout")]
    [Trait("Category", "Fixture")]
    public void ForceCascade_UpdatesCascadedValues()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        var element = fixture.CreateElement("div", style =>
        {
            style.Width.Set(300);
        });

        // Act
        fixture.ForceCascade();

        // Assert - After cascade, the UserRules assigned value should be set
        Assert.Equal(300, element.Style.UserRules.Width.Assigned.AsInteger());
    }

    #endregion

    #region Containing Block Tests

    [Fact]
    [Trait("Category", "Layout")]
    [Trait("Category", "Fixture")]
    public void CreateContainingBlock_CreatesPositionedElement()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();

        // Act
        var cb = fixture.CreateContainingBlock(400, 300);

        // Assert
        Assert.Equal(EBoxPositioning.Relative, cb.Style.UserRules.Positioning.Assigned.AsEnum<EBoxPositioning>());
    }

    [Fact]
    [Trait("Category", "Layout")]
    [Trait("Category", "Fixture")]
    public void CreateAbsolutelyPositioned_CreatesAbsoluteElement()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        var cb = fixture.CreateContainingBlock(400, 300);

        // Act
        var positioned = fixture.CreateAbsolutelyPositioned(cb, style =>
        {
            style.Top.Set(10);
            style.Left.Set(20);
        });

        // Assert
        Assert.Equal(EBoxPositioning.Absolute, positioned.Style.UserRules.Positioning.Assigned.AsEnum<EBoxPositioning>());
        Assert.Same(cb, positioned.parentElement);
    }

    #endregion

    #region Margin Helper Tests

    [Fact]
    [Trait("Category", "Layout")]
    [Trait("Category", "Fixture")]
    public void CreateBlockWithMargins_SetsAllMargins()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();

        // Act
        var element = fixture.CreateBlockWithMargins(100, 50, 10, 20, 30, 40);

        // Assert
        Assert.Equal(10, element.Style.UserRules.Margin_Top.Assigned.AsInteger());
        Assert.Equal(20, element.Style.UserRules.Margin_Right.Assigned.AsInteger());
        Assert.Equal(30, element.Style.UserRules.Margin_Bottom.Assigned.AsInteger());
        Assert.Equal(40, element.Style.UserRules.Margin_Left.Assigned.AsInteger());
    }

    [Fact]
    [Trait("Category", "Layout")]
    [Trait("Category", "Fixture")]
    public void CreateCenteredBlock_SetsAutoMargins()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();

        // Act
        var element = fixture.CreateCenteredBlock(200, 100);

        // Assert
        Assert.True(element.Style.UserRules.Margin_Left.Assigned.IsAuto);
        Assert.True(element.Style.UserRules.Margin_Right.Assigned.IsAuto);
    }

    #endregion

    #region Box Generation Tests

    [Fact]
    [Trait("Category", "Layout")]
    [Trait("Category", "Fixture")]
    public void ForceBoxGeneration_GeneratesBoxForDocumentElement()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();

        // Act
        fixture.ForceBoxGeneration();

        // Assert
        Assert.NotNull(fixture.DocumentElement.Box);
    }

    [Fact]
    [Trait("Category", "Layout")]
    [Trait("Category", "Fixture")]
    public void ForceLayoutUpdate_GeneratesBoxForCreatedElements()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        var element = fixture.CreateBlock(200, 100);

        // Act
        fixture.ForceLayoutUpdate();

        // Assert
        Assert.NotNull(element.Box);
    }

    [Fact]
    [Trait("Category", "Layout")]
    [Trait("Category", "Fixture")]
    public void GetBox_GeneratesBoxIfNotExists()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        var element = fixture.CreateBlock(200, 100);

        // Act
        var box = fixture.GetBox(element);

        // Assert
        Assert.NotNull(box);
    }

    [Fact]
    [Trait("Category", "Layout")]
    [Trait("Category", "Fixture")]
    public void RequireBox_ThrowsForDisplayNone()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        var element = fixture.CreateElement("div", style =>
        {
            style.Display.Set(EDisplayMode.NONE);
        });

        // Act & Assert
        Assert.Throws<System.InvalidOperationException>(() => fixture.RequireBox(element));
    }

    #endregion
}
