using System;
using System.Collections.Generic;
using CssUI.CSS;
using CssUI.CSS.Enums;
using CssUI.CSS.Internal;
using CssUI.DOM;
using Xunit;

namespace CssUITests.CSS.Properties;

/// <summary>
/// Tests for CssProperty change events - FireValueChangeEvent for different stages.
/// The onValueChange event notifies listeners when property values change.
/// </summary>
public class CssPropertyChangeEventTests
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

    #region Assigned Change Event Tests
    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "ChangeEvent")]
    public void AssignedChange_FiresEvent_WithAssignedStage()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);

        var firedStages = new List<EPropertyStage>();
        prop.onValueChange += (stage, property) => firedStages.Add(stage);

        // Act - Set value triggers Update which fires Assigned change
        prop.Set(CssValue.From(5.0));

        // Assert - Assigned stage event fired
        Assert.Contains(EPropertyStage.Assigned, firedStages);
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "ChangeEvent")]
    public void AssignedChange_EventContainsCorrectProperty()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);

        ICssProperty? receivedProperty = null;
        prop.onValueChange += (stage, property) => receivedProperty = property;

        // Act
        prop.Set(CssValue.From(10.0));

        // Assert - Event contains correct property reference
        Assert.Same(prop, receivedProperty);
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "ChangeEvent")]
    public void AssignedChange_MultipleChanges_FiresMultipleEvents()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);

        var eventCount = 0;
        prop.onValueChange += (stage, property) =>
        {
            if (stage == EPropertyStage.Assigned) eventCount++;
        };

        // Act - Multiple changes
        prop.Set(CssValue.From(1.0));
        prop.Set(CssValue.From(2.0));
        prop.Set(CssValue.From(3.0));

        // Assert - Multiple events fired
        Assert.True(eventCount >= 3);
    }
    #endregion

    #region Computed Change Event Tests
    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "ChangeEvent")]
    public void ComputedChange_FiresEvent_WhenUnitChanges()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.Width);
        prop.Set(CssValue.From_Dimension(100.0, ECssUnit.PX));

        var firedStages = new List<EPropertyStage>();
        prop.onValueChange += (stage, property) => firedStages.Add(stage);

        // Act - Unit scale change fires Computed event
        prop.Handle_Unit_Change(ECssUnit.PX);

        // Assert - Computed stage event fired
        Assert.Contains(EPropertyStage.Computed, firedStages);
    }
    #endregion

    #region Actual Change Event Tests
    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "ChangeEvent")]
    public void ActualChange_FiresEvent_WhenDerived()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);

        var firedStages = new List<EPropertyStage>();
        prop.onValueChange += (stage, property) => firedStages.Add(stage);

        // Act - Set value and access Actual to trigger derivation
        prop.Set(CssValue.From(5.0));
        _ = prop.Actual; // Trigger Actual derivation

        // Assert - Actual stage event fired
        Assert.Contains(EPropertyStage.Actual, firedStages);
    }
    #endregion

    #region Event Handler Management
    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "ChangeEvent")]
    public void Event_CanAddMultipleHandlers()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);

        var handler1Called = false;
        var handler2Called = false;

        prop.onValueChange += (stage, property) => handler1Called = true;
        prop.onValueChange += (stage, property) => handler2Called = true;

        // Act
        prop.Set(CssValue.From(5.0));

        // Assert - Both handlers called
        Assert.True(handler1Called);
        Assert.True(handler2Called);
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "ChangeEvent")]
    public void Event_CanRemoveHandler()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);

        var handlerCalled = false;
        Action<EPropertyStage, ICssProperty> handler = (stage, property) => handlerCalled = true;

        prop.onValueChange += handler;
        prop.onValueChange -= handler;

        // Act
        prop.Set(CssValue.From(5.0));

        // Assert - Handler not called after removal
        Assert.False(handlerCalled);
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "ChangeEvent")]
    public void Event_NoHandlers_NoException()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        // No handlers attached

        // Act & Assert - No exception when firing with no handlers
        prop.Set(CssValue.From(5.0));
        Assert.True(true);
    }
    #endregion

    #region Event Stage Information
    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "ChangeEvent")]
    public void Event_StageParameter_IsCorrect()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);

        var receivedStages = new List<EPropertyStage>();
        prop.onValueChange += (stage, property) => receivedStages.Add(stage);

        // Act
        prop.Set(CssValue.From(5.0));
        _ = prop.Actual; // Trigger Actual derivation

        // Assert - Valid stage values received
        foreach (var stage in receivedStages)
        {
            Assert.True(Enum.IsDefined(typeof(EPropertyStage), stage));
        }
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "ChangeEvent")]
    public void Event_PropertyParameter_IsCorrect()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);

        ICssProperty? receivedProp = null;
        prop.onValueChange += (stage, property) => receivedProp = property;

        // Act
        prop.Set(CssValue.From(5.0));

        // Assert - Correct property received
        Assert.NotNull(receivedProp);
        Assert.Equal(ECssPropertyID.FlexGrow, receivedProp.CssName);
    }
    #endregion

    #region Different Property Types Events
    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "ChangeEvent")]
    public void Event_IntProperty_FiresOnChange()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.Width);

        var eventFired = false;
        prop.onValueChange += (stage, property) => eventFired = true;

        // Act
        prop.Set(CssValue.From_Dimension(200.0, ECssUnit.PX));

        // Assert
        Assert.True(eventFired);
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "ChangeEvent")]
    public void Event_NumberProperty_FiresOnChange()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexShrink);

        var eventFired = false;
        prop.onValueChange += (stage, property) => eventFired = true;

        // Act
        prop.Set(CssValue.From(2.0));

        // Assert
        Assert.True(eventFired);
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "ChangeEvent")]
    public void Event_EnumProperty_FiresOnChange()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexDirection);

        var eventFired = false;
        prop.onValueChange += (stage, property) => eventFired = true;

        // Act
        prop.Set(CssValue.From(EFlexDirection.Column));

        // Assert
        Assert.True(eventFired);
    }
    #endregion

    #region Update Method Event Tests
    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "ChangeEvent")]
    public void Update_FiresAssignedEvent_WhenValueChanged()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        prop.Set(CssValue.From(1.0));

        var assignedEventFired = false;
        prop.onValueChange += (stage, property) =>
        {
            if (stage == EPropertyStage.Assigned) assignedEventFired = true;
        };

        // Act - Change value
        prop.Set(CssValue.From(5.0));

        // Assert
        Assert.True(assignedEventFired);
    }

    [Fact]
    [Trait("Category", "CssProperty")]
    [Trait("Category", "ChangeEvent")]
    public void Update_WithComputeNow_FiresActualEvent()
    {
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        doc.documentElement?.appendChild(element);

        var prop = GetProperty(element, ECssPropertyID.FlexGrow);
        prop.Set(CssValue.From(1.0));

        var actualEventFired = false;
        prop.onValueChange += (stage, property) =>
        {
            if (stage == EPropertyStage.Actual) actualEventFired = true;
        };

        // Act - Update with ComputeNow=true
        prop.Set(CssValue.From(10.0));
        prop.Update(ComputeNow: true);

        // Assert - Actual event fired due to ComputeNow
        Assert.True(actualEventFired);
    }
    #endregion
}
