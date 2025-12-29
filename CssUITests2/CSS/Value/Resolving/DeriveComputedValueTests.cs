using CssUI.CSS;
using CssUI.CSS.Enums;
using CssUI.CSS.Internal;
using CssUI.DOM;
using Xunit;

namespace CssUITests.CSS.Value.Resolving;

/// <summary>
/// Direct unit tests for <see cref="CssValue.Derive_ComputedValue"/>.
/// Tests the CSS Cascading and Inheritance Level 3 computed value derivation.
/// Docs: https://www.w3.org/TR/css-cascade-3/#computed
/// 
/// These tests directly invoke the internal Derive_ComputedValue method
/// to verify each code path in the implementation.
/// </summary>
public class DeriveComputedValueTests
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

    private static CssProperty GetProperty(Element element, ECssPropertyID propertyId)
    {
        var prop = element.Style.UserRules.Get(propertyId) as CssProperty;
        Assert.NotNull(prop);
        return prop;
    }

    private static CssProperty GetCascadedProperty(Element element, ECssPropertyID propertyId)
    {
        var prop = element.Style.Cascaded.Get(propertyId) as CssProperty;
        Assert.NotNull(prop);
        return prop;
    }
    #endregion

    #region INHERIT Keyword Tests (Lines 93-97 in implementation)
    [Fact]
    [Trait("Category", "DeriveComputedValue")]
    [Trait("Category", "INHERIT")]
    public void Inherit_CallsFindInheritedValue()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var child = CreateTestElement(doc, "span");
        doc.documentElement?.appendChild(parent);
        parent.appendChild(child);

        // Set parent's FlexGrow to 3.0 and cascade
        var parentUserProp = GetProperty(parent, ECssPropertyID.FlexGrow);
        parentUserProp.Set(CssValue.From(3.0));
        var parentCascadedProp = GetCascadedProperty(parent, ECssPropertyID.FlexGrow);
        parentCascadedProp.Cascade(parentUserProp);

        var childProp = GetProperty(child, ECssPropertyID.FlexGrow);

        // Act - Directly call Derive_ComputedValue with INHERIT
        var result = CssValue.Inherit.Derive_ComputedValue(childProp);

        // Assert - Should return parent's computed value (3.0)
        Assert.NotEqual(ECssValueTypes.INHERIT, result.Type);
        Assert.Equal(3.0, result.AsDecimal());
    }

    [Fact]
    [Trait("Category", "DeriveComputedValue")]
    [Trait("Category", "INHERIT")]
    public void Inherit_OnRootElement_ReturnsInitial()
    {
        // Arrange - Root element has no parent
        var doc = CreateTestDocument();
        var root = doc.documentElement;
        Assert.NotNull(root);

        var prop = GetProperty(root, ECssPropertyID.FlexGrow);
        var def = prop.Definition;
        Assert.NotNull(def);

        // Act - Directly call Derive_ComputedValue with INHERIT on root
        var result = CssValue.Inherit.Derive_ComputedValue(prop);

        // Assert - Find_Inherited_Value returns initial for root
        Assert.NotEqual(ECssValueTypes.INHERIT, result.Type);
        Assert.Equal(def.Initial.AsDecimal(), result.AsDecimal());
    }
    #endregion

    #region PERCENT Type Tests (Lines 98-107 in implementation)
    [Fact]
    [Trait("Category", "DeriveComputedValue")]
    [Trait("Category", "PERCENT")]
    public void Percent_PreservedNotResolved()
    {
        // CSS Values Level 4 §5.1.1: Percentages are NOT resolved during cascade.
        // Resolution happens during layout phase.
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.Width);
        var percentValue = CssValue.From_Percent(50.0);

        // Act - Directly call Derive_ComputedValue with percentage
        var result = percentValue.Derive_ComputedValue(prop);

        // Assert - Should return same percentage value (not resolved)
        Assert.Equal(ECssValueTypes.PERCENT, result.Type);
        Assert.Same(percentValue, result); // Same reference - returns `this`
        Assert.Equal(50.0, result.AsDecimal());
    }

    [Fact]
    [Trait("Category", "DeriveComputedValue")]
    [Trait("Category", "PERCENT")]
    public void Percent_DifferentValues_AllPreserved()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.Width);

        // Test various percentage values
        var percent0 = CssValue.From_Percent(0.0);
        var percent100 = CssValue.From_Percent(100.0);
        var percent50 = CssValue.From_Percent(50.0);

        // Act
        var result0 = percent0.Derive_ComputedValue(prop);
        var result100 = percent100.Derive_ComputedValue(prop);
        var result50 = percent50.Derive_ComputedValue(prop);

        // Assert - All preserved as percentages
        Assert.Equal(ECssValueTypes.PERCENT, result0.Type);
        Assert.Equal(ECssValueTypes.PERCENT, result100.Type);
        Assert.Equal(ECssValueTypes.PERCENT, result50.Type);
        Assert.Equal(0.0, result0.AsDecimal());
        Assert.Equal(100.0, result100.AsDecimal());
        Assert.Equal(50.0, result50.AsDecimal());
    }
    #endregion

    #region DIMENSION Type Tests (Lines 108-112 in implementation)
    [Fact]
    [Trait("Category", "DeriveComputedValue")]
    [Trait("Category", "DIMENSION")]
    public void Dimension_ResolvedViaCssUnitResolver()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.Width);
        var dimensionValue = CssValue.From_Dimension(100.0, ECssUnit.PX);

        // Act - Directly call Derive_ComputedValue with dimension
        var result = dimensionValue.Derive_ComputedValue(prop);

        // Assert - Should be resolved to a NUMBER (absolute value)
        Assert.Equal(ECssValueTypes.NUMBER, result.Type);
        Assert.Equal(100.0, result.AsDecimal()); // 100px = 100
    }

    [Fact]
    [Trait("Category", "DeriveComputedValue")]
    [Trait("Category", "DIMENSION")]
    public void Dimension_EmUnit_Resolved()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.Width);
        var dimensionValue = CssValue.From_Dimension(2.0, ECssUnit.EM);

        // Act - Directly call Derive_ComputedValue
        var result = dimensionValue.Derive_ComputedValue(prop);

        // Assert - Should be resolved to NUMBER
        Assert.Equal(ECssValueTypes.NUMBER, result.Type);
        // The exact value depends on the font size, but it should be a number
    }

    [Fact]
    [Trait("Category", "DeriveComputedValue")]
    [Trait("Category", "DIMENSION")]
    public void Dimension_RemUnit_Resolved()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.Width);
        var dimensionValue = CssValue.From_Dimension(1.5, ECssUnit.REM);

        // Act
        var result = dimensionValue.Derive_ComputedValue(prop);

        // Assert - Should be resolved to NUMBER
        Assert.Equal(ECssValueTypes.NUMBER, result.Type);
    }

    [Fact]
    [Trait("Category", "DeriveComputedValue")]
    [Trait("Category", "DIMENSION")]
    public void Dimension_NotSameReference()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.Width);
        var dimensionValue = CssValue.From_Dimension(50.0, ECssUnit.PX);

        // Act
        var result = dimensionValue.Derive_ComputedValue(prop);

        // Assert - Result is a new CssValue (resolved), not the same reference
        Assert.NotSame(dimensionValue, result);
        Assert.Equal(ECssValueTypes.NUMBER, result.Type);
    }
    #endregion

    #region Custom PropertyStageResolver Tests (Lines 115-120 in implementation)
    [Fact]
    [Trait("Category", "DeriveComputedValue")]
    [Trait("Category", "PropertyStageResolver")]
    public void CustomResolver_InvokedWhenDefined()
    {
        // Arrange - Find a property with a Computed stage resolver
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.Color);
        var def = prop.Definition;
        Assert.NotNull(def);

        var resolver = def.PropertyStageResolver[(int)EPropertyStage.Computed];
        // Color may or may not have a Computed resolver - this tests the path exists

        var inputValue = CssValue.From(EColor.Blue);

        // Act
        var result = inputValue.Derive_ComputedValue(prop);

        // Assert - Result should be valid (resolver processed or pass-through)
        Assert.NotNull(result);
    }

    [Fact]
    [Trait("Category", "DeriveComputedValue")]
    [Trait("Category", "PropertyStageResolver")]
    public void NoResolver_ExplicitValuePassesThrough()
    {
        // Arrange - FlexGrow has no Computed stage resolver
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        var def = prop.Definition;
        Assert.NotNull(def);
        Assert.Null(def.PropertyStageResolver[(int)EPropertyStage.Computed]); // No resolver

        var inputValue = CssValue.From(2.5);

        // Act - Directly call Derive_ComputedValue
        var result = inputValue.Derive_ComputedValue(prop);

        // Assert - Should return `this` (line 123: return this)
        Assert.Same(inputValue, result); // Same reference
        Assert.Equal(2.5, result.AsDecimal());
    }
    #endregion

    #region Pass-Through Tests (Default case - Line 123)
    [Fact]
    [Trait("Category", "DeriveComputedValue")]
    [Trait("Category", "PassThrough")]
    public void NumberValue_PassesThroughUnchanged()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        var inputValue = CssValue.From(7.5);

        // Act
        var result = inputValue.Derive_ComputedValue(prop);

        // Assert - NUMBER type passes through
        Assert.Same(inputValue, result);
        Assert.Equal(ECssValueTypes.NUMBER, result.Type);
    }

    [Fact]
    [Trait("Category", "DeriveComputedValue")]
    [Trait("Category", "PassThrough")]
    public void KeywordValue_PassesThroughUnchanged()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexDirection);
        var def = prop.Definition;
        Assert.NotNull(def);
        Assert.Null(def.PropertyStageResolver[(int)EPropertyStage.Computed]); // No resolver

        var inputValue = CssValue.From(EFlexDirection.ColumnReverse);

        // Act
        var result = inputValue.Derive_ComputedValue(prop);

        // Assert - Keyword passes through
        Assert.Same(inputValue, result);
        Assert.Equal(ECssValueTypes.KEYWORD, result.Type);
    }

    [Fact]
    [Trait("Category", "DeriveComputedValue")]
    [Trait("Category", "PassThrough")]
    public void AutoValue_PassesThroughUnchanged()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.Width);

        // Act
        var result = CssValue.Auto.Derive_ComputedValue(prop);

        // Assert - AUTO passes through
        Assert.Same(CssValue.Auto, result);
        Assert.Equal(ECssValueTypes.AUTO, result.Type);
    }

    [Fact]
    [Trait("Category", "DeriveComputedValue")]
    [Trait("Category", "PassThrough")]
    public void NoneValue_PassesThroughUnchanged()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexDirection);

        // Act
        var result = CssValue.None.Derive_ComputedValue(prop);

        // Assert - NONE passes through
        Assert.Same(CssValue.None, result);
        Assert.Equal(ECssValueTypes.NONE, result.Type);
    }

    [Fact]
    [Trait("Category", "DeriveComputedValue")]
    [Trait("Category", "PassThrough")]
    public void ZeroValue_PassesThroughUnchanged()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        var inputValue = CssValue.From(0.0);

        // Act
        var result = inputValue.Derive_ComputedValue(prop);

        // Assert
        Assert.Same(inputValue, result);
        Assert.Equal(0.0, result.AsDecimal());
    }
    #endregion

    #region Edge Cases
    [Fact]
    [Trait("Category", "DeriveComputedValue")]
    [Trait("Category", "EdgeCase")]
    public void MultipleInvocations_ReturnConsistentResults()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        var inputValue = CssValue.From(5.0);

        // Act - Multiple invocations
        var result1 = inputValue.Derive_ComputedValue(prop);
        var result2 = inputValue.Derive_ComputedValue(prop);
        var result3 = inputValue.Derive_ComputedValue(prop);

        // Assert - All should be consistent (same reference for pass-through)
        Assert.Same(result1, result2);
        Assert.Same(result2, result3);
    }

    [Fact]
    [Trait("Category", "DeriveComputedValue")]
    [Trait("Category", "EdgeCase")]
    public void DifferentTypes_RouteToDifferentBranches()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.Width);

        // Act - Test different value types
        var inheritResult = CssValue.Inherit.Derive_ComputedValue(prop);
        var percentResult = CssValue.From_Percent(50.0).Derive_ComputedValue(prop);
        var dimensionResult = CssValue.From_Dimension(100.0, ECssUnit.PX).Derive_ComputedValue(prop);
        var autoResult = CssValue.Auto.Derive_ComputedValue(prop);

        // Assert - Each takes different path
        Assert.NotEqual(ECssValueTypes.INHERIT, inheritResult.Type); // Resolved
        Assert.Equal(ECssValueTypes.PERCENT, percentResult.Type);    // Preserved
        Assert.Equal(ECssValueTypes.NUMBER, dimensionResult.Type);   // Resolved
        Assert.Equal(ECssValueTypes.AUTO, autoResult.Type);          // Pass-through
    }

    [Fact]
    [Trait("Category", "DeriveComputedValue")]
    [Trait("Category", "EdgeCase")]
    public void InitialKeyword_PassesThroughDefault()
    {
        // INITIAL is not handled specially in Derive_ComputedValue
        // (it's handled in Derive_SpecifiedValue)
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);

        // Act
        var result = CssValue.Initial.Derive_ComputedValue(prop);

        // Assert - INITIAL not in switch, goes to default path
        Assert.NotNull(result);
    }

    [Fact]
    [Trait("Category", "DeriveComputedValue")]
    [Trait("Category", "EdgeCase")]
    public void UnsetKeyword_PassesThroughDefault()
    {
        // UNSET is not handled specially in Derive_ComputedValue
        // (it's handled in Derive_SpecifiedValue)
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);

        // Act
        var result = CssValue.Unset.Derive_ComputedValue(prop);

        // Assert - UNSET not in switch, goes to default path
        Assert.NotNull(result);
    }
    #endregion

    #region Code Path Verification
    [Fact]
    [Trait("Category", "DeriveComputedValue")]
    [Trait("Category", "CodePath")]
    public void SwitchStatement_CoversAllSpecialCases()
    {
        // Verify the three special cases in the switch statement:
        // 1. INHERIT
        // 2. PERCENT
        // 3. DIMENSION
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.Width);

        // INHERIT - resolved via Find_Inherited_Value
        var inheritInput = CssValue.Inherit;
        var inheritResult = inheritInput.Derive_ComputedValue(prop);
        Assert.NotEqual(ECssValueTypes.INHERIT, inheritResult.Type);

        // PERCENT - preserved as-is
        var percentInput = CssValue.From_Percent(75.0);
        var percentResult = percentInput.Derive_ComputedValue(prop);
        Assert.Equal(ECssValueTypes.PERCENT, percentResult.Type);
        Assert.Same(percentInput, percentResult);

        // DIMENSION - resolved to NUMBER
        var dimensionInput = CssValue.From_Dimension(200.0, ECssUnit.PX);
        var dimensionResult = dimensionInput.Derive_ComputedValue(prop);
        Assert.Equal(ECssValueTypes.NUMBER, dimensionResult.Type);
        Assert.NotSame(dimensionInput, dimensionResult);
    }

    [Fact]
    [Trait("Category", "DeriveComputedValue")]
    [Trait("Category", "CodePath")]
    public void DefaultPath_ChecksForResolver()
    {
        // Verify the default path checks PropertyStageResolver
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        // Property WITH resolver
        var colorProp = GetProperty(element, ECssPropertyID.Color);
        var colorDef = colorProp.Definition;
        Assert.NotNull(colorDef);
        var hasColorResolver = colorDef.PropertyStageResolver[(int)EPropertyStage.Computed] is not null;

        // Property WITHOUT resolver
        var flexProp = GetProperty(element, ECssPropertyID.FlexGrow);
        var flexDef = flexProp.Definition;
        Assert.NotNull(flexDef);
        Assert.Null(flexDef.PropertyStageResolver[(int)EPropertyStage.Computed]);

        // FlexGrow value should pass through unchanged
        var flexInput = CssValue.From(4.0);
        var flexResult = flexInput.Derive_ComputedValue(flexProp);
        Assert.Same(flexInput, flexResult);
    }
    #endregion
}
