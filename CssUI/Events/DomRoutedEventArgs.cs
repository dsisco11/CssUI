
namespace CssUI
{
    public class DomRoutedEventArgs
    {
        #region Properties
        /// <summary>
        /// If set to True then the event handling process will stop at the current element.
        /// </summary>
        public bool Handled = false;
        /// <summary>
        /// The original reporting source for this event (determined by hit testing)
        /// </summary>
        public readonly object OriginSource;
        /// <summary>
        /// Object that is raising the event
        /// </summary>
        public object Source;
        #endregion

        public DomRoutedEventArgs(object Source)
        {
            this.OriginSource = Source;
            this.Source = Source;
        }
    }
}
