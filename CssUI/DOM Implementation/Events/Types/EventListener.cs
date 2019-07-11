namespace CssUI.DOM.Events
{
    public class EventListener
    {
        #region Properties
        public EventName type { get; protected set; } = null;
        public EventCallback callback { get; protected set; } = null;
        public bool capture { get; protected set; } = false;
        public bool passive { get; protected set; } = false;
        public bool once { get; protected set; } = false;
        public bool removed { get; set; } = false;
        #endregion

        #region Constructor
        public EventListener(EventName type = null, EventCallback callback = null, bool capture = false, bool once = false, bool passive = false)
        {
            this.type = type;
            this.callback = callback;
            this.capture = capture;
            this.once = once;
            this.passive = passive;
        }
        #endregion
    }
}
