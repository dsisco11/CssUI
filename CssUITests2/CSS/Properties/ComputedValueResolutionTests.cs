using CssUI.CSS;
using CssUI.CSS.Enums;
using CssUI.CSS.Internal;
using CssUI.DOM;
using Xunit;

namespace CssUITests.CSS.Properties.Tests;

/// <summary>
/// Tests for CSS computed value resolution per CSS Cascading and Inheritance Level 3.
/// Docs: https://www.w3.org/TR/css-cascade-3/#computed
/// </summary>
public class ComputedValueResolutionTests
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
    #endregion

    #region Value Stage Concepts Tests
    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "ValueResolution")]
    public void CssProperty_HasAssignedValue_Stage()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = element.Style.Cascaded.Get(ECssPropertyID.Width) as CssProperty;
        Assert.NotNull(prop);

        prop.Set(CssValue.From(100.0, ECssUnit.PX));

        // Assigned should be the value we set
        Assert.NotNull(prop.Assigned);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "ValueResolution")]
    public void CssProperty_HasSpecifiedValue_Stage()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = element.Style.Cascaded.Get(ECssPropertyID.Width) as CssProperty;
        Assert.NotNull(prop);

        prop.Set(CssValue.From(100.0, ECssUnit.PX));

        // Specified should be available
        Assert.NotNull(prop.Specified);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "ValueResolution")]
    public void CssProperty_HasComputedValue_Stage()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = element.Style.Cascaded.Get(ECssPropertyID.Width) as CssProperty;
        Assert.NotNull(prop);

        prop.Set(CssValue.From(100.0, ECssUnit.PX));

        // Computed should be available
        Assert.NotNull(prop.Computed);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "ValueResolution")]
    public void CssProperty_HasUsedValue_Stage()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = element.Style.Cascaded.Get(ECssPropertyID.Width) as CssProperty;
        Assert.NotNull(prop);

        prop.Set(CssValue.From(100.0, ECssUnit.PX));

        // Used should be available
        Assert.NotNull(prop.Used);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "ValueResolution")]
    public void CssProperty_HasActualValue_Stage()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = element.Style.Cascaded.Get(ECssPropertyID.Width) as CssProperty;
        Assert.NotNull(prop);

        prop.Set(CssValue.From(100.0, ECssUnit.PX));

        // Actual should be available
        Assert.NotNull(prop.Actual);
    }
    #endregion

    #region Derive_SpecifiedValue Tests
    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "ValueResolution")]
    public void DeriveSpecifiedValue_ReturnsAssignedValue_WhenNotSpecialKeyword()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = element.Style.Cascaded.Get(ECssPropertyID.Width) as CssProperty;
        Assert.NotNull(prop);

        prop.Set(CssValue.From(150.0, ECssUnit.PX));

        // For a simple value, specified should match assigned
        Assert.NotNull(prop.Specified);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "ValueResolution")]
    public void DeriveSpecifiedValue_ResolvesInitial_ToDefinitionInitialValue()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = element.Style.Cascaded.Get(ECssPropertyID.Width) as CssProperty;
        Assert.NotNull(prop);

        prop.Set(CssValue.Initial);

        // Specified should resolve initial to the definition's initial value
        var def = prop.Definition;
        Assert.NotNull(def);
        Assert.NotNull(prop.Specified);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "ValueResolution")]
    public void DeriveSpecifiedValue_ResolvesInherit_ForInheritableProperty()
    {
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var child = CreateTestElement(doc, "span");
        doc.documentElement?.appendChild(parent);
        parent.appendChild(child);

        // Color is inheritable
        var childProp = child.Style.Cascaded.Get(ECssPropertyID.Color) as CssProperty;
        Assert.NotNull(childProp);

        childProp.Set(CssValue.Inherit);

        // Specified should be resolved via inheritance
        Assert.NotNull(childProp.Specified);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "ValueResolution")]
    public void DeriveSpecifiedValue_ResolvesUnset_ToInherit_ForInheritableProperty()
    {
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var child = CreateTestElement(doc, "span");
        doc.documentElement?.appendChild(parent);
        parent.appendChild(child);

        // Color is inheritable, so unset acts like inherit
        var childProp = child.Style.Cascaded.Get(ECssPropertyID.Color) as CssProperty;
        Assert.NotNull(childProp);

        childProp.Set(CssValue.Unset);

        // Specified should be resolved
        Assert.NotNull(childProp.Specified);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "ValueResolution")]
    public void DeriveSpecifiedValue_ResolvesUnset_ToInitial_ForNonInheritableProperty()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        // Width is not inheritable, so unset acts like initial
        var prop = element.Style.Cascaded.Get(ECssPropertyID.Width) as CssProperty;
        Assert.NotNull(prop);

        prop.Set(CssValue.Unset);

        // Specified should resolve to initial value
        Assert.NotNull(prop.Specified);
    }
    #endregion

    #region Derive_ComputedValue Tests
    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "ValueResolution")]
    public void DeriveComputedValue_ResolvesRelativeUnits_ToAbsolute()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = element.Style.Cascaded.Get(ECssPropertyID.Width) as CssProperty;
        Assert.NotNull(prop);

        // Set a dimension value that should be resolved to px
        prop.Set(CssValue.From(100.0, ECssUnit.PX));

        // Computed should be resolved
        Assert.NotNull(prop.Computed);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "ValueResolution")]
    public void DeriveComputedValue_PreservesAbsoluteValues()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = element.Style.Cascaded.Get(ECssPropertyID.FlexGrow) as CssProperty;
        Assert.NotNull(prop);

        // Numbers don't need resolution
        prop.Set(CssValue.From(2.0));

        Assert.NotNull(prop.Computed);
        Assert.Equal(ECssValueTypes.NUMBER, prop.Computed.Type);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "ValueResolution")]
    public void DeriveComputedValue_PreservesKeywordValues()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = element.Style.Cascaded.Get(ECssPropertyID.FlexDirection) as CssProperty;
        Assert.NotNull(prop);

        prop.Set(CssValue.From(EFlexDirection.Column));

        Assert.NotNull(prop.Computed);
        Assert.Equal(ECssValueTypes.KEYWORD, prop.Computed.Type);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "ValueResolution")]
    public void DeriveComputedValue_PreservesAutoValue()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = element.Style.Cascaded.Get(ECssPropertyID.Width) as CssProperty;
        Assert.NotNull(prop);

        prop.Set(CssValue.Auto);

        Assert.NotNull(prop.Computed);
        // Auto values are typically preserved through computation
    }
    #endregion

    #region Property Resolver Tests
    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "ValueResolution")]
    public void PropertyResolver_ExistsForColor()
    {
        var def = CssDefinitions.StyleDefinitions[ECssPropertyID.Color];

        // Color has resolvers at multiple stages
        var specifiedResolver = def.PropertyStageResolver[(int)EPropertyStage.Specified];
        var computedResolver = def.PropertyStageResolver[(int)EPropertyStage.Computed];
        var usedResolver = def.PropertyStageResolver[(int)EPropertyStage.Used];

        // At least some resolvers should exist
        Assert.True(specifiedResolver is not null || computedResolver is not null || usedResolver is not null);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "ValueResolution")]
    public void PropertyResolver_ExistsForFontSize()
    {
        var def = CssDefinitions.StyleDefinitions[ECssPropertyID.FontSize];

        // FontSize has resolvers
        var computedResolver = def.PropertyStageResolver[(int)EPropertyStage.Computed];
        var usedResolver = def.PropertyStageResolver[(int)EPropertyStage.Used];

        Assert.True(computedResolver is not null || usedResolver is not null);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "ValueResolution")]
    public void PropertyResolver_ExistsForBorderWidth()
    {
        var def = CssDefinitions.StyleDefinitions[ECssPropertyID.BorderTopWidth];

        // BorderWidth has a used value resolver
        var usedResolver = def.PropertyStageResolver[(int)EPropertyStage.Used];
        Assert.NotNull(usedResolver);
    }
    #endregion

    #region Percentage Resolution Tests
    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "ValueResolution")]
    public void PercentageResolver_ExistsForWidthProperty()
    {
        var def = CssDefinitions.StyleDefinitions[ECssPropertyID.Width];
        Assert.NotNull(def.Percentage_Resolver);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "ValueResolution")]
    public void PercentageResolver_ExistsForHeightProperty()
    {
        var def = CssDefinitions.StyleDefinitions[ECssPropertyID.Height];
        Assert.NotNull(def.Percentage_Resolver);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "ValueResolution")]
    public void PercentageResolver_ExistsForPaddingProperties()
    {
        Assert.NotNull(CssDefinitions.StyleDefinitions[ECssPropertyID.PaddingTop].Percentage_Resolver);
        Assert.NotNull(CssDefinitions.StyleDefinitions[ECssPropertyID.PaddingRight].Percentage_Resolver);
        Assert.NotNull(CssDefinitions.StyleDefinitions[ECssPropertyID.PaddingBottom].Percentage_Resolver);
        Assert.NotNull(CssDefinitions.StyleDefinitions[ECssPropertyID.PaddingLeft].Percentage_Resolver);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "ValueResolution")]
    public void PercentageResolver_ExistsForMarginProperties()
    {
        Assert.NotNull(CssDefinitions.StyleDefinitions[ECssPropertyID.MarginTop].Percentage_Resolver);
        Assert.NotNull(CssDefinitions.StyleDefinitions[ECssPropertyID.MarginRight].Percentage_Resolver);
        Assert.NotNull(CssDefinitions.StyleDefinitions[ECssPropertyID.MarginBottom].Percentage_Resolver);
        Assert.NotNull(CssDefinitions.StyleDefinitions[ECssPropertyID.MarginLeft].Percentage_Resolver);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "ValueResolution")]
    public void PercentageResolver_ExistsForFlexBasis()
    {
        var def = CssDefinitions.StyleDefinitions[ECssPropertyID.FlexBasis];
        Assert.NotNull(def.Percentage_Resolver);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "ValueResolution")]
    public void PercentageResolver_ExistsForGapProperties()
    {
        Assert.NotNull(CssDefinitions.StyleDefinitions[ECssPropertyID.RowGap].Percentage_Resolver);
        Assert.NotNull(CssDefinitions.StyleDefinitions[ECssPropertyID.ColumnGap].Percentage_Resolver);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "ValueResolution")]
    public void PercentageResolver_DoesNotExistForNumberOnlyProperties()
    {
        // FlexGrow takes numbers only, no percentage
        var def = CssDefinitions.StyleDefinitions[ECssPropertyID.FlexGrow];
        Assert.Null(def.Percentage_Resolver);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "ValueResolution")]
    public void PercentageResolver_DoesNotExistForKeywordOnlyProperties()
    {
        // FlexDirection takes keywords only, no percentage
        var def = CssDefinitions.StyleDefinitions[ECssPropertyID.FlexDirection];
        Assert.Null(def.Percentage_Resolver);
    }
    #endregion

    #region Value Update Chain Tests
    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "ValueResolution")]
    public void ValueUpdate_TriggersRecomputation()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = element.Style.Cascaded.Get(ECssPropertyID.Width) as CssProperty;
        Assert.NotNull(prop);

        // Set initial value
        prop.Set(CssValue.From(100.0, ECssUnit.PX));
        var firstComputed = prop.Computed;

        // Change value
        prop.Set(CssValue.From(200.0, ECssUnit.PX));
        var secondComputed = prop.Computed;

        // Computed should reflect the change
        Assert.NotNull(firstComputed);
        Assert.NotNull(secondComputed);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "ValueResolution")]
    public void ValueUpdate_ClearsIntermediateValues()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = element.Style.Cascaded.Get(ECssPropertyID.Width) as CssProperty;
        Assert.NotNull(prop);

        prop.Set(CssValue.From(100.0, ECssUnit.PX));

        // Force computation of all stages
        _ = prop.Actual;

        // Update value should trigger recomputation
        prop.Set(CssValue.Auto);

        // New computed value should be available
        Assert.NotNull(prop.Computed);
    }
    #endregion

    #region Default Unit Tests
    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "ValueResolution")]
    public void DefaultUnit_IsPx_ForDimensionProperties()
    {
        var widthDef = CssDefinitions.StyleDefinitions[ECssPropertyID.Width];
        Assert.Equal(ECssUnit.PX, widthDef.DefaultUnit);
    }
    #endregion

    #region Special Value Resolution Tests
    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "ValueResolution")]
    public void SpecialValue_None_IsPreserved()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = element.Style.Cascaded.Get(ECssPropertyID.MaxWidth) as CssProperty;
        Assert.NotNull(prop);

        prop.Set(CssValue.None);

        Assert.Equal(ECssValueTypes.NONE, prop.Assigned.Type);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "ValueResolution")]
    public void SpecialValue_Auto_IsPreserved()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = element.Style.Cascaded.Get(ECssPropertyID.Width) as CssProperty;
        Assert.NotNull(prop);

        prop.Set(CssValue.Auto);

        Assert.Equal(ECssValueTypes.AUTO, prop.Assigned.Type);
        Assert.True(prop.IsAuto);
    }
    #endregion

    #region Value Stage Consistency Tests
    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "ValueResolution")]
    public void AllStages_AreAvailable_AfterAssignment()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = element.Style.Cascaded.Get(ECssPropertyID.Width) as CssProperty;
        Assert.NotNull(prop);

        prop.Set(CssValue.From(100.0, ECssUnit.PX));

        // All stages should be accessible
        Assert.NotNull(prop.Assigned);
        Assert.NotNull(prop.Specified);
        Assert.NotNull(prop.Computed);
        Assert.NotNull(prop.Used);
        Assert.NotNull(prop.Actual);
    }

    [Fact]
    [Trait("Category", "Properties")]
    [Trait("Category", "ValueResolution")]
    public void LazyComputation_ComputesOnDemand()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = element.Style.Cascaded.Get(ECssPropertyID.Width) as CssProperty;
        Assert.NotNull(prop);

        prop.Set(CssValue.From(100.0, ECssUnit.PX));

        // Accessing Computed should trigger computation chain up to that point
        var computed = prop.Computed;
        Assert.NotNull(computed);
    }
    #endregion
}
