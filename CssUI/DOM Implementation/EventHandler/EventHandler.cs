namespace CssUI.DOM.Events
{
    /// <summary>
    /// Acts as a non-capturing event listener
    /// </summary>
    /// <param name="event"></param>
    /// <returns></returns>
    public class EventHandler
    {
        #region Properties
        public EventListener listener = null;
        public EventCallback callback = null;
        #endregion

        #region Constructor
        public EventHandler(EventListener listener, EventCallback callback)
        {
            this.listener = listener;
            this.callback = callback;
        }
        #endregion
    }
}
