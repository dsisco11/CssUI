using System.Collections.Generic;
using System.Threading.Tasks;

namespace CssUI.DOM.Events
{
    public interface IEventTarget
    {
        EventHandlerMap handlerMap { get; }
        LinkedList<EventListener> Listeners { get; }

        void addEventListener(EventName eventName, EventCallback callback, AddEventListenerOptions options = null);
        ValueTask<bool> dispatchEvent(Event @event);
        EventTarget get_the_parent(Event @event);
        void removeEventListener(EventName eventName, EventCallback callback, EventListenerOptions options = null);
    }
}