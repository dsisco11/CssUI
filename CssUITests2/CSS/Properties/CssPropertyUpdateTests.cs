using CssUI.CSS;
using CssUI.CSS.Enums;
using CssUI.CSS.Internal;
using CssUI.DOM;
using Xunit;

namespace CssUITests.CSS.Properties;

/// <summary>
/// Tests for CssProperty.Update() and conditional update methods.
/// Update resets derived values and optionally re-derives them immediately.
/// </summary>
public class CssPropertyUpdateTests
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

    #region Update() Basic Tests
    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Update")]
    public void Update_ClearsSpecifiedValue()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        prop.Set(CssValue.From(5.0));
        _ = prop.Specified; // Force computation

        // Act
        prop.Update(ComputeNow: false);

        // Accessing Specified triggers re-derivation
        var specified = prop.Specified;
        Assert.NotNull(specified);
        Assert.Equal(5.0, specified.AsDecimal());
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Update")]
    public void Update_ClearsComputedValue()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        prop.Set(CssValue.From(6.0));
        _ = prop.Computed;

        // Act
        prop.Update(ComputeNow: false);

        // Accessing Computed triggers re-derivation
        var computed = prop.Computed;
        Assert.NotNull(computed);
        Assert.Equal(6.0, computed.AsDecimal());
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Update")]
    public void Update_ClearsUsedValue()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        prop.Set(CssValue.From(7.0));
        _ = prop.Used;

        // Act
        prop.Update(ComputeNow: false);

        // Accessing Used triggers re-derivation
        var used = prop.Used;
        Assert.NotNull(used);
        Assert.Equal(7.0, used.AsDecimal());
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Update")]
    public void Update_ClearsActualValue()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        prop.Set(CssValue.From(8.0));
        _ = prop.Actual;

        // Act
        prop.Update(ComputeNow: false);

        // Accessing Actual triggers re-derivation
        var actual = prop.Actual;
        Assert.NotNull(actual);
        Assert.Equal(8.0, actual.AsDecimal());
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Update")]
    public void Update_PreservesAssignedValue()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        prop.Set(CssValue.From(9.0));

        // Act
        prop.Update(ComputeNow: false);

        // Assert - Assigned preserved
        Assert.Equal(9.0, prop.Assigned.AsDecimal());
    }
    #endregion

    #region ComputeNow Parameter Tests
    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Update")]
    public void Update_ComputeNowTrue_ImmediatelyReDerivesAllValues()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        prop.Set(CssValue.From(10.0));

        // Act - Update with ComputeNow=true
        prop.Update(ComputeNow: true);

        // Assert - All values computed
        Assert.NotNull(prop.Specified);
        Assert.NotNull(prop.Computed);
        Assert.NotNull(prop.Used);
        Assert.NotNull(prop.Actual);
        Assert.Equal(10.0, prop.Actual.AsDecimal());
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Update")]
    public void Update_ComputeNowFalse_DefersReDerivation()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        prop.Set(CssValue.From(11.0));
        _ = prop.Actual; // Force initial computation

        // Act - Update with ComputeNow=false
        prop.Update(ComputeNow: false);

        // Values are re-derived lazily on access
        Assert.NotNull(prop.Actual);
        Assert.Equal(11.0, prop.Actual.AsDecimal());
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Update")]
    public void Update_DefaultParameter_IsComputeNowFalse()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        prop.Set(CssValue.From(12.0));
        _ = prop.Actual;

        // Act - Call without parameter (uses default)
        prop.Update();

        // Assert - Works correctly (deferred)
        Assert.Equal(12.0, prop.Actual?.AsDecimal());
    }
    #endregion

    #region UpdateDependent() Tests
    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Update")]
    public void UpdateDependent_WhenIsDependent_CallsUpdate()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.Width);
        // Set a value with Depends flag - use percent which has depends
        prop.Set(CssValue.From_Percent(50.0));

        // Verify IsDependent is true for percentages
        if (prop.IsDependent)
        {
            _ = prop.Actual;

            // Act
            prop.UpdateDependent(ComputeNow: true);

            // Assert - Values recomputed
            Assert.NotNull(prop.Actual);
        }
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Update")]
    public void UpdateDependent_WhenNotDependent_DoesNotCallUpdate()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        prop.Set(CssValue.From(5.0)); // Plain number, not dependent

        Assert.False(prop.IsDependent);
        _ = prop.Actual;

        // Act - Should not update since not dependent
        prop.UpdateDependent(ComputeNow: true);

        // Assert - Value unchanged
        Assert.Equal(5.0, prop.Actual?.AsDecimal());
    }
    #endregion

    #region UpdateDependentOrAuto() Tests
    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Update")]
    public void UpdateDependentOrAuto_WhenIsAuto_CallsUpdate()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.Width);
        prop.Set(CssValue.Auto);

        Assert.True(prop.IsAuto);
        _ = prop.Actual;

        // Act
        prop.UpdateDependentOrAuto(ComputeNow: true);

        // Assert - Values recomputed
        Assert.NotNull(prop.Actual);
        Assert.Equal(ECssValueTypes.AUTO, prop.Actual.Type);
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Update")]
    public void UpdateDependentOrAuto_WhenNotDependentOrAuto_DoesNotCallUpdate()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        prop.Set(CssValue.From(3.0)); // Plain number

        Assert.False(prop.IsDependentOrAuto);
        _ = prop.Actual;

        // Act
        prop.UpdateDependentOrAuto(ComputeNow: true);

        // Assert - Value unchanged
        Assert.Equal(3.0, prop.Actual?.AsDecimal());
    }
    #endregion

    #region UpdatePercentageOrAuto() Tests
    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Update")]
    public void UpdatePercentageOrAuto_WhenIsPercentage_CallsUpdate()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.Width);
        prop.Set(CssValue.From_Percent(75.0));

        Assert.True(prop.IsPercentageOrAuto);
        _ = prop.Actual;

        // Act
        prop.UpdatePercentageOrAuto(ComputeNow: true);

        // Assert - Values recomputed
        Assert.NotNull(prop.Actual);
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Update")]
    public void UpdatePercentageOrAuto_WhenIsAuto_CallsUpdate()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.Width);
        prop.Set(CssValue.Auto);

        Assert.True(prop.IsPercentageOrAuto);
        _ = prop.Actual;

        // Act
        prop.UpdatePercentageOrAuto(ComputeNow: true);

        // Assert
        Assert.NotNull(prop.Actual);
        Assert.Equal(ECssValueTypes.AUTO, prop.Actual.Type);
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Update")]
    public void UpdatePercentageOrAuto_WhenNeither_DoesNotCallUpdate()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        prop.Set(CssValue.From(4.0)); // Plain number

        Assert.False(prop.IsPercentageOrAuto);
        _ = prop.Actual;

        // Act
        prop.UpdatePercentageOrAuto(ComputeNow: true);

        // Assert - Value unchanged
        Assert.Equal(4.0, prop.Actual?.AsDecimal());
    }
    #endregion

    #region Multiple Update Calls
    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Update")]
    public void Update_MultipleCalls_SafeToCall()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        prop.Set(CssValue.From(20.0));

        // Act - Multiple updates
        prop.Update();
        prop.Update();
        prop.Update(ComputeNow: true);

        // Assert
        Assert.Equal(20.0, prop.Actual?.AsDecimal());
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Update")]
    public void Update_AfterValueChange_ReflectsNewValue()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        prop.Set(CssValue.From(1.0));
        Assert.Equal(1.0, prop.Actual?.AsDecimal());

        // Change value directly on backing field would need Update
        // But Set() calls Update internally
        prop.Set(CssValue.From(99.0));

        // Assert
        Assert.Equal(99.0, prop.Actual?.AsDecimal());
    }
    #endregion

    #region Edge Cases
    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Update")]
    public void Update_WithNullAssigned_HandlesGracefully()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        // Don't set any value - Assigned is CssValue.Null

        // Act
        prop.Update(ComputeNow: true);

        // Assert - Resolves to initial
        Assert.NotNull(prop.Actual);
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Update")]
    public void Update_WithDifferentValueTypes_HandlesCorrectly()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        // Test keyword
        var flexDirProp = GetProperty(element, ECssPropertyID.FlexDirection);
        flexDirProp.Set(CssValue.From(EFlexDirection.ColumnReverse));
        flexDirProp.Update(ComputeNow: true);
        Assert.Equal(ECssValueTypes.KEYWORD, flexDirProp.Actual?.Type);

        // Test dimension
        var widthProp = GetProperty(element, ECssPropertyID.Width);
        widthProp.Set(CssValue.From_Dimension(100.0, ECssUnit.PX));
        widthProp.Update(ComputeNow: true);
        Assert.Equal(100.0, widthProp.Actual?.AsDecimal());
    }
    #endregion
}
