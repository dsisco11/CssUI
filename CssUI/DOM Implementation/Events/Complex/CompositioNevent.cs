using System;

namespace CssUI.DOM.Events
{
    public class CompositionEvent : UIEvent
    {
        public static Type initType = typeof(CompositionEventInit);
        public readonly string data;

        public CompositionEvent(EEventName type, CompositionEventInit eventInit = null) : base(type, eventInit)
        {
            this.data = eventInit?.data ?? string.Empty;
        }
    }
}
