using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssUI
{

    public abstract class RoutedMouseEventArgs : RoutedEventArgs
    {
        #region Properties
        /// <summary>
        /// X position of the mouse for the event (in screen-space) 
        /// </summary>
        public int X { get; }
        /// <summary>
        /// Y position of the mouse for the event (in screen-space) 
        /// </summary>
        public int Y { get; }
        /// <summary>
        /// screen-space position of the mouse for the event
        /// </summary>
        public ePos Position { get; }
        #endregion

        public RoutedMouseEventArgs(object Source, int X, int Y) : base(Source)
        {
            this.X = X;
            this.Y = Y;
            this.Position = new ePos(X, Y);
        }

        public RoutedMouseEventArgs(object Source, PreviewMouseEventArgs e) : base(Source)
        {
            this.X = e.X;
            this.Y = e.Y;
            this.Position = new ePos(X, Y);
        }
    }

    public abstract class PreviewMouseEventArgs : PreviewEventArgs
    {
        #region Properties
        /// <summary>
        /// X position of the mouse for the event (in screen-space) 
        /// </summary>
        public int X { get; }
        /// <summary>
        /// Y position of the mouse for the event (in screen-space) 
        /// </summary>
        public int Y { get; }
        /// <summary>
        /// screen-space position of the mouse for the event
        /// </summary>
        public ePos Position { get; }
        #endregion

        public PreviewMouseEventArgs(PreviewMouseEventArgs e)
        {
            this.X = e.X;
            this.Y = e.Y;
            this.Position = new ePos(X, Y);
        }

        public PreviewMouseEventArgs(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
            this.Position = new ePos(X, Y);
        }
    }
}
