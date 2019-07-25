using CssUI.DOM.Events;
using CssUI.DOM.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CssUI.DOM.Internal
{
    /// <summary>
    /// Provides common functions related to events
    /// </summary>
    public static class EventCommon
    {

        #region Synthetic Events
        internal static bool fire_synthetic_mouse_event(EventName eventName, Node target, bool not_trusted_flag = false)
        {/* Docs: https://html.spec.whatwg.org/multipage/webappapis.html#fire-a-synthetic-mouse-event */

            /* 1) Initialize event's ctrlKey, shiftKey, altKey, and metaKey attributes according to the current state of the key input device, if any (false for any keys that are not available). */
            /* XXX: Implement abstracted method for getting key states, possibly from end-user supplied Document object? */

            MouseEventInit eventInit = new MouseEventInit(0, 0, 0, 0, Events.EMouseButton.Left, EMouseButtonFlags.Left, target)
            {
                bubbles = true,
                cancelable = true,
            };

            MouseEvent @event = EventCommon.create_event<MouseEvent>(eventName, eventInit);
            @event.View = target.ownerDocument.defaultView;
            @event.Flags |= EEventFlags.Composed;
            if (not_trusted_flag)
                @event.isTrusted = false;

            return EventCommon.dispatch_event_to_target(@event, target);
        }
        #endregion

        #region Common Algorithms

        /// <summary>
        /// Retargets <paramref name="A"/> against object <paramref name="B"/>
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        internal static EventTarget retarget_event(EventTarget A, EventTarget B)
        {/* Docs: https://dom.spec.whatwg.org/#retarget */
            while (A != null)
            {
                if (!(A is Node))
                    return (EventTarget)A;
                if (A is Node nA && nA.getRootNode() is ShadowRoot)
                    return (EventTarget)A;
                if (B is Node nB && DOMCommon.Is_Shadow_Including_Inclusive_Ancestor((A as Node).getRootNode(), B as Node))
                    return (EventTarget)A;

                A = ((A as Node).getRootNode() as DocumentFragment).Host;
            }

            return null;
        }

        /// <summary>
        /// Appends a new <see cref="EventPathItem"/> to the given events Path
        /// </summary>
        /// <param name="event"></param>
        /// <param name="invocationTarget"></param>
        /// <param name="shadowAdjustedTarget"></param>
        /// <param name="relatedTarget"></param>
        /// <param name="touchTargets"></param>
        /// <param name="slotInClosedTree"></param>
        public static void append_to_event_path(Event @event, EventTarget invocationTarget, EventTarget shadowAdjustedTarget, EventTarget relatedTarget, List<EventTarget> touchTargets, bool slotInClosedTree)
        {/* Docs: https://dom.spec.whatwg.org/#concept-event-path-append */
            bool invocationTargetInShadowTree = false;
            if (invocationTarget is Node invTargNode && invTargNode.getRootNode() is ShadowRoot)
                invocationTargetInShadowTree = true;

            bool rootOfClosedTree = false;
            if (invocationTarget is ShadowRoot invTargShadow && invTargShadow.Mode == Enums.EShadowRootMode.Closed)
                rootOfClosedTree = true;

            var newPath = new EventPathItem()
            {
                invocationTarget = invocationTarget,
                invocation_target_in_shadow_tree = invocationTargetInShadowTree,
                shadow_adjusted_target = shadowAdjustedTarget,
                relatedTarget = relatedTarget,
                touch_target_list = touchTargets,
                root_of_closed_tree = rootOfClosedTree,
                slot_in_closed_tree = slotInClosedTree
            };
            @event.Path.Add(newPath);
        }

        public static bool dispatch_event_to_target(Event @event, EventTarget target, bool legacyTargetOverrideFlag = false, bool legacyOutputDidListenersThrowFlag = false)
        {/* Docs: https://dom.spec.whatwg.org/#dispatching-events */
            /* To dispatch an event to a target, with an optional legacy target override flag and an optional legacyOutputDidListenersThrowFlag, run these steps: */
            /* 1) Set event’s dispatch flag. */
            @event.Flags |= EEventFlags.Dispatch;
            /* 2) Let targetOverride be target, if legacy target override flag is not given, and target’s associated Document otherwise. [HTML] */
            EventTarget targetOverride = legacyTargetOverrideFlag && target is Node ? (target as Node).ownerDocument : target;
            EventTarget activationTarget = null;
            bool clearTargets = false;
            /* 4) Let relatedTarget be the result of retargeting event’s relatedTarget against target. */
            EventTarget relatedTarget = EventCommon.retarget_event(@event.relatedTarget, target);
            /* 5) If target is not relatedTarget or target is event’s relatedTarget, then: */
            if (!ReferenceEquals(target, relatedTarget) || ReferenceEquals(target, @event.relatedTarget))
            {
                var touchTargets = new List<EventTarget>();
                foreach (var touchTarget in @event.TouchTargetList)
                {
                    touchTargets.Add(EventCommon.retarget_event(touchTarget, target));
                }
                /* 3) Append to an event path with event, target, targetOverride, relatedTarget, touchTargets, and false. */
                append_to_event_path(@event, target, targetOverride, relatedTarget, touchTargets, false);
                /* 4) Let isActivationEvent be true, if event is a MouseEvent object and event’s type attribute is "click", and false otherwise. */
                bool isActivationEvent = (@event is MouseEvent mouseEvent && mouseEvent.type == EEventName.Click);
                /* 5) If isActivationEvent is true and target has activation behavior, then set activationTarget to target. */
                //if (isActivationEvent && target.)/* This is legacy, we will not have activation behaviour */
                /* 6) Let slotable be target, if target is a slotable and is assigned, and null otherwise. */
                var slotable = (target is Node targetNode && targetNode is ISlottable) ? target : null;
                bool slotInClosedTree = false;
                EventTarget parent = target.get_the_parent(@event);
                while (parent != null)
                {
                    /* 1) If slotable is non-null: */
                    if (slotable != null)
                    {
                        /* 1) Assert: parent is a slot. */
                        if (parent is ISlot)
                        {
                            /* 2) Set slotable to null. */
                            slotable = null;
                            /* 3) If parent’s root is a shadow root whose mode is "closed", then set slot-in-closed-tree to true. */
                            if (((Node)parent).getRootNode() is ShadowRoot parentShadow && parentShadow.Mode == Enums.EShadowRootMode.Closed)
                                slotInClosedTree = true;
                        }
                    }
                    /* 2) If parent is a slotable and is assigned, then set slotable to parent. */
                    if (parent is ISlottable && ((ISlottable)parent).isAssigned)
                        slotable = parent;
                    /* 3) Let relatedTarget be the result of retargeting event’s relatedTarget against parent. */
                    relatedTarget = EventCommon.retarget_event(@event.relatedTarget, parent);
                    /* 4) Let touchTargets be a new list. */
                    touchTargets = new List<EventTarget>();
                    /* 5) For each touchTarget of event’s touch target list, append the result of retargeting touchTarget against parent to touchTargets. */
                    foreach (var touchTarget in @event.TouchTargetList)
                    {
                        touchTargets.Add(EventCommon.retarget_event(touchTarget, parent));
                    }
                    /* 6) If parent is a Window object, or parent is a node and target’s root is a shadow-including inclusive ancestor of parent, then: */
                    if (parent is Window || (parent is Node && DOMCommon.Is_Shadow_Including_Inclusive_Ancestor((target as Node).getRootNode(), parent as Node)))
                    {
                        /* 1) If isActivationEvent is true, event’s bubbles attribute is true, activationTarget is null, and parent has activation behavior, then set activationTarget to parent. */
                        // if (isActivationEvent && @event.bubbles && null == activationTarget)
                        /* 2) Append to an event path with event, parent, null, relatedTarget, touchTargets, and slot-in-closed-tree. */
                        EventCommon.append_to_event_path(@event, parent, null, relatedTarget, touchTargets, slotInClosedTree);
                    }
                    /* 7) Otherwise, if parent is relatedTarget, then set parent to null. */
                    else if (ReferenceEquals(parent, relatedTarget))
                        parent = null;
                    /* 8) Otherwise, set target to parent and then: */
                    else
                    {
                        target = parent;
                        /* 1) If isActivationEvent is true, activationTarget is null, and target has activation behavior, then set activationTarget to target. */
                        //if (isActivationEvent && )
                        /* 2) Append to an event path with event, parent, target, relatedTarget, touchTargets, and slot-in-closed-tree. */
                        EventCommon.append_to_event_path(@event, parent, target, relatedTarget, touchTargets, slotInClosedTree);
                    }
                    /* 9) If parent is non-null, then set parent to the result of invoking parent’s get the parent with event. */
                    if (parent != null)
                        parent = parent.get_the_parent(@event);
                    /* 10) Set slot-in-closed-tree to false. */
                    slotInClosedTree = false;
                }
                /* 10) Let clearTargetsStruct be the last struct in event’s path whose shadow-adjusted target is non-null. */
                var clearTargetsStruct = @event.Path.FindLast(p => null != p.shadow_adjusted_target);
                /* 11) Let clearTargets be true if clearTargetsStruct’s shadow-adjusted target, clearTargetsStruct’s relatedTarget, or an EventTarget object in clearTargetsStruct’s touch target list is a node and its root is a shadow root, and false otherwise. */
                clearTargets = (clearTargetsStruct.shadow_adjusted_target is Node n1 && n1.getRootNode() is ShadowRoot) || (clearTargetsStruct.relatedTarget is Node n2 && n2.getRootNode() is ShadowRoot) || (null != clearTargetsStruct.touch_target_list.Find(t => t is Node n3 && n3.getRootNode() is ShadowRoot));
                /* 12) If activationTarget is non-null and activationTarget has legacy-pre-activation behavior, then run activationTarget’s legacy-pre-activation behavior. */

                /* 13) For each struct in event’s path, in reverse order: */
                for (int i = @event.Path.Count - 1; i >= 0; i--)
                {
                    EventPathItem @struct = @event.Path[i];
                    /* 1) If struct’s shadow-adjusted target is non-null, then set event’s eventPhase attribute to AT_TARGET. */
                    if (@struct.shadow_adjusted_target != null)
                        @event.eventPhase = EEventPhase.AT_TARGET;
                    /* 2) Otherwise, set event’s eventPhase attribute to CAPTURING_PHASE. */
                    else
                        @event.eventPhase = EEventPhase.CAPTURING_PHASE;

                    /* 3) Invoke with struct, event, "capturing", and legacyOutputDidListenersThrowFlag if given. */
                    EventCommon.invoke(@struct, @event, EEventPhase.CAPTURING_PHASE, legacyOutputDidListenersThrowFlag);
                }
                /* 14) For each struct in event’s path: */
                foreach (EventPathItem @struct in @event.Path)
                {
                    /* 1) If struct’s shadow-adjusted target is non-null, then set event’s eventPhase attribute to AT_TARGET. */
                    if (@struct != null)
                        @event.eventPhase = EEventPhase.AT_TARGET;
                    /* 2) Otherwise: */
                    else
                    {
                        if (!@event.bubbles) continue;
                        @event.eventPhase = EEventPhase.BUBBLING_PHASE;
                    }

                    EventCommon.invoke(@struct, @event, EEventPhase.BUBBLING_PHASE, legacyOutputDidListenersThrowFlag);
                }
            }
            /* 6) Set event’s eventPhase attribute to NONE. */
            @event.eventPhase = EEventPhase.NONE;
            /* 7) Set event’s currentTarget attribute to null. */
            @event.currentTarget = null;
            /* 8) Set event’s path to the empty list. */
            @event.Path.Clear();
            /* 9) Unset event’s dispatch flag, stop propagation flag, and stop immediate propagation flag. */
            @event.Flags &= ~(EEventFlags.Dispatch | EEventFlags.StopPropogation | EEventFlags.StopImmediatePropogation);
            /* 10) If clearTargets, then: */
            if (clearTargets)
            {
                @event.target = null;
                @event.relatedTarget = null;
                @event.TouchTargetList.Clear();
            }
            /* 11) If activationTarget is non-null, then: */
            if (activationTarget != null)
            {
                /* 1) If event’s canceled flag is unset, then run activationTarget’s activation behavior with event. */
                // if (0 == (@event.Flags & EEventFlags.Canceled)) activationTarget.a
                /* 2) Otherwise, if activationTarget has legacy-canceled-activation behavior, then run activationTarget’s legacy-canceled-activation behavior. */
            }

            /* 12) Return false if event’s canceled flag is set, and true otherwise. */
            return 0 == (@event.Flags & EEventFlags.Canceled);
        }

        public static void invoke(EventPathItem @struct, Event @event, EEventPhase phase, bool legacyOutputDidListenersThrowFlag = false)
        {/* Docs: https://dom.spec.whatwg.org/#concept-event-listener-invoke */
            /* 1) Set event’s target to the shadow-adjusted target of the last struct in event’s path, that is either struct or preceding struct, whose shadow-adjusted target is non-null. */
            if (@struct.shadow_adjusted_target != null)
                @event.target = @struct.shadow_adjusted_target;
            else
            {
                /* Find the location of struct in events path */
                int structIndex = @event.Path.IndexOf(@struct);
                /* Find the path-item preceeding struct which has a valid shadow-adjusted target */
                for (int i = structIndex - 1; i >= 0; i--)
                {
                    if (@event.Path[i].shadow_adjusted_target != null)
                    {
                        @event.target = @event.Path[i].shadow_adjusted_target;
                        break;
                    }
                }
            }

            @event.relatedTarget = @struct.relatedTarget;
            @event.TouchTargetList = @struct.touch_target_list;
            /* 4) If event’s stop propagation flag is set, then return. */
            if (0 != (@event.Flags & EEventFlags.StopPropogation))
                return;

            @event.currentTarget = @struct.invocationTarget;
            /* 6) Let listeners be a clone of event’s currentTarget attribute value’s event listener list. */
            var listeners = @event.currentTarget.Listeners.ToArray();
            /* 7) Let found be the result of running inner invoke with event, listeners, phase, and legacyOutputDidListenersThrowFlag if given. */
            bool found = EventCommon.inner_invoke(@event, listeners, phase, legacyOutputDidListenersThrowFlag);
            /* 8) If found is false and event’s isTrusted attribute is true, then: */
            if (!found && @event.isTrusted)
            {
                /* Do nothing, this section of the specifications is just firing the event under it's legacy event name */
            }
        }

        public static bool inner_invoke(Event @event, IEnumerable<EventListener> listeners, EEventPhase phase, bool legacyOutputDidListenersThrowFlag = false)
        {/* Docs: https://dom.spec.whatwg.org/#concept-event-listener-inner-invoke */
            bool found = false;
            /* 2) For each listener in listeners, whose removed is false: */
            foreach (EventListener listener in listeners)
            {
                if (!listener.removed)
                    continue;

                if (@event.type != listener.type)
                    continue;

                found = true;
                if (phase == EEventPhase.CAPTURING_PHASE && !listener.capture)
                    continue;

                if (phase == EEventPhase.BUBBLING_PHASE && listener.capture)
                    continue;

                if (listener.once)
                    @event.currentTarget.Listeners.Remove(listener);

                /* Our implementation isnt JavaScript so we dont do step 8 */

                if (listener.passive)
                    @event.Flags |= EEventFlags.InPassiveListener;

                /* 10) Call a user object’s operation with listener’s callback, "handleEvent", « event », and event’s currentTarget attribute value. If this throws an exception, then: */
                listener?.callback?.Method.Invoke(@event.currentTarget, new object[] { @event });

                /* 11) Unset event’s in passive listener flag. */
                @event.Flags &= ~EEventFlags.InPassiveListener;

                /* 13) If event’s stop immediate propagation flag is set, then return found. */
                if (0 != (@event.Flags & EEventFlags.StopImmediatePropogation))
                    return found;
            }
            /* 3) Return found. */
            return found;
        }
        
        public static EventTarget determine_target(EventTarget eventTarget, EventName Name)
        {/* Docs: https://html.spec.whatwg.org/multipage/webappapis.html#determining-the-target-of-an-event-handler */
         /* 1) If eventTarget is not a body element or a frameset element, then return eventTarget. */
            if ((eventTarget is Element element) && 0 != string.Compare("body", element.tagName) && 0 != string.Compare("frameset", element.tagName))
                return eventTarget;
            
            /* 2) If name is not the name of an attribute member of the WindowEventHandlers interface mixin and the Window-reflecting body element event handler set does not contain name, then return eventTarget. */
            if (!EventCommon.Is_Window_Event(Name.EValue) && !EventCommon.Is_Window_Reflecting_Body_Element_Event(Name.EValue))
                return eventTarget;

            /* 3) If eventTarget's node document is not an active document, then return null. */
            if (!DOMCommon.Is_Active_Document((eventTarget as Node).ownerDocument))
                return null;

            /* 4) Return eventTarget's node document's relevant global object. */
            /* For us this just means the actual document */
            return (eventTarget as Node).ownerDocument;
        }

        public static Ty create_event<Ty>(EventName eventName, EventInit eventInit) where Ty : Event
        {/* Docs: https://dom.spec.whatwg.org/#concept-event-create */
            Ty @event = inner_create_event<Ty>(eventName, Window.getTimestamp(), eventInit);
            @event.isTrusted = true;
            return @event;
        }

        public static Ty inner_create_event<Ty>(EventName eventName, DOMHighResTimeStamp timeStamp, dynamic eventInit) where Ty : Event
        {/* Docs: https://dom.spec.whatwg.org/#inner-event-creation-steps */
            var ctor = typeof(Ty).GetConstructors()[0];
            Ty @event = (Ty)ctor.Invoke(new object[] { eventName, eventInit });
            @event.Flags |= EEventFlags.Initialized;
            @event.timeStamp = timeStamp;
            /* 5) Run the event constructing steps with event. */
            @event.run_constructing_steps();
            return @event;
        }
        #endregion

        #region Classification
        /// <summary>
        /// Returns true if the given event-name is 
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public static bool Is_Window_Event(EEventName Name)
        {/* Docs: https://html.spec.whatwg.org/multipage/webappapis.html#windoweventhandlers */
            switch (Name)
            {
                case EEventName.AfterPrint:
                case EEventName.BeforePrint:
                case EEventName.BeforeUnload:
                case EEventName.HashChange:
                case EEventName.LanguageChange:
                case EEventName.Message:
                case EEventName.MessageError:
                case EEventName.Offline:
                case EEventName.Online:
                case EEventName.PageHide:
                case EEventName.PageShow:
                case EEventName.PopState:
                case EEventName.RejectionHandled:
                case EEventName.Storage:
                case EEventName.UnhandledRejection:
                case EEventName.Unload:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns true if the given event-name is a window-reflecting body element event name as defined by the specifications
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public static bool Is_Window_Reflecting_Body_Element_Event(EEventName Name)
        {/* Docs: https://html.spec.whatwg.org/multipage/webappapis.html#window-reflecting-body-element-event-handler-set */
            switch (Name)
            {
                case EEventName.Blur:
                case EEventName.Error:
                case EEventName.Focus:
                case EEventName.Load:
                case EEventName.Resize:
                case EEventName.Scroll:
                    return true;
                default:
                    return false;
            }
        }
        #endregion
    }
}
