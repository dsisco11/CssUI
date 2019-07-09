namespace CssUI.DOM.Events
{
    /// <summary>
    /// Allows event receivers to prevent elements from responding to certain events.
    /// </summary>
    /// <typeparam name="Ty"></typeparam>
    public class DomCancellableEvent<Ty>
    {
        public readonly Ty Args;
        /// <summary>
        /// If True then the element will not act on this event.
        /// </summary>
        public bool Cancel = false;

        public DomCancellableEvent(Ty args)
        {
            Args = args;
        }
    }
}
