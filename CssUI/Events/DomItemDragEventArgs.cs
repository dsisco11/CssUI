﻿
namespace CssUI
{
    public class DomItemDragEventArgs
    {
        /// <summary>
        /// Location when the dragging process began
        /// </summary>
        public readonly ePos Origin = null;
        /// <summary>
        /// Location right now
        /// </summary>
        public readonly ePos Location = null;
        /// <summary>
        /// X-Axis difference between the origin and current locations
        /// </summary>
        public readonly int XDelta = 0;
        /// <summary>
        /// Y-Axis difference between the origin and current locations
        /// </summary>
        public readonly int YDelta = 0;
        /// <summary>
        /// If set to True then the dragging process will be ended
        /// </summary>
        public bool Abort = false;

        public DomItemDragEventArgs(ePos Origin, ePos Current)
        {
            this.Origin = Origin;
            this.Location = Current;

            XDelta = (Current.X - Origin.X);
            YDelta = (Current.Y - Origin.Y);
        }
    }
}