using CssUI.CSS;
using CssUI.CSS.Enums;
using CssUI.CSS.Internal;
using CssUI.Internal;
using Xunit;

namespace CssUITests.CSS.Properties.Tests;

/// <summary>
/// Tests for StyleDefinition - holds specification-defined information about CSS property values and resolution.
/// </summary>
public class StyleDefinitionTests
{
    #region Constructor Tests
    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "StyleDefinition")]
    public void Constructor_SetsNameProperty()
    {
        var def = new StyleDefinition(
            ECssPropertyID.Width,
            Inherited: false,
            EPropertyDirtFlags.Content_Area,
            CssValue.Auto);

        Assert.Equal(ECssPropertyID.Width, def.Name);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "StyleDefinition")]
    public void Constructor_SetsInheritedProperty()
    {
        var inheritedDef = new StyleDefinition(
            ECssPropertyID.Color,
            Inherited: true,
            EPropertyDirtFlags.Visual,
            CssValue.From(EColor.Black));

        var nonInheritedDef = new StyleDefinition(
            ECssPropertyID.Width,
            Inherited: false,
            EPropertyDirtFlags.Content_Area,
            CssValue.Auto);

        Assert.True(inheritedDef.Inherited);
        Assert.False(nonInheritedDef.Inherited);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "StyleDefinition")]
    public void Constructor_SetsFlagsProperty()
    {
        var def = new StyleDefinition(
            ECssPropertyID.Width,
            Inherited: false,
            EPropertyDirtFlags.Content_Area | EPropertyDirtFlags.Flow,
            CssValue.Auto);

        Assert.True((def.Flags & EPropertyDirtFlags.Content_Area) != 0);
        Assert.True((def.Flags & EPropertyDirtFlags.Flow) != 0);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "StyleDefinition")]
    public void Constructor_SetsInitialValue()
    {
        var def = new StyleDefinition(
            ECssPropertyID.FlexGrow,
            Inherited: false,
            EPropertyDirtFlags.Flow,
            CssValue.From(0.0));

        Assert.Equal(ECssValueTypes.NUMBER, def.Initial.Type);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "StyleDefinition")]
    public void Constructor_SetsAllowedTypes()
    {
        var def = new StyleDefinition(
            ECssPropertyID.Width,
            Inherited: false,
            EPropertyDirtFlags.Content_Area,
            CssValue.Auto,
            ECssValueTypes.DIMENSION | ECssValueTypes.PERCENT | ECssValueTypes.AUTO);

        // AllowedTypes should include both provided types and defaults (INITIAL, INHERIT, UNSET)
        Assert.True((def.AllowedTypes & ECssValueTypes.DIMENSION) != 0);
        Assert.True((def.AllowedTypes & ECssValueTypes.PERCENT) != 0);
        Assert.True((def.AllowedTypes & ECssValueTypes.AUTO) != 0);
        Assert.True((def.AllowedTypes & ECssValueTypes.INITIAL) != 0);
        Assert.True((def.AllowedTypes & ECssValueTypes.INHERIT) != 0);
        Assert.True((def.AllowedTypes & ECssValueTypes.UNSET) != 0);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "StyleDefinition")]
    public void Constructor_SetsKeywords()
    {
        var keywords = new[] { "auto", "min-content", "max-content" };
        var def = new StyleDefinition(
            ECssPropertyID.Width,
            Inherited: false,
            EPropertyDirtFlags.Content_Area,
            CssValue.Auto,
            ECssValueTypes.KEYWORD,
            keywords);

        Assert.Contains("auto", def.KeywordWhitelist);
        Assert.Contains("min-content", def.KeywordWhitelist);
        Assert.Contains("max-content", def.KeywordWhitelist);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "StyleDefinition")]
    public void Constructor_SetsIsPrivateFlag()
    {
        var publicDef = new StyleDefinition(
            ECssPropertyID.Width,
            Inherited: false,
            EPropertyDirtFlags.Content_Area,
            CssValue.Auto,
            IsPrivate: false);

        var privateDef = new StyleDefinition(
            ECssPropertyID.DpiX,
            Inherited: true,
            EPropertyDirtFlags.Flow,
            CssValue.Null,
            IsPrivate: true);

        Assert.False(publicDef.IsPrivate);
        Assert.True(privateDef.IsPrivate);
    }
    #endregion

    #region Is_Valid_Value_Type Tests
    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "StyleDefinition")]
    public void IsValidValueType_ReturnsTrueForAllowedType()
    {
        var def = new StyleDefinition(
            ECssPropertyID.Width,
            Inherited: false,
            EPropertyDirtFlags.Content_Area,
            CssValue.Auto,
            ECssValueTypes.DIMENSION | ECssValueTypes.PERCENT);

        Assert.True(def.Is_Valid_Value_Type(ECssValueTypes.DIMENSION));
        Assert.True(def.Is_Valid_Value_Type(ECssValueTypes.PERCENT));
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "StyleDefinition")]
    public void IsValidValueType_ReturnsFalseForDisallowedType()
    {
        var def = new StyleDefinition(
            ECssPropertyID.FlexDirection,
            Inherited: false,
            EPropertyDirtFlags.Flow,
            CssValue.From(EFlexDirection.Row),
            ECssValueTypes.KEYWORD);

        Assert.False(def.Is_Valid_Value_Type(ECssValueTypes.DIMENSION));
        Assert.False(def.Is_Valid_Value_Type(ECssValueTypes.NUMBER));
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "StyleDefinition")]
    public void IsValidValueType_AlwaysAllowsGlobalKeywords()
    {
        var def = new StyleDefinition(
            ECssPropertyID.Width,
            Inherited: false,
            EPropertyDirtFlags.Content_Area,
            CssValue.Auto);

        // Global keywords should always be valid
        Assert.True(def.Is_Valid_Value_Type(ECssValueTypes.INITIAL));
        Assert.True(def.Is_Valid_Value_Type(ECssValueTypes.INHERIT));
        Assert.True(def.Is_Valid_Value_Type(ECssValueTypes.UNSET));
    }
    #endregion

    #region CssDefinitions Registry Tests
    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "StyleDefinition")]
    public void CssDefinitions_ContainsDisplayProperty()
    {
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.Display));
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "StyleDefinition")]
    public void CssDefinitions_ContainsAllBoxModelProperties()
    {
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.Width));
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.Height));
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.MinWidth));
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.MinHeight));
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.MaxWidth));
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.MaxHeight));
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "StyleDefinition")]
    public void CssDefinitions_ContainsAllPaddingProperties()
    {
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.PaddingTop));
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.PaddingRight));
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.PaddingBottom));
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.PaddingLeft));
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "StyleDefinition")]
    public void CssDefinitions_ContainsAllMarginProperties()
    {
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.MarginTop));
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.MarginRight));
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.MarginBottom));
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.MarginLeft));
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "StyleDefinition")]
    public void CssDefinitions_ContainsAllBorderProperties()
    {
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.BorderTopWidth));
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.BorderRightWidth));
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.BorderBottomWidth));
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.BorderLeftWidth));
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.BorderTopColor));
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.BorderRightColor));
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.BorderBottomColor));
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.BorderLeftColor));
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.BorderTopStyle));
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.BorderRightStyle));
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.BorderBottomStyle));
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.BorderLeftStyle));
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "StyleDefinition")]
    public void CssDefinitions_ContainsAllFlexboxProperties()
    {
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.FlexDirection));
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.FlexWrap));
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.FlexGrow));
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.FlexShrink));
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.FlexBasis));
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.Order));
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "StyleDefinition")]
    public void CssDefinitions_ContainsAllGridProperties()
    {
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.GridAutoColumns));
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.GridAutoRows));
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.GridAutoFlow));
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.GridColumnStart));
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.GridColumnEnd));
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.GridRowStart));
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.GridRowEnd));
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.GridTemplateColumns));
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.GridTemplateRows));
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "StyleDefinition")]
    public void CssDefinitions_ContainsAllAlignmentProperties()
    {
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.AlignContent));
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.JustifyContent));
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.AlignItems));
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.AlignSelf));
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.JustifyItems));
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.JustifySelf));
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.RowGap));
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.ColumnGap));
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "StyleDefinition")]
    public void CssDefinitions_ContainsAllFontProperties()
    {
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.FontFamily));
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.FontSize));
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.FontWeight));
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.FontStyle));
        Assert.True(CssDefinitions.StyleDefinitions.ContainsKey(ECssPropertyID.LineHeight));
    }
    #endregion

    #region Property Default Values Tests
    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "StyleDefinition")]
    public void CssDefinitions_DisplayHasCorrectInitialValue()
    {
        var def = CssDefinitions.StyleDefinitions[ECssPropertyID.Display];
        Assert.Equal(ECssValueTypes.KEYWORD, def.Initial.Type);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "StyleDefinition")]
    public void CssDefinitions_WidthHasAutoAsInitialValue()
    {
        var def = CssDefinitions.StyleDefinitions[ECssPropertyID.Width];
        Assert.Equal(ECssValueTypes.AUTO, def.Initial.Type);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "StyleDefinition")]
    public void CssDefinitions_HeightHasAutoAsInitialValue()
    {
        var def = CssDefinitions.StyleDefinitions[ECssPropertyID.Height];
        Assert.Equal(ECssValueTypes.AUTO, def.Initial.Type);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "StyleDefinition")]
    public void CssDefinitions_PaddingHasZeroAsInitialValue()
    {
        var def = CssDefinitions.StyleDefinitions[ECssPropertyID.PaddingTop];
        Assert.Equal(0, def.Initial.AsInteger());
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "StyleDefinition")]
    public void CssDefinitions_MarginHasZeroAsInitialValue()
    {
        var def = CssDefinitions.StyleDefinitions[ECssPropertyID.MarginTop];
        Assert.Equal(0, def.Initial.AsInteger());
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "StyleDefinition")]
    public void CssDefinitions_FlexGrowHasZeroAsInitialValue()
    {
        var def = CssDefinitions.StyleDefinitions[ECssPropertyID.FlexGrow];
        Assert.Equal(ECssValueTypes.NUMBER, def.Initial.Type);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "StyleDefinition")]
    public void CssDefinitions_FlexShrinkHasOneAsInitialValue()
    {
        var def = CssDefinitions.StyleDefinitions[ECssPropertyID.FlexShrink];
        Assert.Equal(ECssValueTypes.NUMBER, def.Initial.Type);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "StyleDefinition")]
    public void CssDefinitions_FlexBasisHasAutoAsInitialValue()
    {
        var def = CssDefinitions.StyleDefinitions[ECssPropertyID.FlexBasis];
        Assert.Equal(ECssValueTypes.AUTO, def.Initial.Type);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "StyleDefinition")]
    public void CssDefinitions_MaxWidthHasNoneAsInitialValue()
    {
        var def = CssDefinitions.StyleDefinitions[ECssPropertyID.MaxWidth];
        Assert.Equal(ECssValueTypes.NONE, def.Initial.Type);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "StyleDefinition")]
    public void CssDefinitions_OpacityHasOneAsInitialValue()
    {
        var def = CssDefinitions.StyleDefinitions[ECssPropertyID.Opacity];
        Assert.Equal(ECssValueTypes.NUMBER, def.Initial.Type);
    }
    #endregion

    #region Property Inheritance Tests
    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "StyleDefinition")]
    public void CssDefinitions_ColorIsInherited()
    {
        var def = CssDefinitions.StyleDefinitions[ECssPropertyID.Color];
        Assert.True(def.Inherited);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "StyleDefinition")]
    public void CssDefinitions_FontPropertiesAreInherited()
    {
        Assert.True(CssDefinitions.StyleDefinitions[ECssPropertyID.FontFamily].Inherited);
        Assert.True(CssDefinitions.StyleDefinitions[ECssPropertyID.FontSize].Inherited);
        Assert.True(CssDefinitions.StyleDefinitions[ECssPropertyID.FontWeight].Inherited);
        Assert.True(CssDefinitions.StyleDefinitions[ECssPropertyID.FontStyle].Inherited);
        Assert.True(CssDefinitions.StyleDefinitions[ECssPropertyID.LineHeight].Inherited);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "StyleDefinition")]
    public void CssDefinitions_DirectionIsInherited()
    {
        var def = CssDefinitions.StyleDefinitions[ECssPropertyID.Direction];
        Assert.True(def.Inherited);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "StyleDefinition")]
    public void CssDefinitions_TextAlignIsInherited()
    {
        var def = CssDefinitions.StyleDefinitions[ECssPropertyID.TextAlign];
        Assert.True(def.Inherited);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "StyleDefinition")]
    public void CssDefinitions_WidowsAndOrphansAreInherited()
    {
        Assert.True(CssDefinitions.StyleDefinitions[ECssPropertyID.Widows].Inherited);
        Assert.True(CssDefinitions.StyleDefinitions[ECssPropertyID.Orphans].Inherited);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "StyleDefinition")]
    public void CssDefinitions_BoxModelPropertiesNotInherited()
    {
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.Width].Inherited);
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.Height].Inherited);
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.PaddingTop].Inherited);
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.MarginTop].Inherited);
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.BorderTopWidth].Inherited);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "StyleDefinition")]
    public void CssDefinitions_FlexboxPropertiesNotInherited()
    {
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.FlexDirection].Inherited);
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.FlexWrap].Inherited);
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.FlexGrow].Inherited);
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.FlexShrink].Inherited);
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.FlexBasis].Inherited);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "StyleDefinition")]
    public void CssDefinitions_GridPropertiesNotInherited()
    {
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.GridAutoColumns].Inherited);
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.GridAutoRows].Inherited);
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.GridColumnStart].Inherited);
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.GridRowStart].Inherited);
    }
    #endregion

    #region Allowed Types Tests
    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "StyleDefinition")]
    public void CssDefinitions_WidthAllowsDimensionAndPercent()
    {
        var def = CssDefinitions.StyleDefinitions[ECssPropertyID.Width];
        Assert.True((def.AllowedTypes & ECssValueTypes.DIMENSION) != 0);
        Assert.True((def.AllowedTypes & ECssValueTypes.PERCENT) != 0);
        Assert.True((def.AllowedTypes & ECssValueTypes.AUTO) != 0);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "StyleDefinition")]
    public void CssDefinitions_FlexGrowAllowsNumberOnly()
    {
        var def = CssDefinitions.StyleDefinitions[ECssPropertyID.FlexGrow];
        Assert.True((def.AllowedTypes & ECssValueTypes.NUMBER) != 0);
        Assert.False((def.AllowedTypes & ECssValueTypes.KEYWORD) != 0);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "StyleDefinition")]
    public void CssDefinitions_FlexDirectionAllowsKeywordOnly()
    {
        var def = CssDefinitions.StyleDefinitions[ECssPropertyID.FlexDirection];
        Assert.True((def.AllowedTypes & ECssValueTypes.KEYWORD) != 0);
        Assert.False((def.AllowedTypes & ECssValueTypes.NUMBER) != 0);
        Assert.False((def.AllowedTypes & ECssValueTypes.DIMENSION) != 0);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "StyleDefinition")]
    public void CssDefinitions_ColorAllowsKeywordAndColorType()
    {
        var def = CssDefinitions.StyleDefinitions[ECssPropertyID.Color];
        Assert.True((def.AllowedTypes & ECssValueTypes.KEYWORD) != 0);
        Assert.True((def.AllowedTypes & ECssValueTypes.COLOR) != 0);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "StyleDefinition")]
    public void CssDefinitions_OpacityAllowsNumberAndInteger()
    {
        var def = CssDefinitions.StyleDefinitions[ECssPropertyID.Opacity];
        Assert.True((def.AllowedTypes & ECssValueTypes.NUMBER) != 0);
        Assert.True((def.AllowedTypes & ECssValueTypes.INTEGER) != 0);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "StyleDefinition")]
    public void CssDefinitions_OrderAllowsIntegerOnly()
    {
        var def = CssDefinitions.StyleDefinitions[ECssPropertyID.Order];
        Assert.True((def.AllowedTypes & ECssValueTypes.INTEGER) != 0);
    }
    #endregion

    #region Keyword Whitelist Tests
    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "StyleDefinition")]
    public void CssDefinitions_FlexDirectionHasCorrectKeywords()
    {
        var def = CssDefinitions.StyleDefinitions[ECssPropertyID.FlexDirection];
        Assert.Contains("row", def.KeywordWhitelist);
        Assert.Contains("row-reverse", def.KeywordWhitelist);
        Assert.Contains("column", def.KeywordWhitelist);
        Assert.Contains("column-reverse", def.KeywordWhitelist);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "StyleDefinition")]
    public void CssDefinitions_FlexWrapHasCorrectKeywords()
    {
        var def = CssDefinitions.StyleDefinitions[ECssPropertyID.FlexWrap];
        Assert.Contains("nowrap", def.KeywordWhitelist);
        Assert.Contains("wrap", def.KeywordWhitelist);
        Assert.Contains("wrap-reverse", def.KeywordWhitelist);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "StyleDefinition")]
    public void CssDefinitions_DisplayHasKeywords()
    {
        var def = CssDefinitions.StyleDefinitions[ECssPropertyID.Display];
        Assert.NotEmpty(def.KeywordWhitelist);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "StyleDefinition")]
    public void CssDefinitions_OverflowHasCorrectKeywords()
    {
        var def = CssDefinitions.StyleDefinitions[ECssPropertyID.OverflowX];
        Assert.Contains("visible", def.KeywordWhitelist);
        Assert.Contains("hidden", def.KeywordWhitelist);
        Assert.Contains("scroll", def.KeywordWhitelist);
        Assert.Contains("auto", def.KeywordWhitelist);
    }
    #endregion

    #region Property Dirt Flags Tests
    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "StyleDefinition")]
    public void CssDefinitions_WidthAffectsContentArea()
    {
        var def = CssDefinitions.StyleDefinitions[ECssPropertyID.Width];
        Assert.True((def.Flags & EPropertyDirtFlags.Content_Area) != 0);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "StyleDefinition")]
    public void CssDefinitions_PaddingAffectsPaddingArea()
    {
        var def = CssDefinitions.StyleDefinitions[ECssPropertyID.PaddingTop];
        Assert.True((def.Flags & EPropertyDirtFlags.Padding_Area) != 0);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "StyleDefinition")]
    public void CssDefinitions_MarginAffectsMarginArea()
    {
        var def = CssDefinitions.StyleDefinitions[ECssPropertyID.MarginTop];
        Assert.True((def.Flags & EPropertyDirtFlags.Margin_Area) != 0);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "StyleDefinition")]
    public void CssDefinitions_BorderWidthAffectsBorderArea()
    {
        var def = CssDefinitions.StyleDefinitions[ECssPropertyID.BorderTopWidth];
        Assert.True((def.Flags & EPropertyDirtFlags.Border_Area) != 0);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "StyleDefinition")]
    public void CssDefinitions_ColorAffectsVisual()
    {
        var def = CssDefinitions.StyleDefinitions[ECssPropertyID.Color];
        Assert.True((def.Flags & EPropertyDirtFlags.Visual) != 0);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "StyleDefinition")]
    public void CssDefinitions_FlexPropertiesAffectFlow()
    {
        Assert.True((CssDefinitions.StyleDefinitions[ECssPropertyID.FlexDirection].Flags & EPropertyDirtFlags.Flow) != 0);
        Assert.True((CssDefinitions.StyleDefinitions[ECssPropertyID.FlexWrap].Flags & EPropertyDirtFlags.Flow) != 0);
        Assert.True((CssDefinitions.StyleDefinitions[ECssPropertyID.FlexGrow].Flags & EPropertyDirtFlags.Flow) != 0);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "StyleDefinition")]
    public void CssDefinitions_FontPropertiesAffectTextAndFlow()
    {
        var fontSizeDef = CssDefinitions.StyleDefinitions[ECssPropertyID.FontSize];
        Assert.True((fontSizeDef.Flags & EPropertyDirtFlags.Text) != 0);
        Assert.True((fontSizeDef.Flags & EPropertyDirtFlags.Flow) != 0);
    }
    #endregion
}
