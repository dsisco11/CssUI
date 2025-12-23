using CssUI.CSS;
using CssUI.CSS.Enums;
using CssUI.CSS.Internal;
using CssUI.DOM;
using Xunit;

namespace CssUITests.CSS.Properties.Tests;

/// <summary>
/// Tests for CSS property inheritance behavior per CSS Cascading and Inheritance Level 3.
/// Docs: https://www.w3.org/TR/css-cascade-3/
/// </summary>
public class PropertyInheritanceTests
{
    #region Helper Methods
    private static Document CreateTestDocument()
    {
        var dom = new DOMImplementation();
        return dom.createDocument("CssUI", "cssui");
    }

    private static Element CreateTestElement(Document doc, string tagName = "div")
    {
        return doc.createElement(tagName, new ElementCreationOptions(string.Empty));
    }

    private static Element CreateElementTree(Document doc)
    {
        var parent = CreateTestElement(doc, "div");
        var child = CreateTestElement(doc, "span");
        parent.appendChild(child);
        return parent;
    }
    #endregion

    #region Inheritable Property Tests
    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Inheritance")]
    public void InheritableProperty_Color_IsMarkedAsInheritable()
    {
        var def = CssDefinitions.StyleDefinitions[ECssPropertyID.Color];
        Assert.True(def.Inherited);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Inheritance")]
    public void InheritableProperty_FontFamily_IsMarkedAsInheritable()
    {
        var def = CssDefinitions.StyleDefinitions[ECssPropertyID.FontFamily];
        Assert.True(def.Inherited);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Inheritance")]
    public void InheritableProperty_FontSize_IsMarkedAsInheritable()
    {
        var def = CssDefinitions.StyleDefinitions[ECssPropertyID.FontSize];
        Assert.True(def.Inherited);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Inheritance")]
    public void InheritableProperty_LineHeight_IsMarkedAsInheritable()
    {
        var def = CssDefinitions.StyleDefinitions[ECssPropertyID.LineHeight];
        Assert.True(def.Inherited);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Inheritance")]
    public void InheritableProperty_Direction_IsMarkedAsInheritable()
    {
        var def = CssDefinitions.StyleDefinitions[ECssPropertyID.Direction];
        Assert.True(def.Inherited);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Inheritance")]
    public void InheritableProperty_WritingMode_IsMarkedAsInheritable()
    {
        var def = CssDefinitions.StyleDefinitions[ECssPropertyID.WritingMode];
        Assert.True(def.Inherited);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Inheritance")]
    public void InheritableProperty_TextAlign_IsMarkedAsInheritable()
    {
        var def = CssDefinitions.StyleDefinitions[ECssPropertyID.TextAlign];
        Assert.True(def.Inherited);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Inheritance")]
    public void InheritableProperty_Orphans_IsMarkedAsInheritable()
    {
        var def = CssDefinitions.StyleDefinitions[ECssPropertyID.Orphans];
        Assert.True(def.Inherited);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Inheritance")]
    public void InheritableProperty_Widows_IsMarkedAsInheritable()
    {
        var def = CssDefinitions.StyleDefinitions[ECssPropertyID.Widows];
        Assert.True(def.Inherited);
    }
    #endregion

    #region Non-Inheritable Property Tests
    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Inheritance")]
    public void NonInheritableProperty_Width_IsNotInheritable()
    {
        var def = CssDefinitions.StyleDefinitions[ECssPropertyID.Width];
        Assert.False(def.Inherited);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Inheritance")]
    public void NonInheritableProperty_Height_IsNotInheritable()
    {
        var def = CssDefinitions.StyleDefinitions[ECssPropertyID.Height];
        Assert.False(def.Inherited);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Inheritance")]
    public void NonInheritableProperty_Margin_IsNotInheritable()
    {
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.MarginTop].Inherited);
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.MarginRight].Inherited);
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.MarginBottom].Inherited);
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.MarginLeft].Inherited);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Inheritance")]
    public void NonInheritableProperty_Padding_IsNotInheritable()
    {
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.PaddingTop].Inherited);
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.PaddingRight].Inherited);
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.PaddingBottom].Inherited);
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.PaddingLeft].Inherited);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Inheritance")]
    public void NonInheritableProperty_Border_IsNotInheritable()
    {
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.BorderTopWidth].Inherited);
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.BorderTopColor].Inherited);
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.BorderTopStyle].Inherited);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Inheritance")]
    public void NonInheritableProperty_Display_IsNotInheritable()
    {
        var def = CssDefinitions.StyleDefinitions[ECssPropertyID.Display];
        Assert.False(def.Inherited);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Inheritance")]
    public void NonInheritableProperty_Positioning_IsNotInheritable()
    {
        var def = CssDefinitions.StyleDefinitions[ECssPropertyID.Positioning];
        Assert.False(def.Inherited);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Inheritance")]
    public void NonInheritableProperty_FlexDirection_IsNotInheritable()
    {
        var def = CssDefinitions.StyleDefinitions[ECssPropertyID.FlexDirection];
        Assert.False(def.Inherited);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Inheritance")]
    public void NonInheritableProperty_FlexGrow_IsNotInheritable()
    {
        var def = CssDefinitions.StyleDefinitions[ECssPropertyID.FlexGrow];
        Assert.False(def.Inherited);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Inheritance")]
    public void NonInheritableProperty_GridProperties_AreNotInheritable()
    {
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.GridAutoColumns].Inherited);
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.GridAutoRows].Inherited);
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.GridAutoFlow].Inherited);
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.GridColumnStart].Inherited);
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.GridRowStart].Inherited);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Inheritance")]
    public void NonInheritableProperty_AlignmentProperties_AreNotInheritable()
    {
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.AlignContent].Inherited);
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.AlignItems].Inherited);
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.AlignSelf].Inherited);
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.JustifyContent].Inherited);
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.JustifyItems].Inherited);
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.JustifySelf].Inherited);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Inheritance")]
    public void NonInheritableProperty_Opacity_IsNotInheritable()
    {
        var def = CssDefinitions.StyleDefinitions[ECssPropertyID.Opacity];
        Assert.False(def.Inherited);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Inheritance")]
    public void NonInheritableProperty_Transform_IsNotInheritable()
    {
        var def = CssDefinitions.StyleDefinitions[ECssPropertyID.Transform];
        Assert.False(def.Inherited);
    }
    #endregion

    #region Inherit Keyword Tests
    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Inheritance")]
    public void InheritKeyword_ForcesInheritance_ForNonInheritableProperty()
    {
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var child = CreateTestElement(doc, "span");

        doc.documentElement?.appendChild(parent);
        parent.appendChild(child);

        // Set parent width
        var parentProp = parent.Style.Cascaded.Get(ECssPropertyID.Width) as CssProperty;
        Assert.NotNull(parentProp);
        parentProp.Set(CssValue.From(200.0, ECssUnit.PX));

        // Set child to inherit (normally width doesn't inherit)
        var childProp = child.Style.Cascaded.Get(ECssPropertyID.Width) as CssProperty;
        Assert.NotNull(childProp);
        childProp.Set(CssValue.Inherit);

        Assert.True(childProp.IsInherited);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Inheritance")]
    public void InheritKeyword_CanBeUsedOnInheritableProperty()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = element.Style.Cascaded.Get(ECssPropertyID.Color) as CssProperty;
        Assert.NotNull(prop);

        prop.Set(CssValue.Inherit);

        Assert.True(prop.IsInherited);
    }
    #endregion

    #region Initial Keyword Tests
    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Inheritance")]
    public void InitialKeyword_ResetsToInitialValue()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = element.Style.Cascaded.Get(ECssPropertyID.Width) as CssProperty;
        Assert.NotNull(prop);

        // First set a value, then reset to initial
        prop.Set(CssValue.From(100.0, ECssUnit.PX));
        prop.Set(CssValue.Initial);

        // Property should now use initial value on resolution
        Assert.Equal(ECssValueTypes.INITIAL, prop.Assigned.Type);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Inheritance")]
    public void InitialKeyword_OverridesInheritedValue()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        // Color is inherited, but initial should still work
        var prop = element.Style.Cascaded.Get(ECssPropertyID.Color) as CssProperty;
        Assert.NotNull(prop);

        prop.Set(CssValue.Initial);

        Assert.Equal(ECssValueTypes.INITIAL, prop.Assigned.Type);
    }
    #endregion

    #region Unset Keyword Tests
    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Inheritance")]
    public void UnsetKeyword_BehavesLikeInherit_ForInheritableProperty()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        // Color is inheritable, so unset should act like inherit
        var prop = element.Style.Cascaded.Get(ECssPropertyID.Color) as CssProperty;
        Assert.NotNull(prop);

        prop.Set(CssValue.Unset);

        Assert.Equal(ECssValueTypes.UNSET, prop.Assigned.Type);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Inheritance")]
    public void UnsetKeyword_BehavesLikeInitial_ForNonInheritableProperty()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        // Width is not inheritable, so unset should act like initial
        var prop = element.Style.Cascaded.Get(ECssPropertyID.Width) as CssProperty;
        Assert.NotNull(prop);

        prop.Set(CssValue.Unset);

        Assert.Equal(ECssValueTypes.UNSET, prop.Assigned.Type);
    }
    #endregion

    #region Find_Inherited_Value Tests
    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Inheritance")]
    public void FindInheritedValue_ReturnsInitialValue_ForRootElement()
    {
        var doc = CreateTestDocument();
        var root = doc.documentElement;
        Assert.NotNull(root);

        var prop = root.Style.Cascaded.Get(ECssPropertyID.Color);
        Assert.NotNull(prop);

        // Root element should return initial value since it can't inherit
        var inheritedValue = prop.Find_Inherited_Value();
        Assert.NotNull(inheritedValue);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Inheritance")]
    public void FindInheritedValue_ReturnsParentComputedValue_ForChildElement()
    {
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var child = CreateTestElement(doc, "span");

        doc.documentElement?.appendChild(parent);
        parent.appendChild(child);

        // For an inheritable property, Find_Inherited_Value should return parent's computed value
        var childProp = child.Style.Cascaded.Get(ECssPropertyID.Color);
        Assert.NotNull(childProp);

        var inheritedValue = childProp.Find_Inherited_Value();
        Assert.NotNull(inheritedValue);
    }
    #endregion

    #region IsInheritable Property Tests
    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Inheritance")]
    public void IsInheritable_MatchesDefinition_ForColor()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = element.Style.Cascaded.Get(ECssPropertyID.Color);
        Assert.NotNull(prop);

        Assert.Equal(prop.Definition.Inherited, prop.IsInheritable);
        Assert.True(prop.IsInheritable);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Inheritance")]
    public void IsInheritable_MatchesDefinition_ForWidth()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = element.Style.Cascaded.Get(ECssPropertyID.Width);
        Assert.NotNull(prop);

        Assert.Equal(prop.Definition.Inherited, prop.IsInheritable);
        Assert.False(prop.IsInheritable);
    }
    #endregion

    #region CSS Spec Compliance Tests
    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Inheritance")]
    [Trait("Category", "Spec")]
    public void InheritanceRules_FollowCssSpecs_ForTextProperties()
    {
        // Per CSS spec, text-related properties should be inherited
        Assert.True(CssDefinitions.StyleDefinitions[ECssPropertyID.Color].Inherited);
        Assert.True(CssDefinitions.StyleDefinitions[ECssPropertyID.FontFamily].Inherited);
        Assert.True(CssDefinitions.StyleDefinitions[ECssPropertyID.FontSize].Inherited);
        Assert.True(CssDefinitions.StyleDefinitions[ECssPropertyID.FontWeight].Inherited);
        Assert.True(CssDefinitions.StyleDefinitions[ECssPropertyID.FontStyle].Inherited);
        Assert.True(CssDefinitions.StyleDefinitions[ECssPropertyID.LineHeight].Inherited);
        Assert.True(CssDefinitions.StyleDefinitions[ECssPropertyID.TextAlign].Inherited);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Inheritance")]
    [Trait("Category", "Spec")]
    public void InheritanceRules_FollowCssSpecs_ForBoxModelProperties()
    {
        // Per CSS spec, box model properties should NOT be inherited
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.Width].Inherited);
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.Height].Inherited);
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.MarginTop].Inherited);
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.PaddingTop].Inherited);
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.BorderTopWidth].Inherited);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Inheritance")]
    [Trait("Category", "Spec")]
    public void InheritanceRules_FollowCssSpecs_ForVisualProperties()
    {
        // Per CSS spec, most visual properties should NOT be inherited (except color)
        Assert.True(CssDefinitions.StyleDefinitions[ECssPropertyID.Color].Inherited);
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.Opacity].Inherited);
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.Transform].Inherited);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Inheritance")]
    [Trait("Category", "Spec")]
    public void InheritanceRules_FollowCssSpecs_ForLayoutProperties()
    {
        // Per CSS spec, layout properties like display/position should NOT be inherited
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.Display].Inherited);
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.Positioning].Inherited);
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.Top].Inherited);
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.Left].Inherited);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Inheritance")]
    [Trait("Category", "Spec")]
    public void InheritanceRules_FollowCssFlexboxSpec()
    {
        // Per CSS Flexbox spec, flex properties should NOT be inherited
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.FlexDirection].Inherited);
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.FlexWrap].Inherited);
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.FlexGrow].Inherited);
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.FlexShrink].Inherited);
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.FlexBasis].Inherited);
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.Order].Inherited);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Inheritance")]
    [Trait("Category", "Spec")]
    public void InheritanceRules_FollowCssGridSpec()
    {
        // Per CSS Grid spec, grid properties should NOT be inherited
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.GridAutoColumns].Inherited);
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.GridAutoRows].Inherited);
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.GridAutoFlow].Inherited);
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.GridColumnStart].Inherited);
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.GridColumnEnd].Inherited);
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.GridRowStart].Inherited);
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.GridRowEnd].Inherited);
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.GridTemplateColumns].Inherited);
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.GridTemplateRows].Inherited);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "Inheritance")]
    [Trait("Category", "Spec")]
    public void InheritanceRules_FollowCssFragmentationSpec()
    {
        // Per CSS Break spec, orphans and widows are inherited
        Assert.True(CssDefinitions.StyleDefinitions[ECssPropertyID.Orphans].Inherited);
        Assert.True(CssDefinitions.StyleDefinitions[ECssPropertyID.Widows].Inherited);
        // But break properties are not
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.BreakBefore].Inherited);
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.BreakAfter].Inherited);
        Assert.False(CssDefinitions.StyleDefinitions[ECssPropertyID.BreakInside].Inherited);
    }
    #endregion
}
