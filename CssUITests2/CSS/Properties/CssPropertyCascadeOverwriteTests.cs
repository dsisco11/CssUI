using System.Threading.Tasks;
using CssUI.CSS;
using CssUI.CSS.Enums;
using CssUI.CSS.Internal;
using CssUI.DOM;
using Xunit;

namespace CssUITests.CSS.Properties;

/// <summary>
/// Tests for CssProperty.Cascade() and Overwrite() methods.
/// Cascade copies values from source property during the cascade algorithm.
/// Overwrite copies values only if they differ.
/// </summary>
public class CssPropertyCascadeOverwriteTests
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

    #region Cascade() Basic Tests
    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Cascade")]
    public void Cascade_CopiesAssignedValueFromSource()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var sourceProp = GetProperty(element, ECssPropertyID.FlexGrow);
        sourceProp.Set(CssValue.From(5.0));

        var targetProp = GetCascadedProperty(element, ECssPropertyID.FlexGrow);

        // Act
        var result = targetProp.Cascade(sourceProp);

        // Assert
        Assert.True(result);
        Assert.Equal(5.0, targetProp.Assigned.AsDecimal());
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Cascade")]
    public void Cascade_ReturnsTrue_WhenValueChanged()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var sourceProp = GetProperty(element, ECssPropertyID.FlexGrow);
        sourceProp.Set(CssValue.From(10.0));

        var targetProp = GetCascadedProperty(element, ECssPropertyID.FlexGrow);

        // Act
        var result = targetProp.Cascade(sourceProp);

        // Assert
        Assert.True(result);
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Cascade")]
    public void Cascade_ReturnsFalse_WhenSourceHasNoValue()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var sourceProp = GetProperty(element, ECssPropertyID.FlexGrow);
        // Don't set any value - Assigned is CssValue.Null (HasValue = false)

        var targetProp = GetCascadedProperty(element, ECssPropertyID.FlexGrow);

        // Act
        var result = targetProp.Cascade(sourceProp);

        // Assert - Source has no value, so no cascade
        Assert.False(result);
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Cascade")]
    public void Cascade_CopiesSelector_ButNotSourcePtr()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var sourceProp = GetProperty(element, ECssPropertyID.FlexGrow);
        sourceProp.Set(CssValue.From(7.0));
        // Store original target SourcePtr
        var originalTargetSourcePtr = GetCascadedProperty(element, ECssPropertyID.FlexGrow).SourcePtr;

        var targetProp = GetCascadedProperty(element, ECssPropertyID.FlexGrow);

        // Act
        targetProp.Cascade(sourceProp);

        // Assert - SourcePtr is NOT copied (tracks owning CssComputedStyle for event routing)
        // This is by design per the comment in CssProperty.Cascade()
        Assert.Same(originalTargetSourcePtr, targetProp.SourcePtr);
        // Selector IS copied
        Assert.Equal(sourceProp.Selector, targetProp.Selector);
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Cascade")]
    public void Cascade_TriggersUpdate()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var sourceProp = GetProperty(element, ECssPropertyID.FlexGrow);
        sourceProp.Set(CssValue.From(8.0));

        var targetProp = GetCascadedProperty(element, ECssPropertyID.FlexGrow);

        // Act
        targetProp.Cascade(sourceProp);

        // Assert - Actual value is derived
        Assert.Equal(8.0, targetProp.Actual?.AsDecimal());
    }
    #endregion

    #region CascadeAsync() Tests
    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Cascade")]
    public async Task CascadeAsync_BehavesSameAsCascade()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var sourceProp = GetProperty(element, ECssPropertyID.FlexGrow);
        sourceProp.Set(CssValue.From(15.0));

        var targetProp = GetCascadedProperty(element, ECssPropertyID.FlexGrow);

        // Act
        var result = await targetProp.CascadeAsync(sourceProp);

        // Assert
        Assert.True(result);
        Assert.Equal(15.0, targetProp.Assigned.AsDecimal());
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Cascade")]
    public async Task CascadeAsync_ReturnsFalse_WhenNoValue()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var sourceProp = GetProperty(element, ECssPropertyID.FlexGrow);
        // No value set

        var targetProp = GetCascadedProperty(element, ECssPropertyID.FlexGrow);

        // Act
        var result = await targetProp.CascadeAsync(sourceProp);

        // Assert
        Assert.False(result);
    }
    #endregion

    #region Overwrite() Basic Tests
    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Overwrite")]
    public void Overwrite_CopiesValue_WhenDifferent()
    {
        // Use two elements so we can test Overwrite on unlocked UserRules properties
        var doc = CreateTestDocument();
        var element1 = CreateTestElement(doc, "div");
        var element2 = CreateTestElement(doc, "span");
        doc.documentElement?.appendChild(element1);
        doc.documentElement?.appendChild(element2);

        var sourceProp = GetProperty(element1, ECssPropertyID.FlexGrow);
        sourceProp.Set(CssValue.From(20.0));

        var targetProp = GetProperty(element2, ECssPropertyID.FlexGrow);
        targetProp.Set(CssValue.From(1.0)); // Different value

        // Act
        var result = targetProp.Overwrite(sourceProp);

        // Assert
        Assert.True(result);
        Assert.Equal(20.0, targetProp.Assigned.AsDecimal());
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Overwrite")]
    public void Overwrite_ReturnsTrue_WhenValueChanged()
    {
        var doc = CreateTestDocument();
        var element1 = CreateTestElement(doc, "div");
        var element2 = CreateTestElement(doc, "span");
        doc.documentElement?.appendChild(element1);
        doc.documentElement?.appendChild(element2);

        var sourceProp = GetProperty(element1, ECssPropertyID.FlexGrow);
        sourceProp.Set(CssValue.From(25.0));

        var targetProp = GetProperty(element2, ECssPropertyID.FlexGrow);
        targetProp.Set(CssValue.From(5.0)); // Different

        // Act
        var result = targetProp.Overwrite(sourceProp);

        // Assert
        Assert.True(result);
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Overwrite")]
    public void Overwrite_ReturnsFalse_WhenValuesAreSame()
    {
        var doc = CreateTestDocument();
        var element1 = CreateTestElement(doc, "div");
        var element2 = CreateTestElement(doc, "span");
        doc.documentElement?.appendChild(element1);
        doc.documentElement?.appendChild(element2);

        var sameValue = CssValue.From(30.0);

        var sourceProp = GetProperty(element1, ECssPropertyID.FlexGrow);
        sourceProp.Set(sameValue);

        var targetProp = GetProperty(element2, ECssPropertyID.FlexGrow);
        targetProp.Set(sameValue); // Same value

        // Act
        var result = targetProp.Overwrite(sourceProp);

        // Assert - Values are same, no overwrite
        Assert.False(result);
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Overwrite")]
    public void Overwrite_CopiesSelector_ButNotSourcePtr()
    {
        var doc = CreateTestDocument();
        var element1 = CreateTestElement(doc, "div");
        var element2 = CreateTestElement(doc, "span");
        doc.documentElement?.appendChild(element1);
        doc.documentElement?.appendChild(element2);

        var sourceProp = GetProperty(element1, ECssPropertyID.FlexGrow);
        sourceProp.Set(CssValue.From(35.0));

        var targetProp = GetProperty(element2, ECssPropertyID.FlexGrow);
        targetProp.Set(CssValue.From(1.0)); // Different
        // Store original target SourcePtr
        var originalTargetSourcePtr = targetProp.SourcePtr;

        // Act
        targetProp.Overwrite(sourceProp);

        // Assert - SourcePtr is NOT copied (tracks owning CssComputedStyle for event routing)
        // This is by design per the comment in CssProperty.Overwrite()
        Assert.Same(originalTargetSourcePtr, targetProp.SourcePtr);
        // Selector IS copied
        Assert.Equal(sourceProp.Selector, targetProp.Selector);
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Overwrite")]
    public void Overwrite_TriggersUpdate_WhenChanged()
    {
        var doc = CreateTestDocument();
        var element1 = CreateTestElement(doc, "div");
        var element2 = CreateTestElement(doc, "span");
        doc.documentElement?.appendChild(element1);
        doc.documentElement?.appendChild(element2);

        var sourceProp = GetProperty(element1, ECssPropertyID.FlexGrow);
        sourceProp.Set(CssValue.From(40.0));

        var targetProp = GetProperty(element2, ECssPropertyID.FlexGrow);
        targetProp.Set(CssValue.From(1.0));

        // Act
        targetProp.Overwrite(sourceProp);

        // Assert - Values re-derived
        Assert.Equal(40.0, targetProp.Actual?.AsDecimal());
    }
    #endregion

    #region OverwriteAsync() Tests
    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Overwrite")]
    public async Task OverwriteAsync_BehavesSameAsOverwrite()
    {
        var doc = CreateTestDocument();
        var element1 = CreateTestElement(doc, "div");
        var element2 = CreateTestElement(doc, "span");
        doc.documentElement?.appendChild(element1);
        doc.documentElement?.appendChild(element2);

        var sourceProp = GetProperty(element1, ECssPropertyID.FlexGrow);
        sourceProp.Set(CssValue.From(45.0));

        var targetProp = GetProperty(element2, ECssPropertyID.FlexGrow);
        targetProp.Set(CssValue.From(1.0));

        // Act
        var result = await targetProp.OverwriteAsync(sourceProp);

        // Assert
        Assert.True(result);
        Assert.Equal(45.0, targetProp.Assigned.AsDecimal());
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Overwrite")]
    public async Task OverwriteAsync_ReturnsFalse_WhenSame()
    {
        var doc = CreateTestDocument();
        var element1 = CreateTestElement(doc, "div");
        var element2 = CreateTestElement(doc, "span");
        doc.documentElement?.appendChild(element1);
        doc.documentElement?.appendChild(element2);

        var sameValue = CssValue.From(50.0);

        var sourceProp = GetProperty(element1, ECssPropertyID.FlexGrow);
        sourceProp.Set(sameValue);

        var targetProp = GetProperty(element2, ECssPropertyID.FlexGrow);
        targetProp.Set(sameValue);

        // Act
        var result = await targetProp.OverwriteAsync(sourceProp);

        // Assert
        Assert.False(result);
    }
    #endregion

    #region Edge Cases
    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Cascade")]
    public void Cascade_WithDifferentValueTypes()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        // Test with dimension
        var sourceProp = GetProperty(element, ECssPropertyID.Width);
        sourceProp.Set(CssValue.From_Dimension(100.0, ECssUnit.PX));

        var targetProp = GetCascadedProperty(element, ECssPropertyID.Width);

        // Act
        var result = targetProp.Cascade(sourceProp);

        // Assert
        Assert.True(result);
        Assert.Equal(ECssValueTypes.DIMENSION, targetProp.Assigned.Type);
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Cascade")]
    public void Cascade_WithKeyword()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var sourceProp = GetProperty(element, ECssPropertyID.FlexDirection);
        sourceProp.Set(CssValue.From(EFlexDirection.Column));

        var targetProp = GetCascadedProperty(element, ECssPropertyID.FlexDirection);

        // Act
        var result = targetProp.Cascade(sourceProp);

        // Assert
        Assert.True(result);
        Assert.Equal(ECssValueTypes.KEYWORD, targetProp.Assigned.Type);
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Cascade")]
    public void Cascade_WithAuto_ReturnsTrue_BecauseAutoIsValidCssValue()
    {
        // AUTO is a valid CSS keyword that should cascade.
        // While it has no backing numeric value (HasBackingValue = false),
        // it does have a value in the CSS sense (HasValue = true).
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var sourceProp = GetProperty(element, ECssPropertyID.Width);
        sourceProp.Set(CssValue.Auto);

        // Verify Auto has a CSS value but no backing numeric value
        Assert.True(sourceProp.Assigned.HasValue);
        Assert.False(sourceProp.Assigned.HasBackingValue);

        var targetProp = GetCascadedProperty(element, ECssPropertyID.Width);

        // Act
        var result = targetProp.Cascade(sourceProp);

        // Assert - AUTO cascades because it's a valid CSS value
        Assert.True(result);
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Overwrite")]
    public void Overwrite_BothNull_ReturnsFalse()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var sourceProp = GetProperty(element, ECssPropertyID.FlexGrow);
        // No value set - CssValue.Null

        var targetProp = GetCascadedProperty(element, ECssPropertyID.FlexGrow);
        // No value set - CssValue.Null

        // Act - Both have CssValue.Null
        var result = targetProp.Overwrite(sourceProp);

        // Assert - Same value (both null), no change
        Assert.False(result);
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Cascade")]
    public void Cascade_MultipleTimes_LastWins()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var source1 = GetProperty(element, ECssPropertyID.FlexGrow);
        source1.Set(CssValue.From(1.0));

        var source2 = GetProperty(element, ECssPropertyID.FlexShrink);
        // Use a different property source to simulate cascade from multiple sources
        var targetProp = GetCascadedProperty(element, ECssPropertyID.FlexGrow);

        // Act - Multiple cascades
        targetProp.Cascade(source1);
        Assert.Equal(1.0, targetProp.Assigned.AsDecimal());

        source1.Set(CssValue.From(99.0));
        targetProp.Cascade(source1);

        // Assert - Last cascade wins
        Assert.Equal(99.0, targetProp.Assigned.AsDecimal());
    }
    #endregion
}
