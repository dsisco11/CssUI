using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CssUI.CSS;
using CssUI.CSS.Enums;
using CssUI.CSS.Internal;
using CssUI.DOM;
using CssUI.DOM.Nodes;
using Xunit;

namespace CssUITests.CSS.Properties;

/// <summary>
/// Tests for verifying that CssComputedStyle events fire correctly at all stages.
/// These tests diagnose the event propagation chain:
/// Property.Set() → Property.Update() → FireValueChangeEvent() → CssComputedStyle.Property_Changed
/// </summary>
[Trait("Category", "Events")]
[Trait("Category", "CssComputedStyle")]
public class CssComputedStyleEventTests
{
    #region Helper Methods

    private static Document CreateTestDocument()
    {
        var dom = new DOMImplementation();
        return dom.createDocument("CssUI", "cssui");
    }

    private static Element CreateTestElement(Document doc, string tagName = "div")
    {
        var element = doc.createElement(tagName, new ElementCreationOptions(string.Empty));
        doc.documentElement?.appendChild(element);
        return element;
    }

    #endregion

    #region Individual Property onValueChange Tests

    /// <summary>
    /// Verifies that setting a property value triggers the onValueChange event on the property itself.
    /// </summary>
    [Fact]
    public void PropertySet_TriggersOnValueChangeEvent()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var computedStyle = new CssComputedStyle("Test", element, ReadOnly: false);
        var widthProperty = computedStyle.Width;

        bool eventFired = false;
        EPropertyStage? firedStage = null;
        ICssProperty? firedProperty = null;

        widthProperty.onValueChange += (stage, prop) =>
        {
            eventFired = true;
            firedStage = stage;
            firedProperty = prop;
        };

        // Act
        widthProperty.Set(100);

        // Assert
        Assert.True(eventFired, "onValueChange event should fire when property value is set");
        Assert.Equal(EPropertyStage.Assigned, firedStage);
        Assert.Same(widthProperty, firedProperty);
    }

    /// <summary>
    /// Verifies that onValueChange does NOT fire when setting the same value.
    /// </summary>
    [Fact]
    public void PropertySet_SameValue_DoesNotTriggerEvent()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var computedStyle = new CssComputedStyle("Test", element, ReadOnly: false);
        var widthProperty = computedStyle.Width;
        widthProperty.Set(100);

        int eventCount = 0;
        widthProperty.onValueChange += (stage, prop) => eventCount++;

        // Act - Set the same value again
        widthProperty.Set(100);

        // Assert
        Assert.Equal(0, eventCount);
    }

    /// <summary>
    /// Verifies that onValueChange fires when changing to a different value.
    /// </summary>
    [Fact]
    public void PropertySet_DifferentValue_TriggersEvent()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var computedStyle = new CssComputedStyle("Test", element, ReadOnly: false);
        var widthProperty = computedStyle.Width;
        widthProperty.Set(100);

        int eventCount = 0;
        widthProperty.onValueChange += (stage, prop) => eventCount++;

        // Act - Set a different value
        widthProperty.Set(200);

        // Assert
        Assert.Equal(1, eventCount);
    }

    #endregion

    #region CssComputedStyle.Property_Changed Event Tests

    /// <summary>
    /// Verifies that CssComputedStyle.Property_Changed event fires when a property is set.
    /// This tests the FirePropertyChanged method invocation from FireValueChangeEvent.
    /// </summary>
    [Fact]
    public void PropertySet_TriggersCssComputedStylePropertyChangedEvent()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var computedStyle = new CssComputedStyle("Test", element, ReadOnly: false);

        bool eventFired = false;
        EPropertyStage? firedStage = null;
        ICssProperty? firedProperty = null;
        EPropertyDirtFlags? firedFlags = null;

        computedStyle.Property_Changed += (stage, prop, flags, stack) =>
        {
            eventFired = true;
            firedStage = stage;
            firedProperty = prop;
            firedFlags = flags;
        };

        // Act
        computedStyle.Width.Set(100);

        // Assert
        Assert.True(eventFired, "CssComputedStyle.Property_Changed should fire when property is set");
        Assert.Equal(EPropertyStage.Assigned, firedStage);
        Assert.Equal(ECssPropertyID.Width, firedProperty?.CssName);
    }

    /// <summary>
    /// Verifies that Property_Changed receives the correct EPropertyDirtFlags from the property definition.
    /// Width property should have Flow flags.
    /// </summary>
    [Fact]
    public void PropertyChanged_ReceivesCorrectDirtFlags()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var computedStyle = new CssComputedStyle("Test", element, ReadOnly: false);

        EPropertyDirtFlags? receivedFlags = null;
        computedStyle.Property_Changed += (stage, prop, flags, stack) =>
        {
            receivedFlags = flags;
        };

        // Act
        computedStyle.Width.Set(100);

        // Assert
        Assert.NotNull(receivedFlags);
        // Width should have Flow flag according to CssDefinitions
        var definition = CssDefinitions.StyleDefinitions[ECssPropertyID.Width];
        Assert.Equal(definition.Flags, receivedFlags);
    }

    /// <summary>
    /// Verifies that Property_Changed receives a valid StackTrace.
    /// </summary>
    [Fact]
    public void PropertyChanged_ReceivesStackTrace()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var computedStyle = new CssComputedStyle("Test", element, ReadOnly: false);

        StackTrace? receivedStack = null;
        computedStyle.Property_Changed += (stage, prop, flags, stack) =>
        {
            receivedStack = stack;
        };

        // Act
        computedStyle.Width.Set(100);

        // Assert
        Assert.NotNull(receivedStack);
    }

    #endregion

    #region Property Source (WeakReference) Tests

    /// <summary>
    /// Verifies that properties have their Source set correctly to the owning CssComputedStyle.
    /// </summary>
    [Fact]
    public void Property_HasCorrectSource()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var computedStyle = new CssComputedStyle("Test", element, ReadOnly: false);

        // Act
        var widthProperty = computedStyle.Width;

        // Assert
        Assert.NotNull(widthProperty.Source);
        Assert.Same(computedStyle, widthProperty.Source);
    }

    /// <summary>
    /// Verifies that all properties in a CssComputedStyle have their Source set.
    /// </summary>
    [Fact]
    public void AllProperties_HaveSourceSet()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var computedStyle = new CssComputedStyle("Test", element, ReadOnly: false);

        // Act & Assert - Test several properties
        Assert.Same(computedStyle, computedStyle.Width.Source);
        Assert.Same(computedStyle, computedStyle.Height.Source);
        Assert.Same(computedStyle, computedStyle.Display.Source);
        Assert.Same(computedStyle, computedStyle.Margin_Top.Source);
        Assert.Same(computedStyle, computedStyle.Padding_Left.Source);
    }

    #endregion

    #region Overwrite Event Tests

    /// <summary>
    /// Verifies that Overwrite() triggers Property_Changed when values differ.
    /// This is required for the cascade to properly propagate change notifications.
    /// </summary>
    [Fact]
    public void Overwrite_TriggersCssComputedStylePropertyChangedEvent()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var sourceStyle = new CssComputedStyle("Source", element, ReadOnly: false);
        var targetStyle = new CssComputedStyle("Target", element, ReadOnly: false);

        sourceStyle.Width.Set(200);
        targetStyle.Width.Set(100);

        bool eventFired = false;
        targetStyle.Property_Changed += (stage, prop, flags, stack) =>
        {
            eventFired = true;
        };

        // Act
        targetStyle.Width.Overwrite(sourceStyle.Width);

        // Assert - Overwrite should fire events when value changes
        Assert.True(eventFired, "Property_Changed should fire when Overwrite changes the value");
    }

    /// <summary>
    /// Verifies that Overwrite() does NOT trigger Property_Changed when values are the same.
    /// </summary>
    [Fact]
    public void Overwrite_SameValue_DoesNotTriggerEvent()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var sourceStyle = new CssComputedStyle("Source", element, ReadOnly: false);
        var targetStyle = new CssComputedStyle("Target", element, ReadOnly: false);

        sourceStyle.Width.Set(100);
        targetStyle.Width.Set(100);

        bool eventFired = false;
        targetStyle.Property_Changed += (stage, prop, flags, stack) =>
        {
            eventFired = true;
        };

        // Act
        targetStyle.Width.Overwrite(sourceStyle.Width);

        // Assert
        Assert.False(eventFired, "Property_Changed should NOT fire when Overwrite doesn't change the value");
    }

    #endregion

    #region Multiple Property Changes Tests

    /// <summary>
    /// Verifies that Property_Changed fires for each property that changes.
    /// </summary>
    [Fact]
    public void MultiplePropertyChanges_FiresSeparateEvents()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var computedStyle = new CssComputedStyle("Test", element, ReadOnly: false);

        var changedProperties = new List<ECssPropertyID>();
        computedStyle.Property_Changed += (stage, prop, flags, stack) =>
        {
            changedProperties.Add(prop.CssName);
        };

        // Act
        computedStyle.Width.Set(100);
        computedStyle.Height.Set(50);
        computedStyle.Margin_Top.Set(10);

        // Assert
        Assert.Equal(3, changedProperties.Count);
        Assert.Contains(ECssPropertyID.Width, changedProperties);
        Assert.Contains(ECssPropertyID.Height, changedProperties);
        Assert.Contains(ECssPropertyID.MarginTop, changedProperties);
    }

    #endregion

    #region FirePropertyChanged Method Tests

    /// <summary>
    /// Verifies that FirePropertyChanged can be called directly and fires the event.
    /// </summary>
    [Fact]
    public void FirePropertyChanged_DirectCall_FiresEvent()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var computedStyle = new CssComputedStyle("Test", element, ReadOnly: false);
        var widthProperty = computedStyle.Width;

        bool eventFired = false;
        computedStyle.Property_Changed += (stage, prop, flags, stack) =>
        {
            eventFired = true;
        };

        // Act - Call FirePropertyChanged directly
        computedStyle.FirePropertyChanged(EPropertyStage.Assigned, widthProperty);

        // Assert
        Assert.True(eventFired, "FirePropertyChanged should invoke the Property_Changed event");
    }

    #endregion

    #region Integration with StyleProperties Tests

    /// <summary>
    /// Verifies that UserRules property changes cascade to fire Cascaded.Property_Changed.
    /// This is essential for NeedsReflow to be set correctly.
    ///
    /// Expected flow:
    /// 1. UserRules.Width.Set(200) fires UserRules.Width.onValueChange
    /// 2. UserRules.Property_Changed fires
    /// 3. StyleProperties calls Cascade() which copies to Cascaded
    /// 4. Cascaded.Property_Changed fires
    /// 5. Element's onProperty_Changed handler sets NeedsReflow
    /// </summary>
    [Fact]
    public void StylePropertyChange_CascadesToCascadedPropertyChange()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = element.Style;

        bool cascadedEventFired = false;
        ECssPropertyID? changedPropertyId = null;

        style.Cascaded.Property_Changed += (stage, prop, flags, stack) =>
        {
            cascadedEventFired = true;
            changedPropertyId = prop.CssName;
        };

        // Act - Change a property in UserRules
        style.UserRules.Width.Set(200);

        // Assert - Cascaded.Property_Changed should fire when UserRules property changes
        Assert.True(cascadedEventFired,
            "Cascaded.Property_Changed should fire when UserRules property changes and cascades");
        Assert.Equal(ECssPropertyID.Width, changedPropertyId);
    }

    /// <summary>
    /// Verifies that UserRules.Property_Changed fires when a property is set.
    /// </summary>
    [Fact]
    public void UserRules_PropertySet_FiresPropertyChangedEvent()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = element.Style;

        bool userRulesEventFired = false;
        style.UserRules.Property_Changed += (stage, prop, flags, stack) =>
        {
            userRulesEventFired = true;
        };

        // Act
        style.UserRules.Width.Set(200);

        // Assert
        Assert.True(userRulesEventFired, "UserRules.Property_Changed should fire when property is set");
    }

    #endregion

    #region Event Chain Diagnostic Tests

    /// <summary>
    /// Verifies the complete event flow from UserRules to Cascaded.
    /// All three events must fire for the layout system to work correctly.
    ///
    /// Expected event chain:
    /// 1. UserRules.Width.onValueChange fires
    /// 2. UserRules.Property_Changed fires
    /// 3. Cascaded.Property_Changed fires (after cascade)
    /// </summary>
    [Fact]
    public void EventChain_UserRulesToCascaded_AllEventsFire()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc);
        var style = element.Style;

        var eventLog = new List<string>();

        // Subscribe to UserRules property's onValueChange
        style.UserRules.Width.onValueChange += (stage, prop) =>
        {
            eventLog.Add($"1. UserRules.Width.onValueChange: stage={stage}");
        };

        // Subscribe to UserRules.Property_Changed
        style.UserRules.Property_Changed += (stage, prop, flags, stack) =>
        {
            eventLog.Add($"2. UserRules.Property_Changed: prop={prop.CssName}, flags={flags}");
        };

        // Subscribe to Cascaded.Property_Changed
        style.Cascaded.Property_Changed += (stage, prop, flags, stack) =>
        {
            eventLog.Add($"3. Cascaded.Property_Changed: prop={prop.CssName}, flags={flags}");
        };

        // Act
        style.UserRules.Width.Set(200);

        // Assert - All three events must fire
        Assert.NotEmpty(eventLog);
        Assert.Contains(eventLog, e => e.StartsWith("1."));
        Assert.Contains(eventLog, e => e.StartsWith("2."));
        Assert.Contains(eventLog, e => e.StartsWith("3."));
    }

    /// <summary>
    /// Verifies that Width property definition has the Flow flag.
    /// Properties affecting layout must have Flow flag for NeedsReflow to be set.
    /// </summary>
    [Fact]
    public void WidthProperty_HasFlowFlag()
    {
        // Arrange & Act
        var definition = CssDefinitions.StyleDefinitions[ECssPropertyID.Width];

        // Assert - Width property must have Flow flag for layout invalidation
        Assert.True(definition.Flags.HasFlag(EPropertyDirtFlags.Flow),
            $"Width property must have Flow flag, actual: {definition.Flags}");
    }

    #endregion
}
