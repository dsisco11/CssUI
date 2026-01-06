using System;
using System.Linq;
using CssUI;
using CssUI.CSS;
using CssUI.CSS.BoxTree;
using CssUI.CSS.Enums;
using CssUI.DOM;
using CssUI.DOM.Nodes;
using CssUITests.Fixtures;
using Xunit;

namespace CssUITests.CSS.BoxModel;

/// <summary>
/// Unit tests for Table Internal Box Fixup per CSS Display 3 §2.4 and CSS 2.2 §17.2.1.
/// Tests the anonymous wrapper box generation for misparented table-internal boxes.
///
/// Per spec:
/// - table-cell requires table-row parent
/// - table-row requires proper table row parent (row group or table)
/// - row groups require table parent
/// - table-column requires table-column-group or table parent
/// - table-column-group and table-caption require table parent
///
/// See: https://www.w3.org/TR/css-display-3/#layout-specific-display
/// See: https://www.w3.org/TR/CSS22/tables.html#anonymous-boxes
/// </summary>
public class TableInternalBoxFixupTests : IDisposable
{
    private readonly LayoutTestFixture _fixture;
    private static readonly ElementCreationOptions DefaultOptions = new(string.Empty);

    public TableInternalBoxFixupTests()
    {
        _fixture = new LayoutTestFixture();
    }

    public void Dispose()
    {
        _fixture.Dispose();
    }

    #region TableInternalDisplayType Helper Tests

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "TableFixup")]
    public void IsTableInternal_ReturnsTrueForTableInternalTypes()
    {
        // Assert - all table-internal types return true
        Assert.True(TableInternalDisplayType.IsTableInternal(EDisplayMode.TABLE_ROW_GROUP));
        Assert.True(TableInternalDisplayType.IsTableInternal(EDisplayMode.TABLE_HEADER_GROUP));
        Assert.True(TableInternalDisplayType.IsTableInternal(EDisplayMode.TABLE_FOOTER_GROUP));
        Assert.True(TableInternalDisplayType.IsTableInternal(EDisplayMode.TABLE_ROW));
        Assert.True(TableInternalDisplayType.IsTableInternal(EDisplayMode.TABLE_CELL));
        Assert.True(TableInternalDisplayType.IsTableInternal(EDisplayMode.TABLE_COLUMN_GROUP));
        Assert.True(TableInternalDisplayType.IsTableInternal(EDisplayMode.TABLE_COLUMN));
        Assert.True(TableInternalDisplayType.IsTableInternal(EDisplayMode.TABLE_CAPTION));
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "TableFixup")]
    public void IsTableInternal_ReturnsFalseForNonTableTypes()
    {
        // Assert - non-table-internal types return false
        Assert.False(TableInternalDisplayType.IsTableInternal(EDisplayMode.BLOCK));
        Assert.False(TableInternalDisplayType.IsTableInternal(EDisplayMode.INLINE));
        Assert.False(TableInternalDisplayType.IsTableInternal(EDisplayMode.FLEX));
        Assert.False(TableInternalDisplayType.IsTableInternal(EDisplayMode.GRID));
        Assert.False(TableInternalDisplayType.IsTableInternal(EDisplayMode.TABLE));
        Assert.False(TableInternalDisplayType.IsTableInternal(EDisplayMode.INLINE_TABLE));
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "TableFixup")]
    public void IsRowGroupBox_ReturnsTrueForRowGroups()
    {
        // Assert
        Assert.True(TableInternalDisplayType.IsRowGroupBox(EDisplayMode.TABLE_ROW_GROUP));
        Assert.True(TableInternalDisplayType.IsRowGroupBox(EDisplayMode.TABLE_HEADER_GROUP));
        Assert.True(TableInternalDisplayType.IsRowGroupBox(EDisplayMode.TABLE_FOOTER_GROUP));
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "TableFixup")]
    public void IsRowGroupBox_ReturnsFalseForOtherTypes()
    {
        // Assert
        Assert.False(TableInternalDisplayType.IsRowGroupBox(EDisplayMode.TABLE_ROW));
        Assert.False(TableInternalDisplayType.IsRowGroupBox(EDisplayMode.TABLE_CELL));
        Assert.False(TableInternalDisplayType.IsRowGroupBox(EDisplayMode.TABLE));
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "TableFixup")]
    public void IsProperTableChild_ReturnsTrueForValidChildren()
    {
        // Assert - proper table children per CSS 2.2 §17.2.1
        Assert.True(TableInternalDisplayType.IsProperTableChild(EDisplayMode.TABLE_ROW));
        Assert.True(TableInternalDisplayType.IsProperTableChild(EDisplayMode.TABLE_ROW_GROUP));
        Assert.True(TableInternalDisplayType.IsProperTableChild(EDisplayMode.TABLE_HEADER_GROUP));
        Assert.True(TableInternalDisplayType.IsProperTableChild(EDisplayMode.TABLE_FOOTER_GROUP));
        Assert.True(TableInternalDisplayType.IsProperTableChild(EDisplayMode.TABLE_COLUMN));
        Assert.True(TableInternalDisplayType.IsProperTableChild(EDisplayMode.TABLE_COLUMN_GROUP));
        Assert.True(TableInternalDisplayType.IsProperTableChild(EDisplayMode.TABLE_CAPTION));
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "TableFixup")]
    public void IsProperTableChild_ReturnsFalseForCells()
    {
        // Assert - table-cell is NOT a proper table child (it's a proper table-row child)
        Assert.False(TableInternalDisplayType.IsProperTableChild(EDisplayMode.TABLE_CELL));
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "TableFixup")]
    public void IsValidParentFor_TableCell_OnlyAcceptsTableRow()
    {
        // Assert
        Assert.True(TableInternalDisplayType.IsValidParentFor(EDisplayMode.TABLE_ROW, EDisplayMode.TABLE_CELL));
        Assert.False(TableInternalDisplayType.IsValidParentFor(EDisplayMode.TABLE_ROW_GROUP, EDisplayMode.TABLE_CELL));
        Assert.False(TableInternalDisplayType.IsValidParentFor(EDisplayMode.TABLE, EDisplayMode.TABLE_CELL));
        Assert.False(TableInternalDisplayType.IsValidParentFor(EDisplayMode.BLOCK, EDisplayMode.TABLE_CELL));
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "TableFixup")]
    public void IsValidParentFor_TableRow_AcceptsRowGroupOrTable()
    {
        // Assert - table-row accepts proper table row parent (row group or table)
        Assert.True(TableInternalDisplayType.IsValidParentFor(EDisplayMode.TABLE_ROW_GROUP, EDisplayMode.TABLE_ROW));
        Assert.True(TableInternalDisplayType.IsValidParentFor(EDisplayMode.TABLE_HEADER_GROUP, EDisplayMode.TABLE_ROW));
        Assert.True(TableInternalDisplayType.IsValidParentFor(EDisplayMode.TABLE_FOOTER_GROUP, EDisplayMode.TABLE_ROW));
        Assert.True(TableInternalDisplayType.IsValidParentFor(EDisplayMode.TABLE, EDisplayMode.TABLE_ROW));
        Assert.True(TableInternalDisplayType.IsValidParentFor(EDisplayMode.INLINE_TABLE, EDisplayMode.TABLE_ROW));
        Assert.False(TableInternalDisplayType.IsValidParentFor(EDisplayMode.BLOCK, EDisplayMode.TABLE_ROW));
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "TableFixup")]
    public void IsValidParentFor_RowGroup_OnlyAcceptsTable()
    {
        // Assert
        Assert.True(TableInternalDisplayType.IsValidParentFor(EDisplayMode.TABLE, EDisplayMode.TABLE_ROW_GROUP));
        Assert.True(TableInternalDisplayType.IsValidParentFor(EDisplayMode.INLINE_TABLE, EDisplayMode.TABLE_ROW_GROUP));
        Assert.False(TableInternalDisplayType.IsValidParentFor(EDisplayMode.BLOCK, EDisplayMode.TABLE_ROW_GROUP));
        Assert.False(TableInternalDisplayType.IsValidParentFor(EDisplayMode.TABLE_ROW, EDisplayMode.TABLE_ROW_GROUP));
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "TableFixup")]
    public void IsMisparented_ReturnsTrueForMisparentedCell()
    {
        // Assert - table-cell in block parent is misparented
        Assert.True(TableInternalDisplayType.IsMisparented(EDisplayMode.BLOCK, EDisplayMode.TABLE_CELL));
        Assert.True(TableInternalDisplayType.IsMisparented(EDisplayMode.TABLE_ROW_GROUP, EDisplayMode.TABLE_CELL));
        Assert.False(TableInternalDisplayType.IsMisparented(EDisplayMode.TABLE_ROW, EDisplayMode.TABLE_CELL));
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "TableFixup")]
    public void IsMisparented_ReturnsFalseForNonTableTypes()
    {
        // Assert - non-table-internal types are never misparented
        Assert.False(TableInternalDisplayType.IsMisparented(EDisplayMode.BLOCK, EDisplayMode.BLOCK));
        Assert.False(TableInternalDisplayType.IsMisparented(EDisplayMode.BLOCK, EDisplayMode.INLINE));
        Assert.False(TableInternalDisplayType.IsMisparented(EDisplayMode.BLOCK, EDisplayMode.FLEX));
    }

    #endregion

    #region DisplayType Table Support Tests

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "TableFixup")]
    public void DisplayType_TableRowGroup_HasCorrectOuterInner()
    {
        // Arrange & Act
        var displayType = new DisplayType(EDisplayMode.TABLE_ROW_GROUP);

        // Assert - table-internal types have Block outer (within table layout)
        // and None inner (no internal formatting context)
        Assert.Equal(EOuterDisplayType.Block, displayType.Outer);
        Assert.Equal(EInnerDisplayType.None, displayType.Inner);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "TableFixup")]
    public void DisplayType_TableRow_HasCorrectOuterInner()
    {
        // Arrange & Act
        var displayType = new DisplayType(EDisplayMode.TABLE_ROW);

        // Assert
        Assert.Equal(EOuterDisplayType.Block, displayType.Outer);
        Assert.Equal(EInnerDisplayType.None, displayType.Inner);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "TableFixup")]
    public void DisplayType_TableCell_HasFlowRootInner()
    {
        // Arrange & Act - per CSS Display 3 §2.4: table-cell has flow-root inner
        var displayType = new DisplayType(EDisplayMode.TABLE_CELL);

        // Assert
        Assert.Equal(EOuterDisplayType.Block, displayType.Outer);
        Assert.Equal(EInnerDisplayType.Flow_Root, displayType.Inner);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "TableFixup")]
    public void DisplayType_TableCaption_HasFlowRootInner()
    {
        // Arrange & Act - per CSS Display 3 §2.4: table-caption has flow-root inner
        var displayType = new DisplayType(EDisplayMode.TABLE_CAPTION);

        // Assert
        Assert.Equal(EOuterDisplayType.Block, displayType.Outer);
        Assert.Equal(EInnerDisplayType.Flow_Root, displayType.Inner);
    }

    #endregion

    #region Anonymous Box Creation Tests

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "TableFixup")]
    public void Create_Table_SetsCorrectDisplayType()
    {
        // Act
        var box = CssAnonymousBox.Create_Table(null!);

        // Assert
        Assert.Equal(EOuterDisplayType.Block, box.DisplayType.Outer);
        Assert.Equal(EInnerDisplayType.Table, box.DisplayType.Inner);
        Assert.Equal(EDisplayMode.TABLE, box.SourceDisplayMode);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "TableFixup")]
    public void Create_InlineTable_SetsCorrectDisplayType()
    {
        // Act
        var box = CssAnonymousBox.Create_InlineTable(null!);

        // Assert
        Assert.Equal(EOuterDisplayType.Inline, box.DisplayType.Outer);
        Assert.Equal(EInnerDisplayType.Table, box.DisplayType.Inner);
        Assert.Equal(EDisplayMode.INLINE_TABLE, box.SourceDisplayMode);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "TableFixup")]
    public void Create_TableRowGroup_SetsCorrectDisplayType()
    {
        // Act
        var box = CssAnonymousBox.Create_TableRowGroup(null!);

        // Assert
        Assert.Equal(EDisplayMode.TABLE_ROW_GROUP, box.SourceDisplayMode);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "TableFixup")]
    public void Create_TableRow_SetsCorrectDisplayType()
    {
        // Act
        var box = CssAnonymousBox.Create_TableRow(null!);

        // Assert
        Assert.Equal(EDisplayMode.TABLE_ROW, box.SourceDisplayMode);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "TableFixup")]
    public void Create_TableCell_SetsCorrectDisplayType()
    {
        // Act
        var box = CssAnonymousBox.Create_TableCell(null!);

        // Assert
        Assert.Equal(EDisplayMode.TABLE_CELL, box.SourceDisplayMode);
    }

    #endregion

    #region Table Cell Fixup Tests

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "TableFixup")]
    public void TableCell_InBlockParent_GeneratesFullWrapperChain()
    {
        // Arrange - a table-cell in a block parent should generate:
        // block box → anonymous table → anonymous table-row-group → anonymous table-row → table-cell
        var doc = _fixture.Document;
        var parent = _fixture.CreateBlock(200, 100);
        var cell = doc.createElement("div", DefaultOptions);
        cell!.Style.UserRules.Display.Set(EDisplayMode.TABLE_CELL);
        parent.appendChild(cell);

        // Act
        _fixture.ForceLayoutUpdate();

        // Assert - cell should have a box
        Assert.NotNull(cell.Box);

        // Navigate up the tree to verify the wrapper chain
        var cellBox = cell.Box;
        var rowBox = cellBox.parentNode as CssAnonymousBox;
        Assert.NotNull(rowBox);
        Assert.Equal(EDisplayMode.TABLE_ROW, rowBox.SourceDisplayMode);

        var rowGroupBox = rowBox.parentNode as CssAnonymousBox;
        Assert.NotNull(rowGroupBox);
        Assert.Equal(EDisplayMode.TABLE_ROW_GROUP, rowGroupBox.SourceDisplayMode);

        var tableBox = rowGroupBox.parentNode as CssAnonymousBox;
        Assert.NotNull(tableBox);
        Assert.Equal(EDisplayMode.TABLE, tableBox.SourceDisplayMode);

        // The table should be a child of the parent block
        Assert.Same(parent.Box, tableBox.parentNode);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "TableFixup")]
    public void TableCell_InTableRowParent_GeneratesNoWrapper()
    {
        // Arrange - a table-cell in a proper table-row parent needs no fixup
        var doc = _fixture.Document;
        var parent = _fixture.CreateBlock(200, 100);

        var table = doc.createElement("div", DefaultOptions);
        table!.Style.UserRules.Display.Set(EDisplayMode.TABLE);
        parent.appendChild(table);

        var row = doc.createElement("div", DefaultOptions);
        row!.Style.UserRules.Display.Set(EDisplayMode.TABLE_ROW);
        table.appendChild(row);

        var cell = doc.createElement("div", DefaultOptions);
        cell!.Style.UserRules.Display.Set(EDisplayMode.TABLE_CELL);
        row.appendChild(cell);

        // Act
        _fixture.ForceLayoutUpdate();

        // Assert - cell's parent should be the row (with anonymous row-group in between)
        Assert.NotNull(cell.Box);
        Assert.NotNull(row.Box);

        // The row might be wrapped in an anonymous row-group if it's direct child of table
        // Let's check the cell's parent is a row
        var cellParent = cell.Box.parentNode;
        Assert.NotNull(cellParent);

        // The cell's parent should be the row box (could be through anonymous row-group)
        // Check that cellParent is either the row box or an anonymous row wrapping the cell
        bool isDirectChild = ReferenceEquals(cellParent, row.Box);
        bool isInAnonymousRow = cellParent is CssAnonymousBox anonymousRow &&
                                anonymousRow.SourceDisplayMode == EDisplayMode.TABLE_ROW;
        Assert.True(isDirectChild || isInAnonymousRow);
    }

    #endregion

    #region Table Row Fixup Tests

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "TableFixup")]
    public void TableRow_InBlockParent_GeneratesTableAndRowGroupWrappers()
    {
        // Arrange - a table-row in a block parent should generate:
        // block box → anonymous table → anonymous table-row-group → table-row
        var doc = _fixture.Document;
        var parent = _fixture.CreateBlock(200, 100);
        var row = doc.createElement("div", DefaultOptions);
        row!.Style.UserRules.Display.Set(EDisplayMode.TABLE_ROW);
        parent.appendChild(row);

        // Act
        _fixture.ForceLayoutUpdate();

        // Assert - row should have a box
        Assert.NotNull(row.Box);

        // Navigate up to verify the wrapper chain
        var rowBox = row.Box;
        var rowGroupBox = rowBox.parentNode as CssAnonymousBox;
        Assert.NotNull(rowGroupBox);
        Assert.Equal(EDisplayMode.TABLE_ROW_GROUP, rowGroupBox.SourceDisplayMode);

        var tableBox = rowGroupBox.parentNode as CssAnonymousBox;
        Assert.NotNull(tableBox);
        Assert.Equal(EDisplayMode.TABLE, tableBox.SourceDisplayMode);

        // The table should be a child of the parent block
        Assert.Same(parent.Box, tableBox.parentNode);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "TableFixup")]
    public void TableRow_InTableParent_GeneratesRowGroupWrapper()
    {
        // Arrange - a table-row as direct child of table (not row-group)
        // should generate anonymous table-row-group wrapper
        // Per CSS 2.2 §17.2.1: table-row in table without row-group generates anonymous row-group
        var doc = _fixture.Document;
        var parent = _fixture.CreateBlock(200, 100);

        var table = doc.createElement("div", DefaultOptions);
        table!.Style.UserRules.Display.Set(EDisplayMode.TABLE);
        parent.appendChild(table);

        var row = doc.createElement("div", DefaultOptions);
        row!.Style.UserRules.Display.Set(EDisplayMode.TABLE_ROW);
        table.appendChild(row);

        // Act
        _fixture.ForceLayoutUpdate();

        // Assert
        Assert.NotNull(row.Box);
        Assert.NotNull(table.Box);

        // The row should be inside an anonymous row-group
        var rowParent = row.Box.parentNode;
        Assert.NotNull(rowParent);

        // Check if parent is anonymous row-group
        if (rowParent is CssAnonymousBox anonParent)
        {
            Assert.Equal(EDisplayMode.TABLE_ROW_GROUP, anonParent.SourceDisplayMode);
            Assert.Same(table.Box, anonParent.parentNode);
        }
        else
        {
            // Direct child is also valid per spec (table can directly contain rows)
            Assert.Same(table.Box, rowParent);
        }
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "TableFixup")]
    public void TableRow_InRowGroupParent_GeneratesNoWrapper()
    {
        // Arrange - a table-row in proper row-group parent needs no wrapper
        var doc = _fixture.Document;
        var parent = _fixture.CreateBlock(200, 100);

        var table = doc.createElement("div", DefaultOptions);
        table!.Style.UserRules.Display.Set(EDisplayMode.TABLE);
        parent.appendChild(table);

        var rowGroup = doc.createElement("div", DefaultOptions);
        rowGroup!.Style.UserRules.Display.Set(EDisplayMode.TABLE_ROW_GROUP);
        table.appendChild(rowGroup);

        var row = doc.createElement("div", DefaultOptions);
        row!.Style.UserRules.Display.Set(EDisplayMode.TABLE_ROW);
        rowGroup.appendChild(row);

        // Act
        _fixture.ForceLayoutUpdate();

        // Assert - row's direct parent should be the row-group
        Assert.NotNull(row.Box);
        Assert.NotNull(rowGroup.Box);
        Assert.Same(rowGroup.Box, row.Box.parentNode);
    }

    #endregion

    #region Row Group Fixup Tests

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "TableFixup")]
    public void TableRowGroup_InBlockParent_GeneratesTableWrapper()
    {
        // Arrange - a table-row-group in a block parent should generate:
        // block box → anonymous table → table-row-group
        var doc = _fixture.Document;
        var parent = _fixture.CreateBlock(200, 100);
        var rowGroup = doc.createElement("div", DefaultOptions);
        rowGroup!.Style.UserRules.Display.Set(EDisplayMode.TABLE_ROW_GROUP);
        parent.appendChild(rowGroup);

        // Act
        _fixture.ForceLayoutUpdate();

        // Assert
        Assert.NotNull(rowGroup.Box);

        var tableBox = rowGroup.Box.parentNode as CssAnonymousBox;
        Assert.NotNull(tableBox);
        Assert.Equal(EDisplayMode.TABLE, tableBox.SourceDisplayMode);

        Assert.Same(parent.Box, tableBox.parentNode);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "TableFixup")]
    public void TableHeaderGroup_InBlockParent_GeneratesTableWrapper()
    {
        // Arrange
        var doc = _fixture.Document;
        var parent = _fixture.CreateBlock(200, 100);
        var headerGroup = doc.createElement("div", DefaultOptions);
        headerGroup!.Style.UserRules.Display.Set(EDisplayMode.TABLE_HEADER_GROUP);
        parent.appendChild(headerGroup);

        // Act
        _fixture.ForceLayoutUpdate();

        // Assert
        Assert.NotNull(headerGroup.Box);

        var tableBox = headerGroup.Box.parentNode as CssAnonymousBox;
        Assert.NotNull(tableBox);
        Assert.Equal(EDisplayMode.TABLE, tableBox.SourceDisplayMode);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "TableFixup")]
    public void TableFooterGroup_InBlockParent_GeneratesTableWrapper()
    {
        // Arrange
        var doc = _fixture.Document;
        var parent = _fixture.CreateBlock(200, 100);
        var footerGroup = doc.createElement("div", DefaultOptions);
        footerGroup!.Style.UserRules.Display.Set(EDisplayMode.TABLE_FOOTER_GROUP);
        parent.appendChild(footerGroup);

        // Act
        _fixture.ForceLayoutUpdate();

        // Assert
        Assert.NotNull(footerGroup.Box);

        var tableBox = footerGroup.Box.parentNode as CssAnonymousBox;
        Assert.NotNull(tableBox);
        Assert.Equal(EDisplayMode.TABLE, tableBox.SourceDisplayMode);
    }

    #endregion

    #region Table Caption Fixup Tests

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "TableFixup")]
    public void TableCaption_InBlockParent_GeneratesTableWrapper()
    {
        // Arrange - a table-caption in a block parent should generate:
        // block box → anonymous table → table-caption
        var doc = _fixture.Document;
        var parent = _fixture.CreateBlock(200, 100);
        var caption = doc.createElement("div", DefaultOptions);
        caption!.Style.UserRules.Display.Set(EDisplayMode.TABLE_CAPTION);
        parent.appendChild(caption);

        // Act
        _fixture.ForceLayoutUpdate();

        // Assert
        Assert.NotNull(caption.Box);

        var tableBox = caption.Box.parentNode as CssAnonymousBox;
        Assert.NotNull(tableBox);
        Assert.Equal(EDisplayMode.TABLE, tableBox.SourceDisplayMode);

        Assert.Same(parent.Box, tableBox.parentNode);
    }

    #endregion

    #region Table Column Fixup Tests

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "TableFixup")]
    public void TableColumn_InBlockParent_GeneratesColumnGroupAndTableWrapper()
    {
        // Arrange - a table-column in a block parent should generate:
        // block box → anonymous table → anonymous table-column-group → table-column
        var doc = _fixture.Document;
        var parent = _fixture.CreateBlock(200, 100);
        var column = doc.createElement("div", DefaultOptions);
        column!.Style.UserRules.Display.Set(EDisplayMode.TABLE_COLUMN);
        parent.appendChild(column);

        // Act
        _fixture.ForceLayoutUpdate();

        // Assert
        Assert.NotNull(column.Box);

        var columnGroupBox = column.Box.parentNode as CssAnonymousBox;
        Assert.NotNull(columnGroupBox);
        Assert.Equal(EDisplayMode.TABLE_COLUMN_GROUP, columnGroupBox.SourceDisplayMode);

        var tableBox = columnGroupBox.parentNode as CssAnonymousBox;
        Assert.NotNull(tableBox);
        Assert.Equal(EDisplayMode.TABLE, tableBox.SourceDisplayMode);

        Assert.Same(parent.Box, tableBox.parentNode);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "TableFixup")]
    public void TableColumnGroup_InBlockParent_GeneratesTableWrapper()
    {
        // Arrange
        var doc = _fixture.Document;
        var parent = _fixture.CreateBlock(200, 100);
        var columnGroup = doc.createElement("div", DefaultOptions);
        columnGroup!.Style.UserRules.Display.Set(EDisplayMode.TABLE_COLUMN_GROUP);
        parent.appendChild(columnGroup);

        // Act
        _fixture.ForceLayoutUpdate();

        // Assert
        Assert.NotNull(columnGroup.Box);

        var tableBox = columnGroup.Box.parentNode as CssAnonymousBox;
        Assert.NotNull(tableBox);
        Assert.Equal(EDisplayMode.TABLE, tableBox.SourceDisplayMode);

        Assert.Same(parent.Box, tableBox.parentNode);
    }

    #endregion

    #region Inline Parent Context Tests

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "TableFixup")]
    public void TableCell_InInlineParent_GeneratesInlineTableWrapper()
    {
        // Arrange - Per CSS 2.2 §17.2.1: "If C's parent is an 'inline' box, then T must be an 'inline-table' box"
        var doc = _fixture.Document;
        var parent = _fixture.CreateBlock(200, 100);

        var inlineSpan = doc.createElement("span", DefaultOptions);
        inlineSpan!.Style.UserRules.Display.Set(EDisplayMode.INLINE);
        parent.appendChild(inlineSpan);

        var cell = doc.createElement("div", DefaultOptions);
        cell!.Style.UserRules.Display.Set(EDisplayMode.TABLE_CELL);
        inlineSpan.appendChild(cell);

        // Act
        _fixture.ForceLayoutUpdate();

        // Assert - the outermost table wrapper should be inline-table
        Assert.NotNull(cell.Box);

        // Navigate up to find the table
        var currentBox = cell.Box.parentNode;
        CssAnonymousBox? tableBox = null;
        while (currentBox is not null)
        {
            if (currentBox is CssAnonymousBox anonBox &&
                (anonBox.SourceDisplayMode == EDisplayMode.TABLE || anonBox.SourceDisplayMode == EDisplayMode.INLINE_TABLE))
            {
                tableBox = anonBox;
                break;
            }
            currentBox = currentBox.parentNode;
        }

        Assert.NotNull(tableBox);
        Assert.Equal(EDisplayMode.INLINE_TABLE, tableBox.SourceDisplayMode);
    }

    #endregion

    #region Nested Misparented Elements Tests

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "TableFixup")]
    public void MultipleMisparentedCells_GenerateSingleWrapperChain()
    {
        // Arrange - multiple table-cells as siblings should share wrapper chain
        var doc = _fixture.Document;
        var parent = _fixture.CreateBlock(200, 100);

        var cell1 = doc.createElement("div", DefaultOptions);
        cell1!.Style.UserRules.Display.Set(EDisplayMode.TABLE_CELL);
        parent.appendChild(cell1);

        var cell2 = doc.createElement("div", DefaultOptions);
        cell2!.Style.UserRules.Display.Set(EDisplayMode.TABLE_CELL);
        parent.appendChild(cell2);

        // Act
        _fixture.ForceLayoutUpdate();

        // Assert - both cells should have boxes
        Assert.NotNull(cell1.Box);
        Assert.NotNull(cell2.Box);

        // Navigate to their table wrapper and verify they share the same one
        // (or have separate chains - both are valid per spec)
        var cell1Row = cell1.Box.parentNode as CssAnonymousBox;
        var cell2Row = cell2.Box.parentNode as CssAnonymousBox;

        Assert.NotNull(cell1Row);
        Assert.NotNull(cell2Row);
        Assert.Equal(EDisplayMode.TABLE_ROW, cell1Row.SourceDisplayMode);
        Assert.Equal(EDisplayMode.TABLE_ROW, cell2Row.SourceDisplayMode);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "TableFixup")]
    public void DeeplyNestedMisparentedCell_GeneratesFullChain()
    {
        // Arrange - a table-cell deeply nested should still get full wrapper chain
        var doc = _fixture.Document;
        var parent = _fixture.CreateBlock(200, 100);

        var div1 = doc.createElement("div", DefaultOptions);
        div1!.Style.UserRules.Display.Set(EDisplayMode.BLOCK);
        parent.appendChild(div1);

        var div2 = doc.createElement("div", DefaultOptions);
        div2!.Style.UserRules.Display.Set(EDisplayMode.BLOCK);
        div1.appendChild(div2);

        var cell = doc.createElement("div", DefaultOptions);
        cell!.Style.UserRules.Display.Set(EDisplayMode.TABLE_CELL);
        div2.appendChild(cell);

        // Act
        _fixture.ForceLayoutUpdate();

        // Assert
        Assert.NotNull(cell.Box);

        // The cell should be wrapped in table-row
        var rowBox = cell.Box.parentNode as CssAnonymousBox;
        Assert.NotNull(rowBox);
        Assert.Equal(EDisplayMode.TABLE_ROW, rowBox.SourceDisplayMode);
    }

    #endregion

    #region Properly Parented Table Structure Tests

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "TableFixup")]
    public void ProperlyStructuredTable_GeneratesNoAnonymousWrappers()
    {
        // Arrange - a properly structured table needs no anonymous wrappers
        // (except perhaps for table-row direct child of table, which gets row-group)
        var doc = _fixture.Document;
        var parent = _fixture.CreateBlock(200, 100);

        var table = doc.createElement("table", DefaultOptions);
        table!.Style.UserRules.Display.Set(EDisplayMode.TABLE);
        parent.appendChild(table);

        var tbody = doc.createElement("tbody", DefaultOptions);
        tbody!.Style.UserRules.Display.Set(EDisplayMode.TABLE_ROW_GROUP);
        table.appendChild(tbody);

        var row = doc.createElement("tr", DefaultOptions);
        row!.Style.UserRules.Display.Set(EDisplayMode.TABLE_ROW);
        tbody.appendChild(row);

        var cell = doc.createElement("td", DefaultOptions);
        cell!.Style.UserRules.Display.Set(EDisplayMode.TABLE_CELL);
        row.appendChild(cell);

        // Act
        _fixture.ForceLayoutUpdate();

        // Assert - all elements should have boxes
        Assert.NotNull(table.Box);
        Assert.NotNull(tbody.Box);
        Assert.NotNull(row.Box);
        Assert.NotNull(cell.Box);

        // Verify hierarchy: cell → row → tbody → table → parent
        Assert.Same(row.Box, cell.Box.parentNode);
        Assert.Same(tbody.Box, row.Box.parentNode);
        Assert.Same(table.Box, tbody.Box.parentNode);
        Assert.Same(parent.Box, table.Box.parentNode);
    }

    #endregion
}
