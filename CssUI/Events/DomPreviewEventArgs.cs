
namespace CssUI
{
    /// <summary>
    /// A tunneling event is one which traverses the element tree from top to bottom and can be halted along the way.
    /// Used for "Preview-type" events
    /// <para>Preview events are basically used to find the starting point for a bubbling event sequence</para>
    /// <para>In addition preview events allow higher level elements to prevent certain events from reaching controls within them if they need to</para>
    /// </summary>
    public class DomPreviewEventArgs
    {
        #region Properties
        /// <summary>
        /// If set to True then the event tunneling process will halt at the current element and trigger the counterpart bubbling event process.
        /// </summary>
        public bool Handled = false;
        /// <summary>
        /// The element which will currently trigger the bubbling event process if Handled is set to True
        /// </summary>
        public object Handler = null;
        #endregion
        
        public DomPreviewEventArgs()
        {
        }

        public DomPreviewEventArgs(DomPreviewEventArgs e)
        {
            Handled = e.Handled;
            Handler = e.Handler;
        }
    }
}
