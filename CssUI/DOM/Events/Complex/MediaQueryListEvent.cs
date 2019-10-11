namespace CssUI.DOM.Events
{
    public class MediaQueryListEvent : Event
    {
        #region Properties
        public readonly string media;
        public readonly bool matches;
        #endregion


        public MediaQueryListEvent(EventName name, MediaQueryListEventInit eventInit) : base(name, eventInit)
        {
            this.media = eventInit.media;
            this.matches = eventInit.matches;
        }
    }
}
