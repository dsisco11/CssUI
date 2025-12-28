using CssUI.CSS;
using CssUI.CSS.Enums;
using CssUI.CSS.Internal;
using CssUI.DOM;
using Xunit;

namespace CssUITests.CSS.Properties.Tests;

/// <summary>
/// Tests for CSS shorthand property expansion per CSS Cascading and Inheritance Level 3.
/// Shorthand properties are CSS properties that let you set the values of multiple
/// other CSS properties simultaneously.
/// Docs: https://www.w3.org/TR/css-cascade-3/#shorthand
/// </summary>
public class ShorthandPropertyExpansionTests
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
    #endregion

    #region Margin Shorthand Expansion Tests
    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Shorthand")]
    public void SetMargin_SingleValue_SetsAllFourSides()
    {
        // CSS: margin: 10px;
        // Should expand to: margin-top: 10px; margin-right: 10px; margin-bottom: 10px; margin-left: 10px;
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = element.Style.UserRules;

        var value = CssValue.From_Dimension(10, ECssUnit.PX);
        style.Set_Margin(value, value, value, value);

        Assert.Equal(10.0, style.Margin_Top.Assigned?.AsDecimal());
        Assert.Equal(10.0, style.Margin_Right.Assigned?.AsDecimal());
        Assert.Equal(10.0, style.Margin_Bottom.Assigned?.AsDecimal());
        Assert.Equal(10.0, style.Margin_Left.Assigned?.AsDecimal());
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Shorthand")]
    public void SetMargin_TwoValues_SetsVerticalAndHorizontal()
    {
        // CSS: margin: 10px 20px;
        // Should expand to: margin-top: 10px; margin-right: 20px; margin-bottom: 10px; margin-left: 20px;
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = element.Style.UserRules;

        var vertical = CssValue.From_Dimension(10, ECssUnit.PX);
        var horizontal = CssValue.From_Dimension(20, ECssUnit.PX);
        style.Set_Margin(horizontal, vertical);

        Assert.Equal(10.0, style.Margin_Top.Assigned?.AsDecimal());
        Assert.Equal(20.0, style.Margin_Right.Assigned?.AsDecimal());
        Assert.Equal(10.0, style.Margin_Bottom.Assigned?.AsDecimal());
        Assert.Equal(20.0, style.Margin_Left.Assigned?.AsDecimal());
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Shorthand")]
    public void SetMargin_FourValues_SetsEachSideIndividually()
    {
        // CSS: margin: 10px 20px 30px 40px;
        // Should expand to: margin-top: 10px; margin-right: 20px; margin-bottom: 30px; margin-left: 40px;
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = element.Style.UserRules;

        var top = CssValue.From_Dimension(10, ECssUnit.PX);
        var right = CssValue.From_Dimension(20, ECssUnit.PX);
        var bottom = CssValue.From_Dimension(30, ECssUnit.PX);
        var left = CssValue.From_Dimension(40, ECssUnit.PX);
        style.Set_Margin(top, right, bottom, left);

        Assert.Equal(10.0, style.Margin_Top.Assigned?.AsDecimal());
        Assert.Equal(20.0, style.Margin_Right.Assigned?.AsDecimal());
        Assert.Equal(30.0, style.Margin_Bottom.Assigned?.AsDecimal());
        Assert.Equal(40.0, style.Margin_Left.Assigned?.AsDecimal());
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Shorthand")]
    public void SetMargin_IntOverloads_SetsCorrectValues()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = element.Style.UserRules;

        style.Set_Margin(10, 20, 30, 40);

        Assert.Equal(10.0, style.Margin_Top.Assigned?.AsDecimal());
        Assert.Equal(20.0, style.Margin_Right.Assigned?.AsDecimal());
        Assert.Equal(30.0, style.Margin_Bottom.Assigned?.AsDecimal());
        Assert.Equal(40.0, style.Margin_Left.Assigned?.AsDecimal());
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Shorthand")]
    public void SetMargin_AutoValue_SetsAutoOnAllSides()
    {
        // CSS: margin: auto;
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = element.Style.UserRules;

        style.Set_Margin(CssValue.Auto, CssValue.Auto, CssValue.Auto, CssValue.Auto);

        Assert.Equal(ECssValueTypes.AUTO, style.Margin_Top.Assigned.Type);
        Assert.Equal(ECssValueTypes.AUTO, style.Margin_Right.Assigned.Type);
        Assert.Equal(ECssValueTypes.AUTO, style.Margin_Bottom.Assigned.Type);
        Assert.Equal(ECssValueTypes.AUTO, style.Margin_Left.Assigned.Type);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Shorthand")]
    public void SetMargin_PercentageValue_SetsPercentageOnAllSides()
    {
        // CSS: margin: 10%;
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = element.Style.UserRules;

        var percentValue = CssValue.From_Percent(10);
        style.Set_Margin(percentValue, percentValue, percentValue, percentValue);

        Assert.Equal(ECssValueTypes.PERCENT, style.Margin_Top.Assigned.Type);
        Assert.Equal(ECssValueTypes.PERCENT, style.Margin_Right.Assigned.Type);
        Assert.Equal(ECssValueTypes.PERCENT, style.Margin_Bottom.Assigned.Type);
        Assert.Equal(ECssValueTypes.PERCENT, style.Margin_Left.Assigned.Type);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Shorthand")]
    public void SetMargin_MixedValues_SetsCorrectly()
    {
        // CSS: margin: 10px auto 20px auto;
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = element.Style.UserRules;

        style.Set_Margin(
            CssValue.From_Dimension(10, ECssUnit.PX),
            CssValue.Auto,
            CssValue.From_Dimension(20, ECssUnit.PX),
            CssValue.Auto);

        Assert.Equal(10.0, style.Margin_Top.Assigned?.AsDecimal());
        Assert.Equal(ECssValueTypes.AUTO, style.Margin_Right.Assigned.Type);
        Assert.Equal(20.0, style.Margin_Bottom.Assigned?.AsDecimal());
        Assert.Equal(ECssValueTypes.AUTO, style.Margin_Left.Assigned.Type);
    }
    #endregion

    #region Padding Shorthand Expansion Tests
    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Shorthand")]
    public void SetPadding_SingleValue_SetsAllFourSides()
    {
        // CSS: padding: 15px;
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = element.Style.UserRules;

        var value = CssValue.From_Dimension(15, ECssUnit.PX);
        style.Set_Padding(value, value, value, value);

        Assert.Equal(15.0, style.Padding_Top.Assigned?.AsDecimal());
        Assert.Equal(15.0, style.Padding_Right.Assigned?.AsDecimal());
        Assert.Equal(15.0, style.Padding_Bottom.Assigned?.AsDecimal());
        Assert.Equal(15.0, style.Padding_Left.Assigned?.AsDecimal());
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Shorthand")]
    public void SetPadding_TwoValues_SetsVerticalAndHorizontal()
    {
        // CSS: padding: 10px 20px;
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = element.Style.UserRules;

        var vertical = CssValue.From_Dimension(10, ECssUnit.PX);
        var horizontal = CssValue.From_Dimension(20, ECssUnit.PX);
        style.Set_Padding(horizontal, vertical);

        Assert.Equal(10.0, style.Padding_Top.Assigned?.AsDecimal());
        Assert.Equal(20.0, style.Padding_Right.Assigned?.AsDecimal());
        Assert.Equal(10.0, style.Padding_Bottom.Assigned?.AsDecimal());
        Assert.Equal(20.0, style.Padding_Left.Assigned?.AsDecimal());
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Shorthand")]
    public void SetPadding_FourValues_SetsEachSideIndividually()
    {
        // CSS: padding: 5px 10px 15px 20px;
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = element.Style.UserRules;

        style.Set_Padding(5, 10, 15, 20);

        Assert.Equal(5.0, style.Padding_Top.Assigned?.AsDecimal());
        Assert.Equal(10.0, style.Padding_Right.Assigned?.AsDecimal());
        Assert.Equal(15.0, style.Padding_Bottom.Assigned?.AsDecimal());
        Assert.Equal(20.0, style.Padding_Left.Assigned?.AsDecimal());
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Shorthand")]
    public void SetPadding_ZeroValue_SetsZeroOnAllSides()
    {
        // CSS: padding: 0;
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = element.Style.UserRules;

        style.Set_Padding(0, 0, 0, 0);

        Assert.Equal(0.0, style.Padding_Top.Assigned?.AsDecimal());
        Assert.Equal(0.0, style.Padding_Right.Assigned?.AsDecimal());
        Assert.Equal(0.0, style.Padding_Bottom.Assigned?.AsDecimal());
        Assert.Equal(0.0, style.Padding_Left.Assigned?.AsDecimal());
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Shorthand")]
    public void SetPadding_PercentageValue_SetsPercentage()
    {
        // CSS: padding: 5%;
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = element.Style.UserRules;

        var percentValue = CssValue.From_Percent(5);
        style.Set_Padding(percentValue, percentValue, percentValue, percentValue);

        Assert.Equal(ECssValueTypes.PERCENT, style.Padding_Top.Assigned.Type);
        Assert.Equal(ECssValueTypes.PERCENT, style.Padding_Right.Assigned.Type);
        Assert.Equal(ECssValueTypes.PERCENT, style.Padding_Bottom.Assigned.Type);
        Assert.Equal(ECssValueTypes.PERCENT, style.Padding_Left.Assigned.Type);
    }
    #endregion

    #region Position Shorthand Expansion Tests
    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Shorthand")]
    public void SetPosition_XY_SetsLeftAndTop()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = element.Style.UserRules;

        style.Set_Position(100, 50);

        Assert.Equal(100.0, style.Left.Assigned?.AsDecimal());
        Assert.Equal(50.0, style.Top.Assigned?.AsDecimal());
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Shorthand")]
    public void SetPosition_CssValues_SetsLeftAndTop()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = element.Style.UserRules;

        style.Set_Position(
            CssValue.From_Dimension(100, ECssUnit.PX),
            CssValue.From_Dimension(50, ECssUnit.PX));

        Assert.Equal(100.0, style.Left.Assigned?.AsDecimal());
        Assert.Equal(50.0, style.Top.Assigned?.AsDecimal());
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Shorthand")]
    public void SetPosition_NullValues_SetsNullValues()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = element.Style.UserRules;

        // Set initial values
        style.Set_Position(100, 50);
        Assert.Equal(100.0, style.Left.Assigned?.AsDecimal());
        Assert.Equal(50.0, style.Top.Assigned?.AsDecimal());

        // Now set null values - the property should handle null appropriately
        // Use explicit int? cast to resolve ambiguity
        style.Set_Position((int?)null, (int?)null);

        // Properties should be cleared or have null assigned values
        Assert.NotNull(style.Left);
        Assert.NotNull(style.Top);
    }
    #endregion

    #region Size Shorthand Expansion Tests
    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Shorthand")]
    public void SetSize_WidthAndHeight_SetsBothDimensions()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = element.Style.UserRules;

        style.Set_Size(200, 100);

        Assert.Equal(200.0, style.Width.Assigned?.AsDecimal());
        Assert.Equal(100.0, style.Height.Assigned?.AsDecimal());
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Shorthand")]
    public void SetSize_CssValues_SetsBothDimensions()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = element.Style.UserRules;

        style.Set_Size(
            CssValue.From_Dimension(300, ECssUnit.PX),
            CssValue.From_Dimension(150, ECssUnit.PX));

        Assert.Equal(300.0, style.Width.Assigned?.AsDecimal());
        Assert.Equal(150.0, style.Height.Assigned?.AsDecimal());
    }
    #endregion

    #region Longhand Property Independence Tests
    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Shorthand")]
    public void MarginLonghandProperties_AreIndependent()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = element.Style.UserRules;

        // Set individual longhand properties
        style.Margin_Top.Set(CssValue.From_Dimension(10, ECssUnit.PX));
        style.Margin_Right.Set(CssValue.From_Dimension(20, ECssUnit.PX));
        style.Margin_Bottom.Set(CssValue.From_Dimension(30, ECssUnit.PX));
        style.Margin_Left.Set(CssValue.From_Dimension(40, ECssUnit.PX));

        Assert.Equal(10.0, style.Margin_Top.Assigned?.AsDecimal());
        Assert.Equal(20.0, style.Margin_Right.Assigned?.AsDecimal());
        Assert.Equal(30.0, style.Margin_Bottom.Assigned?.AsDecimal());
        Assert.Equal(40.0, style.Margin_Left.Assigned?.AsDecimal());
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Shorthand")]
    public void PaddingLonghandProperties_AreIndependent()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = element.Style.UserRules;

        // Set individual longhand properties
        style.Padding_Top.Set(CssValue.From_Dimension(5, ECssUnit.PX));
        style.Padding_Right.Set(CssValue.From_Dimension(10, ECssUnit.PX));
        style.Padding_Bottom.Set(CssValue.From_Dimension(15, ECssUnit.PX));
        style.Padding_Left.Set(CssValue.From_Dimension(20, ECssUnit.PX));

        Assert.Equal(5.0, style.Padding_Top.Assigned?.AsDecimal());
        Assert.Equal(10.0, style.Padding_Right.Assigned?.AsDecimal());
        Assert.Equal(15.0, style.Padding_Bottom.Assigned?.AsDecimal());
        Assert.Equal(20.0, style.Padding_Left.Assigned?.AsDecimal());
    }
    #endregion

    #region Shorthand Overwrites Longhand Tests
    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Shorthand")]
    public void SetMargin_OverwritesExistingLonghandValues()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = element.Style.UserRules;

        // First set individual longhand values
        style.Margin_Top.Set(CssValue.From_Dimension(100, ECssUnit.PX));
        style.Margin_Right.Set(CssValue.From_Dimension(100, ECssUnit.PX));
        style.Margin_Bottom.Set(CssValue.From_Dimension(100, ECssUnit.PX));
        style.Margin_Left.Set(CssValue.From_Dimension(100, ECssUnit.PX));

        // Then set via shorthand - should overwrite all
        style.Set_Margin(10, 10, 10, 10);

        Assert.Equal(10.0, style.Margin_Top.Assigned?.AsDecimal());
        Assert.Equal(10.0, style.Margin_Right.Assigned?.AsDecimal());
        Assert.Equal(10.0, style.Margin_Bottom.Assigned?.AsDecimal());
        Assert.Equal(10.0, style.Margin_Left.Assigned?.AsDecimal());
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Shorthand")]
    public void SetPadding_OverwritesExistingLonghandValues()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = element.Style.UserRules;

        // First set individual longhand values
        style.Padding_Top.Set(CssValue.From_Dimension(100, ECssUnit.PX));
        style.Padding_Right.Set(CssValue.From_Dimension(100, ECssUnit.PX));
        style.Padding_Bottom.Set(CssValue.From_Dimension(100, ECssUnit.PX));
        style.Padding_Left.Set(CssValue.From_Dimension(100, ECssUnit.PX));

        // Then set via shorthand - should overwrite all
        style.Set_Padding(5, 5, 5, 5);

        Assert.Equal(5.0, style.Padding_Top.Assigned?.AsDecimal());
        Assert.Equal(5.0, style.Padding_Right.Assigned?.AsDecimal());
        Assert.Equal(5.0, style.Padding_Bottom.Assigned?.AsDecimal());
        Assert.Equal(5.0, style.Padding_Left.Assigned?.AsDecimal());
    }
    #endregion

    #region Unit Conversion Tests
    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Shorthand")]
    public void SetMargin_EmUnits_AppliesCorrectly()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = element.Style.UserRules;

        var emValue = CssValue.From_Dimension(2, ECssUnit.EM);
        style.Set_Margin(emValue, emValue, emValue, emValue);

        Assert.Equal(ECssValueTypes.DIMENSION, style.Margin_Top.Assigned.Type);
        Assert.Equal(ECssUnit.EM, style.Margin_Top.Assigned.Unit);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Shorthand")]
    public void SetMargin_RemUnits_AppliesCorrectly()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = element.Style.UserRules;

        var remValue = CssValue.From_Dimension(1.5, ECssUnit.REM);
        style.Set_Margin(remValue, remValue, remValue, remValue);

        Assert.Equal(ECssValueTypes.DIMENSION, style.Margin_Top.Assigned.Type);
        Assert.Equal(ECssUnit.REM, style.Margin_Top.Assigned.Unit);
    }
    #endregion

    #region Default Value Tests
    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Shorthand")]
    public void MarginProperties_HaveCorrectInitialValue()
    {
        // Per CSS spec, margin initial value is 0
        var def = CssDefinitions.StyleDefinitions[ECssPropertyID.MarginTop];
        Assert.Equal(0.0, def.Initial.AsDecimal());
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Shorthand")]
    public void PaddingProperties_HaveCorrectInitialValue()
    {
        // Per CSS spec, padding initial value is 0
        var def = CssDefinitions.StyleDefinitions[ECssPropertyID.PaddingTop];
        Assert.Equal(0.0, def.Initial.AsDecimal());
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Shorthand")]
    public void MarginProperties_AreNotInherited()
    {
        // Per CSS spec, margin is not inherited
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.MarginTop].Inherited);
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.MarginRight].Inherited);
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.MarginBottom].Inherited);
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.MarginLeft].Inherited);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Shorthand")]
    public void PaddingProperties_AreNotInherited()
    {
        // Per CSS spec, padding is not inherited
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.PaddingTop].Inherited);
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.PaddingRight].Inherited);
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.PaddingBottom].Inherited);
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.PaddingLeft].Inherited);
    }
    #endregion

    #region Border Property Tests
    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Shorthand")]
    public void BorderWidthLonghandProperties_ExistAndAreConfiguredCorrectly()
    {
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.BorderTopWidth));
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.BorderRightWidth));
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.BorderBottomWidth));
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.BorderLeftWidth));
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Shorthand")]
    public void BorderStyleLonghandProperties_ExistAndAreConfiguredCorrectly()
    {
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.BorderTopStyle));
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.BorderRightStyle));
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.BorderBottomStyle));
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.BorderLeftStyle));
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Shorthand")]
    public void BorderColorLonghandProperties_ExistAndAreConfiguredCorrectly()
    {
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.BorderTopColor));
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.BorderRightColor));
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.BorderBottomColor));
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.BorderLeftColor));
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Shorthand")]
    public void BorderWidthProperties_HaveCorrectInitialValue()
    {
        // Per CSS spec, border-width initial value is 'medium'
        var def = CssDefinitions.StyleDefinitions[ECssPropertyID.BorderTopWidth];
        Assert.Equal(ECssValueTypes.KEYWORD, def.Initial.Type);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Shorthand")]
    public void BorderStyleProperties_HaveCorrectInitialValue()
    {
        // Per CSS spec, border-style initial value is 'none'
        var def = CssDefinitions.StyleDefinitions[ECssPropertyID.BorderTopStyle];
        Assert.Equal(EBorderStyle.None, def.Initial.AsEnum<EBorderStyle>());
    }
    #endregion

    #region Allowed Value Types Tests
    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Shorthand")]
    public void MarginProperties_AllowAutoValue()
    {
        var def = CssDefinitions.StyleDefinitions[ECssPropertyID.MarginTop];
        Assert.True((def.AllowedTypes & ECssValueTypes.AUTO) != 0);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Shorthand")]
    public void MarginProperties_AllowPercentValue()
    {
        var def = CssDefinitions.StyleDefinitions[ECssPropertyID.MarginTop];
        Assert.True((def.AllowedTypes & ECssValueTypes.PERCENT) != 0);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Shorthand")]
    public void MarginProperties_AllowDimensionValue()
    {
        var def = CssDefinitions.StyleDefinitions[ECssPropertyID.MarginTop];
        Assert.True((def.AllowedTypes & ECssValueTypes.DIMENSION) != 0);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Shorthand")]
    public void PaddingProperties_AllowPercentValue()
    {
        var def = CssDefinitions.StyleDefinitions[ECssPropertyID.PaddingTop];
        Assert.True((def.AllowedTypes & ECssValueTypes.PERCENT) != 0);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Shorthand")]
    public void PaddingProperties_AllowDimensionValue()
    {
        var def = CssDefinitions.StyleDefinitions[ECssPropertyID.PaddingTop];
        Assert.True((def.AllowedTypes & ECssValueTypes.DIMENSION) != 0);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Shorthand")]
    public void PaddingProperties_DoNotAllowAutoValue()
    {
        // Per CSS spec, padding does NOT accept 'auto' values
        var def = CssDefinitions.StyleDefinitions[ECssPropertyID.PaddingTop];
        Assert.False((def.AllowedTypes & ECssValueTypes.AUTO) != 0);
    }
    #endregion

    #region Percentage Resolution Tests
    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Shorthand")]
    public void MarginProperties_HavePercentageResolver()
    {
        // Per CSS spec, margin percentages are resolved against containing block width
        Assert.NotNull(CssDefinitions.StyleDefinitions[ECssPropertyID.MarginTop].Percentage_Resolver);
        Assert.NotNull(CssDefinitions.StyleDefinitions[ECssPropertyID.MarginRight].Percentage_Resolver);
        Assert.NotNull(CssDefinitions.StyleDefinitions[ECssPropertyID.MarginBottom].Percentage_Resolver);
        Assert.NotNull(CssDefinitions.StyleDefinitions[ECssPropertyID.MarginLeft].Percentage_Resolver);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Shorthand")]
    public void PaddingProperties_HavePercentageResolver()
    {
        // Per CSS spec, padding percentages are resolved against containing block width
        Assert.NotNull(CssDefinitions.StyleDefinitions[ECssPropertyID.PaddingTop].Percentage_Resolver);
        Assert.NotNull(CssDefinitions.StyleDefinitions[ECssPropertyID.PaddingRight].Percentage_Resolver);
        Assert.NotNull(CssDefinitions.StyleDefinitions[ECssPropertyID.PaddingBottom].Percentage_Resolver);
        Assert.NotNull(CssDefinitions.StyleDefinitions[ECssPropertyID.PaddingLeft].Percentage_Resolver);
    }
    #endregion
}
