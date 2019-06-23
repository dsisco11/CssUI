using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssUI
{
    public class MouseWheelEventArgs : RoutedMouseEventArgs
    {
        #region Properties
        public int Delta { get; }
        public int Value { get; }
        #endregion

        public MouseWheelEventArgs(object Source, int X, int Y, int Value, int Delta) : base(Source, X, Y)
        {
            this.Delta = Delta;
            this.Value = Value;
        }

        public MouseWheelEventArgs(object Source, PreviewMouseWheelEventArgs e) : base(Source, e)
        {
            this.Delta = e.Delta;
            this.Value = e.Value;
        }
    }

    public class PreviewMouseWheelEventArgs : PreviewMouseEventArgs
    {
        #region Properties
        public int Delta { get; }
        public int Value { get; }
        #endregion

        public PreviewMouseWheelEventArgs(int X, int Y, int Value, int Delta) : base(X, Y)
        {
            this.Delta = Delta;
            this.Value = Value;
        }

        public PreviewMouseWheelEventArgs(PreviewMouseWheelEventArgs e) : base(e)
        {
            this.Delta = e.Delta;
            this.Value = e.Value;
        }
    }
}
