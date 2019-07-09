namespace CssUI.DOM.Events
{
    /// <summary>
    /// An EventTarget object represents a target to which an event can be dispatched when something has occurred.
    /// </summary>
    public interface IEventTarget
    {
        void addEventListener(EEventType type, IEventListener callback, AddEventListenerOptions options = null);
        void removeEventListener(EEventType type, IEventListener callback, EventListenerOptions options = null);
        bool dispatchEvent(Event Event);
        IEventTarget get_the_parent(Event @event);
    }
}
