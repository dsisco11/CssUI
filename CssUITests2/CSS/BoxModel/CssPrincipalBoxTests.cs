using CssUI;
using CssUI.CSS;
using CssUI.CSS.BoxTree;
using CssUI.CSS.Enums;
using CssUI.CSS.Formatting;
using CssUI.DOM;
using Xunit;

namespace CssUITests.CSS.BoxModel;

/// <summary>
/// Unit tests for CssPrincipalBox and related box model types.
/// See: https://www.w3.org/TR/css-box-3/#box-model
/// 
/// Note: CssPrincipalBox cannot be instantiated directly in tests due to
/// the Size setter restriction. These tests focus on:
/// - DisplayType struct and helper methods
/// - Rect4f struct operations
/// - Formatting context instantiation
/// - Related enums and static methods
/// </summary>
public class CssPrincipalBoxTests
{
    #region DisplayType Struct Tests

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Display")]
    public void DisplayType_Block_HasBlockOuterAndFlowRootInner()
    {
        // Act
        var displayType = new DisplayType(EDisplayMode.BLOCK);

        // Assert
        Assert.Equal(EOuterDisplayType.Block, displayType.Outer);
        Assert.Equal(EInnerDisplayType.Flow_Root, displayType.Inner);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Display")]
    public void DisplayType_Inline_HasInlineOuterAndFlowInner()
    {
        // Act
        var displayType = new DisplayType(EDisplayMode.INLINE);

        // Assert
        Assert.Equal(EOuterDisplayType.Inline, displayType.Outer);
        Assert.Equal(EInnerDisplayType.Flow, displayType.Inner);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Display")]
    public void DisplayType_InlineBlock_HasInlineOuterAndFlowRootInner()
    {
        // Act
        var displayType = new DisplayType(EDisplayMode.INLINE_BLOCK);

        // Assert
        Assert.Equal(EOuterDisplayType.Inline, displayType.Outer);
        Assert.Equal(EInnerDisplayType.Flow_Root, displayType.Inner);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Display")]
    public void DisplayType_Flex_HasBlockOuterAndFlexInner()
    {
        // Act
        var displayType = new DisplayType(EDisplayMode.FLEX);

        // Assert
        Assert.Equal(EOuterDisplayType.Block, displayType.Outer);
        Assert.Equal(EInnerDisplayType.Flex, displayType.Inner);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Display")]
    public void DisplayType_InlineFlex_HasInlineOuterAndFlexInner()
    {
        // Act
        var displayType = new DisplayType(EDisplayMode.INLINE_FLEX);

        // Assert
        Assert.Equal(EOuterDisplayType.Inline, displayType.Outer);
        Assert.Equal(EInnerDisplayType.Flex, displayType.Inner);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Display")]
    public void DisplayType_Grid_HasBlockOuterAndGridInner()
    {
        // Act
        var displayType = new DisplayType(EDisplayMode.GRID);

        // Assert
        Assert.Equal(EOuterDisplayType.Block, displayType.Outer);
        Assert.Equal(EInnerDisplayType.Grid, displayType.Inner);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Display")]
    public void DisplayType_InlineGrid_HasInlineOuterAndGridInner()
    {
        // Act
        var displayType = new DisplayType(EDisplayMode.INLINE_GRID);

        // Assert
        Assert.Equal(EOuterDisplayType.Inline, displayType.Outer);
        Assert.Equal(EInnerDisplayType.Grid, displayType.Inner);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Display")]
    public void DisplayType_None_HasNoneOuterAndNoneInner()
    {
        // Act
        var displayType = new DisplayType(EDisplayMode.NONE);

        // Assert
        Assert.Equal(EOuterDisplayType.None, displayType.Outer);
        Assert.Equal(EInnerDisplayType.None, displayType.Inner);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Display")]
    public void DisplayType_FlowRoot_HasBlockOuterAndFlowRootInner()
    {
        // Act
        var displayType = new DisplayType(EDisplayMode.FLOW_ROOT);

        // Assert
        Assert.Equal(EOuterDisplayType.Block, displayType.Outer);
        Assert.Equal(EInnerDisplayType.Flow_Root, displayType.Inner);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Display")]
    public void DisplayType_RunIn_HasRunInOuterAndFlowInner()
    {
        // Act
        var displayType = new DisplayType(EDisplayMode.RUN_IN);

        // Assert
        Assert.Equal(EOuterDisplayType.Run_In, displayType.Outer);
        Assert.Equal(EInnerDisplayType.Flow, displayType.Inner);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Display")]
    public void DisplayType_Table_HasBlockOuterAndTableInner()
    {
        // Act
        var displayType = new DisplayType(EDisplayMode.TABLE);

        // Assert
        Assert.Equal(EOuterDisplayType.Block, displayType.Outer);
        Assert.Equal(EInnerDisplayType.Table, displayType.Inner);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Display")]
    public void DisplayType_InlineTable_HasInlineOuterAndTableInner()
    {
        // Act
        var displayType = new DisplayType(EDisplayMode.INLINE_TABLE);

        // Assert
        Assert.Equal(EOuterDisplayType.Inline, displayType.Outer);
        Assert.Equal(EInnerDisplayType.Table, displayType.Inner);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Display")]
    public void DisplayType_IsBlockLevel_TrueForBlock()
    {
        // Act
        var displayType = new DisplayType(EDisplayMode.BLOCK);

        // Assert
        Assert.True(displayType.IsBlockLevel);
        Assert.False(displayType.IsInlineLevel);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Display")]
    public void DisplayType_IsInlineLevel_TrueForInline()
    {
        // Act
        var displayType = new DisplayType(EDisplayMode.INLINE);

        // Assert
        Assert.True(displayType.IsInlineLevel);
        Assert.False(displayType.IsBlockLevel);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Display")]
    public void DisplayType_IsBlockContainer_TrueForFlowRoot()
    {
        // Act
        var displayType = new DisplayType(EDisplayMode.BLOCK);

        // Assert
        Assert.True(displayType.IsBlockContainer);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Display")]
    public void DisplayType_IsBlockContainer_FalseForFlex()
    {
        // Act - Flex inner display type is not flow_root
        var displayType = new DisplayType(EDisplayMode.FLEX);

        // Assert
        Assert.False(displayType.IsBlockContainer);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Display")]
    public void DisplayType_From_CreatesCorrectDisplayType()
    {
        // Act
        var displayType = DisplayType.From(EDisplayMode.INLINE_BLOCK);

        // Assert
        Assert.Equal(EOuterDisplayType.Inline, displayType.Outer);
        Assert.Equal(EInnerDisplayType.Flow_Root, displayType.Inner);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Display")]
    public void DisplayType_GetOuter_ReturnsCorrectOuter()
    {
        // Act
        var outer = DisplayType.Get_Outer(EDisplayMode.FLEX);

        // Assert
        Assert.Equal(EOuterDisplayType.Block, outer);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Display")]
    public void DisplayType_GetInner_ReturnsCorrectInner()
    {
        // Act
        var inner = DisplayType.Get_Inner(EDisplayMode.GRID);

        // Assert
        Assert.Equal(EInnerDisplayType.Grid, inner);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Display")]
    public void DisplayType_ListItem_HasBlockOuterAndFlowRootInner()
    {
        // Act
        var displayType = new DisplayType(EDisplayMode.LIST_ITEM);

        // Assert
        Assert.Equal(EOuterDisplayType.Block, displayType.Outer);
        Assert.Equal(EInnerDisplayType.Flow_Root, displayType.Inner);
    }

    #endregion

    #region Rect4f Tests

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Rect4f")]
    public void Rect4f_Constructor_SetsAllEdges()
    {
        // Act
        var rect = new Rect4f(10, 200, 110, 100);

        // Assert
        Assert.Equal(10, rect.Top);
        Assert.Equal(200, rect.Right);
        Assert.Equal(110, rect.Bottom);
        Assert.Equal(100, rect.Left);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Rect4f")]
    public void Rect4f_Width_CalculatedFromLeftAndRight()
    {
        // Arrange
        var rect = new Rect4f(0, 200, 100, 50);

        // Act & Assert
        Assert.Equal(150, rect.Width); // Right - Left = 200 - 50 = 150
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Rect4f")]
    public void Rect4f_Height_CalculatedFromTopAndBottom()
    {
        // Arrange
        var rect = new Rect4f(10, 100, 60, 0);

        // Act & Assert
        Assert.Equal(50, rect.Height); // Bottom - Top = 60 - 10 = 50
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Rect4f")]
    public void Rect4f_SingleValueConstructor_SetsAllEdges()
    {
        // Act
        var rect = new Rect4f(10);

        // Assert
        Assert.Equal(10, rect.Top);
        Assert.Equal(10, rect.Right);
        Assert.Equal(10, rect.Bottom);
        Assert.Equal(10, rect.Left);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Rect4f")]
    public void Rect4f_Zero_HasZeroEdges()
    {
        // Assert
        Assert.Equal(0, Rect4f.Zero.Top);
        Assert.Equal(0, Rect4f.Zero.Right);
        Assert.Equal(0, Rect4f.Zero.Bottom);
        Assert.Equal(0, Rect4f.Zero.Left);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Rect4f")]
    public void Rect4f_Addition_AddsToAllEdges()
    {
        // Arrange
        var rect = new Rect4f(10, 20, 30, 40);

        // Act
        var result = rect + 5;

        // Assert
        Assert.Equal(15, result.Top);
        Assert.Equal(25, result.Right);
        Assert.Equal(35, result.Bottom);
        Assert.Equal(45, result.Left);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Rect4f")]
    public void Rect4f_Subtraction_SubtractsFromAllEdges()
    {
        // Arrange
        var rect = new Rect4f(10, 20, 30, 40);

        // Act
        var result = rect - 5;

        // Assert
        Assert.Equal(5, result.Top);
        Assert.Equal(15, result.Right);
        Assert.Equal(25, result.Bottom);
        Assert.Equal(35, result.Left);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Rect4f")]
    public void Rect4f_AdditionOfRects_AddsCorrespondingEdges()
    {
        // Arrange
        var a = new Rect4f(10, 20, 30, 40);
        var b = new Rect4f(1, 2, 3, 4);

        // Act
        var result = a + b;

        // Assert
        Assert.Equal(11, result.Top);
        Assert.Equal(22, result.Right);
        Assert.Equal(33, result.Bottom);
        Assert.Equal(44, result.Left);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Rect4f")]
    public void Rect4f_ToString_ReturnsFormattedString()
    {
        // Arrange
        var rect = new Rect4f(1, 2, 3, 4);

        // Act
        var str = rect.ToString();

        // Assert
        Assert.Equal("Rect4f(1, 2, 3, 4)", str);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Rect4f")]
    public void Rect4f_Equality_SameValues_AreEqual()
    {
        // Arrange
        var a = new Rect4f(10, 20, 30, 40);
        var b = new Rect4f(10, 20, 30, 40);

        // Assert
        Assert.Equal(a, b);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Rect4f")]
    public void Rect4f_Equality_DifferentValues_AreNotEqual()
    {
        // Arrange
        var a = new Rect4f(10, 20, 30, 40);
        var b = new Rect4f(10, 20, 30, 41);

        // Assert
        Assert.NotEqual(a, b);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Rect4f")]
    public void Rect4f_Multiplication_MultipliesAllEdges()
    {
        // Arrange
        var rect = new Rect4f(10, 20, 30, 40);

        // Act
        var result = rect * 2;

        // Assert
        Assert.Equal(20, result.Top);
        Assert.Equal(40, result.Right);
        Assert.Equal(60, result.Bottom);
        Assert.Equal(80, result.Left);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Rect4f")]
    public void Rect4f_Division_DividesAllEdges()
    {
        // Arrange
        var rect = new Rect4f(20, 40, 60, 80);

        // Act
        var result = rect / 2;

        // Assert
        Assert.Equal(10, result.Top);
        Assert.Equal(20, result.Right);
        Assert.Equal(30, result.Bottom);
        Assert.Equal(40, result.Left);
    }

    #endregion

    #region EBoxDisplayGroup Tests

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Display")]
    public void EBoxDisplayGroup_HasExpectedValues()
    {
        // Assert - verify enum values exist
        Assert.True(System.Enum.IsDefined(typeof(EBoxDisplayGroup), "INVALID"));
        Assert.True(System.Enum.IsDefined(typeof(EBoxDisplayGroup), "INLINE"));
        Assert.True(System.Enum.IsDefined(typeof(EBoxDisplayGroup), "BLOCK"));
        Assert.True(System.Enum.IsDefined(typeof(EBoxDisplayGroup), "INLINE_BLOCK"));
        Assert.True(System.Enum.IsDefined(typeof(EBoxDisplayGroup), "FLOATING"));
        Assert.True(System.Enum.IsDefined(typeof(EBoxDisplayGroup), "ABSOLUTELY_POSITIONED"));
    }

    #endregion

    #region Formatting Context Creation Tests

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "FormattingContext")]
    public void BlockFormattingContext_CanBeInstantiated()
    {
        // Act
        var context = new BlockFormattingContext();

        // Assert
        Assert.NotNull(context);
        Assert.IsAssignableFrom<IFormattingContext>(context);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "FormattingContext")]
    public void FlexFormattingContext_CanBeInstantiated()
    {
        // Act
        var context = new FlexFormattingContext();

        // Assert
        Assert.NotNull(context);
        Assert.IsAssignableFrom<IFormattingContext>(context);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "FormattingContext")]
    public void GridFormattingContext_CanBeInstantiated()
    {
        // Act
        var context = new GridFormattingContext();

        // Assert
        Assert.NotNull(context);
        Assert.IsAssignableFrom<IFormattingContext>(context);
    }

    #endregion

    #region EOuterDisplayType and EInnerDisplayType Tests

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Display")]
    public void EOuterDisplayType_HasExpectedValues()
    {
        // Verify all enum values that exist
        Assert.True(System.Enum.IsDefined(typeof(EOuterDisplayType), "None"));
        Assert.True(System.Enum.IsDefined(typeof(EOuterDisplayType), "Inline"));
        Assert.True(System.Enum.IsDefined(typeof(EOuterDisplayType), "Block"));
        Assert.True(System.Enum.IsDefined(typeof(EOuterDisplayType), "Run_In"));
        // Note: Contents is not yet implemented in EOuterDisplayType
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Display")]
    public void EInnerDisplayType_HasExpectedValues()
    {
        Assert.True(System.Enum.IsDefined(typeof(EInnerDisplayType), "None"));
        Assert.True(System.Enum.IsDefined(typeof(EInnerDisplayType), "Flow"));
        Assert.True(System.Enum.IsDefined(typeof(EInnerDisplayType), "Flow_Root"));
        Assert.True(System.Enum.IsDefined(typeof(EInnerDisplayType), "Flex"));
        Assert.True(System.Enum.IsDefined(typeof(EInnerDisplayType), "Grid"));
        Assert.True(System.Enum.IsDefined(typeof(EInnerDisplayType), "Table"));
    }

    #endregion

    #region IntrinsicSize Tests

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "IntrinsicSize")]
    public void IntrinsicSize_Zero_HasZeroValues()
    {
        // Act
        var size = IntrinsicSize.Zero;

        // Assert
        Assert.Equal(0, size.Inline.MinContent);
        Assert.Equal(0, size.Inline.MaxContent);
        Assert.Equal(0, size.Block.MinContent);
        Assert.Equal(0, size.Block.MaxContent);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "IntrinsicSize")]
    public void IntrinsicSize_Constructor_SetsValues()
    {
        // Act
        var size = new IntrinsicSize(10, 100, 20, 200);

        // Assert
        Assert.Equal(10, size.Inline.MinContent);
        Assert.Equal(100, size.Inline.MaxContent);
        Assert.Equal(20, size.Block.MinContent);
        Assert.Equal(200, size.Block.MaxContent);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "IntrinsicSize")]
    public void IntrinsicSize_Definite_CreatesSameMinAndMax()
    {
        // Act - Definite creates same min and max values
        var size = IntrinsicSize.Definite(100, 200);

        // Assert
        Assert.Equal(100, size.Inline.MinContent);
        Assert.Equal(100, size.Inline.MaxContent);
        Assert.Equal(200, size.Block.MinContent);
        Assert.Equal(200, size.Block.MaxContent);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "IntrinsicSize")]
    public void IntrinsicAxisSize_Zero_HasZeroMinAndMax()
    {
        // Act
        var size = IntrinsicAxisSize.Zero;

        // Assert
        Assert.Equal(0, size.MinContent);
        Assert.Equal(0, size.MaxContent);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "IntrinsicSize")]
    public void IntrinsicAxisSize_Definite_CreatesSameMinAndMax()
    {
        // Act
        var size = IntrinsicAxisSize.Definite(150);

        // Assert
        Assert.Equal(150, size.MinContent);
        Assert.Equal(150, size.MaxContent);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "IntrinsicSize")]
    public void IntrinsicAxisSize_Max_ReturnsMaxOfBoth()
    {
        // Arrange
        var a = new IntrinsicAxisSize(10, 100);
        var b = new IntrinsicAxisSize(20, 80);

        // Act
        var max = IntrinsicAxisSize.Max(a, b);

        // Assert
        Assert.Equal(20, max.MinContent); // max(10, 20)
        Assert.Equal(100, max.MaxContent); // max(100, 80)
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "IntrinsicSize")]
    public void IntrinsicAxisSize_Sum_AddsBothComponents()
    {
        // Arrange
        var a = new IntrinsicAxisSize(10, 100);
        var b = new IntrinsicAxisSize(20, 80);

        // Act
        var sum = IntrinsicAxisSize.Sum(a, b);

        // Assert
        Assert.Equal(30, sum.MinContent); // 10 + 20
        Assert.Equal(180, sum.MaxContent); // 100 + 80
    }

    #endregion

    #region CssRect Tests

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "CssRect")]
    public void CssRect_DefaultConstructor_InitializesToZero()
    {
        // Act
        var rect = new CssRect();

        // Assert
        Assert.Equal(0, rect.Top);
        Assert.Equal(0, rect.Right);
        Assert.Equal(0, rect.Bottom);
        Assert.Equal(0, rect.Left);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "CssRect")]
    public void CssRect_ParameterizedConstructor_SetsAllValues()
    {
        // Act
        var rect = new CssRect(10, 20, 30, 40);

        // Assert
        Assert.Equal(10, rect.Left);
        Assert.Equal(20, rect.Top);
        Assert.Equal(30, rect.Right);
        Assert.Equal(40, rect.Bottom);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "CssRect")]
    public void CssRect_FieldAssignment_UpdatesValues()
    {
        // Arrange
        var rect = new CssRect();

        // Act
        rect.Left = 5;
        rect.Top = 10;
        rect.Right = 15;
        rect.Bottom = 20;

        // Assert
        Assert.Equal(5, rect.Left);
        Assert.Equal(10, rect.Top);
        Assert.Equal(15, rect.Right);
        Assert.Equal(20, rect.Bottom);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "CssRect")]
    public void CssRect_NegativeValues_AllowedForOffsets()
    {
        // Negative margins/offsets are valid in CSS
        var rect = new CssRect(-10, -5, 100, 50);

        Assert.Equal(-10, rect.Left);
        Assert.Equal(-5, rect.Top);
        Assert.Equal(100, rect.Right);
        Assert.Equal(50, rect.Bottom);
    }

    #endregion

    #region CssBoxArea Tests

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "CssBoxArea")]
    public void CssBoxArea_DefaultConstructor_InitializesToZero()
    {
        // Act
        var area = new CssBoxArea();

        // Assert
        Assert.Equal(0, area.Top);
        Assert.Equal(0, area.Right);
        Assert.Equal(0, area.Bottom);
        Assert.Equal(0, area.Left);
        Assert.Equal(0, area.Width);
        Assert.Equal(0, area.Height);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "CssBoxArea")]
    public void CssBoxArea_EdgeConstructor_SetsEdges()
    {
        // Act
        var area = new CssBoxArea(10, 100, 50, 20);

        // Assert
        Assert.Equal(10, area.Top);
        Assert.Equal(100, area.Right);
        Assert.Equal(50, area.Bottom);
        Assert.Equal(20, area.Left);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "CssBoxArea")]
    public void CssBoxArea_EdgeConstructor_CalculatesWidthAndHeight()
    {
        // Arrange/Act
        var area = new CssBoxArea(0, 100, 50, 10);

        // Assert - Width = Right - Left, Height = Bottom - Top
        Assert.Equal(90, area.Width); // 100 - 10
        Assert.Equal(50, area.Height); // 50 - 0
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "CssBoxArea")]
    public void CssBoxArea_CopyConstructor_CreatesCopy()
    {
        // Arrange
        var original = new CssBoxArea(10, 100, 50, 20);

        // Act
        var copy = new CssBoxArea(original);

        // Assert
        Assert.Equal(original.Top, copy.Top);
        Assert.Equal(original.Right, copy.Right);
        Assert.Equal(original.Bottom, copy.Bottom);
        Assert.Equal(original.Left, copy.Left);
        Assert.Equal(original.Width, copy.Width);
        Assert.Equal(original.Height, copy.Height);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "CssBoxArea")]
    public void CssBoxArea_SetDimensions_UpdatesWidthAndHeight()
    {
        // Arrange
        var area = new CssBoxArea();

        // Act
        area.Set_Dimensions(200, 100);

        // Assert
        Assert.Equal(200, area.Width);
        Assert.Equal(100, area.Height);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "CssBoxArea")]
    public void CssBoxArea_SetTRBL_UpdatesEdges()
    {
        // Arrange
        var area = new CssBoxArea();

        // Act
        area.Set_TRBL(10, 100, 50, 20);

        // Assert
        Assert.Equal(10, area.Top);
        Assert.Equal(100, area.Right);
        Assert.Equal(50, area.Bottom);
        Assert.Equal(20, area.Left);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "CssBoxArea")]
    public void CssBoxArea_UpdateBounds_SetsPositionAndSize()
    {
        // Arrange
        var area = new CssBoxArea();

        // Act
        area.Update_Bounds(10, 20, 100, 50);

        // Assert
        Assert.Equal(10, area.X);
        Assert.Equal(20, area.Y);
        Assert.Equal(100, area.Width);
        Assert.Equal(50, area.Height);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "CssBoxArea")]
    public void CssBoxArea_GetPos_ReturnsPosition()
    {
        // Arrange
        var area = new CssBoxArea();
        area.Update_Bounds(15, 25, 100, 50);

        // Act
        var pos = area.Get_Pos();

        // Assert
        Assert.Equal(15, pos.X);
        Assert.Equal(25, pos.Y);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "CssBoxArea")]
    public void CssBoxArea_GetDimensions_ReturnsDimensions()
    {
        // Arrange
        var area = new CssBoxArea();
        area.Update_Bounds(10, 20, 150, 75);

        // Act
        var dims = area.Get_Dimensions();

        // Assert
        Assert.Equal(150, dims.Width);
        Assert.Equal(75, dims.Height);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "CssBoxArea")]
    public void CssBoxArea_GetRect_ReturnsCssRect()
    {
        // Arrange
        var area = new CssBoxArea(10, 100, 50, 20);

        // Act
        var rect = area.Get_Rect();

        // Assert
        Assert.Equal(10, rect.Top);
        Assert.Equal(100, rect.Right);
        Assert.Equal(50, rect.Bottom);
        Assert.Equal(20, rect.Left);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "CssBoxArea")]
    public void CssBoxArea_SizeProperties_CanBeSetAndRead()
    {
        // Arrange
        var area = new CssBoxArea();

        // Act
        area.Size_Top = 5;
        area.Size_Right = 10;
        area.Size_Bottom = 15;
        area.Size_Left = 20;

        // Assert
        Assert.Equal(5, area.Size_Top);
        Assert.Equal(10, area.Size_Right);
        Assert.Equal(15, area.Size_Bottom);
        Assert.Equal(20, area.Size_Left);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "CssBoxArea")]
    public void CssBoxArea_SizeHorizontal_ReturnsSumOfLeftAndRight()
    {
        // Arrange
        var area = new CssBoxArea();
        area.Size_Left = 10;
        area.Size_Right = 15;

        // Assert
        Assert.Equal(25, area.Size_Horizontal);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "CssBoxArea")]
    public void CssBoxArea_SizeVertical_ReturnsSumOfTopAndBottom()
    {
        // Arrange
        var area = new CssBoxArea();
        area.Size_Top = 5;
        area.Size_Bottom = 20;

        // Assert
        Assert.Equal(25, area.Size_Vertical);
    }

    #endregion
}
