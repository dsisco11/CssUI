namespace CssUI.DOM.Events
{
    public class EventListener : IEventListener
    {
        #region Properties
        public string type { get; protected set; } = null;
        public IEventListener callback { get; protected set; } = null;
        public bool capture { get; protected set; } = false;
        public bool passive { get; protected set; } = false;
        public bool once { get; protected set; } = false;
        public bool removed { get; set; } = false;
        #endregion

        #region Constructor
        public EventListener(string type, IEventListener callback, bool capture, bool once = false, bool passive = false)
        {
            this.type = type;
            this.callback = callback;
            this.capture = capture;
            this.once = once;
            this.passive = passive;
        }
        #endregion


        public void handleEvent(Event Event)
        {

        }
    }
}
