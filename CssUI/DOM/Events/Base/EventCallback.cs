using System;

namespace CssUI.DOM.Events
{
    //public delegate void EventCallback(Event @event);
    public delegate Action<Event> EventCallback(Event @event);
}
