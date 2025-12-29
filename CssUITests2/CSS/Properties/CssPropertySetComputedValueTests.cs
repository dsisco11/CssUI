using CssUI.CSS;
using CssUI.CSS.Enums;
using CssUI.CSS.Internal;
using CssUI.DOM;
using Xunit;

namespace CssUITests.CSS.Properties;

/// <summary>
/// Tests for CssProperty.Set_Computed_Value() method.
/// This method allows direct manipulation of the Computed value for BoxModel integration.
/// </summary>
public class CssPropertySetComputedValueTests
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

    #region Set_Computed_Value Basic Tests
    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Set_Computed_Value")]
    public void SetComputedValue_UpdatesComputedDirectly()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.Width);

        // Set initial value via normal pipeline
        prop.Set(CssValue.From_Dimension(100.0, ECssUnit.PX));
        Assert.Equal(100.0, prop.Computed?.AsDecimal());

        // Act - Set computed value directly
        prop.Set_Computed_Value(CssValue.From(200.0));

        // Assert - Computed updated directly
        Assert.NotNull(prop.Computed);
        Assert.Equal(200.0, prop.Computed.AsDecimal());
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Set_Computed_Value")]
    public void SetComputedValue_BypassesSpecifiedDerivation()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.Width);

        // Set assigned to percentage
        prop.Set(CssValue.From_Percent(50.0));
        Assert.Equal(ECssValueTypes.PERCENT, prop.Specified?.Type);

        // Act - Set computed directly to absolute value
        prop.Set_Computed_Value(CssValue.From(150.0));

        // Assert - Computed is NUMBER, not derived from Specified
        Assert.NotNull(prop.Computed);
        Assert.Equal(ECssValueTypes.NUMBER, prop.Computed.Type);
        Assert.Equal(150.0, prop.Computed.AsDecimal());
        // Specified unchanged
        Assert.Equal(ECssValueTypes.PERCENT, prop.Specified?.Type);
    }
    #endregion

    #region Used/Actual Re-derivation Tests
    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Set_Computed_Value")]
    public void SetComputedValue_TriggersUsedReDerivation()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.Width);
        prop.Set(CssValue.From_Dimension(100.0, ECssUnit.PX));
        Assert.Equal(100.0, prop.Used?.AsDecimal());

        // Act
        prop.Set_Computed_Value(CssValue.From(300.0));

        // Assert - Used re-derived from new Computed
        Assert.NotNull(prop.Used);
        Assert.Equal(300.0, prop.Used.AsDecimal());
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Set_Computed_Value")]
    public void SetComputedValue_TriggersActualReDerivation()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.Width);
        prop.Set(CssValue.From_Dimension(100.0, ECssUnit.PX));
        Assert.Equal(100.0, prop.Actual?.AsDecimal());

        // Act
        prop.Set_Computed_Value(CssValue.From(400.0));

        // Assert - Actual re-derived from new Used (which came from new Computed)
        Assert.NotNull(prop.Actual);
        Assert.Equal(400.0, prop.Actual.AsDecimal());
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Set_Computed_Value")]
    public void SetComputedValue_FullDerivationChain()
    {
        // Test that Set_Computed_Value triggers: Computed → Used → Actual
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        prop.Set(CssValue.From(1.0));

        // Act
        prop.Set_Computed_Value(CssValue.From(5.0));

        // Assert - All downstream values updated
        Assert.Equal(5.0, prop.Computed?.AsDecimal());
        Assert.Equal(5.0, prop.Used?.AsDecimal());
        Assert.Equal(5.0, prop.Actual?.AsDecimal());
    }
    #endregion

    #region Numeric Value Tests
    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Set_Computed_Value")]
    public void SetComputedValue_WithNumericValue()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);

        // Act
        prop.Set_Computed_Value(CssValue.From(7.5));

        // Assert
        Assert.NotNull(prop.Computed);
        Assert.Equal(ECssValueTypes.NUMBER, prop.Computed.Type);
        Assert.Equal(7.5, prop.Computed.AsDecimal());
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Set_Computed_Value")]
    public void SetComputedValue_WithZero()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        prop.Set(CssValue.From(5.0)); // Non-zero first

        // Act
        prop.Set_Computed_Value(CssValue.From(0.0));

        // Assert
        Assert.NotNull(prop.Computed);
        Assert.Equal(0.0, prop.Computed.AsDecimal());
        Assert.Equal(0.0, prop.Actual?.AsDecimal());
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Set_Computed_Value")]
    public void SetComputedValue_WithNegativeValue()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);

        // Act - Some properties might accept negative values
        prop.Set_Computed_Value(CssValue.From(-10.0));

        // Assert
        Assert.NotNull(prop.Computed);
        Assert.Equal(-10.0, prop.Computed.AsDecimal());
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Set_Computed_Value")]
    public void SetComputedValue_WithLargeValue()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.Width);

        // Act
        prop.Set_Computed_Value(CssValue.From(10000.0));

        // Assert
        Assert.NotNull(prop.Computed);
        Assert.Equal(10000.0, prop.Computed.AsDecimal());
    }
    #endregion

    #region Auto Value Tests (Edge Case)
    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Set_Computed_Value")]
    public void SetComputedValue_WithAuto()
    {
        // Auto is typically resolved during layout, but can be set directly
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.Width);
        prop.Set(CssValue.From_Dimension(100.0, ECssUnit.PX)); // Dimension first

        // Act - Set to Auto
        prop.Set_Computed_Value(CssValue.Auto);

        // Assert
        Assert.NotNull(prop.Computed);
        Assert.Equal(ECssValueTypes.AUTO, prop.Computed.Type);
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Set_Computed_Value")]
    public void SetComputedValue_FromAutoToNumeric()
    {
        // Common scenario: Auto resolved to concrete value during layout
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.Width);
        prop.Set(CssValue.Auto); // Start with Auto

        // Act - BoxModel resolves Auto to concrete value
        prop.Set_Computed_Value(CssValue.From(250.0));

        // Assert - Computed is now numeric
        Assert.NotNull(prop.Computed);
        Assert.Equal(ECssValueTypes.NUMBER, prop.Computed.Type);
        Assert.Equal(250.0, prop.Computed.AsDecimal());
        // Assigned still Auto
        Assert.Equal(ECssValueTypes.AUTO, prop.Assigned.Type);
    }
    #endregion

    #region Multiple Calls Tests
    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Set_Computed_Value")]
    public void SetComputedValue_MultipleCalls_EachUpdates()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.Width);

        // Multiple Set_Computed_Value calls
        prop.Set_Computed_Value(CssValue.From(100.0));
        Assert.Equal(100.0, prop.Computed?.AsDecimal());

        prop.Set_Computed_Value(CssValue.From(200.0));
        Assert.Equal(200.0, prop.Computed?.AsDecimal());

        prop.Set_Computed_Value(CssValue.From(300.0));
        Assert.Equal(300.0, prop.Computed?.AsDecimal());
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Set_Computed_Value")]
    public void SetComputedValue_ThenSet_AssignedOverrides()
    {
        // Setting Assigned after Set_Computed_Value should re-derive from Assigned
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.Width);

        // Set computed directly
        prop.Set_Computed_Value(CssValue.From(500.0));
        Assert.Equal(500.0, prop.Computed?.AsDecimal());

        // Now set Assigned - should trigger full re-derivation
        prop.Set(CssValue.From_Dimension(100.0, ECssUnit.PX));

        // Assert - Computed re-derived from Assigned
        Assert.Equal(100.0, prop.Computed?.AsDecimal());
    }
    #endregion

    #region Different Value Types
    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Set_Computed_Value")]
    public void SetComputedValue_WithKeyword()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexDirection);

        // Act
        prop.Set_Computed_Value(CssValue.From(EFlexDirection.ColumnReverse));

        // Assert
        Assert.NotNull(prop.Computed);
        Assert.Equal(ECssValueTypes.KEYWORD, prop.Computed.Type);
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Set_Computed_Value")]
    public void SetComputedValue_WithNone()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexDirection);

        // Act
        prop.Set_Computed_Value(CssValue.None);

        // Assert
        Assert.NotNull(prop.Computed);
        Assert.Equal(ECssValueTypes.NONE, prop.Computed.Type);
    }
    #endregion

    #region Integration with Revert
    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Set_Computed_Value")]
    public void SetComputedValue_CallsRevertInternally()
    {
        // Set_Computed_Value calls Revert(true) to clear Used/Actual before setting
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.Width);
        prop.Set(CssValue.From_Dimension(100.0, ECssUnit.PX));

        // Get initial Used/Actual
        var initialUsed = prop.Used;
        var initialActual = prop.Actual;
        Assert.NotNull(initialUsed);
        Assert.NotNull(initialActual);

        // Act
        prop.Set_Computed_Value(CssValue.From(999.0));

        // Assert - Used/Actual re-derived (not the same values)
        Assert.NotEqual(initialUsed.AsDecimal(), prop.Used?.AsDecimal());
        Assert.NotEqual(initialActual.AsDecimal(), prop.Actual?.AsDecimal());
        Assert.Equal(999.0, prop.Used?.AsDecimal());
        Assert.Equal(999.0, prop.Actual?.AsDecimal());
    }
    #endregion
}
