using System;

namespace CssUI.DOM.Events
{
    public class CompositionEvent : UIEvent
    {/* Docs: https://w3c.github.io/uievents/#idl-compositionevent */
        #region Properties
        public static Type initType = typeof(CompositionEventInit);
        public readonly string data;
        #endregion

        #region Constructor
        public CompositionEvent(EEventName type, CompositionEventInit eventInit = null) : base(type, eventInit)
        {
            this.data = eventInit?.data ?? string.Empty;
        }
        #endregion
    }
}
