using System;

namespace CssUI.DOM.Events
{
    public class FocusEvent : UIEvent
    {
        public static Type initType = typeof(FocusEventInit);
        /// <summary>
        /// Used to identify a secondary EventTarget related to a Focus event, depending on the type of event.
        /// </summary>
        public readonly EventTarget relatedTarget = null;

        public FocusEvent(EEventName type, FocusEventInit eventInit) : base(type, eventInit)
        {
            this.relatedTarget = eventInit.relatedTarget;
        }
    }
}
