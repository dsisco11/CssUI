﻿
namespace CssUI
{

    public abstract class DomRoutedMouseEventArgs : DomRoutedEventArgs
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
        public Vec2i Position { get; }
        #endregion

        public DomRoutedMouseEventArgs(object Source, int X, int Y) : base(Source)
        {
            this.X = X;
            this.Y = Y;
            this.Position = new Vec2i(X, Y);
        }

        public DomRoutedMouseEventArgs(object Source, DomPreviewMouseEventArgs e) : base(Source)
        {
            this.X = e.X;
            this.Y = e.Y;
            this.Position = new Vec2i(X, Y);
        }
    }

    public abstract class DomPreviewMouseEventArgs : DomPreviewEventArgs
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
        public Vec2i Position { get; }
        #endregion

        public DomPreviewMouseEventArgs(DomPreviewMouseEventArgs e)
        {
            this.X = e.X;
            this.Y = e.Y;
            this.Position = new Vec2i(X, Y);
        }

        public DomPreviewMouseEventArgs(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
            this.Position = new Vec2i(X, Y);
        }
    }
}
