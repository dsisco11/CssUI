namespace CssUI
{
    public class DomMouseButtonEventArgs : DomRoutedMouseEventArgs
    {
        #region Properties
        /// <summary>
        /// The mouse button that triggered this event
        /// </summary>
        public readonly EMouseButton Button;
        /// <summary>
        /// Whether the culprit mouse button is pressed
        /// </summary>
        public readonly bool IsPressed;
        #endregion

        public DomMouseButtonEventArgs(object Source, int X, int Y, EMouseButton Button, bool IsPressed) : base(Source, X, Y)
        {
            this.Button = Button;
            this.IsPressed = IsPressed;
        }

        public DomMouseButtonEventArgs(object Source, DomPreviewMouseButtonEventArgs e) : base(Source, e)
        {
            this.Button = e.Button;
            this.IsPressed = e.IsPressed;
        }
    }

    public class DomPreviewMouseButtonEventArgs : DomPreviewMouseEventArgs
    {
        #region Properties
        /// <summary>
        /// The mouse button that triggered this event
        /// </summary>
        public readonly EMouseButton Button;
        /// <summary>
        /// Whether the culprit mouse button is pressed
        /// </summary>
        public readonly bool IsPressed;
        #endregion

        public DomPreviewMouseButtonEventArgs(DomPreviewMouseButtonEventArgs e) : base(e)
        {
            this.Button = e.Button;
            this.IsPressed = e.IsPressed;
        }

        public DomPreviewMouseButtonEventArgs(int X, int Y, EMouseButton Button, bool IsPressed) : base(X, Y)
        {
            this.Button = Button;
            this.IsPressed = IsPressed;
        }
    }
}
