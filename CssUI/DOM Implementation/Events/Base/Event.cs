using CssUI.DOM.Nodes;
using System.Collections.Generic;

namespace CssUI.DOM.Events
{
    public class Event
    {
        #region Properties
        public EEventFlags Flags { get; internal set; } = 0x0;
        /// <summary>
        /// Returns the type of event, e.g. "click", "hashchange", or "submit".
        /// </summary>
        public readonly EventName type = EventName.Empty;// string.Empty;

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
        public EventTarget target { get; internal set; } = null;

        /// <summary>
        /// Returns the object whose event listener’s callback is currently being invoked.
        /// </summary>
        public EventTarget relatedTarget { get; internal set; } = null;

        public EventTarget currentTarget { get; internal set; } = null;

        /// <summary>
        /// Returns the event’s phase, which is one of NONE, CAPTURING_PHASE, AT_TARGET, and BUBBLING_PHASE.
        /// </summary>
        public EEventPhase eventPhase { get; internal set; } = EEventPhase.NONE;

        /// <summary>
        /// Returns true if event was dispatched by the user agent, and false otherwise.
        /// For the purposes of CssUI this is meaningless but could atleast still differentiate between system events versus custom ones or something
        /// </summary>
        public bool isTrusted { get; internal set; } = false;

        /// <summary>
        /// Returns the event’s timestamp as the number of milliseconds measured relative to the time origin.
        /// </summary>
        public readonly DOMHighResTimeStamp timeStamp;

        public List<EventPathItem> Path { get; internal set; } = new List<EventPathItem>();

        public List<EventTarget> TouchTargetList { get; internal set; } = new List<EventTarget>();
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
        public Event(EventName type, EventInit eventInitDict = null)
        {
            /* To initialize an event, with type, bubbles, and cancelable, run these steps:*/
            /* 1) Set event’s initialized flag. */
            Flags |= EEventFlags.Initialized;
            /* 2) Unset event’s stop propagation flag, stop immediate propagation flag, and canceled flag. */
            Flags &= ~(EEventFlags.StopPropogation | EEventFlags.StopImmediatePropogation | EEventFlags.Canceled);
            /* 3) Set event’s isTrusted attribute to false. */
            isTrusted = true;
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
        public List<EventTarget> composedPath()
        {/* Docs: https://dom.spec.whatwg.org/#dom-event-composedpath */
            var composedPath = new List<EventTarget>();
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
        #endregion
    }
}
