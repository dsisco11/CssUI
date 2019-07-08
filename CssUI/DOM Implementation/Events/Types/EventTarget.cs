using CssUI.DOM.Exceptions;
using System;
using System.Collections.Generic;

namespace CssUI.DOM
{
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

        #region Helpers
        /// <summary>
        /// Finds a specific listener from our list matching the given values
        /// </summary>
        /// <param name="type"></param>
        /// <param name="callback"></param>
        /// <param name="capture"></param>
        /// <returns></returns>
        protected IEventListener Find_Listener(string type, IEventListener callback, bool capture)
        {
            foreach (var listener in Listeners)
            {
                if (listener.type == type && listener.callback == callback && listener.capture == capture)
                    return listener;
            }

            return null;
        }

        #endregion


        public void addEventListener(string type, IEventListener callback, AddEventListenerOptions options = null)
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
            var srch = Find_Listener(type, callback, capture);
            if (ReferenceEquals(srch, null))
            {
                Listeners.Add(callback);
            }

            var newListener = new EventListener(type, callback, capture, once, passive);
            Listeners.Add(newListener);
        }

        public void removeEventListener(string type, IEventListener callback, EventListenerOptions options = null)
        {
            bool capture = false;
            if (!ReferenceEquals(options, null))
            {
                capture = options.capture;
            }

            /* 3) If the context object’s event listener list contains an event listener whose type is type, callback is callback, and capture is capture, then remove an event listener with the context object and that event listener. */
            IEventListener found = Find_Listener(type, callback, capture);
            if (!ReferenceEquals(found, null))
            {
                found.removed = true;
                Listeners.Remove(found);
            }
        }

        public bool dispatchEvent(Event Event)
        {
            /* The dispatchEvent(event) method, when invoked, must run these steps: */
            /* 1) If event’s dispatch flag is set, or if its initialized flag is not set, then throw an "InvalidStateError" DOMException. */
            if (0 != (Event.Flags & EEventFlags.Dispatch) || 0 == (Event.Flags & EEventFlags.Initialized))
            {
                throw new InvalidStateError();
            }

            /* 2) Initialize event’s isTrusted attribute to false. */
            Event.isTrusted = false;

            /* 3) Return the result of dispatching event to the context object. */
            /* To dispatch an event to a target, with an optional legacy target override flag and an optional legacyOutputDidListenersThrowFlag, run these steps: */
            /* 1) Set event’s dispatch flag. */
            Event.Flags |= EEventFlags.Dispatch;
            /* 2) Let targetOverride be target, if legacy target override flag is not given, and target’s associated Document otherwise. [HTML] */
            IEventTarget targetOverride;


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
