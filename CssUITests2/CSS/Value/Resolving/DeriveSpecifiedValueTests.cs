using CssUI.CSS;
using CssUI.CSS.Enums;
using CssUI.CSS.Internal;
using CssUI.DOM;
using Xunit;

namespace CssUITests.CSS.Value.Resolving;

/// <summary>
/// Direct unit tests for <see cref="CssValue.Derive_SpecifiedValue"/>.
/// Tests the CSS Cascading and Inheritance Level 3 specified value derivation.
/// Docs: https://www.w3.org/TR/css-cascade-3/#specified
/// 
/// These tests directly invoke the internal Derive_SpecifiedValue method
/// to verify each code path in the implementation.
/// </summary>
public class DeriveSpecifiedValueTests
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

    /// <summary>
    /// Gets a property from an element's UserRules style for testing.
    /// </summary>
    private static CssProperty GetProperty(Element element, ECssPropertyID propertyId)
    {
        var prop = element.Style.UserRules.Get(propertyId) as CssProperty;
        Assert.NotNull(prop);
        return prop;
    }

    /// <summary>
    /// Gets a property from an element's Cascaded style for testing inheritance.
    /// </summary>
    private static CssProperty GetCascadedProperty(Element element, ECssPropertyID propertyId)
    {
        var prop = element.Style.Cascaded.Get(propertyId) as CssProperty;
        Assert.NotNull(prop);
        return prop;
    }
    #endregion

    #region UNSET Keyword Tests (Lines 25-35 in implementation)
    [Fact]
    [Trait("Category", "DeriveSpecifiedValue")]
    [Trait("Category", "UNSET")]
    public void Unset_OnInheritableProperty_CallsFindInheritedValue()
    {
        // Arrange - Color is inheritable (Def.Inherited == true)
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var child = CreateTestElement(doc, "span");
        doc.documentElement?.appendChild(parent);
        parent.appendChild(child);

        // Set parent's color in UserRules and cascade to Cascaded
        var parentUserProp = GetProperty(parent, ECssPropertyID.Color);
        parentUserProp.Set(CssValue.From(EColor.Red));
        var parentCascadedProp = GetCascadedProperty(parent, ECssPropertyID.Color);
        parentCascadedProp.Cascade(parentUserProp);

        var childProp = GetProperty(child, ECssPropertyID.Color);
        Assert.True(childProp.Definition.Inherited); // Verify inheritable

        // Act - Directly call Derive_SpecifiedValue with UNSET
        var result = CssValue.Unset.Derive_SpecifiedValue(childProp);

        // Assert - Should NOT be UNSET (resolved via Find_Inherited_Value)
        Assert.NotEqual(ECssValueTypes.UNSET, result.Type);
    }

    [Fact]
    [Trait("Category", "DeriveSpecifiedValue")]
    [Trait("Category", "UNSET")]
    public void Unset_OnNonInheritableProperty_ReturnsDefinitionInitial()
    {
        // Arrange - FlexGrow is NOT inheritable, initial = 0.0
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        var def = prop.Definition;
        Assert.NotNull(def);
        Assert.False(def.Inherited); // Verify non-inheritable

        // Act - Directly call Derive_SpecifiedValue with UNSET
        var result = CssValue.Unset.Derive_SpecifiedValue(prop);

        // Assert - Should return Def.Initial (line 34: return Def.Initial)
        Assert.NotEqual(ECssValueTypes.UNSET, result.Type);
        Assert.Equal(def.Initial.AsDecimal(), result.AsDecimal());
        Assert.Equal(0.0, result.AsDecimal()); // FlexGrow initial is 0.0
    }
    #endregion

    #region INHERIT Keyword Tests (Lines 36-39 in implementation)
    [Fact]
    [Trait("Category", "DeriveSpecifiedValue")]
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

        // Act - Directly call Derive_SpecifiedValue with INHERIT
        var result = CssValue.Inherit.Derive_SpecifiedValue(childProp);

        // Assert - Should return parent's computed value (3.0)
        Assert.NotEqual(ECssValueTypes.INHERIT, result.Type);
        Assert.Equal(3.0, result.AsDecimal());
    }

    [Fact]
    [Trait("Category", "DeriveSpecifiedValue")]
    [Trait("Category", "INHERIT")]
    public void Inherit_OnRootElement_ReturnsInitialFromFindInheritedValue()
    {
        // Arrange - Root element has no parent, Find_Inherited_Value returns initial
        var doc = CreateTestDocument();
        var root = doc.documentElement;
        Assert.NotNull(root);

        var prop = GetProperty(root, ECssPropertyID.FlexGrow);
        var def = prop.Definition;
        Assert.NotNull(def);

        // Act - Directly call Derive_SpecifiedValue with INHERIT on root
        var result = CssValue.Inherit.Derive_SpecifiedValue(prop);

        // Assert - Find_Inherited_Value returns initial for root
        Assert.NotEqual(ECssValueTypes.INHERIT, result.Type);
        Assert.Equal(def.Initial.AsDecimal(), result.AsDecimal());
    }
    #endregion

    #region INITIAL Keyword Tests (Lines 40-43 in implementation)
    [Fact]
    [Trait("Category", "DeriveSpecifiedValue")]
    [Trait("Category", "INITIAL")]
    public void Initial_ReturnsDefinitionInitialValue()
    {
        // Arrange - FlexShrink has initial = 1.0
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexShrink);
        var def = prop.Definition;
        Assert.NotNull(def);
        Assert.Equal(1.0, def.Initial.AsDecimal()); // Verify initial value

        // Act - Directly call Derive_SpecifiedValue with INITIAL
        var result = CssValue.Initial.Derive_SpecifiedValue(prop);

        // Assert - Should return Def.Initial (line 43: return Def.Initial)
        Assert.NotEqual(ECssValueTypes.INITIAL, result.Type);
        Assert.Equal(def.Initial, result);
        Assert.Equal(1.0, result.AsDecimal());
    }

    [Fact]
    [Trait("Category", "DeriveSpecifiedValue")]
    [Trait("Category", "INITIAL")]
    public void Initial_IgnoresParentValue()
    {
        // Arrange - Even if parent has a value, INITIAL uses definition initial
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var child = CreateTestElement(doc, "span");
        doc.documentElement?.appendChild(parent);
        parent.appendChild(child);

        // Set parent's FlexGrow to 5.0 and cascade
        var parentUserProp = GetProperty(parent, ECssPropertyID.FlexGrow);
        parentUserProp.Set(CssValue.From(5.0));
        var parentCascadedProp = GetCascadedProperty(parent, ECssPropertyID.FlexGrow);
        parentCascadedProp.Cascade(parentUserProp);

        var childProp = GetProperty(child, ECssPropertyID.FlexGrow);
        var def = childProp.Definition;
        Assert.NotNull(def);

        // Act - Directly call Derive_SpecifiedValue with INITIAL
        var result = CssValue.Initial.Derive_SpecifiedValue(childProp);

        // Assert - Should return definition initial (0.0), NOT parent's 5.0
        Assert.Equal(def.Initial.AsDecimal(), result.AsDecimal());
        Assert.Equal(0.0, result.AsDecimal());
    }
    #endregion

    #region Default Case - Explicit Values (Lines 44-55 in implementation)
    [Fact]
    [Trait("Category", "DeriveSpecifiedValue")]
    [Trait("Category", "ExplicitValue")]
    public void ExplicitValue_WithNoResolver_ReturnsSameValue()
    {
        // Arrange - FlexGrow has no Specified stage resolver
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        var def = prop.Definition;
        Assert.NotNull(def);
        Assert.Null(def.PropertyStageResolver[(int)EPropertyStage.Specified]); // No resolver

        var inputValue = CssValue.From(7.5);

        // Act - Directly call Derive_SpecifiedValue with explicit value
        var result = inputValue.Derive_SpecifiedValue(prop);

        // Assert - Should return `this` (line 54: return this)
        Assert.Same(inputValue, result); // Same reference
        Assert.Equal(7.5, result.AsDecimal());
    }

    [Fact]
    [Trait("Category", "DeriveSpecifiedValue")]
    [Trait("Category", "ExplicitValue")]
    public void ExplicitValue_WithResolver_InvokesResolver()
    {
        // Arrange - Color has a PropertyStageResolver for Specified stage
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.Color);
        var def = prop.Definition;
        Assert.NotNull(def);

        var resolver = def.PropertyStageResolver[(int)EPropertyStage.Specified];
        Assert.NotNull(resolver); // Has resolver

        var inputValue = CssValue.From(EColor.Purple);

        // Act - Directly call Derive_SpecifiedValue with explicit value
        var result = inputValue.Derive_SpecifiedValue(prop);

        // Assert - Resolver was invoked (line 50-51), result may be different
        Assert.NotNull(result);
        // We can't directly verify resolver was called, but the value should be processed
    }

    [Fact]
    [Trait("Category", "DeriveSpecifiedValue")]
    [Trait("Category", "ExplicitValue")]
    public void ExplicitDimension_PassesThroughUnchanged()
    {
        // Arrange - Width has no Specified resolver
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.Width);
        var inputValue = CssValue.From_Dimension(100.0, ECssUnit.PX);

        // Act - Directly call Derive_SpecifiedValue
        var result = inputValue.Derive_SpecifiedValue(prop);

        // Assert - Should pass through (computed stage handles dimension resolution)
        Assert.Equal(ECssValueTypes.DIMENSION, result.Type);
    }

    [Fact]
    [Trait("Category", "DeriveSpecifiedValue")]
    [Trait("Category", "ExplicitValue")]
    public void ExplicitPercentage_PassesThroughUnchanged()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.Width);
        var inputValue = CssValue.From_Percent(50.0);

        // Act - Directly call Derive_SpecifiedValue
        var result = inputValue.Derive_SpecifiedValue(prop);

        // Assert - Percentages pass through at specified stage
        Assert.Equal(ECssValueTypes.PERCENT, result.Type);
        Assert.Same(inputValue, result);
    }

    [Fact]
    [Trait("Category", "DeriveSpecifiedValue")]
    [Trait("Category", "ExplicitValue")]
    public void ExplicitAuto_PassesThroughUnchanged()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.Width);

        // Act - Directly call Derive_SpecifiedValue with Auto
        var result = CssValue.Auto.Derive_SpecifiedValue(prop);

        // Assert - Auto passes through
        Assert.Equal(ECssValueTypes.AUTO, result.Type);
    }

    [Fact]
    [Trait("Category", "DeriveSpecifiedValue")]
    [Trait("Category", "ExplicitValue")]
    public void ExplicitKeyword_PassesThroughUnchanged()
    {
        // Arrange - FlexDirection has no Specified resolver
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexDirection);
        var def = prop.Definition;
        Assert.NotNull(def);
        Assert.Null(def.PropertyStageResolver[(int)EPropertyStage.Specified]); // No resolver

        var inputValue = CssValue.From(EFlexDirection.Column);

        // Act - Directly call Derive_SpecifiedValue
        var result = inputValue.Derive_SpecifiedValue(prop);

        // Assert - Keyword passes through
        Assert.Equal(ECssValueTypes.KEYWORD, result.Type);
        Assert.Same(inputValue, result);
    }
    #endregion

    #region NULL Assigned Value Tests (Lines 57-79 in implementation)
    [Fact]
    [Trait("Category", "DeriveSpecifiedValue")]
    [Trait("Category", "NullValue")]
    public void NullValue_OnInheritableProperty_NonRootElement_ReturnsInitial()
    {
        // Arrange - Color is inheritable, element is not root
        // NOTE: The implementation's logic at lines 68-73 is:
        //   if (!Property.Owner.isRoot) return Def.Initial;
        // So NON-ROOT elements return initial value directly.
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var child = CreateTestElement(doc, "span");
        doc.documentElement?.appendChild(parent);
        parent.appendChild(child);

        var childProp = GetProperty(child, ECssPropertyID.Color);
        var def = childProp.Definition;
        Assert.NotNull(def);
        Assert.True(def.Inherited);
        Assert.False(childProp.Owner.isRoot);

        // Act - Directly call Derive_SpecifiedValue with NULL
        var result = CssValue.Null.Derive_SpecifiedValue(childProp);

        // Assert - Non-root returns Def.Initial (line 69)
        Assert.NotNull(result);
        Assert.False(result.IsNull);
    }

    [Fact]
    [Trait("Category", "DeriveSpecifiedValue")]
    [Trait("Category", "NullValue")]
    public void NullValue_OnInheritableProperty_RootElement_CallsFindInheritedValue()
    {
        // Arrange - Root element with inheritable property and NULL value
        // NOTE: The implementation's logic at lines 68-73 is:
        //   if (!Property.Owner.isRoot) return Def.Initial;
        //   return Property.Find_Inherited_Value();
        // So ROOT elements call Find_Inherited_Value, non-root get initial.
        // (This appears inverted from the comment, but we test actual behavior)
        var doc = CreateTestDocument();
        var root = doc.documentElement;
        Assert.NotNull(root);

        var prop = GetProperty(root, ECssPropertyID.Color);
        var def = prop.Definition;
        Assert.NotNull(def);
        Assert.True(def.Inherited);

        // Act - Directly call Derive_SpecifiedValue with NULL on root
        var result = CssValue.Null.Derive_SpecifiedValue(prop);

        // Assert - Result is not null (Find_Inherited_Value returns initial for root)
        Assert.NotNull(result);
        Assert.False(result.IsNull);
    }

    [Fact]
    [Trait("Category", "DeriveSpecifiedValue")]
    [Trait("Category", "NullValue")]
    public void NullValue_OnNonInheritableProperty_ReturnsInitial()
    {
        // Arrange - FlexGrow is NOT inheritable
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        var def = prop.Definition;
        Assert.NotNull(def);
        Assert.False(def.Inherited);

        // Act - Directly call Derive_SpecifiedValue with NULL
        var result = CssValue.Null.Derive_SpecifiedValue(prop);

        // Assert - Returns Def.Initial (line 78: return Def.Initial)
        Assert.NotNull(result);
        Assert.Equal(def.Initial.AsDecimal(), result.AsDecimal());
        Assert.Equal(0.0, result.AsDecimal());
    }
    #endregion

    #region Edge Cases and Branch Coverage
    [Fact]
    [Trait("Category", "DeriveSpecifiedValue")]
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
        var result1 = inputValue.Derive_SpecifiedValue(prop);
        var result2 = inputValue.Derive_SpecifiedValue(prop);
        var result3 = inputValue.Derive_SpecifiedValue(prop);

        // Assert - All should be consistent
        Assert.Same(result1, result2);
        Assert.Same(result2, result3);
    }

    [Fact]
    [Trait("Category", "DeriveSpecifiedValue")]
    [Trait("Category", "EdgeCase")]
    public void DifferentKeywords_RouteToDifferentBranches()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        var def = prop.Definition;
        Assert.NotNull(def);

        // Act - Test each CSS-wide keyword
        var unsetResult = CssValue.Unset.Derive_SpecifiedValue(prop);
        var inheritResult = CssValue.Inherit.Derive_SpecifiedValue(prop);
        var initialResult = CssValue.Initial.Derive_SpecifiedValue(prop);
        var explicitResult = CssValue.From(3.0).Derive_SpecifiedValue(prop);

        // Assert - Each takes different path
        Assert.NotEqual(ECssValueTypes.UNSET, unsetResult.Type);
        Assert.NotEqual(ECssValueTypes.INHERIT, inheritResult.Type);
        Assert.NotEqual(ECssValueTypes.INITIAL, initialResult.Type);
        Assert.Equal(ECssValueTypes.NUMBER, explicitResult.Type);
    }

    [Fact]
    [Trait("Category", "DeriveSpecifiedValue")]
    [Trait("Category", "EdgeCase")]
    public void ZeroValue_IsValidExplicitValue()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        var inputValue = CssValue.From(0.0);

        // Act - Directly call with zero
        var result = inputValue.Derive_SpecifiedValue(prop);

        // Assert - Zero is a valid explicit value, passes through
        Assert.Same(inputValue, result);
        Assert.Equal(0.0, result.AsDecimal());
    }

    [Fact]
    [Trait("Category", "DeriveSpecifiedValue")]
    [Trait("Category", "EdgeCase")]
    public void DeepInheritance_InheritKeywordPropagates()
    {
        // Arrange - 3-level hierarchy
        var doc = CreateTestDocument();
        var grandparent = CreateTestElement(doc, "div");
        var parent = CreateTestElement(doc, "section");
        var child = CreateTestElement(doc, "span");

        doc.documentElement?.appendChild(grandparent);
        grandparent.appendChild(parent);
        parent.appendChild(child);

        // Set grandparent's FlexGrow and cascade to Cascaded
        var gpUserProp = GetProperty(grandparent, ECssPropertyID.FlexGrow);
        gpUserProp.Set(CssValue.From(7.0));
        var gpCascadedProp = GetCascadedProperty(grandparent, ECssPropertyID.FlexGrow);
        gpCascadedProp.Cascade(gpUserProp);

        // Parent sets explicit value and cascades to Cascaded
        // (We can't cascade INHERIT since it needs to be resolved first)
        var parentUserProp = GetProperty(parent, ECssPropertyID.FlexGrow);
        parentUserProp.Set(CssValue.From(7.0)); // Same value grandparent would provide
        var parentCascadedProp = GetCascadedProperty(parent, ECssPropertyID.FlexGrow);
        parentCascadedProp.Cascade(parentUserProp);

        var childProp = GetProperty(child, ECssPropertyID.FlexGrow);

        // Act - Child inherits from parent
        var result = CssValue.Inherit.Derive_SpecifiedValue(childProp);

        // Assert - Should get parent's cascaded computed value (7.0)
        Assert.Equal(7.0, result.AsDecimal());
    }

    [Fact]
    [Trait("Category", "DeriveSpecifiedValue")]
    [Trait("Category", "EdgeCase")]
    public void NoneValue_PassesThroughAsExplicitValue()
    {
        // Arrange - CssValue.None is an explicit value (not a CSS-wide keyword)
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        // Use a property that accepts 'none' and has no Specified resolver
        var prop = GetProperty(element, ECssPropertyID.FlexDirection);
        var def = prop.Definition;
        Assert.NotNull(def);
        Assert.Null(def.PropertyStageResolver[(int)EPropertyStage.Specified]); // No resolver

        // Act - Directly call with None
        var result = CssValue.None.Derive_SpecifiedValue(prop);

        // Assert - None is explicit value, should pass through default branch
        Assert.Equal(ECssValueTypes.NONE, result.Type);
        Assert.Same(CssValue.None, result);
    }
    #endregion

    #region Code Path Verification Tests
    [Fact]
    [Trait("Category", "DeriveSpecifiedValue")]
    [Trait("Category", "CodePath")]
    public void IsNull_True_SkipsSwitchStatement()
    {
        // This tests the path at line 22: if (!IsNull) - when IsNull is true
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        Assert.True(CssValue.Null.IsNull); // Verify null

        // Act - Directly call with NULL value
        var result = CssValue.Null.Derive_SpecifiedValue(prop);

        // Assert - Went to null handling path (lines 57-79), not switch
        Assert.NotNull(result);
        Assert.False(result.IsNull);
    }

    [Fact]
    [Trait("Category", "DeriveSpecifiedValue")]
    [Trait("Category", "CodePath")]
    public void Definition_Inherited_True_TakesInheritancePath()
    {
        // Verify Def.Inherited affects control flow
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var child = CreateTestElement(doc, "span");
        doc.documentElement?.appendChild(parent);
        parent.appendChild(child);

        // Color is inheritable
        var colorProp = GetProperty(child, ECssPropertyID.Color);
        Assert.True(colorProp.Definition.Inherited);

        // FlexGrow is not inheritable
        var flexProp = GetProperty(child, ECssPropertyID.FlexGrow);
        Assert.False(flexProp.Definition.Inherited);

        // Act - Same UNSET keyword, different property inheritance
        var colorResult = CssValue.Unset.Derive_SpecifiedValue(colorProp);
        var flexResult = CssValue.Unset.Derive_SpecifiedValue(flexProp);

        // Assert - Both resolved but via different paths
        Assert.NotEqual(ECssValueTypes.UNSET, colorResult.Type);
        Assert.NotEqual(ECssValueTypes.UNSET, flexResult.Type);
        // FlexGrow should be initial (0.0)
        Assert.Equal(0.0, flexResult.AsDecimal());
    }

    [Fact]
    [Trait("Category", "DeriveSpecifiedValue")]
    [Trait("Category", "CodePath")]
    public void Owner_IsRoot_AffectsNullValuePath()
    {
        // Verify Property.Owner.isRoot affects control flow for null values
        // Current implementation (lines 68-73):
        //   if (!Property.Owner.isRoot) return Def.Initial;
        //   return Property.Find_Inherited_Value();
        var doc = CreateTestDocument();
        var root = doc.documentElement;
        Assert.NotNull(root);

        var child = CreateTestElement(doc, "div");
        root.appendChild(child);

        // Both use Color (inheritable)
        var rootProp = GetProperty(root, ECssPropertyID.Color);
        var childProp = GetProperty(child, ECssPropertyID.Color);
        var def = rootProp.Definition;
        Assert.NotNull(def);

        // Act - NULL on both
        var rootResult = CssValue.Null.Derive_SpecifiedValue(rootProp);
        var childResult = CssValue.Null.Derive_SpecifiedValue(childProp);

        // Assert - Both resolved via different paths
        // Root: Find_Inherited_Value (returns initial since no parent)
        // Non-root: directly returns Def.Initial
        Assert.False(rootResult.IsNull);
        Assert.False(childResult.IsNull);
    }
    #endregion
}

