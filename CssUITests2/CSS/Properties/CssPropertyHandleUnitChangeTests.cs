using CssUI.CSS;
using CssUI.CSS.Enums;
using CssUI.CSS.Internal;
using CssUI.DOM;
using Xunit;

namespace CssUITests.CSS.Properties;

/// <summary>
/// Tests for CssProperty.Handle_Unit_Change() method.
/// Handle_Unit_Change notifies the property that a unit scale has changed.
/// If the property uses that unit, it fires a change event.
/// </summary>
public class CssPropertyHandleUnitChangeTests
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

    #region Unit Matching Tests
    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Handle_Unit_Change")]
    public void HandleUnitChange_MatchingUnit_PropertyUsesUnit()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.Width);
        prop.Set(CssValue.From_Dimension(100.0, ECssUnit.PX));

        // Verify Specified has PX unit
        Assert.NotNull(prop.Specified);
        Assert.Equal(ECssUnit.PX, prop.Specified.Unit);

        // Act - Handle unit change for PX
        // No exception should occur
        prop.Handle_Unit_Change(ECssUnit.PX);

        // Assert - Property still valid
        Assert.NotNull(prop.Specified);
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Handle_Unit_Change")]
    public void HandleUnitChange_DifferentUnit_NoEffect()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.Width);
        prop.Set(CssValue.From_Dimension(100.0, ECssUnit.PX));

        // Act - Handle unit change for EM (different unit)
        prop.Handle_Unit_Change(ECssUnit.EM);

        // Assert - Property unchanged
        Assert.NotNull(prop.Specified);
        Assert.Equal(ECssUnit.PX, prop.Specified.Unit);
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Handle_Unit_Change")]
    public void HandleUnitChange_PropertyWithNoUnit_NoEffect()
    {
        // Arrange - Use a property with number (no unit)
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        prop.Set(CssValue.From(3.0)); // NUMBER has no unit

        // Act - Handle unit change for any unit
        prop.Handle_Unit_Change(ECssUnit.PX);
        prop.Handle_Unit_Change(ECssUnit.EM);

        // Assert - No effect on unitless value
        Assert.NotNull(prop.Specified);
        Assert.Equal(3.0, prop.Specified.AsDecimal());
    }
    #endregion

    #region Different Unit Types
    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Handle_Unit_Change")]
    public void HandleUnitChange_WithPX()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.Width);
        prop.Set(CssValue.From_Dimension(200.0, ECssUnit.PX));

        // Act
        prop.Handle_Unit_Change(ECssUnit.PX);

        // Assert - No exception, property still works
        Assert.NotNull(prop.Computed);
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Handle_Unit_Change")]
    public void HandleUnitChange_WithEM_RelativeUnit()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.Width);
        prop.Set(CssValue.From_Dimension(2.0, ECssUnit.EM));

        Assert.Equal(ECssUnit.EM, prop.Specified?.Unit);

        // Act - EM scale changed
        prop.Handle_Unit_Change(ECssUnit.EM);

        // Assert - Property handles the notification
        Assert.NotNull(prop.Specified);
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Handle_Unit_Change")]
    public void HandleUnitChange_WithREM()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.Width);
        prop.Set(CssValue.From_Dimension(1.5, ECssUnit.REM));

        Assert.Equal(ECssUnit.REM, prop.Specified?.Unit);

        // Act
        prop.Handle_Unit_Change(ECssUnit.REM);

        // Assert
        Assert.NotNull(prop.Specified);
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Handle_Unit_Change")]
    public void HandleUnitChange_WithPercent_NoUnit()
    {
        // Percent values have their own type (PERCENT), not a unit
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.Width);
        prop.Set(CssValue.From_Percent(50.0));

        // Percent values don't have a dimension unit
        Assert.Equal(ECssUnit.None, prop.Specified?.Unit);

        // Act - Unit changes don't affect percent values
        prop.Handle_Unit_Change(ECssUnit.PX);

        // Assert
        Assert.Equal(ECssValueTypes.PERCENT, prop.Specified?.Type);
    }
    #endregion

    #region Edge Cases
    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Handle_Unit_Change")]
    public void HandleUnitChange_NullSpecified_NoException()
    {
        // Property with no value should handle gracefully
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        // Don't set any value

        // Act - Should not throw even if Specified is derived lazily
        prop.Handle_Unit_Change(ECssUnit.PX);

        // Assert - No exception
        Assert.True(true);
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Handle_Unit_Change")]
    public void HandleUnitChange_MultipleUnits_OnlyMatchingAffects()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.Width);
        prop.Set(CssValue.From_Dimension(100.0, ECssUnit.PX));

        // Act - Try various units
        prop.Handle_Unit_Change(ECssUnit.EM);   // No match
        prop.Handle_Unit_Change(ECssUnit.REM);  // No match
        prop.Handle_Unit_Change(ECssUnit.VW);   // No match
        prop.Handle_Unit_Change(ECssUnit.PX);   // Match

        // Assert - Property still valid
        Assert.Equal(ECssUnit.PX, prop.Specified?.Unit);
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Handle_Unit_Change")]
    public void HandleUnitChange_Keyword_NoUnit()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexDirection);
        prop.Set(CssValue.From(EFlexDirection.Column));

        // Keywords don't have units
        Assert.Equal(ECssUnit.None, prop.Specified?.Unit);

        // Act
        prop.Handle_Unit_Change(ECssUnit.PX);

        // Assert - No effect
        Assert.Equal(ECssValueTypes.KEYWORD, prop.Specified?.Type);
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Handle_Unit_Change")]
    public void HandleUnitChange_Auto_NoUnit()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.Width);
        prop.Set(CssValue.Auto);

        // Act
        prop.Handle_Unit_Change(ECssUnit.PX);

        // Assert - No effect on AUTO
        Assert.Equal(ECssValueTypes.AUTO, prop.Specified?.Type);
    }
    #endregion

    #region Multiple Properties
    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Handle_Unit_Change")]
    public void HandleUnitChange_DifferentPropertiesSameUnit()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var widthProp = GetProperty(element, ECssPropertyID.Width);
        var heightProp = GetProperty(element, ECssPropertyID.Height);

        widthProp.Set(CssValue.From_Dimension(100.0, ECssUnit.PX));
        heightProp.Set(CssValue.From_Dimension(200.0, ECssUnit.PX));

        // Act - Both properties use PX
        widthProp.Handle_Unit_Change(ECssUnit.PX);
        heightProp.Handle_Unit_Change(ECssUnit.PX);

        // Assert - Both properties valid
        Assert.Equal(ECssUnit.PX, widthProp.Specified?.Unit);
        Assert.Equal(ECssUnit.PX, heightProp.Specified?.Unit);
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Handle_Unit_Change")]
    public void HandleUnitChange_DifferentPropertiesDifferentUnits()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var widthProp = GetProperty(element, ECssPropertyID.Width);
        var heightProp = GetProperty(element, ECssPropertyID.Height);

        widthProp.Set(CssValue.From_Dimension(100.0, ECssUnit.PX));
        heightProp.Set(CssValue.From_Dimension(50.0, ECssUnit.EM));

        // Act - Unit change for PX only affects width
        widthProp.Handle_Unit_Change(ECssUnit.PX);
        heightProp.Handle_Unit_Change(ECssUnit.PX); // No effect on EM property

        // Assert
        Assert.Equal(ECssUnit.PX, widthProp.Specified?.Unit);
        Assert.Equal(ECssUnit.EM, heightProp.Specified?.Unit);
    }
    #endregion
}
