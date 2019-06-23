
namespace CssUI
{
    public class DomMouseMoveEventArgs : DomRoutedMouseEventArgs
    {
        #region Properties
        /// <summary>
        /// Movement distance on the X-Axis
        /// </summary>
        public int Xdelta { get; }
        /// <summary>
        /// Movement distance on the Y-Axis
        /// </summary>
        public int Ydelta { get; }
        #endregion

        public DomMouseMoveEventArgs(object Source, int X, int Y, int Xdelta, int Ydelta) : base(Source, X, Y)
        {
            this.Xdelta = Xdelta;
            this.Ydelta = Ydelta;
        }

        public DomMouseMoveEventArgs(object Source, DomPreviewMouseMoveEventArgs e) : base(Source, e)
        {
            this.Xdelta = e.Xdelta;
            this.Ydelta = e.Ydelta;
        }
    }

    public class DomPreviewMouseMoveEventArgs : DomPreviewMouseEventArgs
    {
        #region Properties
        /// <summary>
        /// Movement distance on the X-Axis
        /// </summary>
        public int Xdelta { get; }
        /// <summary>
        /// Movement distance on the Y-Axis
        /// </summary>
        public int Ydelta { get; }
        #endregion

        public DomPreviewMouseMoveEventArgs(int X, int Y, int Xdelta, int Ydelta) : base(X, Y)
        {
            this.Xdelta = Xdelta;
            this.Ydelta = Ydelta;
        }

        public DomPreviewMouseMoveEventArgs(DomPreviewMouseMoveEventArgs e) : base(e)
        {
            this.Xdelta = e.Xdelta;
            this.Ydelta = e.Ydelta;
        }
    }
}
