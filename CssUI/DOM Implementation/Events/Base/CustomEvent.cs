namespace CssUI.DOM.Events
{
    public class CustomEvent : Event
    {/* Docs: https://dom.spec.whatwg.org/#interface-customevent */

        public dynamic detail { get; protected set; } = null;

        public CustomEvent(EEventType type, CustomEventInit eventInit = null) : base(type, eventInit)
        {
            this.detail = eventInit?.detail;
        }

    }
}
