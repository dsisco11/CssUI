namespace CssUI.DOM.Events
{
    public class UIEvent : Event
    {
        #region Properties
        /// <summary>
        /// The view attribute identifies the Window from which the event was generated.
        /// </summary>
        public Window View { get; private set; } = null;
        /// <summary>
        /// Specifies some detail information about the Event, depending on the type of event.
        /// </summary>
        public long Detail { get; private set; } = 0;
        #endregion

        #region Constructors
        public UIEvent(EEventType type, UIEventInit eventInit = null) : base(type, eventInit)
        {
            this.View = eventInit.view;
            this.Detail = eventInit.detail;
        }
        #endregion

    }
}
