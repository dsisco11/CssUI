using CssUI.CSS;
using CssUI.CSS.BoxTree;
using CssUI.CSS.Enums;
using CssUI.DOM;
using CssUITests.Fixtures;
using Xunit;

namespace CssUITests.CSS.BoxModel;

/// <summary>
/// Tests for multiple box generation per CSS Display 3 §2.2.
/// Specifically tests table element generation of wrapper + grid boxes.
/// Spec: https://www.w3.org/TR/css-display-3/#valdef-display-table
/// </summary>
[Trait("Category", "MultipleBoxGeneration")]
[Trait("Category", "Table")]
[Trait("Category", "BoxGeneration")]
public class MultipleBoxGenerationTests
{
    private static Element CreateTableElement(LayoutTestFixture fixture, Element parent)
    {
        var element = fixture.Document.createElement("table", new ElementCreationOptions(string.Empty));
        parent.appendChild(element);
        element.Style.UserRules.Display.Set(EDisplayMode.TABLE);
        return element;
    }

    private static Element CreateElement(LayoutTestFixture fixture, string tagName, Element parent, EDisplayMode display = EDisplayMode.BLOCK)
    {
        var element = fixture.Document.createElement(tagName, new ElementCreationOptions(string.Empty));
        parent.appendChild(element);
        element.Style.UserRules.Display.Set(display);
        return element;
    }

    #region Table Wrapper + Grid Box Generation Tests

    [Fact]
    public void Table_GeneratesWrapperBox()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        var table = CreateTableElement(fixture, fixture.Body);

        // Act
        fixture.ForceLayoutUpdate();

        // Assert
        Assert.NotNull(table.Box);
        Assert.IsType<CssTableWrapperBox>(table.Box);
    }

    [Fact]
    public void Table_WrapperBox_ContainsGridBox()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        var table = CreateTableElement(fixture, fixture.Body);

        // Act
        fixture.ForceLayoutUpdate();

        // Assert
        Assert.NotNull(table.Box);
        var wrapperBox = Assert.IsType<CssTableWrapperBox>(table.Box);
        Assert.NotNull(wrapperBox.GridBox);
        Assert.IsType<CssTableGridBox>(wrapperBox.GridBox);
    }

    [Fact]
    public void Table_GridBox_IsChildOfWrapper()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        var table = CreateTableElement(fixture, fixture.Body);

        // Act
        fixture.ForceLayoutUpdate();

        // Assert
        Assert.NotNull(table.Box);
        var wrapperBox = Assert.IsType<CssTableWrapperBox>(table.Box);
        Assert.Contains(wrapperBox.GridBox, wrapperBox.childNodes);
        Assert.Equal(wrapperBox, wrapperBox.GridBox?.parentNode);
    }

    [Fact]
    public void Table_WrapperBox_HasBlockOuterDisplay()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        var table = CreateTableElement(fixture, fixture.Body);

        // Act
        fixture.ForceLayoutUpdate();

        // Assert
        Assert.NotNull(table.Box);
        var wrapperBox = Assert.IsType<CssTableWrapperBox>(table.Box);
        Assert.Equal(EOuterDisplayType.Block, wrapperBox.DisplayType.Outer);
    }

    [Fact]
    public void Table_WrapperBox_EstablishesBlockFormattingContext()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        var table = CreateTableElement(fixture, fixture.Body);

        // Act
        fixture.ForceLayoutUpdate();

        // Assert
        Assert.NotNull(table.Box);
        var wrapperBox = Assert.IsType<CssTableWrapperBox>(table.Box);
        // flow-root inner display type establishes a BFC
        Assert.Equal(EInnerDisplayType.Flow_Root, wrapperBox.DisplayType.Inner);
    }

    [Fact]
    public void Table_GridBox_HasTableInnerDisplay()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        var table = CreateTableElement(fixture, fixture.Body);

        // Act
        fixture.ForceLayoutUpdate();

        // Assert
        Assert.NotNull(table.Box);
        var wrapperBox = Assert.IsType<CssTableWrapperBox>(table.Box);
        Assert.NotNull(wrapperBox.GridBox);
        Assert.Equal(EInnerDisplayType.Table, wrapperBox.GridBox.DisplayType.Inner);
    }

    #endregion

    #region Table Children Routing Tests

    [Fact]
    public void Table_Children_AddedToGridBox_NotWrapper()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        var table = CreateTableElement(fixture, fixture.Body);
        var row = CreateElement(fixture, "tr", table, EDisplayMode.TABLE_ROW);

        // Act
        fixture.ForceLayoutUpdate();

        // Assert
        Assert.NotNull(table.Box);
        var wrapperBox = Assert.IsType<CssTableWrapperBox>(table.Box);
        Assert.NotNull(wrapperBox.GridBox);

        // The grid box should be the only child of the wrapper
        Assert.Single(wrapperBox.childNodes);
        Assert.Equal(wrapperBox.GridBox, wrapperBox.firstChild);

        // The row's box should be a child of the grid box, not the wrapper
        Assert.NotNull(row.Box);
        Assert.Contains(row.Box, wrapperBox.GridBox.childNodes);
        Assert.Equal(wrapperBox.GridBox, row.Box.parentNode);
    }

    [Fact]
    public void Table_MultipleChildren_AllAddedToGridBox()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        var table = CreateTableElement(fixture, fixture.Body);
        var row1 = CreateElement(fixture, "tr", table, EDisplayMode.TABLE_ROW);
        var row2 = CreateElement(fixture, "tr", table, EDisplayMode.TABLE_ROW);
        var row3 = CreateElement(fixture, "tr", table, EDisplayMode.TABLE_ROW);

        // Act
        fixture.ForceLayoutUpdate();

        // Assert
        Assert.NotNull(table.Box);
        var wrapperBox = Assert.IsType<CssTableWrapperBox>(table.Box);
        Assert.NotNull(wrapperBox.GridBox);

        // All three rows should be children of the grid box
        Assert.Equal(3, wrapperBox.GridBox.childNodes.Count);
        Assert.Contains(row1.Box, wrapperBox.GridBox.childNodes);
        Assert.Contains(row2.Box, wrapperBox.GridBox.childNodes);
        Assert.Contains(row3.Box, wrapperBox.GridBox.childNodes);
    }

    #endregion

    #region Originating Element Tests

    [Fact]
    public void Table_WrapperBox_HasOriginatingElement()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        var table = CreateTableElement(fixture, fixture.Body);

        // Act
        fixture.ForceLayoutUpdate();

        // Assert
        Assert.NotNull(table.Box);
        var wrapperBox = Assert.IsType<CssTableWrapperBox>(table.Box);
        Assert.Equal(table, wrapperBox.OriginatingElement);
    }

    [Fact]
    public void Table_GridBox_HasOriginatingElement()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        var table = CreateTableElement(fixture, fixture.Body);

        // Act
        fixture.ForceLayoutUpdate();

        // Assert
        Assert.NotNull(table.Box);
        var wrapperBox = Assert.IsType<CssTableWrapperBox>(table.Box);
        Assert.NotNull(wrapperBox.GridBox);
        Assert.Equal(table, wrapperBox.GridBox.OriginatingElement);
    }

    #endregion

    #region Non-Table Elements Tests

    [Fact]
    public void NonTableElement_DoesNotGenerateTableBoxes()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        var div = CreateElement(fixture, "div", fixture.Body, EDisplayMode.BLOCK);

        // Act
        fixture.ForceLayoutUpdate();

        // Assert
        Assert.NotNull(div.Box);
        Assert.IsNotType<CssTableWrapperBox>(div.Box);
        Assert.IsType<CssPrincipalBox>(div.Box);
    }

    [Fact]
    public void FlexElement_DoesNotGenerateTableBoxes()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        var flex = CreateElement(fixture, "div", fixture.Body, EDisplayMode.FLEX);

        // Act
        fixture.ForceLayoutUpdate();

        // Assert
        Assert.NotNull(flex.Box);
        Assert.IsNotType<CssTableWrapperBox>(flex.Box);
        Assert.IsType<CssPrincipalBox>(flex.Box);
    }

    [Fact]
    public void GridElement_DoesNotGenerateTableBoxes()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        var grid = CreateElement(fixture, "div", fixture.Body, EDisplayMode.GRID);

        // Act
        fixture.ForceLayoutUpdate();

        // Assert
        Assert.NotNull(grid.Box);
        Assert.IsNotType<CssTableWrapperBox>(grid.Box);
        Assert.IsType<CssPrincipalBox>(grid.Box);
    }

    #endregion

    #region Box Tree Structure Tests

    [Fact]
    public void Table_BoxTreeStructure_IsCorrect()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        var table = CreateTableElement(fixture, fixture.Body);
        var row = CreateElement(fixture, "tr", table, EDisplayMode.TABLE_ROW);
        var cell = CreateElement(fixture, "td", row, EDisplayMode.TABLE_CELL);

        // Act
        fixture.ForceLayoutUpdate();

        // Assert
        Assert.NotNull(table.Box);
        var wrapperBox = Assert.IsType<CssTableWrapperBox>(table.Box);
        Assert.NotNull(wrapperBox.GridBox);

        // Verify structure: wrapper -> grid -> row -> cell
        Assert.Equal(fixture.Body.Box, wrapperBox.parentNode);
        Assert.Equal(wrapperBox, wrapperBox.GridBox.parentNode);
        Assert.NotNull(row.Box);
        Assert.Equal(wrapperBox.GridBox, row.Box.parentNode);
        Assert.NotNull(cell.Box);
        Assert.Equal(row.Box, cell.Box.parentNode);
    }

    #endregion

    #region ToString Tests (for debugging)

    [Fact]
    public void Table_WrapperBox_ToStringReturnsDescriptiveName()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        var table = CreateTableElement(fixture, fixture.Body);

        // Act
        fixture.ForceLayoutUpdate();

        // Assert
        Assert.NotNull(table.Box);
        var wrapperBox = Assert.IsType<CssTableWrapperBox>(table.Box);
        var toString = wrapperBox.ToString();
        Assert.Contains("TableWrapperBox", toString);
        Assert.Contains("table", toString.ToLowerInvariant());
    }

    [Fact]
    public void Table_GridBox_ToStringReturnsDescriptiveName()
    {
        // Arrange
        using var fixture = new LayoutTestFixture();
        var table = CreateTableElement(fixture, fixture.Body);

        // Act
        fixture.ForceLayoutUpdate();

        // Assert
        Assert.NotNull(table.Box);
        var wrapperBox = Assert.IsType<CssTableWrapperBox>(table.Box);
        Assert.NotNull(wrapperBox.GridBox);
        var toString = wrapperBox.GridBox.ToString();
        Assert.Contains("TableGridBox", toString);
        Assert.Contains("table", toString.ToLowerInvariant());
    }

    #endregion
}
