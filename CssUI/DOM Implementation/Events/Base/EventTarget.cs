using CssUI.DOM.Exceptions;
using CssUI.DOM.Internal;
using CssUI.Internal;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace CssUI.DOM.Events
{
    /// <summary>
    /// An EventTarget object represents a target to which an event can be dispatched when something has occurred.
    /// </summary>
    public class EventTarget : IDisposable, IEventTarget
    {
        #region Properties
        public LinkedList<EventListener> Listeners { get; private set; } = new LinkedList<EventListener>();
        /// <summary>
        /// Map of <see cref="EEventName"/>s to a list of their <see cref="EventHandler"/>s
        /// </summary>
        /// Docs: https://html.spec.whatwg.org/multipage/webappapis.html#event-handler-map
        public EventHandlerMap handlerMap { get; private set; }

        internal virtual bool has_activation_behaviour { get => false; }
        internal virtual bool has_legacy_activation_behaviour { get => false; }
        #endregion

        #region Constructor
        public EventTarget()
        {
            handlerMap = new EventHandlerMap(this);
        }
        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    /* Docs: https://html.spec.whatwg.org/multipage/webappapis.html#erase-all-event-listeners-and-handlers */
                    handlerMap.Dispose();
                    foreach (var Listener in Listeners)
                    {
                        Listener.removed = true;
                    }
                    Listeners.Clear();
                }
                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion

        #region Internal Utility
        /// <summary>
        /// Finds a specific listener from our list matching the given values
        /// </summary>
        /// <param name="type"></param>
        /// <param name="callback"></param>
        /// <param name="capture"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal EventListener Find_Listener(EventName type, EventCallback callback, bool capture)
        {
            foreach (var listener in Listeners)
            {
                if (listener.type == type && listener.callback == callback && listener.capture == capture)
                    return listener;
            }

            return null;
        }

        internal virtual void activation_behaviour(Event @event)
        {/* Docs: https://dom.spec.whatwg.org/#eventtarget-activation-behavior */
        }

        internal virtual void legacy_pre_activation_behaviour()
        {/* Docs: https://dom.spec.whatwg.org/#eventtarget-legacy-pre-activation-behavior */
        }

        internal virtual void legacy_canceled_pre_activation_behaviour()
        {/* Docs: https://dom.spec.whatwg.org/#eventtarget-legacy-pre-activation-behavior */
        }

        #endregion

        public virtual EventTarget get_the_parent(Event @event)
        {/* Docs: https://dom.spec.whatwg.org/#get-the-parent */
            return null;
        }

        /// <summary>
        /// Adds an event listener to this object.
        /// </summary>
        /// <param name="eventName">Name of the event to listen for</param>
        /// <param name="callback">Callback that fires when the event is triggered.</param>
        /// <param name="options"></param>
        public void addEventListener(EventName eventName, EventCallback callback, AddEventListenerOptions options = null)
        {/* Docs: https://dom.spec.whatwg.org/#dom-eventtarget-addeventlistener */
            bool capture = false;
            bool once = false;
            bool passive = false;

            if (!ReferenceEquals(options, null))
            {
                capture = options.capture;
                once = options.once;
                passive = options.passive;
            }
            /* 2) Add an event listener with the context object and an event listener whose type is type, callback is callback, capture is capture, passive is passive, and once is once. */
            /* To add an event listener given an EventTarget object eventTarget and an event listener listener, run these steps: */
            /* 1) ... */
            /* 2) If listener’s callback is null, then return. */
            if (ReferenceEquals(callback, null))
                return;

            /* 3) If eventTarget’s event listener list does not contain an event listener whose type is listener’s type, callback is listener’s callback, and capture is listener’s capture, then append listener to eventTarget’s event listener list. */
            var srch = Find_Listener(eventName, callback, capture);
            if (ReferenceEquals(srch, null))
            {
                var newListener = new EventListener(eventName, callback, capture, once, passive);
                Listeners.AddLast(newListener);
            }
        }

        /// <summary>
        /// Removes an event listener from this object.
        /// </summary>
        /// <param name="eventName">Name of the event</param>
        /// <param name="callback">Callback that fires when the event is triggered.</param>
        /// <param name="options"></param>
        public void removeEventListener(EventName eventName, EventCallback callback, EventListenerOptions options = null)
        {
            bool capture = false;
            if (!ReferenceEquals(options, null))
            {
                capture = options.capture;
            }

            /* 3) If the context object’s event listener list contains an event listener whose type is type, callback is callback, and capture is capture, then remove an event listener with the context object and that event listener. */
            var found = Find_Listener(eventName, callback, capture);
            if (!ReferenceEquals(found, null))
            {
                found.removed = true;
                Listeners.Remove(found);
            }
        }

        /// <summary>
        /// Dispatches a synthetic event event to this object and returns true if either event’s cancelable attribute value is false or its preventDefault() method was not invoked, and false otherwise.
        /// </summary>
        /// <param name="Event"></param>
        /// <returns></returns>
        public bool dispatchEvent(Event Event)
        {/* Docs: https://dom.spec.whatwg.org/#dispatching-events */
            /* The dispatchEvent(event) method, when invoked, must run these steps: */
            /* 1) If event’s dispatch flag is set, or if its initialized flag is not set, then throw an "InvalidStateError" DOMException. */
            if (0 != (Event.Flags & EEventFlags.Dispatch) || 0 == (Event.Flags & EEventFlags.Initialized))
            {
                throw new InvalidStateError();
            }

            /* 2) Initialize event’s isTrusted attribute to false. */
            Event.isTrusted = false;

            /* 3) Return the result of dispatching event to the context object. */
            return EventCommon.dispatch_event_to_target(Event, this);
        }
    }
}
