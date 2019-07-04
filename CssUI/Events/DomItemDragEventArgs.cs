
namespace CssUI
{
    public class DomItemDragEventArgs
    {
        /// <summary>
        /// Location when the dragging process began
        /// </summary>
        public readonly Vec2i Origin = null;
        /// <summary>
        /// Location right now
        /// </summary>
        public readonly Vec2i Location = null;
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

        public DomItemDragEventArgs(Vec2i Origin, Vec2i Current)
        {
            this.Origin = Origin;
            this.Location = Current;

            XDelta = (Current.X - Origin.X);
            YDelta = (Current.Y - Origin.Y);
        }
    }
}
