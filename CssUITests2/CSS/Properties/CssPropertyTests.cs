using CssUI.CSS;
using CssUI.CSS.Enums;
using CssUI.CSS.Internal;
using CssUI.DOM;
using CssUI.Internal;
using Xunit;

namespace CssUITests.CSS.Properties.Tests;

/// <summary>
/// Tests for CssProperty - represents a CSS property with assigned, specified, computed, used, and actual value stages.
/// </summary>
public class CssPropertyTests
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
        return doc.createElement(tagName, new ElementCreationOptions(string.Empty));
    }
    #endregion

    #region Property Assignment Tests
    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "CssProperty")]
    public void Property_AssignedValue_CanBeSet()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = element.Style.Cascaded.Get(ECssPropertyID.Width);

        Assert.NotNull(prop);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "CssProperty")]
    public void Property_HasDefinition()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = element.Style.Cascaded.Get(ECssPropertyID.Width);

        Assert.NotNull(prop?.Definition);
        Assert.Equal(ECssPropertyID.Width, prop.Definition.Name);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "CssProperty")]
    public void Property_CssName_MatchesPropertyId()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = element.Style.Cascaded.Get(ECssPropertyID.MarginTop);

        Assert.NotNull(prop);
        Assert.Equal(ECssPropertyID.MarginTop, prop.CssName);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "CssProperty")]
    public void Property_Owner_ReferencesElement()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = element.Style.Cascaded.Get(ECssPropertyID.Width);

        Assert.NotNull(prop);
        Assert.Same(element, prop.Owner);
    }
    #endregion

    #region IsAuto Tests
    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "CssProperty")]
    public void IsAuto_ReturnsTrue_WhenAssignedAuto()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        // Use UserRules instead of Cascaded (which is read-only)
        var prop = element.Style.UserRules.Get(ECssPropertyID.Width) as CssProperty;
        Assert.NotNull(prop);

        prop.Set(CssValue.Auto);

        Assert.True(prop.IsAuto);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "CssProperty")]
    public void IsAuto_ReturnsFalse_WhenAssignedValue()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        // Use UserRules instead of Cascaded (which is read-only)
        var prop = element.Style.UserRules.Get(ECssPropertyID.Width) as CssProperty;
        Assert.NotNull(prop);

        prop.Set(CssValue.From(100.0, ECssUnit.PX));

        Assert.False(prop.IsAuto);
    }
    #endregion

    #region IsInherited Tests
    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "CssProperty")]
    public void IsInherited_ReturnsTrue_WhenAssignedInherit()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        // Use UserRules instead of Cascaded (which is read-only)
        var prop = element.Style.UserRules.Get(ECssPropertyID.Width) as CssProperty;
        Assert.NotNull(prop);

        prop.Set(CssValue.Inherit);

        Assert.True(prop.IsInherited);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "CssProperty")]
    public void IsInherited_ReturnsFalse_WhenAssignedExplicitValue()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        // Use UserRules instead of Cascaded (which is read-only)
        var prop = element.Style.UserRules.Get(ECssPropertyID.Width) as CssProperty;
        Assert.NotNull(prop);

        prop.Set(CssValue.From(50.0, ECssUnit.PX));

        Assert.False(prop.IsInherited);
    }
    #endregion

    #region IsInheritable Tests
    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "CssProperty")]
    public void IsInheritable_ReturnsTrue_ForInheritableProperties()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var colorProp = element.Style.Cascaded.Get(ECssPropertyID.Color);

        Assert.NotNull(colorProp);
        Assert.True(colorProp.IsInheritable);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "CssProperty")]
    public void IsInheritable_ReturnsFalse_ForNonInheritableProperties()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var widthProp = element.Style.Cascaded.Get(ECssPropertyID.Width);

        Assert.NotNull(widthProp);
        Assert.False(widthProp.IsInheritable);
    }
    #endregion

    #region Serialize Tests
    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "CssProperty")]
    public void Serialize_ReturnsPropertyNameAndValue()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        // Use UserRules instead of Cascaded (which is read-only)
        var prop = element.Style.UserRules.Get(ECssPropertyID.Width) as CssProperty;
        Assert.NotNull(prop);

        prop.Set(CssValue.Auto);

        var serialized = prop.Serialize();

        Assert.Contains("width", serialized);
    }
    #endregion

    #region Definition Access Tests
    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "CssProperty")]
    public void Definition_ReturnsCorrectInitialValue()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = element.Style.Cascaded.Get(ECssPropertyID.Width);
        Assert.NotNull(prop);

        var def = prop.Definition;
        Assert.NotNull(def);
        Assert.Equal(ECssValueTypes.AUTO, def.Initial.Type);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "CssProperty")]
    public void Definition_ReturnsCorrectInheritanceFlag()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var colorProp = element.Style.Cascaded.Get(ECssPropertyID.Color);
        var widthProp = element.Style.Cascaded.Get(ECssPropertyID.Width);

        Assert.NotNull(colorProp?.Definition);
        Assert.NotNull(widthProp?.Definition);
        Assert.True(colorProp.Definition.Inherited);
        Assert.False(widthProp.Definition.Inherited);
    }
    #endregion

    #region Global Keywords Tests
    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "CssProperty")]
    public void Property_AcceptsInitialKeyword()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        // Use UserRules instead of Cascaded (which is read-only)
        var prop = element.Style.UserRules.Get(ECssPropertyID.Width) as CssProperty;
        Assert.NotNull(prop);

        // Should not throw
        prop.Set(CssValue.Initial);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "CssProperty")]
    public void Property_AcceptsInheritKeyword()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        // Use UserRules instead of Cascaded (which is read-only)
        var prop = element.Style.UserRules.Get(ECssPropertyID.Width) as CssProperty;
        Assert.NotNull(prop);

        // Should not throw
        prop.Set(CssValue.Inherit);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "CssProperty")]
    public void Property_AcceptsUnsetKeyword()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        // Use UserRules instead of Cascaded (which is read-only)
        var prop = element.Style.UserRules.Get(ECssPropertyID.Width) as CssProperty;
        Assert.NotNull(prop);

        // Should not throw
        prop.Set(CssValue.Unset);
    }
    #endregion

    #region Property Type Validation Tests
    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "CssProperty")]
    public void Property_AcceptsDimensionValue_WhenAllowed()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        // Use UserRules instead of Cascaded (which is read-only)
        var prop = element.Style.UserRules.Get(ECssPropertyID.Width) as CssProperty;
        Assert.NotNull(prop);

        // Width accepts dimensions
        prop.Set(CssValue.From(100.0, ECssUnit.PX));

        Assert.True(prop.HasValue);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "CssProperty")]
    public void Property_AcceptsNumberValue_ForFlexGrow()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        // Use UserRules instead of Cascaded (which is read-only)
        // FlexGrow is now initialized per W3C CSS Flexbox spec
        var prop = element.Style.UserRules.Get(ECssPropertyID.FlexGrow) as CssProperty;
        Assert.NotNull(prop);

        // FlexGrow accepts numbers
        prop.Set(CssValue.From(2.0));

        Assert.True(prop.HasValue);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "CssProperty")]
    public void Property_AcceptsKeywordValue_ForDisplay()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        // Use UserRules instead of Cascaded (which is read-only)
        var prop = element.Style.UserRules.Get(ECssPropertyID.Display) as CssProperty;
        Assert.NotNull(prop);

        // Display accepts keywords
        prop.Set(CssValue.From(EDisplayMode.BLOCK));

        Assert.True(prop.HasValue);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "CssProperty")]
    public void Property_AcceptsIntegerValue_ForOrder()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        // Use UserRules instead of Cascaded (which is read-only)
        // Order is now initialized per W3C CSS Flexbox spec
        var prop = element.Style.UserRules.Get(ECssPropertyID.Order) as CssProperty;
        Assert.NotNull(prop);

        // Order accepts integers
        prop.Set(CssValue.From(5));

        Assert.True(prop.HasValue);
    }
    #endregion

    #region Property Instance Tests
    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "CssProperty")]
    public void MultipleElements_HaveIndependentProperties()
    {
        var doc = CreateTestDocument();
        var element1 = CreateTestElement(doc, "div");
        var element2 = CreateTestElement(doc, "span");
        doc.documentElement?.appendChild(element1);
        doc.documentElement?.appendChild(element2);

        // Use UserRules instead of Cascaded (which is read-only)
        var prop1 = element1.Style.UserRules.Get(ECssPropertyID.Width) as CssProperty;
        var prop2 = element2.Style.UserRules.Get(ECssPropertyID.Width) as CssProperty;

        Assert.NotNull(prop1);
        Assert.NotNull(prop2);

        prop1.Set(CssValue.From(100.0, ECssUnit.PX));
        prop2.Set(CssValue.From(200.0, ECssUnit.PX));

        // Properties should be independent
        Assert.NotSame(prop1, prop2);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "CssProperty")]
    public void Property_Source_ReturnsComputedStyle()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = element.Style.Cascaded.Get(ECssPropertyID.Width);

        Assert.NotNull(prop?.Source);
    }
    #endregion

    #region Style Properties Collection Tests
    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "CssProperty")]
    public void StyleProperties_HasCascadedStyleSet()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        Assert.NotNull(element.Style);
        Assert.NotNull(element.Style.Cascaded);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "CssProperty")]
    public void StyleProperties_CanAccessAllBoxModelProperties()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var style = element.Style.Cascaded;

        // All these should not throw and return valid properties
        Assert.NotNull(style.Get(ECssPropertyID.Width));
        Assert.NotNull(style.Get(ECssPropertyID.Height));
        Assert.NotNull(style.Get(ECssPropertyID.MinWidth));
        Assert.NotNull(style.Get(ECssPropertyID.MinHeight));
        Assert.NotNull(style.Get(ECssPropertyID.MaxWidth));
        Assert.NotNull(style.Get(ECssPropertyID.MaxHeight));
        Assert.NotNull(style.Get(ECssPropertyID.PaddingTop));
        Assert.NotNull(style.Get(ECssPropertyID.PaddingRight));
        Assert.NotNull(style.Get(ECssPropertyID.PaddingBottom));
        Assert.NotNull(style.Get(ECssPropertyID.PaddingLeft));
        Assert.NotNull(style.Get(ECssPropertyID.MarginTop));
        Assert.NotNull(style.Get(ECssPropertyID.MarginRight));
        Assert.NotNull(style.Get(ECssPropertyID.MarginBottom));
        Assert.NotNull(style.Get(ECssPropertyID.MarginLeft));
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "CssProperty")]
    public void StyleProperties_CanAccessAllFlexboxProperties()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        // Use UserRules for modification tests
        var style = element.Style.UserRules;

        Assert.NotNull(style.Get(ECssPropertyID.FlexDirection));
        Assert.NotNull(style.Get(ECssPropertyID.FlexWrap));
        Assert.NotNull(style.Get(ECssPropertyID.FlexGrow));
        Assert.NotNull(style.Get(ECssPropertyID.FlexShrink));
        Assert.NotNull(style.Get(ECssPropertyID.FlexBasis));
        Assert.NotNull(style.Get(ECssPropertyID.Order));
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "CssProperty")]
    public void StyleProperties_CanAccessAllAlignmentProperties()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        // Use UserRules for modification tests
        var style = element.Style.UserRules;

        Assert.NotNull(style.Get(ECssPropertyID.AlignContent));
        Assert.NotNull(style.Get(ECssPropertyID.JustifyContent));
        Assert.NotNull(style.Get(ECssPropertyID.AlignItems));
        Assert.NotNull(style.Get(ECssPropertyID.AlignSelf));
        Assert.NotNull(style.Get(ECssPropertyID.JustifyItems));
        Assert.NotNull(style.Get(ECssPropertyID.JustifySelf));
    }
    #endregion
}
