using CssUI.DOM.Nodes;
using System.Collections.Generic;

namespace CssUI.DOM.Events
{
    public abstract class Event
    {
        #region Properties
        public EEventFlags Flags { get; internal set; } = 0x0;
        /// <summary>
        /// Returns the type of event, e.g. "click", "hashchange", or "submit".
        /// </summary>
        public readonly EEventType type = 0x0;// string.Empty;

        /// <summary>
        /// Returns true or false depending on how event was initialized. True if event goes through its target’s ancestors in reverse tree order, and false otherwise.
        /// </summary>
        public readonly bool bubbles;

        /// <summary>
        /// Returns true or false depending on how event was initialized. Its return value does not always carry meaning, but true can indicate that part of the operation during which event was dispatched, can be canceled by invoking the preventDefault() method.
        /// </summary>
        public readonly bool cancelable;

        /// <summary>
        /// Returns the object to which event is dispatched (its target).
        /// </summary>
        public readonly IEventTarget target = null;

        /// <summary>
        /// Returns the object whose event listener’s callback is currently being invoked.
        /// </summary>
        public IEventTarget relatedTarget { get; private set; } = null;

        public IEventTarget currentTarget { get; protected set; } = null;

        /// <summary>
        /// Returns the event’s phase, which is one of NONE, CAPTURING_PHASE, AT_TARGET, and BUBBLING_PHASE.
        /// </summary>
        public EEventPhase eventPhase { get; protected set; } = EEventPhase.NONE;

        /// <summary>
        /// Returns true if event was dispatched by the user agent, and false otherwise.
        /// For the purposes of CssUI this is meaningless but could atleast still differentiate between system events versus custom ones or something
        /// </summary>
        public bool isTrusted { get; internal set; } = false;

        /// <summary>
        /// Returns the event’s timestamp as the number of milliseconds measured relative to the time origin.
        /// </summary>
        public readonly DOMHighResTimeStamp timeStamp;

        public List<EventPathItem> Path { get; private set; } = new List<EventPathItem>();

        public List<EventTarget> TouchTargetList { get; private set; } = new List<EventTarget>();
        #endregion

        #region Accessors
        /// <summary>
        /// Returns true or false depending on how event was initialized. True if event invokes listeners past a ShadowRoot node that is the root of its target, and false otherwise.
        /// </summary>
        public bool composed
        {/* The composed attribute’s getter, when invoked, must return true if the context object’s composed flag is set, and false otherwise. */
            get => 0 != (Flags & EEventFlags.Composed);
        }

        /// <summary>
        /// Returns true if preventDefault() was invoked successfully to indicate cancelation, and false otherwise.
        /// </summary>
        public bool defaultPrevented
        {
            /* The defaultPrevented attribute’s getter, when invoked, must return true if the context object’s canceled flag is set, and false otherwise. */
            get => 0 != (Flags & EEventFlags.Canceled);
        }

        public bool cancelBubble {
            get => 0!=(this.Flags & EEventFlags.StopPropogation);
            set
            {
                if (value) this.Flags |= EEventFlags.StopPropogation;
            }
        }

        public bool canceled
        {
            get => 0!=(Flags & EEventFlags.Canceled);
            set
            {/* To set the canceled flag, given an event event, if event’s cancelable attribute value is true and event’s in passive listener flag is unset, then set event’s canceled flag, and do nothing otherwise. */
                if (value && this.cancelable && 0 == (Flags & EEventFlags.InPassiveListener)) Flags |= EEventFlags.Canceled;
            }
        }

        public bool returnValue
        {
            /* The returnValue attribute’s getter, when invoked, must return false if context object’s canceled flag is set, and true otherwise. */
            get => 0 == (Flags & EEventFlags.Canceled);
            /* The returnValue attribute’s setter, when invoked, must set the canceled flag with the context object if the given value is false, and do nothing otherwise. */
            set
            {
                if (!value) Flags |= EEventFlags.Canceled;
            }
        }
        #endregion


        #region Constructor
        /// <summary>
        /// Returns a new event whose type attribute value is set to type. The eventInitDict argument allows for setting the bubbles and cancelable attributes via object members of the same name.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="eventInitDict">allows for setting the bubbles and cancelable attributes via object members of the same name</param>
        public Event(EEventType type, EventInit eventInitDict = null)
        {
            /* To initialize an event, with type, bubbles, and cancelable, run these steps:*/
            /* 1) Set event’s initialized flag. */
            Flags |= EEventFlags.Initialized;
            /* 2) Unset event’s stop propagation flag, stop immediate propagation flag, and canceled flag. */
            Flags &= ~(EEventFlags.StopPropogation | EEventFlags.StopImmediatePropogation | EEventFlags.Canceled);
            /* 3) Set event’s isTrusted attribute to false. */
            isTrusted = false;// redundant but whatever
            /* 4) Set event’s target to null. */
            target = null;// again, redundant but whatever
            /* 5) Set event’s type attribute to type. */
            this.type = type;
            if (!ReferenceEquals(eventInitDict, null))
            {
                /* 6) Set event’s bubbles attribute to bubbles. */
                this.bubbles = eventInitDict.bubbles;
                /* 7) Set event’s cancelable attribute to cancelable. */
                this.cancelable = eventInitDict.cancelable;
            }

            this.timeStamp = Window.getTimestamp();
        }
        #endregion


        /// <summary>
        /// Returns the invocation target objects of event’s path (objects on which listeners will be invoked), except for any nodes in shadow trees of which the shadow root’s mode is "closed" that are not reachable from event’s currentTarget.
        /// </summary>
        /// <returns></returns>
        public List<IEventTarget> composedPath()
        {/* Docs: https://dom.spec.whatwg.org/#dom-event-composedpath */
            List<IEventTarget> composedPath = new List<IEventTarget>();
            if (this.Path.Count <= 0)
            {
                return composedPath;
            }

            composedPath.Add(this.currentTarget);
            int currentTargetIndex = 0;
            int currentTargetHiddenSubtreeLevel = 0;

            int index = this.Path.Count - 1;
            while(index >= 0)
            {
                EventPathItem pathItem = this.Path[index];
                if (pathItem.root_of_closed_tree) currentTargetHiddenSubtreeLevel++;

                if (ReferenceEquals(pathItem.invocationTarget, this.currentTarget))
                {
                    currentTargetIndex = index;
                    break;
                }

                if (pathItem.slot_in_closed_tree) currentTargetHiddenSubtreeLevel--;

                index--;
            }

            int currentHiddenLevel = currentTargetHiddenSubtreeLevel;
            int maxHiddenLevel = currentTargetHiddenSubtreeLevel;
            /* 11) Set index to currentTargetIndex − 1. */
            index = currentTargetIndex - 1;
            /* 12) While index is greater than or equal to 0: */
            while (index >= 0)
            {
                EventPathItem pathItem = this.Path[index];
                /* 1) If path[index]'s root-of-closed-tree is true, then increase currentHiddenLevel by 1. */
                if (pathItem.root_of_closed_tree) currentHiddenLevel++;
                /* 2) If currentHiddenLevel is less than or equal to maxHiddenLevel, then prepend path[index]'s invocation target to composedPath. */
                if (currentHiddenLevel <= maxHiddenLevel)
                {
                    composedPath.Insert(0, pathItem.invocationTarget);
                }
                if (pathItem.slot_in_closed_tree)
                {
                    currentHiddenLevel--;
                    if (currentHiddenLevel < maxHiddenLevel)
                    {
                        maxHiddenLevel = currentHiddenLevel;
                    }
                }

                index--;
            }

            /* 13) Set currentHiddenLevel and maxHiddenLevel to currentTargetHiddenSubtreeLevel. */
            currentHiddenLevel = maxHiddenLevel = currentTargetHiddenSubtreeLevel;
            /* 14) Set index to currentTargetIndex + 1. */
            index = currentTargetIndex + 1;
            /* 15) While index is less than path’s size: */
            while(index < this.Path.Count)
            {
                /* 1) If path[index]'s slot-in-closed-tree is true, then increase currentHiddenLevel by 1. */
                if (Path[index].slot_in_closed_tree) currentHiddenLevel++;
                /* 2) If currentHiddenLevel is less than or equal to maxHiddenLevel, then append path[index]'s invocation target to composedPath. */
                if (currentHiddenLevel <= maxHiddenLevel) composedPath.Add(Path[index].invocationTarget);
                /* 3) If path[index]'s root-of-closed-tree is true, then: */
                if (Path[index].root_of_closed_tree)
                {
                    /* 1) Decrease currentHiddenLevel by 1. */
                    currentHiddenLevel--;
                    /* 2) If currentHiddenLevel is less than maxHiddenLevel, then set maxHiddenLevel to currentHiddenLevel. */
                    if (currentHiddenLevel < maxHiddenLevel) maxHiddenLevel = currentHiddenLevel;
                }
                /* 4) Increase index by 1. */
                index++;
            }
            /* 16) Return composedPath. */
            return composedPath;

        }

        /// <summary>
        /// If invoked when the cancelable attribute value is true, and while executing a listener for the event with passive set to false, signals to the operation that caused event to be dispatched that it needs to be canceled.
        /// </summary>
        public void preventDefault()
        {/* The preventDefault() method, when invoked, must set the canceled flag with the context object. */
            Flags |= EEventFlags.Canceled;
        }

        /// <summary>
        /// When dispatched in a tree, invoking this method prevents event from reaching any objects other than the current object.
        /// </summary>
        public void stopPropagation()
        {
            this.Flags |= EEventFlags.StopPropogation;
        }

        /// <summary>
        /// Invoking this method prevents event from reaching any registered event listeners after the current one finishes running and, when dispatched in a tree, also prevents event from reaching any other objects.
        /// </summary>
        public void stopImmediatePropagation()
        {
            this.Flags |= EEventFlags.StopPropogation;
            this.Flags |= EEventFlags.StopImmediatePropogation;
        }

        #region Internal Utility

        /// <summary>
        /// Appends a new <see cref="EventPathItem"/> to the given events Path
        /// </summary>
        /// <param name="event"></param>
        /// <param name="invocationTarget"></param>
        /// <param name="shadowAdjustedTarget"></param>
        /// <param name="relatedTarget"></param>
        /// <param name="touchTargets"></param>
        /// <param name="slotInClosedTree"></param>
        internal static void append_to_event_path(Event @event, IEventTarget invocationTarget, IEventTarget shadowAdjustedTarget, IEventTarget relatedTarget, List<IEventTarget> touchTargets, bool slotInClosedTree)
        {/* Docs: https://dom.spec.whatwg.org/#concept-event-path-append */
            bool invocationTargetInShadowTree = false;
            if (invocationTarget is Node invTargNode && invTargNode.getRootNode() is ShadowRoot)
                invocationTargetInShadowTree = true;

            bool rootOfClosedTree = false;
            if (invocationTarget is ShadowRoot invTargShadow && invTargShadow.Mode == Enums.EShadowRootMode.Closed)
                rootOfClosedTree = true;

            var newPath = new EventPathItem() {
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

        internal static void dispatch_event_to_target(Event @event, IEventTarget target)
        {/* Docs: https://dom.spec.whatwg.org/#dispatching-events */
            /* To dispatch an event to a target, with an optional legacy target override flag and an optional legacyOutputDidListenersThrowFlag, run these steps: */
            /* 1) Set event’s dispatch flag. */
            @event.Flags |= EEventFlags.Dispatch;
            /* 2) Let targetOverride be target, if legacy target override flag is not given, and target’s associated Document otherwise. [HTML] */
            IEventTarget targetOverride = target;
            IEventTarget activationTarget = null;
            /* 4) Let relatedTarget be the result of retargeting event’s relatedTarget against target. */
            IEventTarget relatedTarget = DOMCommon.retarget_event(@event.relatedTarget, target);
            /* 5) If target is not relatedTarget or target is event’s relatedTarget, then: */
            if (!ReferenceEquals(target, relatedTarget) || ReferenceEquals(target, @event.relatedTarget))
            {
                var touchTargets = new List<IEventTarget>();
                foreach (var touchTarget in @event.TouchTargetList)
                {
                    touchTargets.Add(DOMCommon.retarget_event(touchTarget, target));
                }
                /* 3) Append to an event path with event, target, targetOverride, relatedTarget, touchTargets, and false. */
                append_to_event_path(@event, target, targetOverride, relatedTarget, touchTargets, false);
                /* 4) Let isActivationEvent be true, if event is a MouseEvent object and event’s type attribute is "click", and false otherwise. */
                bool isActivationEvent = (@event is MouseEvent mouseEvent && mouseEvent.type == EEventType.click);
                /* 5) If isActivationEvent is true and target has activation behavior, then set activationTarget to target. */
                //if (isActivationEvent && target.)/* This is legacy, we will not have activation behaviour */
                /* 6) Let slotable be target, if target is a slotable and is assigned, and null otherwise. */
                var slotable = (target is Node targetNode && targetNode.isSlottable) ? target : null;
                bool slotInClosedTree = false;
                IEventTarget parent = target.get_the_parent(@event);
                while (!ReferenceEquals(null, parent))
                {
                    /* 1) If slotable is non-null: */
                    if (!ReferenceEquals(null, slotable))
                    {
                        /* 1) Assert: parent is a slot. */
                        if (DOMCommon.Is_Slot((Node)parent))
                        {
                            /* 2) Set slotable to null. */
                            slotable = null;
                            /* 3) If parent’s root is a shadow root whose mode is "closed", then set slot-in-closed-tree to true. */
                            if (((Node)parent).getRootNode() is ShadowRoot parentShadow && parentShadow.Mode == Enums.EShadowRootMode.Closed)
                                slotInClosedTree = true;
                        }
                    }
                    /* 2) If parent is a slotable and is assigned, then set slotable to parent. */
                    if (((Node)parent).isSlottable && ((Node)parent).isAssigned)
                        slotable = parent;
                    /* 3) Let relatedTarget be the result of retargeting event’s relatedTarget against parent. */
                    relatedTarget = DOMCommon.retarget_event(@event.relatedTarget, parent);
                    /* 4) Let touchTargets be a new list. */
                    touchTargets = new List<IEventTarget>();
                    /* 5) For each touchTarget of event’s touch target list, append the result of retargeting touchTarget against parent to touchTargets. */
                    foreach (var touchTarget in @event.TouchTargetList)
                    {
                        touchTargets.Add(DOMCommon.retarget_event(touchTarget, parent));
                    }
                    /* 6) If parent is a Window object, or parent is a node and target’s root is a shadow-including inclusive ancestor of parent, then: */
                    if (parent is Window || (parent is Node && DOMCommon.Is_Shadow_Including_Inclusive_Ancestor((target as Node).getRootNode(), parent)))
                    {
                        /* 1) If isActivationEvent is true, event’s bubbles attribute is true, activationTarget is null, and parent has activation behavior, then set activationTarget to parent. */
                        // if (isActivationEvent && @event.bubbles && ReferenceEquals(null, activationTarget))
                        /* 2) Append to an event path with event, parent, null, relatedTarget, touchTargets, and slot-in-closed-tree. */
                        Event.append_to_event_path(@event, parent, null, relatedTarget, touchTargets, slotInClosedTree);
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
                        Event.append_to_event_path(@event, parent, target, relatedTarget, touchTargets, slotInClosedTree);
                    }
                    /* 9) If parent is non-null, then set parent to the result of invoking parent’s get the parent with event. */
                    if (!ReferenceEquals(null, parent))
                        parent = parent.get_the_parent(@event);
                    /* 10) Set slot-in-closed-tree to false. */
                    slotInClosedTree = false;
                }
                /* 10) Let clearTargetsStruct be the last struct in event’s path whose shadow-adjusted target is non-null. */
                var clearTargetsStruct = @event.Path.FindLast(p => !ReferenceEquals(null, p.shadow_adjusted_target));
                /* 11) Let clearTargets be true if clearTargetsStruct’s shadow-adjusted target, clearTargetsStruct’s relatedTarget, or an EventTarget object in clearTargetsStruct’s touch target list is a node and its root is a shadow root, and false otherwise. */
                bool clearTargets = (clearTargetsStruct.shadow_adjusted_target is Node n1 && n1.getRootNode() is ShadowRoot) || (clearTargetsStruct.relatedTarget is Node n2 && n2.getRootNode() is ShadowRoot) || !ReferenceEquals(null, clearTargetsStruct.touch_target_list.Find(t => t is Node n3 && n3.getRootNode() is ShadowRoot));
                /* 12) If activationTarget is non-null and activationTarget has legacy-pre-activation behavior, then run activationTarget’s legacy-pre-activation behavior. */


            }
        }

        #endregion
    }
}
