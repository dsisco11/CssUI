using CssUI.DOM.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace CssUI.DOM.Events
{
    /// <summary>
    /// An EventTarget object represents a target to which an event can be dispatched when something has occurred.
    /// </summary>
    public class EventTarget : IEventTarget, IDisposable
    {
        #region Properties
        protected List<IEventListener> Listeners = new List<IEventListener>();
        #endregion

        #region Constructor
        public EventTarget ()
        {
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
        internal IEventListener Find_Listener(string type, IEventListener callback, bool capture)
        {
            foreach (var listener in Listeners)
            {
                if (0==string.Compare(listener.type, type) && listener.callback == callback && listener.capture == capture)
                    return listener;
            }

            return null;
        }

        internal virtual void activation_behaviour()// Legacy
        {/* Docs: https://dom.spec.whatwg.org/#eventtarget-activation-behavior */
        }

        #endregion

        public virtual IEventTarget get_the_parent(Event @event)
        {/* Docs: https://dom.spec.whatwg.org/#get-the-parent */
            return null;
        }

        /// <summary>
        /// Adds an event listener to this object.
        /// </summary>
        /// <param name="eventName">Name of the event to listen for</param>
        /// <param name="callback">Callback that fires when the event is triggered.</param>
        /// <param name="options"></param>
        public void addEventListener(string eventName, IEventListener callback, AddEventListenerOptions options = null)
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

            if (ReferenceEquals(callback, null))
                return;

            /* 2) Add an event listener with the context object and an event listener whose type is type, callback is callback, capture is capture, passive is passive, and once is once. */
            var srch = Find_Listener(eventName, callback, capture);
            if (ReferenceEquals(srch, null))
            {
                Listeners.Add(callback);
            }

            var newListener = new EventListener(eventName, callback, capture, once, passive);
            Listeners.Add(newListener);
        }

        /// <summary>
        /// Removes an event listener from this object.
        /// </summary>
        /// <param name="eventName">Name of the event</param>
        /// <param name="callback">Callback that fires when the event is triggered.</param>
        /// <param name="options"></param>
        public void removeEventListener(string eventName, IEventListener callback, EventListenerOptions options = null)
        {
            bool capture = false;
            if (!ReferenceEquals(options, null))
            {
                capture = options.capture;
            }

            /* 3) If the context object’s event listener list contains an event listener whose type is type, callback is callback, and capture is capture, then remove an event listener with the context object and that event listener. */
            IEventListener found = Find_Listener(eventName, callback, capture);
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
            return Event.dispatch_event_to_target(Event, this);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    foreach(IEventListener o in Listeners)
                    {
                        o.removed = true;
                    }
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
    }
}
