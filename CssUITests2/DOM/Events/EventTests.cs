using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CssUI.DOM;
using CssUI.DOM.Events;
using CssUI.DOM.Exceptions;
using CssUI.DOM.Nodes;
using Xunit;

namespace CssUITests.DOM.Events;

/// <summary>
/// Unit tests for DOM Events (addEventListener, removeEventListener, dispatchEvent).
/// Spec: https://dom.spec.whatwg.org/#dom-eventtarget-addeventlistener
/// </summary>
public class EventTests
{
    #region Test Infrastructure

    private static Document CreateHTMLDocument()
    {
        var dom = new DOMImplementation();
        return dom.createHTMLDocument("Test");
    }

    private static Element CreateElement(Document doc, string tagName)
    {
        return doc.createElement(tagName, new ElementCreationOptions(string.Empty));
    }

    #endregion

    #region addEventListener Tests

    [Fact]
    public void AddEventListener_WithCallback_AddsToListeners()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var div = CreateElement(doc, "div");
        bool called = false;
        EventCallback callback = (e) => called = true;

        // Act
        div.addEventListener(new EventName(EEventName.Click), callback);

        // Assert
        Assert.Single(div.Listeners);
    }

    [Fact]
    public void AddEventListener_NullCallback_DoesNotAdd()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var div = CreateElement(doc, "div");

        // Act
        div.addEventListener(new EventName(EEventName.Click), null!);

        // Assert
        Assert.Empty(div.Listeners);
    }

    [Fact]
    public void AddEventListener_SameCallbackTwice_OnlyAddsOnce()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var div = CreateElement(doc, "div");
        EventCallback callback = (e) => { };

        // Act
        div.addEventListener(new EventName(EEventName.Click), callback);
        div.addEventListener(new EventName(EEventName.Click), callback);

        // Assert - Per spec, duplicate listeners should not be added
        Assert.Single(div.Listeners);
    }

    [Fact]
    public void AddEventListener_DifferentCallbacks_AddsBoth()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var div = CreateElement(doc, "div");
        EventCallback callback1 = (e) => { };
        EventCallback callback2 = (e) => { };

        // Act
        div.addEventListener(new EventName(EEventName.Click), callback1);
        div.addEventListener(new EventName(EEventName.Click), callback2);

        // Assert
        Assert.Equal(2, div.Listeners.Count);
    }

    [Fact]
    public void AddEventListener_DifferentEvents_AddsBoth()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var div = CreateElement(doc, "div");
        EventCallback callback = (e) => { };

        // Act
        div.addEventListener(new EventName(EEventName.Click), callback);
        div.addEventListener(new EventName(EEventName.Focus), callback);

        // Assert
        Assert.Equal(2, div.Listeners.Count);
    }

    [Fact]
    public void AddEventListener_SameCallbackDifferentCapture_AddsBoth()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var div = CreateElement(doc, "div");
        EventCallback callback = (e) => { };

        // Act
        div.addEventListener(new EventName(EEventName.Click), callback, new AddEventListenerOptions(capture: false));
        div.addEventListener(new EventName(EEventName.Click), callback, new AddEventListenerOptions(capture: true));

        // Assert - Different capture values means different listeners
        Assert.Equal(2, div.Listeners.Count);
    }

    [Fact]
    public void AddEventListener_WithOnceOption_SetsOnceFlag()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var div = CreateElement(doc, "div");
        EventCallback callback = (e) => { };

        // Act
        div.addEventListener(new EventName(EEventName.Click), callback, new AddEventListenerOptions(once: true));

        // Assert
        var listener = div.Listeners.First!.Value;
        Assert.True(listener.once);
    }

    [Fact]
    public void AddEventListener_WithPassiveOption_SetsPassiveFlag()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var div = CreateElement(doc, "div");
        EventCallback callback = (e) => { };

        // Act
        div.addEventListener(new EventName(EEventName.Click), callback, new AddEventListenerOptions(passive: true));

        // Assert
        var listener = div.Listeners.First!.Value;
        Assert.True(listener.passive);
    }

    #endregion

    #region removeEventListener Tests

    [Fact]
    public void RemoveEventListener_ExistingListener_RemovesIt()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var div = CreateElement(doc, "div");
        EventCallback callback = (e) => { };
        div.addEventListener(new EventName(EEventName.Click), callback);

        // Act
        div.removeEventListener(new EventName(EEventName.Click), callback);

        // Assert
        Assert.Empty(div.Listeners);
    }

    [Fact]
    public void RemoveEventListener_NonExistentListener_DoesNothing()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var div = CreateElement(doc, "div");
        EventCallback callback1 = (e) => { };
        EventCallback callback2 = (e) => { };
        div.addEventListener(new EventName(EEventName.Click), callback1);

        // Act
        div.removeEventListener(new EventName(EEventName.Click), callback2);

        // Assert
        Assert.Single(div.Listeners);
    }

    [Fact]
    public void RemoveEventListener_WrongEventType_DoesNotRemove()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var div = CreateElement(doc, "div");
        EventCallback callback = (e) => { };
        div.addEventListener(new EventName(EEventName.Click), callback);

        // Act
        div.removeEventListener(new EventName(EEventName.Focus), callback);

        // Assert
        Assert.Single(div.Listeners);
    }

    [Fact]
    public void RemoveEventListener_WrongCapture_DoesNotRemove()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var div = CreateElement(doc, "div");
        EventCallback callback = (e) => { };
        div.addEventListener(new EventName(EEventName.Click), callback, new AddEventListenerOptions(capture: true));

        // Act
        div.removeEventListener(new EventName(EEventName.Click), callback, new EventListenerOptions(capture: false));

        // Assert
        Assert.Single(div.Listeners);
    }

    [Fact]
    public void RemoveEventListener_MatchingCapture_Removes()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var div = CreateElement(doc, "div");
        EventCallback callback = (e) => { };
        div.addEventListener(new EventName(EEventName.Click), callback, new AddEventListenerOptions(capture: true));

        // Act
        div.removeEventListener(new EventName(EEventName.Click), callback, new EventListenerOptions(capture: true));

        // Assert
        Assert.Empty(div.Listeners);
    }

    [Fact]
    public void RemoveEventListener_SetsRemovedFlag()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var div = CreateElement(doc, "div");
        EventCallback callback = (e) => { };
        div.addEventListener(new EventName(EEventName.Click), callback);
        var listener = div.Listeners.First!.Value;

        // Act
        div.removeEventListener(new EventName(EEventName.Click), callback);

        // Assert - The listener should have its removed flag set
        Assert.True(listener.removed);
    }

    #endregion

    #region dispatchEvent Tests

    [Fact]
    public async Task DispatchEvent_CallsListener()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var div = CreateElement(doc, "div");
        bool called = false;
        EventCallback callback = (e) => called = true;
        div.addEventListener(new EventName(EEventName.Click), callback);

        // Act
        var evt = new Event(new EventName(EEventName.Click));
        await div.dispatchEvent(evt);

        // Assert
        Assert.True(called);
    }

    [Fact]
    public async Task DispatchEvent_SetsTargetProperty()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var div = CreateElement(doc, "div");
        Event? receivedEvent = null;
        EventCallback callback = (e) => receivedEvent = e;
        div.addEventListener(new EventName(EEventName.Click), callback);

        // Act
        var evt = new Event(new EventName(EEventName.Click));
        await div.dispatchEvent(evt);

        // Assert
        Assert.NotNull(receivedEvent);
        Assert.Same(div, receivedEvent!.target);
    }

    [Fact]
    public async Task DispatchEvent_SetsCurrentTargetProperty()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var div = CreateElement(doc, "div");
        EventTarget? currentTarget = null;
        EventCallback callback = (e) => currentTarget = e.currentTarget;
        div.addEventListener(new EventName(EEventName.Click), callback);

        // Act
        var evt = new Event(new EventName(EEventName.Click));
        await div.dispatchEvent(evt);

        // Assert
        Assert.Same(div, currentTarget);
    }

    [Fact]
    public async Task DispatchEvent_SetsIsTrustedToFalse()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var div = CreateElement(doc, "div");
        bool? isTrusted = null;
        EventCallback callback = (e) => isTrusted = e.isTrusted;
        div.addEventListener(new EventName(EEventName.Click), callback);

        // Act
        var evt = new Event(new EventName(EEventName.Click));
        await div.dispatchEvent(evt);

        // Assert - Per spec, dispatchEvent sets isTrusted to false
        Assert.False(isTrusted);
    }

    [Fact]
    public async Task DispatchEvent_ReturnsTrue_WhenNotCanceled()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var div = CreateElement(doc, "div");
        div.addEventListener(new EventName(EEventName.Click), (e) => { });

        // Act
        var evt = new Event(new EventName(EEventName.Click), new EventInit { cancelable = true });
        var result = await div.dispatchEvent(evt);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DispatchEvent_ReturnsFalse_WhenCanceled()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var div = CreateElement(doc, "div");
        div.addEventListener(new EventName(EEventName.Click), (e) => e.preventDefault());

        // Act
        var evt = new Event(new EventName(EEventName.Click), new EventInit { cancelable = true });
        var result = await div.dispatchEvent(evt);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void DispatchEvent_ThrowsInvalidStateError_WhenDispatchFlagSet()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var div = CreateElement(doc, "div");

        // Create an event and manually set dispatch flag (simulating mid-dispatch)
        var evt = new Event(new EventName(EEventName.Click));

        // Dispatch once to set flags through internal mechanisms
        // Then try to dispatch again during first dispatch
        Exception? caughtException = null;
        div.addEventListener(new EventName(EEventName.Click), (e) =>
        {
            try
            {
                // Attempt to dispatch the same event again
                _ = div.dispatchEvent(e).AsTask().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                caughtException = ex;
            }
        });

        // Act
        _ = div.dispatchEvent(evt).AsTask().GetAwaiter().GetResult();

        // Assert - Per spec, dispatching an event with dispatch flag set throws InvalidStateError
        Assert.IsType<InvalidStateError>(caughtException);
    }

    [Fact]
    public async Task DispatchEvent_OnceListener_RemovedAfterFirstCall()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var div = CreateElement(doc, "div");
        int callCount = 0;
        EventCallback callback = (e) => callCount++;
        div.addEventListener(new EventName(EEventName.Click), callback, new AddEventListenerOptions(once: true));

        // Act
        var evt1 = new Event(new EventName(EEventName.Click));
        await div.dispatchEvent(evt1);
        var evt2 = new Event(new EventName(EEventName.Click));
        await div.dispatchEvent(evt2);

        // Assert - Listener with once=true should only be called once
        Assert.Equal(1, callCount);
    }

    [Fact]
    public async Task DispatchEvent_MultipleListeners_AllCalled()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var div = CreateElement(doc, "div");
        int callCount = 0;
        div.addEventListener(new EventName(EEventName.Click), (e) => callCount++);
        div.addEventListener(new EventName(EEventName.Click), (e) => callCount++);
        div.addEventListener(new EventName(EEventName.Click), (e) => callCount++);

        // Act
        var evt = new Event(new EventName(EEventName.Click));
        await div.dispatchEvent(evt);

        // Assert
        Assert.Equal(3, callCount);
    }

    #endregion

    #region Event Bubbling Tests

    [Fact]
    public async Task EventBubbling_BubblesUp_WhenBubblesTrue()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var parent = CreateElement(doc, "div");
        var child = CreateElement(doc, "span");
        parent.appendChild(child);
        doc.body!.appendChild(parent);

        var callOrder = new List<string>();
        parent.addEventListener(new EventName(EEventName.Click), (e) => callOrder.Add("parent"));
        child.addEventListener(new EventName(EEventName.Click), (e) => callOrder.Add("child"));

        // Act
        var evt = new Event(new EventName(EEventName.Click), new EventInit { bubbles = true });
        await child.dispatchEvent(evt);

        // Assert - Event should bubble from child to parent
        Assert.Equal(2, callOrder.Count);
        Assert.Equal("child", callOrder[0]);
        Assert.Equal("parent", callOrder[1]);
    }

    [Fact]
    public async Task EventBubbling_DoesNotBubble_WhenBubblesFalse()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var parent = CreateElement(doc, "div");
        var child = CreateElement(doc, "span");
        parent.appendChild(child);
        doc.body!.appendChild(parent);

        bool parentCalled = false;
        bool childCalled = false;
        parent.addEventListener(new EventName(EEventName.Click), (e) => parentCalled = true);
        child.addEventListener(new EventName(EEventName.Click), (e) => childCalled = true);

        // Act
        var evt = new Event(new EventName(EEventName.Click), new EventInit { bubbles = false });
        await child.dispatchEvent(evt);

        // Assert - Non-bubbling event should not reach parent
        Assert.True(childCalled);
        Assert.False(parentCalled);
    }

    [Fact]
    public async Task EventCapturing_CapturesDown_BeforeBubbling()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var parent = CreateElement(doc, "div");
        var child = CreateElement(doc, "span");
        parent.appendChild(child);
        doc.body!.appendChild(parent);

        var callOrder = new List<string>();
        parent.addEventListener(new EventName(EEventName.Click), (e) => callOrder.Add("parent-capture"), new AddEventListenerOptions(capture: true));
        parent.addEventListener(new EventName(EEventName.Click), (e) => callOrder.Add("parent-bubble"));
        child.addEventListener(new EventName(EEventName.Click), (e) => callOrder.Add("child"));

        // Act
        var evt = new Event(new EventName(EEventName.Click), new EventInit { bubbles = true });
        await child.dispatchEvent(evt);

        // Assert - Capture phase runs first, then target, then bubble
        Assert.Equal(3, callOrder.Count);
        Assert.Equal("parent-capture", callOrder[0]);
        Assert.Equal("child", callOrder[1]);
        Assert.Equal("parent-bubble", callOrder[2]);
    }

    [Fact]
    public async Task EventPhase_IsCapturing_DuringCapturePhase()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var parent = CreateElement(doc, "div");
        var child = CreateElement(doc, "span");
        parent.appendChild(child);
        doc.body!.appendChild(parent);

        EEventPhase? capturedPhase = null;
        parent.addEventListener(new EventName(EEventName.Click), (e) => capturedPhase = e.eventPhase, new AddEventListenerOptions(capture: true));

        // Act
        var evt = new Event(new EventName(EEventName.Click), new EventInit { bubbles = true });
        await child.dispatchEvent(evt);

        // Assert
        Assert.Equal(EEventPhase.CAPTURING_PHASE, capturedPhase);
    }

    [Fact]
    public async Task EventPhase_IsAtTarget_OnTargetElement()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var div = CreateElement(doc, "div");
        doc.body!.appendChild(div);

        EEventPhase? phase = null;
        div.addEventListener(new EventName(EEventName.Click), (e) => phase = e.eventPhase);

        // Act
        var evt = new Event(new EventName(EEventName.Click));
        await div.dispatchEvent(evt);

        // Assert
        Assert.Equal(EEventPhase.AT_TARGET, phase);
    }

    [Fact]
    public async Task EventPhase_IsBubbling_DuringBubblePhase()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var parent = CreateElement(doc, "div");
        var child = CreateElement(doc, "span");
        parent.appendChild(child);
        doc.body!.appendChild(parent);

        EEventPhase? bubblePhase = null;
        parent.addEventListener(new EventName(EEventName.Click), (e) => bubblePhase = e.eventPhase);

        // Act
        var evt = new Event(new EventName(EEventName.Click), new EventInit { bubbles = true });
        await child.dispatchEvent(evt);

        // Assert
        Assert.Equal(EEventPhase.BUBBLING_PHASE, bubblePhase);
    }

    #endregion

    #region stopPropagation Tests

    [Fact]
    public async Task StopPropagation_StopsEventFromBubbling()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var parent = CreateElement(doc, "div");
        var child = CreateElement(doc, "span");
        parent.appendChild(child);
        doc.body!.appendChild(parent);

        bool parentCalled = false;
        child.addEventListener(new EventName(EEventName.Click), (e) => e.stopPropagation());
        parent.addEventListener(new EventName(EEventName.Click), (e) => parentCalled = true);

        // Act
        var evt = new Event(new EventName(EEventName.Click), new EventInit { bubbles = true });
        await child.dispatchEvent(evt);

        // Assert
        Assert.False(parentCalled);
    }

    [Fact]
    public async Task StopPropagation_OtherListenersOnSameTargetStillCalled()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var div = CreateElement(doc, "div");
        doc.body!.appendChild(div);

        bool firstCalled = false;
        bool secondCalled = false;
        div.addEventListener(new EventName(EEventName.Click), (e) => { firstCalled = true; e.stopPropagation(); });
        div.addEventListener(new EventName(EEventName.Click), (e) => secondCalled = true);

        // Act
        var evt = new Event(new EventName(EEventName.Click));
        await div.dispatchEvent(evt);

        // Assert - stopPropagation doesn't stop other listeners on same target
        Assert.True(firstCalled);
        Assert.True(secondCalled);
    }

    [Fact]
    public async Task StopImmediatePropagation_StopsAllRemainingListeners()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var div = CreateElement(doc, "div");
        doc.body!.appendChild(div);

        bool firstCalled = false;
        bool secondCalled = false;
        div.addEventListener(new EventName(EEventName.Click), (e) => { firstCalled = true; e.stopImmediatePropagation(); });
        div.addEventListener(new EventName(EEventName.Click), (e) => secondCalled = true);

        // Act
        var evt = new Event(new EventName(EEventName.Click));
        await div.dispatchEvent(evt);

        // Assert - stopImmediatePropagation stops remaining listeners on same target
        Assert.True(firstCalled);
        Assert.False(secondCalled);
    }

    [Fact]
    public async Task StopImmediatePropagation_StopsEventFromBubbling()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var parent = CreateElement(doc, "div");
        var child = CreateElement(doc, "span");
        parent.appendChild(child);
        doc.body!.appendChild(parent);

        bool parentCalled = false;
        child.addEventListener(new EventName(EEventName.Click), (e) => e.stopImmediatePropagation());
        parent.addEventListener(new EventName(EEventName.Click), (e) => parentCalled = true);

        // Act
        var evt = new Event(new EventName(EEventName.Click), new EventInit { bubbles = true });
        await child.dispatchEvent(evt);

        // Assert
        Assert.False(parentCalled);
    }

    [Fact]
    public async Task StopPropagation_DuringCapture_StopsRemainingCapture()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var grandparent = CreateElement(doc, "div");
        var parent = CreateElement(doc, "div");
        var child = CreateElement(doc, "span");
        grandparent.appendChild(parent);
        parent.appendChild(child);
        doc.body!.appendChild(grandparent);

        bool parentCaptureCalled = false;
        grandparent.addEventListener(new EventName(EEventName.Click), (e) => e.stopPropagation(), new AddEventListenerOptions(capture: true));
        parent.addEventListener(new EventName(EEventName.Click), (e) => parentCaptureCalled = true, new AddEventListenerOptions(capture: true));

        // Act
        var evt = new Event(new EventName(EEventName.Click), new EventInit { bubbles = true });
        await child.dispatchEvent(evt);

        // Assert
        Assert.False(parentCaptureCalled);
    }

    #endregion

    #region target vs currentTarget Tests

    [Fact]
    public async Task Target_AlwaysPointsToDispatchedElement()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var parent = CreateElement(doc, "div");
        var child = CreateElement(doc, "span");
        parent.appendChild(child);
        doc.body!.appendChild(parent);

        EventTarget? targetAtParent = null;
        parent.addEventListener(new EventName(EEventName.Click), (e) => targetAtParent = e.target);

        // Act
        var evt = new Event(new EventName(EEventName.Click), new EventInit { bubbles = true });
        await child.dispatchEvent(evt);

        // Assert - target should be child even when event is handled at parent
        Assert.Same(child, targetAtParent);
    }

    [Fact]
    public async Task CurrentTarget_PointsToCurrentHandler()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var parent = CreateElement(doc, "div");
        var child = CreateElement(doc, "span");
        parent.appendChild(child);
        doc.body!.appendChild(parent);

        EventTarget? currentTargetAtParent = null;
        EventTarget? currentTargetAtChild = null;
        parent.addEventListener(new EventName(EEventName.Click), (e) => currentTargetAtParent = e.currentTarget);
        child.addEventListener(new EventName(EEventName.Click), (e) => currentTargetAtChild = e.currentTarget);

        // Act
        var evt = new Event(new EventName(EEventName.Click), new EventInit { bubbles = true });
        await child.dispatchEvent(evt);

        // Assert - currentTarget should be the element where the listener is attached
        Assert.Same(child, currentTargetAtChild);
        Assert.Same(parent, currentTargetAtParent);
    }

    [Fact]
    public async Task CurrentTarget_IsNullAfterDispatch()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var div = CreateElement(doc, "div");
        div.addEventListener(new EventName(EEventName.Click), (e) => { });

        // Act
        var evt = new Event(new EventName(EEventName.Click));
        await div.dispatchEvent(evt);

        // Assert - Per spec, currentTarget is null after dispatch completes
        Assert.Null(evt.currentTarget);
    }

    #endregion

    #region Custom Events Tests

    [Fact]
    public void CustomEvent_CanBeCreatedWithDetail()
    {
        // Arrange & Act
        var customData = new { message = "Hello", count = 42 };
        var evt = new CustomEvent(EEventName.CUSTOM, new CustomEventInit { detail = customData });

        // Assert
        Assert.Equal(customData, evt.detail);
    }

    [Fact]
    public async Task CustomEvent_DetailAccessibleInHandler()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var div = CreateElement(doc, "div");
        dynamic? receivedDetail = null;
        div.addEventListener(new EventName(EEventName.CUSTOM), (e) =>
        {
            if (e is CustomEvent ce)
                receivedDetail = ce.detail;
        });

        // Act
        var customData = "custom data";
        var evt = new CustomEvent(EEventName.CUSTOM, new CustomEventInit { detail = customData });
        await div.dispatchEvent(evt);

        // Assert
        Assert.Equal(customData, receivedDetail);
    }

    [Fact]
    public void CustomEventName_CanUseStringName()
    {
        // Arrange & Act
        var eventName = new EventName("my-custom-event");
        var evt = new Event(eventName);

        // Assert
        Assert.True(eventName.IsCustom);
    }

    [Fact]
    public async Task CustomEvent_BubblesWhenConfigured()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var parent = CreateElement(doc, "div");
        var child = CreateElement(doc, "span");
        parent.appendChild(child);
        doc.body!.appendChild(parent);

        bool parentCalled = false;
        parent.addEventListener(new EventName(EEventName.CUSTOM), (e) => parentCalled = true);

        // Act
        var evt = new CustomEvent(EEventName.CUSTOM, new CustomEventInit { bubbles = true });
        await child.dispatchEvent(evt);

        // Assert
        Assert.True(parentCalled);
    }

    [Fact]
    public async Task CustomEvent_CanBeCanceled()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var div = CreateElement(doc, "div");
        div.addEventListener(new EventName(EEventName.CUSTOM), (e) => e.preventDefault());

        // Act
        var evt = new CustomEvent(EEventName.CUSTOM, new CustomEventInit { cancelable = true });
        var result = await div.dispatchEvent(evt);

        // Assert
        Assert.False(result);
        Assert.True(evt.defaultPrevented);
    }

    #endregion

    #region preventDefault Tests

    [Fact]
    public void PreventDefault_SetsCanceledFlag()
    {
        // Arrange
        var evt = new Event(new EventName(EEventName.Click), new EventInit { cancelable = true });

        // Act
        evt.preventDefault();

        // Assert
        Assert.True(evt.defaultPrevented);
    }

    [Fact]
    public async Task PreventDefault_OnNonCancelable_HasNoEffect()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var div = CreateElement(doc, "div");
        div.addEventListener(new EventName(EEventName.Click), (e) => e.preventDefault());

        // Act
        var evt = new Event(new EventName(EEventName.Click), new EventInit { cancelable = false });
        var result = await div.dispatchEvent(evt);

        // Assert - Non-cancelable events can't be prevented
        Assert.True(result);
    }

    #endregion

    #region Event Properties Tests

    [Fact]
    public void Event_HasTimestamp()
    {
        // Arrange & Act
        var evt = new Event(new EventName(EEventName.Click));

        // Assert - Timestamp should be set
        Assert.NotNull(evt.timeStamp);
    }

    [Fact]
    public void Event_TypeMatchesConstructorArgument()
    {
        // Arrange & Act
        var evt = new Event(new EventName(EEventName.Click));

        // Assert
        Assert.Equal(EEventName.Click, evt.type.EnumValue);
    }

    [Fact]
    public void Event_BubblesDefaultsToFalse()
    {
        // Arrange & Act
        var evt = new Event(new EventName(EEventName.Click));

        // Assert
        Assert.False(evt.bubbles);
    }

    [Fact]
    public void Event_CancelableDefaultsToFalse()
    {
        // Arrange & Act
        var evt = new Event(new EventName(EEventName.Click));

        // Assert
        Assert.False(evt.cancelable);
    }

    [Fact]
    public void Event_ComposedPath_ReturnsEmptyBeforeDispatch()
    {
        // Arrange
        var evt = new Event(new EventName(EEventName.Click));

        // Act
        var path = evt.composedPath();

        // Assert
        Assert.Empty(path);
    }

    #endregion
}
