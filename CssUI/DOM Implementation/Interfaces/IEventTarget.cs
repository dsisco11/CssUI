using System.Collections.Generic;

namespace CssUI.DOM.Events
{
    public interface IEventTarget
    {
        EventHandlerMap handlerMap { get; }
        LinkedList<EventListener> Listeners { get; }

        void addEventListener(EventName eventName, EventCallback callback, AddEventListenerOptions options = null);
        bool dispatchEvent(Event Event);
        EventTarget get_the_parent(Event @event);
        void removeEventListener(EventName eventName, EventCallback callback, EventListenerOptions options = null);
    }
}