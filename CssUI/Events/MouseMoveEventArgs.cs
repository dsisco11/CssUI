using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssUI
{
    public class MouseMoveEventArgs : RoutedMouseEventArgs
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

        public MouseMoveEventArgs(object Source, int X, int Y, int Xdelta, int Ydelta) : base(Source, X, Y)
        {
            this.Xdelta = Xdelta;
            this.Ydelta = Ydelta;
        }

        public MouseMoveEventArgs(object Source, PreviewMouseMoveEventArgs e) : base(Source, e)
        {
            this.Xdelta = e.Xdelta;
            this.Ydelta = e.Ydelta;
        }
    }

    public class PreviewMouseMoveEventArgs : PreviewMouseEventArgs
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

        public PreviewMouseMoveEventArgs(int X, int Y, int Xdelta, int Ydelta) : base(X, Y)
        {
            this.Xdelta = Xdelta;
            this.Ydelta = Ydelta;
        }

        public PreviewMouseMoveEventArgs(PreviewMouseMoveEventArgs e) : base(e)
        {
            this.Xdelta = e.Xdelta;
            this.Ydelta = e.Ydelta;
        }
    }
}
