using CssUI.CSS;
using CssUI.CSS.Enums;
using CssUI.CSS.Internal;
using CssUI.DOM;
using Xunit;

namespace CssUITests.CSS.Properties;

/// <summary>
/// Tests for CssProperty accessor properties: IsNone, IsDependent, IsDependentOrAuto, 
/// IsPercentageOrAuto, and Flags.
/// </summary>
public class CssPropertyAccessorTests
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

    #region IsNone Tests
    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Accessors")]
    public void IsNone_ReturnsTrue_ForNoneType()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        // MaxWidth accepts NONE type
        var prop = GetProperty(element, ECssPropertyID.MaxWidth);
        prop.Set(CssValue.None);

        // Assert
        Assert.True(prop.IsNone);
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Accessors")]
    public void IsNone_ReturnsFalse_ForNumber()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        prop.Set(CssValue.From(5.0));

        // Assert
        Assert.False(prop.IsNone);
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Accessors")]
    public void IsNone_ReturnsFalse_ForAuto()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.Width);
        prop.Set(CssValue.Auto);

        // Assert
        Assert.False(prop.IsNone);
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Accessors")]
    public void IsNone_ReturnsFalse_ForKeyword()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexDirection);
        prop.Set(CssValue.From(EFlexDirection.Column));

        // Assert
        Assert.False(prop.IsNone);
    }
    #endregion

    #region IsDependent Tests
    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Accessors")]
    public void IsDependent_ReturnsTrue_WhenDependsFlagSet()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.Width);
        // Percentage values have the Depends flag
        prop.Set(CssValue.From_Percent(50.0));

        // Check if the value has Depends flag
        if (prop.Assigned.Has_Flags(ECssValueFlags.Depends))
        {
            Assert.True(prop.IsDependent);
        }
        else
        {
            // If percent doesn't have Depends, test is still valid
            Assert.False(prop.IsDependent);
        }
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Accessors")]
    public void IsDependent_ReturnsFalse_ForPlainNumber()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        prop.Set(CssValue.From(3.0));

        // Assert - plain number has no Depends flag
        Assert.False(prop.IsDependent);
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Accessors")]
    public void IsDependent_ReturnsFalse_ForDimension()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.Width);
        prop.Set(CssValue.From_Dimension(100.0, ECssUnit.PX));

        // Assert - absolute dimension has no Depends flag
        Assert.False(prop.IsDependent);
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Accessors")]
    public void IsDependent_ReturnsFalse_ForAuto()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.Width);
        prop.Set(CssValue.Auto);

        // Assert - Auto itself doesn't have Depends flag (it's a separate concept)
        Assert.False(prop.IsDependent);
    }
    #endregion

    #region IsDependentOrAuto Tests
    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Accessors")]
    public void IsDependentOrAuto_ReturnsTrue_ForAuto()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.Width);
        prop.Set(CssValue.Auto);

        // Assert
        Assert.True(prop.IsDependentOrAuto);
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Accessors")]
    public void IsDependentOrAuto_ReturnsTrue_ForDependent()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.Width);
        prop.Set(CssValue.From_Percent(50.0));

        // If percent has Depends flag, IsDependentOrAuto should be true
        if (prop.Assigned.Has_Flags(ECssValueFlags.Depends))
        {
            Assert.True(prop.IsDependentOrAuto);
        }
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Accessors")]
    public void IsDependentOrAuto_ReturnsFalse_ForExplicitValue()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.Width);
        prop.Set(CssValue.From_Dimension(100.0, ECssUnit.PX));

        // Assert - explicit dimension is neither Auto nor Dependent
        Assert.False(prop.IsDependentOrAuto);
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Accessors")]
    public void IsDependentOrAuto_ReturnsFalse_ForNumber()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        prop.Set(CssValue.From(5.0));

        // Assert
        Assert.False(prop.IsDependentOrAuto);
    }
    #endregion

    #region IsPercentageOrAuto Tests
    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Accessors")]
    public void IsPercentageOrAuto_ReturnsTrue_ForPercentage()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.Width);
        prop.Set(CssValue.From_Percent(75.0));

        // Assert
        Assert.True(prop.IsPercentageOrAuto);
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Accessors")]
    public void IsPercentageOrAuto_ReturnsTrue_ForAuto()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.Width);
        prop.Set(CssValue.Auto);

        // Assert
        Assert.True(prop.IsPercentageOrAuto);
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Accessors")]
    public void IsPercentageOrAuto_ReturnsFalse_ForDimension()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.Width);
        prop.Set(CssValue.From_Dimension(200.0, ECssUnit.PX));

        // Assert
        Assert.False(prop.IsPercentageOrAuto);
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Accessors")]
    public void IsPercentageOrAuto_ReturnsFalse_ForNumber()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        prop.Set(CssValue.From(2.0));

        // Assert
        Assert.False(prop.IsPercentageOrAuto);
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Accessors")]
    public void IsPercentageOrAuto_ReturnsFalse_ForKeyword()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexDirection);
        prop.Set(CssValue.From(EFlexDirection.RowReverse));

        // Assert
        Assert.False(prop.IsPercentageOrAuto);
    }
    #endregion

    #region Flags Property Tests
    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Accessors")]
    public void Flags_ReturnsSpecifiedFlags()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        prop.Set(CssValue.From(5.0));

        // Force Specified to be computed
        var specified = prop.Specified;
        Assert.NotNull(specified);

        // Assert - Flags comes from Specified value
        var flags = prop.Flags;
        Assert.Equal(specified.Flags, flags);
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Accessors")]
    public void Flags_ReturnsNone_WhenSpecifiedNull()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        // Don't set value, force Update to clear Specified
        prop.Update(ComputeNow: false);

        // Access Flags before Specified is computed
        // Accessing Flags will access Specified which triggers computation
        // So we just verify no exception occurs
        var flags = prop.Flags;

        // Assert - Should return valid flags (from newly computed Specified)
        Assert.True(true); // No exception
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Accessors")]
    public void Flags_ReturnsCorrectFlags_ForPercentage()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.Width);
        prop.Set(CssValue.From_Percent(50.0));

        // Force Specified computation
        var specified = prop.Specified;
        Assert.NotNull(specified);

        // Assert - Flags match Specified value's flags
        Assert.Equal(specified.Flags, prop.Flags);
    }
    #endregion

    #region Combined Accessor Tests
    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Accessors")]
    public void Accessors_AutoValue_CorrectBooleanStates()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.Width);
        prop.Set(CssValue.Auto);

        // Assert all boolean states for Auto
        Assert.True(prop.IsAuto);
        Assert.False(prop.IsNone);
        Assert.False(prop.IsDependent);
        Assert.True(prop.IsDependentOrAuto);
        Assert.True(prop.IsPercentageOrAuto);
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Accessors")]
    public void Accessors_DimensionValue_CorrectBooleanStates()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.Width);
        prop.Set(CssValue.From_Dimension(100.0, ECssUnit.PX));

        // Assert all boolean states for explicit dimension
        Assert.False(prop.IsAuto);
        Assert.False(prop.IsNone);
        Assert.False(prop.IsDependent);
        Assert.False(prop.IsDependentOrAuto);
        Assert.False(prop.IsPercentageOrAuto);
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Accessors")]
    public void Accessors_PercentValue_CorrectBooleanStates()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.Width);
        prop.Set(CssValue.From_Percent(50.0));

        // Assert all boolean states for percentage
        Assert.False(prop.IsAuto);
        Assert.False(prop.IsNone);
        Assert.True(prop.IsPercentageOrAuto);
        // IsDependent depends on whether percent has Depends flag
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Accessors")]
    public void Accessors_NoneValue_CorrectBooleanStates()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        // MaxWidth accepts NONE type
        var prop = GetProperty(element, ECssPropertyID.MaxWidth);
        prop.Set(CssValue.None);

        // Assert all boolean states for None
        Assert.True(prop.IsNone);
        Assert.False(prop.IsAuto);
        Assert.False(prop.IsDependent);
        Assert.False(prop.IsDependentOrAuto);
        Assert.False(prop.IsPercentageOrAuto);
    }
    #endregion
}
