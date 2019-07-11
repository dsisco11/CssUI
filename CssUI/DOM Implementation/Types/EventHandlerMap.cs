using CssUI.DOM.Events;
using CssUI.DOM.Internal;
using CssUI.Internal;
using System;
using System.Collections.Generic;

namespace CssUI.DOM.Events
{
    public class EventHandlerMap : IDisposable
    {/* Docs: https://html.spec.whatwg.org/multipage/webappapis.html#event-handler-idl-attributes */

        #region Static
        private static int MAX_VALUE;
        static EventHandlerMap()
        {
            int max = 0;
            var NameList = Enum.GetValues(typeof(EEventName));
            foreach (var Name in NameList)
            {
                max = (max < (int)Name) ? (int)Name : max;
            }

            MAX_VALUE = max;
        }
        #endregion

        #region Properties
        private EventTarget Owner;
        private Dictionary<EventName, LinkedList<EventHandler>> Map;
        #endregion

        #region Constructor
        public EventHandlerMap(EventTarget Owner)
        {
            this.Owner = Owner;
            Map = new Dictionary<EventName, LinkedList<EventHandler>>(0);
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
                    foreach (KeyValuePair<EventName, LinkedList<EventHandler>> kv in Map)
                    {
                        kv.Value.Clear();
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
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

        #region Manipulation
        public IEnumerable<EventHandler> this[EventName Name]
        {
            get
            {
                /* Verify the event is accepted by this element */
                var eventTarget = EventCommon.determine_target(Owner, Name);
                if (ReferenceEquals(null, eventTarget))
                    return null;/* This event is not valid for the owning element */

                if (!Map.TryGetValue(Name, out LinkedList<EventHandler> handlerList))
                    return new EventHandler[]{ };

                return handlerList;
            }
        }

        public bool Add(EventName Name, EventCallback Callback)
        {
            /* Verify the event is accepted by this element */
            var eventTarget = EventCommon.determine_target(Owner, Name);
            if (ReferenceEquals(null, eventTarget))
                return false;/* This event is not valid for the owning element */

            if (!Map.TryGetValue(Name, out LinkedList<EventHandler> handlerList))
            {/* We have no handlers for this event yet */
                handlerList = new LinkedList<EventHandler>();
                Map.Add(Name, handlerList);
            }

            /* Theres no need for CssUI to do an extra processing step on event handler callbacks now so we will just give the listener the callback */
            var listener = new EventListener(Name, (Event e) => Callback.Invoke(e));
            var handler = new EventHandler(listener, Callback);
            handlerList.AddLast(handler);
            return true;
        }

        public bool Remove(EventName Name, EventCallback Callback)
        {
            /* Verify the event is accepted by this element */
            var eventTarget = EventCommon.determine_target(Owner, Name);
            if (ReferenceEquals(null, eventTarget))
                return false;/* This event is not valid for the owning element */

            if (!Map.TryGetValue(Name, out LinkedList<EventHandler> handlerList))
            {/* We have no handlers for this event yet */
                return false;
            }

            bool found = false;
            var node = handlerList.First;
            while (!ReferenceEquals(null, node))
            {
                var handler = node.Value;
                if (ReferenceEquals(Callback, handler.callback))
                {
                    handlerList.Remove(node);
                    if (!ReferenceEquals(null, handler.listener))
                    {
                        handler.listener.removed = true;
                        Owner.Listeners.Remove(handler.listener);
                    }
                    found = true;
                    break;
                }

                node = node.Next;
            }

            return found;
        }
        #endregion

        #region Event Handler Processing
/*
        /// <summary>
        /// </summary>
        /// <param name="eventTarget"></param>
        /// <param name="Name"></param>
        /// <param name="event"></param>
        internal static void Process_Handler(EventTarget eventTarget, EventName Name, Event @event)
        {*//* Docs: https://html.spec.whatwg.org/multipage/webappapis.html#the-event-handler-processing-algorithm */

            /* Since CssUI's event handler system allows multiple callbacks per handler we do things slightly different from the DOM specifications *//*
            var handlerList = eventTarget.handlerMap[Name];
            foreach(EventHandler handler in handlerList)
            {
                *//* Invoke callback with one argument, the value of which is the Event object event, with the callback this value set to event's currentTarget. Let return value be the callback's return value. [WEBIDL] *//*
                handler.callback?.Invoke(@event);
            }

        }
*/
        #endregion

    }
}
