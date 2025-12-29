using CssUI.CSS;
using CssUI.CSS.Enums;
using CssUI.CSS.Internal;
using CssUI.DOM;
using Xunit;

namespace CssUITests.CSS.Properties;

/// <summary>
/// Tests for CssProperty.Revert() method.
/// Revert causes the property to re-interpret Used and Actual values from Computed.
/// </summary>
public class CssPropertyRevertTests
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

    #region Revert Clears Values Tests
    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Revert")]
    public void Revert_ClearsUsedValue()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        prop.Set(CssValue.From(5.0));

        // Force Used to be computed
        var usedBefore = prop.Used;
        Assert.NotNull(usedBefore);

        // Act
        prop.Revert();

        // Assert - Used is null after Revert (will be re-derived on access)
        // Access Used to trigger re-derivation
        var usedAfter = prop.Used;
        Assert.NotNull(usedAfter);
        Assert.Equal(5.0, usedAfter.AsDecimal()); // Same value after re-derivation
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Revert")]
    public void Revert_ClearsActualValue()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        prop.Set(CssValue.From(7.0));

        // Force Actual to be computed
        var actualBefore = prop.Actual;
        Assert.NotNull(actualBefore);

        // Act
        prop.Revert();

        // Assert - Actual re-derives correctly
        var actualAfter = prop.Actual;
        Assert.NotNull(actualAfter);
        Assert.Equal(7.0, actualAfter.AsDecimal());
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Revert")]
    public void Revert_PreservesComputedValue()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        prop.Set(CssValue.From(3.0));

        var computedBefore = prop.Computed;
        Assert.NotNull(computedBefore);

        // Act
        prop.Revert();

        // Assert - Computed is preserved (only Used/Actual cleared)
        var computedAfter = prop.Computed;
        Assert.NotNull(computedAfter);
        Assert.Equal(computedBefore.AsDecimal(), computedAfter.AsDecimal());
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Revert")]
    public void Revert_PreservesSpecifiedValue()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        prop.Set(CssValue.From(4.0));

        var specifiedBefore = prop.Specified;
        Assert.NotNull(specifiedBefore);

        // Act
        prop.Revert();

        // Assert - Specified is preserved
        var specifiedAfter = prop.Specified;
        Assert.NotNull(specifiedAfter);
        Assert.Equal(specifiedBefore.AsDecimal(), specifiedAfter.AsDecimal());
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Revert")]
    public void Revert_PreservesAssignedValue()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        var assignedValue = CssValue.From(6.0);
        prop.Set(assignedValue);

        // Act
        prop.Revert();

        // Assert - Assigned is preserved
        Assert.Equal(6.0, prop.Assigned.AsDecimal());
    }
    #endregion

    #region Suppress Parameter Tests
    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Revert")]
    public void Revert_SuppressTrue_UpdatesOldUsedTracker()
    {
        // When suppress=true, oldUsed tracker is updated to prevent change events
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        prop.Set(CssValue.From(8.0));
        _ = prop.Actual; // Force computation

        // Act - Revert with suppress=true
        prop.Revert(suppress: true);

        // Assert - No exception, values can still be accessed
        Assert.NotNull(prop.Used);
        Assert.NotNull(prop.Actual);
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Revert")]
    public void Revert_SuppressFalse_DoesNotUpdateOldUsedTracker()
    {
        // When suppress=false (default), change events may fire
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        prop.Set(CssValue.From(9.0));
        _ = prop.Actual; // Force computation

        // Act - Revert with suppress=false (default)
        prop.Revert(suppress: false);

        // Assert - Values re-derive correctly
        Assert.NotNull(prop.Used);
        Assert.NotNull(prop.Actual);
        Assert.Equal(9.0, prop.Actual.AsDecimal());
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Revert")]
    public void Revert_DefaultParameter_IsSuppressFalse()
    {
        // Default parameter is suppress: false
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        prop.Set(CssValue.From(2.0));
        _ = prop.Actual;

        // Act - Call without parameter (uses default)
        prop.Revert();

        // Assert - Works correctly
        Assert.Equal(2.0, prop.Actual?.AsDecimal());
    }
    #endregion

    #region Re-derivation on Access Tests
    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Revert")]
    public void Revert_UsedReDerivedOnNextAccess()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        prop.Set(CssValue.From(10.0));

        // Initial access
        Assert.Equal(10.0, prop.Used?.AsDecimal());

        // Revert
        prop.Revert();

        // Next access triggers re-derivation
        var used = prop.Used;
        Assert.NotNull(used);
        Assert.Equal(10.0, used.AsDecimal());
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Revert")]
    public void Revert_ActualReDerivedOnNextAccess()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        prop.Set(CssValue.From(11.0));

        // Initial access
        Assert.Equal(11.0, prop.Actual?.AsDecimal());

        // Revert
        prop.Revert();

        // Next access triggers re-derivation
        var actual = prop.Actual;
        Assert.NotNull(actual);
        Assert.Equal(11.0, actual.AsDecimal());
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Revert")]
    public void Revert_LazyReDerivation_OnlyWhenAccessed()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        prop.Set(CssValue.From(12.0));
        _ = prop.Actual; // Force initial computation

        // Revert - clears Used/Actual but doesn't re-derive yet
        prop.Revert();

        // Access Actual - this should trigger re-derivation chain
        var actual = prop.Actual;
        Assert.NotNull(actual);
        Assert.Equal(12.0, actual.AsDecimal());
    }
    #endregion

    #region Multiple Revert Calls
    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Revert")]
    public void Revert_MultipleCalls_SafeToCall()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        prop.Set(CssValue.From(15.0));
        _ = prop.Actual;

        // Act - Multiple reverts
        prop.Revert();
        prop.Revert();
        prop.Revert();

        // Assert - Still works
        Assert.Equal(15.0, prop.Actual?.AsDecimal());
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Revert")]
    public void Revert_AfterValueChange_ReDeriveFromNewComputed()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        prop.Set(CssValue.From(1.0));
        Assert.Equal(1.0, prop.Actual?.AsDecimal());

        // Change value
        prop.Set(CssValue.From(20.0));

        // Revert
        prop.Revert();

        // Assert - Re-derives from new computed value
        Assert.Equal(20.0, prop.Actual?.AsDecimal());
    }
    #endregion

    #region Integration Tests
    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Revert")]
    public void Revert_WithSetComputedValue_ReDeriveFromDirectlySetComputed()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        prop.Set(CssValue.From(1.0));

        // Set computed directly
        prop.Set_Computed_Value(CssValue.From(50.0));
        Assert.Equal(50.0, prop.Actual?.AsDecimal());

        // Revert
        prop.Revert();

        // Assert - Re-derives from the directly-set computed value
        Assert.Equal(50.0, prop.Actual?.AsDecimal());
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Revert")]
    public void Revert_DifferentValueTypes_HandledCorrectly()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        // Test with keyword
        var flexDirProp = GetProperty(element, ECssPropertyID.FlexDirection);
        flexDirProp.Set(CssValue.From(EFlexDirection.Column));
        _ = flexDirProp.Actual;
        flexDirProp.Revert();
        Assert.Equal(ECssValueTypes.KEYWORD, flexDirProp.Actual?.Type);

        // Test with Auto
        var widthProp = GetProperty(element, ECssPropertyID.Width);
        widthProp.Set(CssValue.Auto);
        _ = widthProp.Actual;
        widthProp.Revert();
        Assert.Equal(ECssValueTypes.AUTO, widthProp.Actual?.Type);
    }
    #endregion
}
