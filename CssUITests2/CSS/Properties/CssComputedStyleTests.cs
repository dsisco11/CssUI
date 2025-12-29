using System;
using System.Linq;
using CssUI;
using CssUI.CSS;
using CssUI.CSS.Enums;
using CssUI.CSS.Internal;
using CssUI.DOM;
using Xunit;

namespace CssUITests.CSS.Properties.Tests;

/// <summary>
/// Tests for CssComputedStyle - the property collection container that holds all CSS property instances for an element.
/// </summary>
public class CssComputedStyleTests
{
    #region Helper Methods
    /// <summary>
    /// Creates a document for testing CSS properties on elements
    /// </summary>
    private static Document CreateTestDocument()
    {
        var dom = new DOMImplementation();
        return dom.createDocument("CssUI", "cssui");
    }

    /// <summary>
    /// Creates an element attached to a document for CSS property testing
    /// </summary>
    private static Element CreateTestElement(Document doc, string tagName = "div")
    {
        var element = doc.createElement(tagName, new ElementCreationOptions(string.Empty));
        doc.documentElement?.appendChild(element);
        return element;
    }

    /// <summary>
    /// Gets the Cascaded style from an element (ReadOnly - result of cascade)
    /// </summary>
    private static CssComputedStyle GetCascadedStyle(Element element)
    {
        return element.Style.Cascaded;
    }

    /// <summary>
    /// Gets the UserRules style from an element (Writable - for user modifications)
    /// </summary>
    private static CssComputedStyle GetUserRulesStyle(Element element)
    {
        return element.Style.UserRules;
    }
    #endregion

    #region 12.8.1 Property Access Tests

    #region Get() Method Tests
    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "PropertyAccess")]
    public void Get_WithValidPropertyID_ReturnsPropertyInstance()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetCascadedStyle(element);

        // Act
        var property = style.Get(ECssPropertyID.Width);

        // Assert
        Assert.NotNull(property);
        Assert.Equal(ECssPropertyID.Width, property.CssName);
    }

    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "PropertyAccess")]
    public void Get_WithHeight_ReturnsIntProperty()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetCascadedStyle(element);

        // Act
        var property = style.Get(ECssPropertyID.Height);

        // Assert
        Assert.NotNull(property);
        Assert.IsType<IntProperty>(property);
    }

    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "PropertyAccess")]
    public void Get_WithDisplay_ReturnsEnumProperty()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetCascadedStyle(element);

        // Act
        var property = style.Get(ECssPropertyID.Display);

        // Assert
        Assert.NotNull(property);
        Assert.IsType<EnumProperty<EDisplayMode>>(property);
    }

    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "PropertyAccess")]
    public void Get_WithUnregisteredPropertyID_ReturnsNull()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetCascadedStyle(element);

        // Act - Use a very high property ID that won't be registered
        var property = style.Get((ECssPropertyID)99999);

        // Assert
        Assert.Null(property);
    }

    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "PropertyAccess")]
    public void Get_WithNegativePropertyID_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetCascadedStyle(element);

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => style.Get((ECssPropertyID)(-1)));
    }

    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "PropertyAccess")]
    public void Get_MultipleCalls_ReturnsSameInstance()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetCascadedStyle(element);

        // Act
        var property1 = style.Get(ECssPropertyID.Width);
        var property2 = style.Get(ECssPropertyID.Width);

        // Assert
        Assert.Same(property1, property2);
    }

    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "PropertyAccess")]
    public void Get_DifferentPropertyIDs_ReturnsDifferentInstances()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetCascadedStyle(element);

        // Act
        var widthProp = style.Get(ECssPropertyID.Width);
        var heightProp = style.Get(ECssPropertyID.Height);

        // Assert
        Assert.NotSame(widthProp, heightProp);
    }
    #endregion

    #region Indexer Tests
    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "PropertyAccess")]
    public void Indexer_WithValidPropertyID_ReturnsPropertyInstance()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetCascadedStyle(element);

        // Act
        var property = style[ECssPropertyID.MarginTop];

        // Assert
        Assert.NotNull(property);
        Assert.Equal(ECssPropertyID.MarginTop, property.CssName);
    }

    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "PropertyAccess")]
    public void Indexer_ReturnsSameInstanceAsGet()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetCascadedStyle(element);

        // Act
        var fromIndexer = style[ECssPropertyID.PaddingLeft];
        var fromGet = style.Get(ECssPropertyID.PaddingLeft);

        // Assert
        Assert.Same(fromIndexer, fromGet);
    }
    #endregion

    #region Typed Property Accessor Tests
    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "PropertyAccess")]
    public void Width_Property_ReturnsIntProperty()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetCascadedStyle(element);

        // Act
        var widthProp = style.Width;

        // Assert
        Assert.NotNull(widthProp);
        Assert.IsType<IntProperty>(widthProp);
    }

    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "PropertyAccess")]
    public void Height_Property_ReturnsIntProperty()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetCascadedStyle(element);

        // Act
        var heightProp = style.Height;

        // Assert
        Assert.NotNull(heightProp);
        Assert.IsType<IntProperty>(heightProp);
    }

    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "PropertyAccess")]
    public void Display_Property_ReturnsEnumPropertyOfDisplayMode()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetCascadedStyle(element);

        // Act
        var displayProp = style.Display;

        // Assert
        Assert.NotNull(displayProp);
        Assert.IsType<EnumProperty<EDisplayMode>>(displayProp);
    }

    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "PropertyAccess")]
    public void BoxSizing_Property_ReturnsEnumPropertyOfBoxSizingMode()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetCascadedStyle(element);

        // Act
        var boxSizingProp = style.BoxSizing;

        // Assert
        Assert.NotNull(boxSizingProp);
        Assert.IsType<EnumProperty<EBoxSizingMode>>(boxSizingProp);
    }

    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "PropertyAccess")]
    public void TypedAccessor_ReturnsSameInstanceAsGet()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetCascadedStyle(element);

        // Act
        var fromTyped = style.Width;
        var fromGet = style.Get(ECssPropertyID.Width);

        // Assert
        Assert.Same(fromTyped, fromGet);
    }

    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "PropertyAccess")]
    public void Margin_Properties_ReturnCorrectInstances()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetCascadedStyle(element);

        // Act
        var marginTop = style.Margin_Top;
        var marginRight = style.Margin_Right;
        var marginBottom = style.Margin_Bottom;
        var marginLeft = style.Margin_Left;

        // Assert
        Assert.NotNull(marginTop);
        Assert.NotNull(marginRight);
        Assert.NotNull(marginBottom);
        Assert.NotNull(marginLeft);
        Assert.Equal(ECssPropertyID.MarginTop, marginTop.CssName);
        Assert.Equal(ECssPropertyID.MarginRight, marginRight.CssName);
        Assert.Equal(ECssPropertyID.MarginBottom, marginBottom.CssName);
        Assert.Equal(ECssPropertyID.MarginLeft, marginLeft.CssName);
    }

    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "PropertyAccess")]
    public void Padding_Properties_ReturnCorrectInstances()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetCascadedStyle(element);

        // Act
        var paddingTop = style.Padding_Top;
        var paddingRight = style.Padding_Right;
        var paddingBottom = style.Padding_Bottom;
        var paddingLeft = style.Padding_Left;

        // Assert
        Assert.NotNull(paddingTop);
        Assert.NotNull(paddingRight);
        Assert.NotNull(paddingBottom);
        Assert.NotNull(paddingLeft);
        Assert.Equal(ECssPropertyID.PaddingTop, paddingTop.CssName);
        Assert.Equal(ECssPropertyID.PaddingRight, paddingRight.CssName);
        Assert.Equal(ECssPropertyID.PaddingBottom, paddingBottom.CssName);
        Assert.Equal(ECssPropertyID.PaddingLeft, paddingLeft.CssName);
    }

    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "PropertyAccess")]
    public void Position_Properties_ReturnCorrectInstances()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetCascadedStyle(element);

        // Act
        var top = style.Top;
        var right = style.Right;
        var bottom = style.Bottom;
        var left = style.Left;

        // Assert
        Assert.NotNull(top);
        Assert.NotNull(right);
        Assert.NotNull(bottom);
        Assert.NotNull(left);
        Assert.Equal(ECssPropertyID.Top, top.CssName);
        Assert.Equal(ECssPropertyID.Right, right.CssName);
        Assert.Equal(ECssPropertyID.Bottom, bottom.CssName);
        Assert.Equal(ECssPropertyID.Left, left.CssName);
    }

    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "PropertyAccess")]
    public void X_Property_PointsToLeft()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetCascadedStyle(element);

        // Act
        var x = style.X;
        var left = style.Left;

        // Assert
        Assert.Same(x, left);
    }

    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "PropertyAccess")]
    public void Y_Property_PointsToTop()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetCascadedStyle(element);

        // Act
        var y = style.Y;
        var top = style.Top;

        // Assert
        Assert.Same(y, top);
    }
    #endregion

    #region Flexbox Property Accessors
    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "PropertyAccess")]
    [Trait("Category", "Flexbox")]
    public void FlexDirection_Property_ReturnsEnumProperty()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetCascadedStyle(element);

        // Act
        var flexDirection = style.FlexDirection;

        // Assert
        Assert.NotNull(flexDirection);
        Assert.IsType<EnumProperty<EFlexDirection>>(flexDirection);
        Assert.Equal(ECssPropertyID.FlexDirection, flexDirection.CssName);
    }

    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "PropertyAccess")]
    [Trait("Category", "Flexbox")]
    public void FlexWrap_Property_ReturnsEnumProperty()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetCascadedStyle(element);

        // Act
        var flexWrap = style.FlexWrap;

        // Assert
        Assert.NotNull(flexWrap);
        Assert.IsType<EnumProperty<EFlexWrap>>(flexWrap);
        Assert.Equal(ECssPropertyID.FlexWrap, flexWrap.CssName);
    }

    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "PropertyAccess")]
    [Trait("Category", "Flexbox")]
    public void FlexGrow_Property_ReturnsNumberProperty()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetCascadedStyle(element);

        // Act
        var flexGrow = style.FlexGrow;

        // Assert
        Assert.NotNull(flexGrow);
        Assert.IsType<NumberProperty>(flexGrow);
        Assert.Equal(ECssPropertyID.FlexGrow, flexGrow.CssName);
    }

    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "PropertyAccess")]
    [Trait("Category", "Flexbox")]
    public void FlexShrink_Property_ReturnsNumberProperty()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetCascadedStyle(element);

        // Act
        var flexShrink = style.FlexShrink;

        // Assert
        Assert.NotNull(flexShrink);
        Assert.IsType<NumberProperty>(flexShrink);
        Assert.Equal(ECssPropertyID.FlexShrink, flexShrink.CssName);
    }

    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "PropertyAccess")]
    [Trait("Category", "Flexbox")]
    public void FlexBasis_Property_ReturnsIntProperty()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetCascadedStyle(element);

        // Act
        var flexBasis = style.FlexBasis;

        // Assert
        Assert.NotNull(flexBasis);
        Assert.IsType<IntProperty>(flexBasis);
        Assert.Equal(ECssPropertyID.FlexBasis, flexBasis.CssName);
    }

    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "PropertyAccess")]
    [Trait("Category", "Flexbox")]
    public void Order_Property_ReturnsIntProperty()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetCascadedStyle(element);

        // Act
        var order = style.Order;

        // Assert
        Assert.NotNull(order);
        Assert.IsType<IntProperty>(order);
        Assert.Equal(ECssPropertyID.Order, order.CssName);
    }
    #endregion

    #region Alignment Property Accessors
    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "PropertyAccess")]
    [Trait("Category", "Alignment")]
    public void AlignContent_Property_ReturnsEnumProperty()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetCascadedStyle(element);

        // Act
        var alignContent = style.AlignContent;

        // Assert
        Assert.NotNull(alignContent);
        Assert.IsType<EnumProperty<EAlignContent>>(alignContent);
        Assert.Equal(ECssPropertyID.AlignContent, alignContent.CssName);
    }

    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "PropertyAccess")]
    [Trait("Category", "Alignment")]
    public void JustifyContent_Property_ReturnsEnumProperty()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetCascadedStyle(element);

        // Act
        var justifyContent = style.JustifyContent;

        // Assert
        Assert.NotNull(justifyContent);
        Assert.IsType<EnumProperty<EJustifyContent>>(justifyContent);
        Assert.Equal(ECssPropertyID.JustifyContent, justifyContent.CssName);
    }

    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "PropertyAccess")]
    [Trait("Category", "Alignment")]
    public void AlignItems_Property_ReturnsEnumProperty()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetCascadedStyle(element);

        // Act
        var alignItems = style.AlignItems;

        // Assert
        Assert.NotNull(alignItems);
        Assert.IsType<EnumProperty<EAlignItems>>(alignItems);
        Assert.Equal(ECssPropertyID.AlignItems, alignItems.CssName);
    }

    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "PropertyAccess")]
    [Trait("Category", "Alignment")]
    public void AlignSelf_Property_ReturnsEnumProperty()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetCascadedStyle(element);

        // Act
        var alignSelf = style.AlignSelf;

        // Assert
        Assert.NotNull(alignSelf);
        Assert.IsType<EnumProperty<EAlignItems>>(alignSelf);
        Assert.Equal(ECssPropertyID.AlignSelf, alignSelf.CssName);
    }

    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "PropertyAccess")]
    [Trait("Category", "Alignment")]
    public void Gap_Properties_ReturnIntProperties()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetCascadedStyle(element);

        // Act
        var rowGap = style.RowGap;
        var columnGap = style.ColumnGap;

        // Assert
        Assert.NotNull(rowGap);
        Assert.NotNull(columnGap);
        Assert.IsType<IntProperty>(rowGap);
        Assert.IsType<IntProperty>(columnGap);
        Assert.Equal(ECssPropertyID.RowGap, rowGap.CssName);
        Assert.Equal(ECssPropertyID.ColumnGap, columnGap.CssName);
    }
    #endregion

    #region Grid Property Accessors
    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "PropertyAccess")]
    [Trait("Category", "Grid")]
    public void GridAutoFlow_Property_ReturnsEnumProperty()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetCascadedStyle(element);

        // Act
        var gridAutoFlow = style.GridAutoFlow;

        // Assert
        Assert.NotNull(gridAutoFlow);
        Assert.IsType<EnumProperty<EGridAutoFlow>>(gridAutoFlow);
        Assert.Equal(ECssPropertyID.GridAutoFlow, gridAutoFlow.CssName);
    }

    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "PropertyAccess")]
    [Trait("Category", "Grid")]
    public void GridPlacement_Properties_ReturnIntProperties()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetCascadedStyle(element);

        // Act
        var colStart = style.GridColumnStart;
        var colEnd = style.GridColumnEnd;
        var rowStart = style.GridRowStart;
        var rowEnd = style.GridRowEnd;

        // Assert
        Assert.NotNull(colStart);
        Assert.NotNull(colEnd);
        Assert.NotNull(rowStart);
        Assert.NotNull(rowEnd);
        Assert.IsType<IntProperty>(colStart);
        Assert.IsType<IntProperty>(colEnd);
        Assert.IsType<IntProperty>(rowStart);
        Assert.IsType<IntProperty>(rowEnd);
    }

    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "PropertyAccess")]
    [Trait("Category", "Grid")]
    public void GridTemplate_Properties_ReturnStringProperties()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetCascadedStyle(element);

        // Act
        var templateCols = style.GridTemplateColumns;
        var templateRows = style.GridTemplateRows;

        // Assert
        Assert.NotNull(templateCols);
        Assert.NotNull(templateRows);
        Assert.IsType<StringProperty>(templateCols);
        Assert.IsType<StringProperty>(templateRows);
    }
    #endregion

    #region Border Property Accessors
    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "PropertyAccess")]
    public void Border_Style_Properties_ReturnEnumProperties()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetCascadedStyle(element);

        // Act
        var topStyle = style.Border_Top_Style;
        var rightStyle = style.Border_Right_Style;
        var bottomStyle = style.Border_Bottom_Style;
        var leftStyle = style.Border_Left_Style;

        // Assert
        Assert.NotNull(topStyle);
        Assert.NotNull(rightStyle);
        Assert.NotNull(bottomStyle);
        Assert.NotNull(leftStyle);
        Assert.IsType<EnumProperty<EBorderStyle>>(topStyle);
        Assert.IsType<EnumProperty<EBorderStyle>>(rightStyle);
        Assert.IsType<EnumProperty<EBorderStyle>>(bottomStyle);
        Assert.IsType<EnumProperty<EBorderStyle>>(leftStyle);
    }

    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "PropertyAccess")]
    public void Border_Width_Properties_ReturnIntProperties()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetCascadedStyle(element);

        // Act
        var topWidth = style.Border_Top_Width;
        var rightWidth = style.Border_Right_Width;
        var bottomWidth = style.Border_Bottom_Width;
        var leftWidth = style.Border_Left_Width;

        // Assert
        Assert.NotNull(topWidth);
        Assert.NotNull(rightWidth);
        Assert.NotNull(bottomWidth);
        Assert.NotNull(leftWidth);
        Assert.IsType<IntProperty>(topWidth);
        Assert.IsType<IntProperty>(rightWidth);
        Assert.IsType<IntProperty>(bottomWidth);
        Assert.IsType<IntProperty>(leftWidth);
    }
    #endregion

    #region Min/Max Size Property Accessors
    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "PropertyAccess")]
    public void MinSize_Properties_ReturnIntProperties()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetCascadedStyle(element);

        // Act
        var minWidth = style.Min_Width;
        var minHeight = style.Min_Height;

        // Assert
        Assert.NotNull(minWidth);
        Assert.NotNull(minHeight);
        Assert.IsType<IntProperty>(minWidth);
        Assert.IsType<IntProperty>(minHeight);
    }

    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "PropertyAccess")]
    public void MaxSize_Properties_ReturnNullableIntProperties()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetCascadedStyle(element);

        // Act
        var maxWidth = style.Max_Width;
        var maxHeight = style.Max_Height;

        // Assert
        Assert.NotNull(maxWidth);
        Assert.NotNull(maxHeight);
        Assert.IsType<NullableIntProperty>(maxWidth);
        Assert.IsType<NullableIntProperty>(maxHeight);
    }
    #endregion

    #region Font Property Accessors
    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "PropertyAccess")]
    public void Font_Properties_ReturnCorrectTypes()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetCascadedStyle(element);

        // Act
        var fontWeight = style.FontWeight;
        var fontStyle = style.FontStyle;
        var fontSize = style.FontSize;
        var fontFamily = style.FontFamily;

        // Assert
        Assert.NotNull(fontWeight);
        Assert.NotNull(fontStyle);
        Assert.NotNull(fontSize);
        Assert.NotNull(fontFamily);
        Assert.IsType<IntProperty>(fontWeight);
        Assert.IsType<EnumProperty<EFontStyle>>(fontStyle);
        Assert.IsType<NumberProperty>(fontSize);
        Assert.IsType<MultiStringProperty>(fontFamily);
    }
    #endregion

    #region Other Property Accessors
    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "PropertyAccess")]
    public void Color_Property_ReturnsColorProperty()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetCascadedStyle(element);

        // Act
        var color = style.Color;

        // Assert
        Assert.NotNull(color);
        Assert.IsType<ColorProperty>(color);
    }

    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "PropertyAccess")]
    public void Opacity_Property_ReturnsNumberProperty()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetCascadedStyle(element);

        // Act
        var opacity = style.Opacity;

        // Assert
        Assert.NotNull(opacity);
        Assert.IsType<NumberProperty>(opacity);
    }

    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "PropertyAccess")]
    public void Transform_Property_ReturnsTransformListProperty()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetCascadedStyle(element);

        // Act
        var transform = style.Transform;

        // Assert
        Assert.NotNull(transform);
        Assert.IsType<TransformListProperty>(transform);
    }

    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "PropertyAccess")]
    public void Overflow_Properties_ReturnEnumProperties()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetCascadedStyle(element);

        // Act
        var overflowX = style.Overflow_X;
        var overflowY = style.Overflow_Y;

        // Assert
        Assert.NotNull(overflowX);
        Assert.NotNull(overflowY);
        Assert.IsType<EnumProperty<EOverflowMode>>(overflowX);
        Assert.IsType<EnumProperty<EOverflowMode>>(overflowY);
    }

    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "PropertyAccess")]
    public void ObjectFit_Property_ReturnsEnumProperty()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetCascadedStyle(element);

        // Act
        var objectFit = style.ObjectFit;

        // Assert
        Assert.NotNull(objectFit);
        Assert.IsType<EnumProperty<EObjectFit>>(objectFit);
    }

    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "PropertyAccess")]
    public void ObjectPosition_Property_ReturnsPositionProperty()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetCascadedStyle(element);

        // Act
        var objectPosition = style.ObjectPosition;

        // Assert
        Assert.NotNull(objectPosition);
        Assert.IsType<PositionProperty>(objectPosition);
    }
    #endregion

    #region Fragmentation Property Accessors
    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "PropertyAccess")]
    [Trait("Category", "Fragmentation")]
    public void Break_Properties_ReturnEnumProperties()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetCascadedStyle(element);

        // Act
        var breakBefore = style.BreakBefore;
        var breakAfter = style.BreakAfter;
        var breakInside = style.BreakInside;

        // Assert
        Assert.NotNull(breakBefore);
        Assert.NotNull(breakAfter);
        Assert.NotNull(breakInside);
        Assert.IsType<EnumProperty<EBreakValue>>(breakBefore);
        Assert.IsType<EnumProperty<EBreakValue>>(breakAfter);
        Assert.IsType<EnumProperty<EBreakValue>>(breakInside);
    }

    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "PropertyAccess")]
    [Trait("Category", "Fragmentation")]
    public void Orphans_And_Widows_Properties_ReturnIntProperties()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetCascadedStyle(element);

        // Act
        var orphans = style.Orphans;
        var widows = style.Widows;

        // Assert
        Assert.NotNull(orphans);
        Assert.NotNull(widows);
        Assert.IsType<IntProperty>(orphans);
        Assert.IsType<IntProperty>(widows);
    }

    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "PropertyAccess")]
    [Trait("Category", "Fragmentation")]
    public void BoxDecorationBreak_Property_ReturnsEnumProperty()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetCascadedStyle(element);

        // Act
        var boxDecorationBreak = style.BoxDecorationBreak;

        // Assert
        Assert.NotNull(boxDecorationBreak);
        Assert.IsType<EnumProperty<EBoxDecorationBreak>>(boxDecorationBreak);
    }
    #endregion

    #region GetAll() Method Tests
    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "PropertyAccess")]
    public void GetAll_ReturnsAllRegisteredProperties()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetCascadedStyle(element);

        // Act
        var allProperties = style.GetAll().Where(p => p != null).ToList();

        // Assert
        Assert.NotEmpty(allProperties);
        // Should have at least the common properties
        Assert.Contains(allProperties, p => p.CssName == ECssPropertyID.Width);
        Assert.Contains(allProperties, p => p.CssName == ECssPropertyID.Height);
        Assert.Contains(allProperties, p => p.CssName == ECssPropertyID.Display);
    }

    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "PropertyAccess")]
    public void GetAll_WithPredicate_FiltersProperties()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetCascadedStyle(element);

        // Act - Get only IntProperty instances
        var intProperties = style.GetAll(p => p is IntProperty).ToList();

        // Assert
        Assert.NotEmpty(intProperties);
        Assert.All(intProperties, p => Assert.IsType<IntProperty>(p));
    }
    #endregion

    #endregion

    #region 12.8.2 SetProperties Flag Tracking Tests

    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "SetProperties")]
    public void SetProperties_InitialState_IsEmpty()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetUserRulesStyle(element);

        // Assert - SetProperties should be empty initially (no properties assigned yet)
        Assert.True(style.SetProperties.IsEmpty());
        Assert.Equal(0, style.SetProperties.ActiveFlags);
    }

    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "SetProperties")]
    public void SetProperties_AfterAssignment_FlagIsSet()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetUserRulesStyle(element);

        // Verify style is not read-only for this test
        Assert.False(style.ReadOnly);

        // Act - Assign a value to width property
        var widthProperty = style.Width;
        widthProperty.Assigned = CssValue.From_Dimension(100.0, ECssUnit.PX);

        // Assert - The SetProperties flag should now be set for Width
        Assert.False(style.SetProperties.IsEmpty());
        Assert.True(style.SetProperties.GetFlag(ECssPropertyID.Width));
        Assert.Equal(1, style.SetProperties.ActiveFlags);
    }

    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "SetProperties")]
    public void SetProperties_UnassignedProperty_FlagNotSet()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetUserRulesStyle(element);

        // Act - Assign value to width only
        var widthProperty = style.Width;
        widthProperty.Assigned = CssValue.From_Dimension(100.0, ECssUnit.PX);

        // Assert - Height flag should NOT be set (never assigned)
        Assert.False(style.SetProperties.GetFlag(ECssPropertyID.Height));
        // But width should be set
        Assert.True(style.SetProperties.GetFlag(ECssPropertyID.Width));
    }

    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "SetProperties")]
    public void SetProperties_MultipleAssignments_AllFlagsSet()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetUserRulesStyle(element);

        // Act - Assign multiple properties
        style.Width.Assigned = CssValue.From_Dimension(100.0, ECssUnit.PX);
        style.Height.Assigned = CssValue.From_Dimension(200.0, ECssUnit.PX);
        style.Margin_Top.Assigned = CssValue.From_Dimension(10.0, ECssUnit.PX);

        // Assert - All three flags should be set
        Assert.True(style.SetProperties.GetFlag(ECssPropertyID.Width));
        Assert.True(style.SetProperties.GetFlag(ECssPropertyID.Height));
        Assert.True(style.SetProperties.GetFlag(ECssPropertyID.MarginTop));
        Assert.Equal(3, style.SetProperties.ActiveFlags);
    }

    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "SetProperties")]
    public void SetProperties_AssignSamePropertyTwice_FlagStaysSet()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetUserRulesStyle(element);

        // Act - Assign width twice
        style.Width.Assigned = CssValue.From_Dimension(100.0, ECssUnit.PX);
        style.Width.Assigned = CssValue.From_Dimension(200.0, ECssUnit.PX);

        // Assert - Flag should still be set, count should be 1 (not 2)
        Assert.True(style.SetProperties.GetFlag(ECssPropertyID.Width));
        Assert.Equal(1, style.SetProperties.ActiveFlags);
    }

    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "SetProperties")]
    public void SetProperties_AssignNullValue_ThrowsArgumentNullException()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetUserRulesStyle(element);

        // Act & Assert - Assigning null throws ArgumentNullException
        Assert.Throws<ArgumentNullException>(() => style.Width.Assigned = null!);
    }

    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "SetProperties")]
    public void SetProperties_AssignCssValueNull_ThrowsCssException()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetUserRulesStyle(element);

        // Act & Assert - Assigning CssValue.Null throws CssException
        Assert.Throws<CssException>(() => style.Width.Assigned = CssValue.Null);
    }

    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "SetProperties")]
    public void SetProperties_CanEnumerateSetFlags()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetUserRulesStyle(element);

        // Act - Set multiple properties
        style.Width.Assigned = CssValue.From_Dimension(100.0, ECssUnit.PX);
        style.Height.Assigned = CssValue.From_Dimension(200.0, ECssUnit.PX);
        style.Display.Assigned = CssValue.From(EDisplayMode.FLEX);

        // Assert - Can enumerate all set flags
        var setFlags = style.SetProperties.ToList();
        Assert.Equal(3, setFlags.Count);
        Assert.Contains(ECssPropertyID.Width, setFlags);
        Assert.Contains(ECssPropertyID.Height, setFlags);
        Assert.Contains(ECssPropertyID.Display, setFlags);
    }

    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "SetProperties")]
    public void SetProperties_ClearFlag_RemovesFlag()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetUserRulesStyle(element);

        // Set properties
        style.Width.Assigned = CssValue.From_Dimension(100.0, ECssUnit.PX);
        style.Height.Assigned = CssValue.From_Dimension(200.0, ECssUnit.PX);
        Assert.Equal(2, style.SetProperties.ActiveFlags);

        // Act - Clear one flag
        style.SetProperties.ClearFlag(ECssPropertyID.Width);

        // Assert - Only one flag remains
        Assert.False(style.SetProperties.GetFlag(ECssPropertyID.Width));
        Assert.True(style.SetProperties.GetFlag(ECssPropertyID.Height));
        Assert.Equal(1, style.SetProperties.ActiveFlags);
    }

    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "SetProperties")]
    public void SetProperties_Clear_RemovesAllFlags()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetUserRulesStyle(element);

        // Set multiple properties
        style.Width.Assigned = CssValue.From_Dimension(100.0, ECssUnit.PX);
        style.Height.Assigned = CssValue.From_Dimension(200.0, ECssUnit.PX);
        style.Display.Assigned = CssValue.From(EDisplayMode.FLEX);
        Assert.Equal(3, style.SetProperties.ActiveFlags);

        // Act - Clear all
        style.SetProperties.Clear();

        // Assert - All flags cleared
        Assert.True(style.SetProperties.IsEmpty());
        Assert.Equal(0, style.SetProperties.ActiveFlags);
    }

    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "SetProperties")]
    public void SetProperties_AssignInitial_FlagStillSet()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetUserRulesStyle(element);

        // Set property to a value first
        style.Width.Assigned = CssValue.From_Dimension(100.0, ECssUnit.PX);
        Assert.True(style.SetProperties.GetFlag(ECssPropertyID.Width));

        // Act - Assign CssValue.Initial (CSS "initial" keyword)
        // This is still an explicit assignment, so the flag should remain set
        style.Width.Assigned = CssValue.Initial;

        // Assert - Flag is still set (property was explicitly set, even to initial)
        Assert.True(style.SetProperties.GetFlag(ECssPropertyID.Width));
        Assert.Equal(CssValue.Initial, style.Width.Assigned);
    }

    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "SetProperties")]
    public void SetProperties_AssignUnset_FlagStillSet()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetUserRulesStyle(element);

        // Set property to a value first
        style.Width.Assigned = CssValue.From_Dimension(100.0, ECssUnit.PX);
        Assert.True(style.SetProperties.GetFlag(ECssPropertyID.Width));

        // Act - Assign CssValue.Unset (CSS "unset" keyword)
        // This is still an explicit assignment, so the flag should remain set
        style.Width.Assigned = CssValue.Unset;

        // Assert - Flag is still set (property was explicitly set, even to unset)
        Assert.True(style.SetProperties.GetFlag(ECssPropertyID.Width));
        Assert.Equal(CssValue.Unset, style.Width.Assigned);
    }

    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "SetProperties")]
    public void SetProperties_AssignInherit_FlagStillSet()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetUserRulesStyle(element);

        // Set property to a value first
        style.Width.Assigned = CssValue.From_Dimension(100.0, ECssUnit.PX);
        Assert.True(style.SetProperties.GetFlag(ECssPropertyID.Width));

        // Act - Assign CssValue.Inherit (CSS "inherit" keyword)
        // This is still an explicit assignment, so the flag should remain set
        style.Width.Assigned = CssValue.Inherit;

        // Assert - Flag is still set (property was explicitly set, even to inherit)
        Assert.True(style.SetProperties.GetFlag(ECssPropertyID.Width));
        Assert.Equal(CssValue.Inherit, style.Width.Assigned);
    }

    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "SetProperties")]
    public void SetProperties_ClearFlagAfterAssignment_PropertyResetComplete()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetUserRulesStyle(element);

        // Set property
        style.Width.Assigned = CssValue.From_Dimension(100.0, ECssUnit.PX);
        Assert.True(style.SetProperties.GetFlag(ECssPropertyID.Width));
        Assert.Equal(1, style.SetProperties.ActiveFlags);

        // Act - Clear the flag to truly "reset" the property tracking
        style.SetProperties.ClearFlag(ECssPropertyID.Width);

        // Assert - Property no longer tracked as set
        Assert.False(style.SetProperties.GetFlag(ECssPropertyID.Width));
        Assert.Equal(0, style.SetProperties.ActiveFlags);
        Assert.True(style.SetProperties.IsEmpty());

        // Note: The property's Assigned value is still there (CssProperty doesn't clear),
        // but the tracking flag is cleared, which matters for cascade processing
    }
    #endregion

    #region 12.8.3 ReadOnly Enforcement Tests

    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "ReadOnly")]
    public void ReadOnly_CascadedStyle_IsTrue()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetCascadedStyle(element);

        // Assert - Cascaded style is read-only (it's the result of cascade)
        Assert.True(style.ReadOnly);
    }

    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "ReadOnly")]
    public void ReadOnly_UserRulesStyle_IsFalse()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetUserRulesStyle(element);

        // Assert - UserRules style is writable
        Assert.False(style.ReadOnly);
    }

    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "ReadOnly")]
    public void ReadOnly_False_AllowsPropertyChanges()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetUserRulesStyle(element);

        // Ensure not read-only
        Assert.False(style.ReadOnly);

        // Act & Assert - Should not throw
        var exception = Record.Exception(() =>
        {
            style.Width.Assigned = CssValue.From_Dimension(100.0, ECssUnit.PX);
        });
        Assert.Null(exception);

        // Verify the value was set
        Assert.Equal(CssValue.From_Dimension(100.0, ECssUnit.PX), style.Width.Assigned);
    }
    #endregion

    #region 12.8.5 Selector and Origin Tests

    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "Selector")]
    public void Selector_Initially_IsNull()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetCascadedStyle(element);

        // Assert - Selector may be null for element styles
        // The exact initial state depends on how the style was created
        // For element.Style.Cascaded, it typically has no selector
        Assert.Null(style.Selector);
    }

    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "Selector")]
    public void Name_ReturnsIdentifiableString()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetCascadedStyle(element);

        // Act
        var name = style.Name;

        // Assert - Name should be a non-empty string
        Assert.NotNull(name);
        Assert.NotEmpty(name);
        Assert.Contains("CssComputedStyle", name);
    }

    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "Selector")]
    public void Name_CanBeSet()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetCascadedStyle(element);

        // Act
        style.Name = "TestStyle";

        // Assert
        Assert.Equal("TestStyle", style.Name);
    }

    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "Selector")]
    public void ID_IsUnique()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element1 = CreateTestElement(doc, "div");
        var element2 = CreateTestElement(doc, "span");
        var style1 = GetCascadedStyle(element1);
        var style2 = GetCascadedStyle(element2);

        // Assert - Each CssComputedStyle should have a unique ID
        Assert.NotEqual(style1.ID, style2.ID);
    }

    [Fact]
    [Trait("Category", "CssComputedStyle")]
    [Trait("Category", "Selector")]
    public void ID_IsNonZero()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = GetCascadedStyle(element);

        // Assert - ID should be a positive value
        Assert.True(style.ID > 0);
    }
    #endregion
}
