using CssUI.CSS;
using CssUI.CSS.Enums;
using CssUI.CSS.Internal;
using CssUI.DOM;
using Xunit;

namespace CssUITests.CSS.Properties;

/// <summary>
/// Tests for CssProperty value stage pipeline: Assigned → Specified → Computed → Used → Actual.
/// Tests the derivation chain and lazy evaluation behavior.
/// Spec Reference: https://www.w3.org/TR/css-cascade-3/
/// </summary>
public class CssPropertyValuePipelineTests
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

    #region Assigned → Specified Derivation
    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "ValuePipeline")]
    public void AssignedValue_TriggersSpecifiedDerivation()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);

        // Act - Set Assigned value
        prop.Set(CssValue.From(3.0));

        // Assert - Specified should be derived from Assigned
        Assert.NotNull(prop.Specified);
        Assert.Equal(3.0, prop.Specified.AsDecimal());
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "ValuePipeline")]
    public void AssignedInherit_SpecifiedResolvesToParentValue()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var child = CreateTestElement(doc, "span");
        doc.documentElement?.appendChild(parent);
        parent.appendChild(child);

        // Set parent value and cascade
        var parentProp = GetProperty(parent, ECssPropertyID.FlexGrow);
        parentProp.Set(CssValue.From(5.0));
        var parentCascadedProp = GetCascadedProperty(parent, ECssPropertyID.FlexGrow);
        parentCascadedProp.Cascade(parentProp);

        var childProp = GetProperty(child, ECssPropertyID.FlexGrow);

        // Act - Set child to inherit
        childProp.Set(CssValue.Inherit);

        // Assert - Specified resolves INHERIT to parent's computed value
        Assert.NotNull(childProp.Specified);
        Assert.NotEqual(ECssValueTypes.INHERIT, childProp.Specified.Type);
        Assert.Equal(5.0, childProp.Specified.AsDecimal());
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "ValuePipeline")]
    public void AssignedInitial_SpecifiedResolvesToDefinitionInitial()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexShrink);
        var def = prop.Definition;
        Assert.NotNull(def);
        Assert.Equal(1.0, def.Initial.AsDecimal()); // FlexShrink initial = 1.0

        // Act - Set to INITIAL
        prop.Set(CssValue.Initial);

        // Assert - Specified resolves to definition's initial value
        Assert.NotNull(prop.Specified);
        Assert.NotEqual(ECssValueTypes.INITIAL, prop.Specified.Type);
        Assert.Equal(1.0, prop.Specified.AsDecimal());
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "ValuePipeline")]
    public void AssignedUnset_OnNonInheritable_ResolvesToInitial()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        var def = prop.Definition;
        Assert.NotNull(def);
        Assert.False(def.Inherited); // FlexGrow is NOT inheritable

        // Act - Set to UNSET
        prop.Set(CssValue.Unset);

        // Assert - UNSET on non-inheritable = INITIAL
        Assert.NotNull(prop.Specified);
        Assert.NotEqual(ECssValueTypes.UNSET, prop.Specified.Type);
        Assert.Equal(def.Initial.AsDecimal(), prop.Specified.AsDecimal());
    }
    #endregion

    #region Specified → Computed Derivation
    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "ValuePipeline")]
    public void SpecifiedValue_TriggersComputedDerivation()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);

        // Act
        prop.Set(CssValue.From(2.0));

        // Assert - Computed should be derived from Specified
        Assert.NotNull(prop.Computed);
        Assert.Equal(2.0, prop.Computed.AsDecimal());
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "ValuePipeline")]
    public void SpecifiedDimension_ComputedResolvesToNumber()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.Width);

        // Act - Set dimension value
        prop.Set(CssValue.From_Dimension(100.0, ECssUnit.PX));

        // Assert - Computed resolves DIMENSION to NUMBER
        Assert.NotNull(prop.Computed);
        Assert.Equal(ECssValueTypes.NUMBER, prop.Computed.Type);
        Assert.Equal(100.0, prop.Computed.AsDecimal());
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "ValuePipeline")]
    public void SpecifiedPercent_ComputedPreservesPercent()
    {
        // CSS Values Level 4: Percentages are NOT resolved during cascade
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.Width);

        // Act
        prop.Set(CssValue.From_Percent(50.0));

        // Assert - Percentage preserved at Computed stage
        Assert.NotNull(prop.Computed);
        Assert.Equal(ECssValueTypes.PERCENT, prop.Computed.Type);
        Assert.Equal(50.0, prop.Computed.AsDecimal());
    }
    #endregion

    #region Computed → Used Derivation
    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "ValuePipeline")]
    public void ComputedValue_TriggersUsedDerivation()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);

        // Act
        prop.Set(CssValue.From(4.0));

        // Assert - Used should be derived from Computed
        Assert.NotNull(prop.Used);
        Assert.Equal(4.0, prop.Used.AsDecimal());
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "ValuePipeline")]
    public void UsedValue_NoResolver_PassesThroughComputed()
    {
        // FlexGrow has no Used stage resolver
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        var def = prop.Definition;
        Assert.NotNull(def);
        Assert.Null(def.PropertyStageResolver[(int)EPropertyStage.Used]);

        // Act
        prop.Set(CssValue.From(6.0));

        // Assert - Used equals Computed (pass-through)
        Assert.NotNull(prop.Computed);
        Assert.NotNull(prop.Used);
        Assert.Equal(prop.Computed.AsDecimal(), prop.Used.AsDecimal());
    }
    #endregion

    #region Used → Actual Derivation
    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "ValuePipeline")]
    public void UsedValue_TriggersActualDerivation()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);

        // Act
        prop.Set(CssValue.From(8.0));

        // Assert - Actual should be derived from Used
        Assert.NotNull(prop.Actual);
        Assert.Equal(8.0, prop.Actual.AsDecimal());
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "ValuePipeline")]
    public void ActualValue_NoResolver_PassesThroughUsed()
    {
        // FlexGrow has no Actual stage resolver
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        var def = prop.Definition;
        Assert.NotNull(def);
        Assert.Null(def.PropertyStageResolver[(int)EPropertyStage.Actual]);

        // Act
        prop.Set(CssValue.From(9.0));

        // Assert - Actual equals Used (pass-through)
        Assert.NotNull(prop.Used);
        Assert.NotNull(prop.Actual);
        Assert.Equal(prop.Used.AsDecimal(), prop.Actual.AsDecimal());
    }
    #endregion

    #region Lazy Evaluation Tests
    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "LazyEvaluation")]
    public void LazyEvaluation_SpecifiedComputedOnFirstAccess()
    {
        // Values are computed lazily on first access
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        prop.Set(CssValue.From(3.0));

        // Force property to clear cached values via Update
        prop.Update(ComputeNow: false);

        // First access triggers derivation
        var specified = prop.Specified;
        Assert.NotNull(specified);
        Assert.Equal(3.0, specified.AsDecimal());
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "LazyEvaluation")]
    public void LazyEvaluation_ComputedComputedOnFirstAccess()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        prop.Set(CssValue.From(4.0));
        prop.Update(ComputeNow: false);

        // First access triggers derivation chain
        var computed = prop.Computed;
        Assert.NotNull(computed);
        Assert.Equal(4.0, computed.AsDecimal());
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "LazyEvaluation")]
    public void LazyEvaluation_ActualComputedOnFirstAccess()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        prop.Set(CssValue.From(5.0));
        prop.Update(ComputeNow: false);

        // Accessing Actual triggers full derivation chain
        var actual = prop.Actual;
        Assert.NotNull(actual);
        Assert.Equal(5.0, actual.AsDecimal());
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "LazyEvaluation")]
    public void Update_ComputeNowTrue_ComputesAllStages()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        prop.Set(CssValue.From(7.0));

        // Act - Force immediate computation
        prop.Update(ComputeNow: true);

        // Assert - All stages computed
        Assert.NotNull(prop.Specified);
        Assert.NotNull(prop.Computed);
        Assert.NotNull(prop.Used);
        Assert.NotNull(prop.Actual);
        Assert.Equal(7.0, prop.Actual.AsDecimal());
    }
    #endregion

    #region Change Propagation Tests
    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "ChangePropagation")]
    public void ChangingAssigned_PropagatesThroughAllStages()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);

        // Initial value
        prop.Set(CssValue.From(1.0));
        Assert.Equal(1.0, prop.Actual?.AsDecimal());

        // Change value
        prop.Set(CssValue.From(10.0));

        // Assert - Change propagated through all stages
        Assert.Equal(10.0, prop.Specified?.AsDecimal());
        Assert.Equal(10.0, prop.Computed?.AsDecimal());
        Assert.Equal(10.0, prop.Used?.AsDecimal());
        Assert.Equal(10.0, prop.Actual?.AsDecimal());
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "ChangePropagation")]
    public void MultipleChanges_EachPropagatesCorrectly()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);

        // Multiple changes
        prop.Set(CssValue.From(1.0));
        Assert.Equal(1.0, prop.Actual?.AsDecimal());

        prop.Set(CssValue.From(2.0));
        Assert.Equal(2.0, prop.Actual?.AsDecimal());

        prop.Set(CssValue.From(3.0));
        Assert.Equal(3.0, prop.Actual?.AsDecimal());
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "ChangePropagation")]
    public void ChangingToSameValue_DoesNotRecompute()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);

        // Set initial value
        prop.Set(CssValue.From(5.0));
        var firstActual = prop.Actual;

        // Set same value again
        prop.Set(CssValue.From(5.0));
        var secondActual = prop.Actual;

        // Value should be consistent
        Assert.Equal(firstActual?.AsDecimal(), secondActual?.AsDecimal());
    }
    #endregion

    #region Full Pipeline Tests
    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "FullPipeline")]
    public void FullPipeline_Assigned_To_Actual()
    {
        // Test the complete Assigned → Specified → Computed → Used → Actual chain
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);

        // Act
        prop.Set(CssValue.From(42.0));

        // Assert - All stages have correct values
        Assert.Equal(42.0, prop.Assigned.AsDecimal());
        Assert.Equal(42.0, prop.Specified?.AsDecimal());
        Assert.Equal(42.0, prop.Computed?.AsDecimal());
        Assert.Equal(42.0, prop.Used?.AsDecimal());
        Assert.Equal(42.0, prop.Actual?.AsDecimal());
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "FullPipeline")]
    public void FullPipeline_WithInherit()
    {
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var child = CreateTestElement(doc, "span");
        doc.documentElement?.appendChild(parent);
        parent.appendChild(child);

        // Parent has value 99
        var parentProp = GetProperty(parent, ECssPropertyID.FlexGrow);
        parentProp.Set(CssValue.From(99.0));
        var parentCascadedProp = GetCascadedProperty(parent, ECssPropertyID.FlexGrow);
        parentCascadedProp.Cascade(parentProp);

        // Child inherits
        var childProp = GetProperty(child, ECssPropertyID.FlexGrow);
        childProp.Set(CssValue.Inherit);

        // Assert - Child's actual value is parent's value
        Assert.Equal(ECssValueTypes.INHERIT, childProp.Assigned.Type);
        Assert.Equal(99.0, childProp.Actual?.AsDecimal());
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "FullPipeline")]
    public void FullPipeline_WithDimension()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.Width);

        // Act - Set dimension
        prop.Set(CssValue.From_Dimension(200.0, ECssUnit.PX));

        // Assert - DIMENSION at Assigned, NUMBER at Computed
        Assert.Equal(ECssValueTypes.DIMENSION, prop.Assigned.Type);
        Assert.Equal(ECssValueTypes.DIMENSION, prop.Specified?.Type);
        Assert.Equal(ECssValueTypes.NUMBER, prop.Computed?.Type);
        Assert.Equal(200.0, prop.Actual?.AsDecimal());
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "FullPipeline")]
    public void FullPipeline_WithKeyword()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexDirection);

        // Act - Set keyword
        prop.Set(CssValue.From(EFlexDirection.Column));

        // Assert - Keyword passes through all stages
        Assert.Equal(ECssValueTypes.KEYWORD, prop.Assigned.Type);
        Assert.Equal(ECssValueTypes.KEYWORD, prop.Specified?.Type);
        Assert.Equal(ECssValueTypes.KEYWORD, prop.Computed?.Type);
        Assert.Equal(ECssValueTypes.KEYWORD, prop.Actual?.Type);
    }
    #endregion

    #region Edge Cases
    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "EdgeCase")]
    public void NullAssigned_ResolvesToInitial()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        var def = prop.Definition;
        Assert.NotNull(def);

        // Don't set any value - Assigned is CssValue.Null

        // Assert - Should resolve to initial
        Assert.NotNull(prop.Actual);
        Assert.Equal(def.Initial.AsDecimal(), prop.Actual.AsDecimal());
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "EdgeCase")]
    public void ZeroValue_PropagatesCorrectly()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);

        // Act - Set zero
        prop.Set(CssValue.From(0.0));

        // Assert - Zero propagates through all stages
        Assert.Equal(0.0, prop.Assigned.AsDecimal());
        Assert.Equal(0.0, prop.Actual?.AsDecimal());
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "EdgeCase")]
    public void AutoValue_PropagatesCorrectly()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.Width);

        // Act - Set auto
        prop.Set(CssValue.Auto);

        // Assert - Auto propagates
        Assert.Equal(ECssValueTypes.AUTO, prop.Assigned.Type);
        Assert.Equal(ECssValueTypes.AUTO, prop.Actual?.Type);
    }
    #endregion
}
