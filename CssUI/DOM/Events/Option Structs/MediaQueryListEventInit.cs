namespace CssUI.DOM.Events
{
    public class MediaQueryListEventInit : EventInit
    {
        #region Properties
        public readonly string media;
        public readonly bool matches;
        #endregion

        public MediaQueryListEventInit(string media, bool matches) : base()
        {
            this.media = media;
            this.matches = matches;
        }
    }
}
