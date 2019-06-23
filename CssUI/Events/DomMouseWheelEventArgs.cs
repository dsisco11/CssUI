
namespace CssUI
{
    public class DomMouseWheelEventArgs : DomRoutedMouseEventArgs
    {
        #region Properties
        public int Delta { get; }
        public int Value { get; }
        #endregion

        public DomMouseWheelEventArgs(object Source, int X, int Y, int Value, int Delta) : base(Source, X, Y)
        {
            this.Delta = Delta;
            this.Value = Value;
        }

        public DomMouseWheelEventArgs(object Source, DomPreviewMouseWheelEventArgs e) : base(Source, e)
        {
            this.Delta = e.Delta;
            this.Value = e.Value;
        }
    }

    public class DomPreviewMouseWheelEventArgs : DomPreviewMouseEventArgs
    {
        #region Properties
        public int Delta { get; }
        public int Value { get; }
        #endregion

        public DomPreviewMouseWheelEventArgs(int X, int Y, int Value, int Delta) : base(X, Y)
        {
            this.Delta = Delta;
            this.Value = Value;
        }

        public DomPreviewMouseWheelEventArgs(DomPreviewMouseWheelEventArgs e) : base(e)
        {
            this.Delta = e.Delta;
            this.Value = e.Value;
        }
    }
}
