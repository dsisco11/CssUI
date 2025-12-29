using System;
using CssUI.CSS;
using CssUI.CSS.Enums;
using CssUI.CSS.Internal;
using CssUI.DOM;
using Xunit;

namespace CssUITests.CSS.Properties;

/// <summary>
/// Tests for CssProperty locked behavior - read-only properties that protect against external modification.
/// Properties in Cascaded style are locked; UserRules properties are unlocked.
/// </summary>
public class CssPropertyLockedTests
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

    private static CssProperty GetCascadedProperty(Element element, ECssPropertyID propertyId)
    {
        var prop = element.Style.Cascaded.Get(propertyId) as CssProperty;
        Assert.NotNull(prop);
        return prop;
    }

    private static CssProperty GetUserRulesProperty(Element element, ECssPropertyID propertyId)
    {
        var prop = element.Style.UserRules.Get(propertyId) as CssProperty;
        Assert.NotNull(prop);
        return prop;
    }
    #endregion

    #region Locked Property Basic Tests
    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Locked")]
    public void Locked_CascadedProperty_IsTrue()
    {
        // Cascaded style properties are read-only
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetCascadedProperty(element, ECssPropertyID.Width);

        // Assert - Cascaded properties are locked
        Assert.True(prop.Locked);
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Locked")]
    public void Locked_UserRulesProperty_IsFalse()
    {
        // UserRules style properties are writable
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetUserRulesProperty(element, ECssPropertyID.Width);

        // Assert - UserRules properties are unlocked
        Assert.False(prop.Locked);
    }
    #endregion

    #region Setting Assigned on Locked Property
    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Locked")]
    public void SetAssigned_OnLockedProperty_ThrowsInvalidOperationException()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var lockedProp = GetCascadedProperty(element, ECssPropertyID.Width);
        Assert.True(lockedProp.Locked);

        // Act & Assert - Setting value on locked property throws
        Assert.Throws<InvalidOperationException>(() =>
        {
            lockedProp.Set(CssValue.From_Dimension(100.0, ECssUnit.PX));
        });
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Locked")]
    public void SetAssigned_OnUnlockedProperty_Succeeds()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var unlockedProp = GetUserRulesProperty(element, ECssPropertyID.Width);
        Assert.False(unlockedProp.Locked);

        // Act - Setting value on unlocked property succeeds
        unlockedProp.Set(CssValue.From_Dimension(100.0, ECssUnit.PX));

        // Assert
        Assert.Equal(100.0, unlockedProp.Assigned.AsDecimal());
    }
    #endregion

    #region Cascade Bypasses Lock
    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Locked")]
    public void Cascade_BypassesLock_InternalOperation()
    {
        // Cascade is an internal operation that should work on locked properties
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        // Source property (unlocked)
        var sourceProp = GetUserRulesProperty(element, ECssPropertyID.FlexGrow);
        sourceProp.Set(CssValue.From(5.0));

        // Target property (locked - Cascaded)
        var targetProp = GetCascadedProperty(element, ECssPropertyID.FlexGrow);
        Assert.True(targetProp.Locked);

        // Act - Cascade should work despite lock
        var result = targetProp.Cascade(sourceProp);

        // Assert - Cascade succeeds (internal operation bypasses lock)
        Assert.True(result);
        Assert.Equal(5.0, targetProp.Assigned.AsDecimal());
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Locked")]
    public void Overwrite_OnLockedProperty_BypassesLock()
    {
        // Overwrite is an internal operation similar to Cascade
        var doc = CreateTestDocument();
        var element1 = CreateTestElement(doc, "div");
        var element2 = CreateTestElement(doc, "span");
        doc.documentElement?.appendChild(element1);
        doc.documentElement?.appendChild(element2);

        // Source property (unlocked)
        var sourceProp = GetUserRulesProperty(element1, ECssPropertyID.FlexGrow);
        sourceProp.Set(CssValue.From(10.0));

        // Target property (locked - Cascaded)
        var targetProp = GetCascadedProperty(element2, ECssPropertyID.FlexGrow);
        Assert.True(targetProp.Locked);

        // Act - Overwrite should work despite lock
        var result = targetProp.Overwrite(sourceProp);

        // Assert - Overwrite succeeds
        Assert.True(result);
        Assert.Equal(10.0, targetProp.Assigned.AsDecimal());
    }
    #endregion

    #region Reading Values from Locked Property
    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Locked")]
    public void ReadAssigned_FromLockedProperty_Works()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var lockedProp = GetCascadedProperty(element, ECssPropertyID.FlexGrow);

        // Act - Reading Assigned works
        var assigned = lockedProp.Assigned;

        // Assert - Can read (value may be Null initially)
        Assert.NotNull(assigned);
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Locked")]
    public void ReadSpecified_FromLockedProperty_Works()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var lockedProp = GetCascadedProperty(element, ECssPropertyID.FlexGrow);

        // Act - Reading Specified works (triggers derivation)
        var specified = lockedProp.Specified;

        // Assert - Can read
        Assert.NotNull(specified);
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Locked")]
    public void ReadComputed_FromLockedProperty_Works()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var lockedProp = GetCascadedProperty(element, ECssPropertyID.FlexGrow);

        // Act - Reading Computed works
        var computed = lockedProp.Computed;

        // Assert - Can read
        Assert.NotNull(computed);
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Locked")]
    public void ReadUsed_FromLockedProperty_Works()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var lockedProp = GetCascadedProperty(element, ECssPropertyID.FlexGrow);

        // Act - Reading Used works
        var used = lockedProp.Used;

        // Assert - Can read
        Assert.NotNull(used);
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Locked")]
    public void ReadActual_FromLockedProperty_Works()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var lockedProp = GetCascadedProperty(element, ECssPropertyID.FlexGrow);

        // Act - Reading Actual works
        var actual = lockedProp.Actual;

        // Assert - Can read
        Assert.NotNull(actual);
    }
    #endregion

    #region Locked Property with Cascaded Values
    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Locked")]
    public void LockedProperty_AfterCascade_HasCorrectValues()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        // Set value in UserRules
        var userProp = GetUserRulesProperty(element, ECssPropertyID.FlexGrow);
        userProp.Set(CssValue.From(7.0));

        // Cascade to locked Cascaded property
        var cascadedProp = GetCascadedProperty(element, ECssPropertyID.FlexGrow);
        cascadedProp.Cascade(userProp);

        // Assert - All value stages work
        Assert.Equal(7.0, cascadedProp.Assigned.AsDecimal());
        Assert.Equal(7.0, cascadedProp.Specified?.AsDecimal());
        Assert.Equal(7.0, cascadedProp.Computed?.AsDecimal());
        Assert.Equal(7.0, cascadedProp.Used?.AsDecimal());
        Assert.Equal(7.0, cascadedProp.Actual?.AsDecimal());
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Locked")]
    public void LockedProperty_AccessorsWork()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var lockedProp = GetCascadedProperty(element, ECssPropertyID.Width);

        // Assert - Boolean accessors work on locked property
        var isAuto = lockedProp.IsAuto;
        var isNone = lockedProp.IsNone;
        var isDependent = lockedProp.IsDependent;
        var flags = lockedProp.Flags;
        var hasValue = lockedProp.HasValue;

        // Just verify no exceptions
        Assert.True(true);
    }
    #endregion

    #region Locking Different Property Types
    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Locked")]
    public void LockedProperty_IntProperty_ThrowsOnSet()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var lockedProp = GetCascadedProperty(element, ECssPropertyID.Width);

        // Assert - IntProperty throws on set
        Assert.Throws<InvalidOperationException>(() =>
        {
            lockedProp.Set(CssValue.From_Dimension(200.0, ECssUnit.PX));
        });
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Locked")]
    public void LockedProperty_NumberProperty_ThrowsOnSet()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var lockedProp = GetCascadedProperty(element, ECssPropertyID.FlexGrow);

        // Assert - NumberProperty throws on set
        Assert.Throws<InvalidOperationException>(() =>
        {
            lockedProp.Set(CssValue.From(3.0));
        });
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "Locked")]
    public void LockedProperty_EnumProperty_ThrowsOnSet()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var lockedProp = GetCascadedProperty(element, ECssPropertyID.FlexDirection);

        // Assert - EnumProperty throws on set
        Assert.Throws<InvalidOperationException>(() =>
        {
            lockedProp.Set(CssValue.From(EFlexDirection.Column));
        });
    }
    #endregion
}
