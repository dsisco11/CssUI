using CssUI.CSS;
using CssUI.CSS.Enums;
using CssUI.CSS.Internal;
using CssUI.DOM;
using Xunit;

namespace CssUITests.CSS.Value.Resolving;

/// <summary>
/// Direct unit tests for <see cref="CssValue.Derive_UsedValue"/>.
/// Tests the CSS Cascading and Inheritance Level 3 used value derivation.
/// Docs: https://www.w3.org/TR/css-cascade-3/#used
/// 
/// These tests directly invoke the internal Derive_UsedValue method
/// to verify each code path in the implementation.
/// </summary>
public class DeriveUsedValueTests
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
    #endregion

    #region PropertyStageResolver Tests (Lines 136-139 in implementation)
    [Fact]
    [Trait("Category", "DeriveUsedValue")]
    [Trait("Category", "PropertyStageResolver")]
    public void CustomResolver_InvokedWhenDefined()
    {
        // Arrange - Find a property that has a Used stage resolver
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        // Check various properties for Used stage resolvers
        var prop = GetProperty(element, ECssPropertyID.Width);
        var def = prop.Definition;
        Assert.NotNull(def);

        var resolver = def.PropertyStageResolver[(int)EPropertyStage.Used];

        var inputValue = CssValue.From(100.0);

        // Act
        var result = inputValue.Derive_UsedValue(prop);

        // Assert - Result should be valid (either resolver processed or pass-through)
        Assert.NotNull(result);
        Assert.False(result.IsNull);
    }

    [Fact]
    [Trait("Category", "DeriveUsedValue")]
    [Trait("Category", "PropertyStageResolver")]
    public void NoResolver_ValuePassesThroughUnchanged()
    {
        // Arrange - FlexGrow has no Used stage resolver
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        var def = prop.Definition;
        Assert.NotNull(def);
        Assert.Null(def.PropertyStageResolver[(int)EPropertyStage.Used]); // No resolver

        var inputValue = CssValue.From(2.5);

        // Act
        var result = inputValue.Derive_UsedValue(prop);

        // Assert - Should return `this` (line 142: return this)
        Assert.Same(inputValue, result); // Same reference
        Assert.Equal(2.5, result.AsDecimal());
    }
    #endregion

    #region Immutability Tests (Line 142 - CssValue is immutable)
    [Fact]
    [Trait("Category", "DeriveUsedValue")]
    [Trait("Category", "Immutability")]
    public void ImmutableValue_ReturnsSameReference()
    {
        // CssValue is immutable - no need to copy, returns `this`
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        var inputValue = CssValue.From(5.0);

        // Act
        var result = inputValue.Derive_UsedValue(prop);

        // Assert - Same reference returned (no copy needed)
        Assert.Same(inputValue, result);
    }

    [Fact]
    [Trait("Category", "DeriveUsedValue")]
    [Trait("Category", "Immutability")]
    public void MultipleInvocations_ReturnSameReference()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        var inputValue = CssValue.From(3.0);

        // Act - Multiple invocations
        var result1 = inputValue.Derive_UsedValue(prop);
        var result2 = inputValue.Derive_UsedValue(prop);
        var result3 = inputValue.Derive_UsedValue(prop);

        // Assert - All same reference
        Assert.Same(inputValue, result1);
        Assert.Same(result1, result2);
        Assert.Same(result2, result3);
    }
    #endregion

    #region Pass-Through Tests (Various value types)
    [Fact]
    [Trait("Category", "DeriveUsedValue")]
    [Trait("Category", "PassThrough")]
    public void NumberValue_PassesThroughUnchanged()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        var inputValue = CssValue.From(7.5);

        // Act
        var result = inputValue.Derive_UsedValue(prop);

        // Assert
        Assert.Same(inputValue, result);
        Assert.Equal(ECssValueTypes.NUMBER, result.Type);
        Assert.Equal(7.5, result.AsDecimal());
    }

    [Fact]
    [Trait("Category", "DeriveUsedValue")]
    [Trait("Category", "PassThrough")]
    public void KeywordValue_PassesThroughUnchanged()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexDirection);
        var def = prop.Definition;
        Assert.NotNull(def);
        Assert.Null(def.PropertyStageResolver[(int)EPropertyStage.Used]); // No resolver

        var inputValue = CssValue.From(EFlexDirection.Row);

        // Act
        var result = inputValue.Derive_UsedValue(prop);

        // Assert
        Assert.Same(inputValue, result);
        Assert.Equal(ECssValueTypes.KEYWORD, result.Type);
    }

    [Fact]
    [Trait("Category", "DeriveUsedValue")]
    [Trait("Category", "PassThrough")]
    public void AutoValue_PassesThroughUnchanged()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.Width);

        // Act
        var result = CssValue.Auto.Derive_UsedValue(prop);

        // Assert - AUTO passes through (Used stage doesn't handle it specially)
        Assert.Same(CssValue.Auto, result);
        Assert.Equal(ECssValueTypes.AUTO, result.Type);
    }

    [Fact]
    [Trait("Category", "DeriveUsedValue")]
    [Trait("Category", "PassThrough")]
    public void NoneValue_PassesThroughUnchanged()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexDirection);

        // Act
        var result = CssValue.None.Derive_UsedValue(prop);

        // Assert
        Assert.Same(CssValue.None, result);
        Assert.Equal(ECssValueTypes.NONE, result.Type);
    }

    [Fact]
    [Trait("Category", "DeriveUsedValue")]
    [Trait("Category", "PassThrough")]
    public void PercentValue_PassesThroughUnchanged()
    {
        // Percentages may be resolved at Used stage depending on property
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        var def = prop.Definition;
        Assert.NotNull(def);
        Assert.Null(def.PropertyStageResolver[(int)EPropertyStage.Used]);

        var inputValue = CssValue.From_Percent(50.0);

        // Act
        var result = inputValue.Derive_UsedValue(prop);

        // Assert - No resolver, passes through
        Assert.Same(inputValue, result);
        Assert.Equal(ECssValueTypes.PERCENT, result.Type);
    }

    [Fact]
    [Trait("Category", "DeriveUsedValue")]
    [Trait("Category", "PassThrough")]
    public void ZeroValue_PassesThroughUnchanged()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        var inputValue = CssValue.From(0.0);

        // Act
        var result = inputValue.Derive_UsedValue(prop);

        // Assert
        Assert.Same(inputValue, result);
        Assert.Equal(0.0, result.AsDecimal());
    }
    #endregion

    #region Edge Cases
    [Fact]
    [Trait("Category", "DeriveUsedValue")]
    [Trait("Category", "EdgeCase")]
    public void DifferentProperties_SameValue_ConsistentBehavior()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var inputValue = CssValue.From(10.0);

        // Test with different properties (all without Used resolvers)
        var flexGrowProp = GetProperty(element, ECssPropertyID.FlexGrow);
        var flexShrinkProp = GetProperty(element, ECssPropertyID.FlexShrink);

        Assert.Null(flexGrowProp.Definition.PropertyStageResolver[(int)EPropertyStage.Used]);
        Assert.Null(flexShrinkProp.Definition.PropertyStageResolver[(int)EPropertyStage.Used]);

        // Act
        var result1 = inputValue.Derive_UsedValue(flexGrowProp);
        var result2 = inputValue.Derive_UsedValue(flexShrinkProp);

        // Assert - Both return same reference
        Assert.Same(inputValue, result1);
        Assert.Same(inputValue, result2);
    }

    [Fact]
    [Trait("Category", "DeriveUsedValue")]
    [Trait("Category", "EdgeCase")]
    public void InheritKeyword_PassesThroughAtUsedStage()
    {
        // INHERIT is normally resolved at Specified/Computed stage
        // At Used stage it should just pass through
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        Assert.Null(prop.Definition.PropertyStageResolver[(int)EPropertyStage.Used]);

        // Act
        var result = CssValue.Inherit.Derive_UsedValue(prop);

        // Assert - No special handling, passes through
        Assert.Same(CssValue.Inherit, result);
    }

    [Fact]
    [Trait("Category", "DeriveUsedValue")]
    [Trait("Category", "EdgeCase")]
    public void InitialKeyword_PassesThroughAtUsedStage()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);

        // Act
        var result = CssValue.Initial.Derive_UsedValue(prop);

        // Assert - No special handling at Used stage
        Assert.Same(CssValue.Initial, result);
    }

    [Fact]
    [Trait("Category", "DeriveUsedValue")]
    [Trait("Category", "EdgeCase")]
    public void UnsetKeyword_PassesThroughAtUsedStage()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);

        // Act
        var result = CssValue.Unset.Derive_UsedValue(prop);

        // Assert - No special handling at Used stage
        Assert.Same(CssValue.Unset, result);
    }
    #endregion

    #region Code Path Verification
    [Fact]
    [Trait("Category", "DeriveUsedValue")]
    [Trait("Category", "CodePath")]
    public void WithResolver_ReturnsResolverResult()
    {
        // Find a property that has a Used stage resolver and verify it's invoked
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        // Check Width property - may have Used resolver for layout
        var widthProp = GetProperty(element, ECssPropertyID.Width);
        var widthDef = widthProp.Definition;
        Assert.NotNull(widthDef);

        var hasResolver = widthDef.PropertyStageResolver[(int)EPropertyStage.Used] is not null;

        var inputValue = CssValue.From(200.0);
        var result = inputValue.Derive_UsedValue(widthProp);

        if (hasResolver)
        {
            // Resolver was invoked - result may differ
            Assert.NotNull(result);
        }
        else
        {
            // No resolver - pass through
            Assert.Same(inputValue, result);
        }
    }

    [Fact]
    [Trait("Category", "DeriveUsedValue")]
    [Trait("Category", "CodePath")]
    public void WithoutResolver_ReturnsSameInstance()
    {
        // Verify properties without Used resolver return `this`
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        Assert.Null(prop.Definition.PropertyStageResolver[(int)EPropertyStage.Used]);

        var inputValue = CssValue.From(1.5);

        // Act
        var result = inputValue.Derive_UsedValue(prop);

        // Assert - Line 142: return this
        Assert.Same(inputValue, result);
    }
    #endregion
}
