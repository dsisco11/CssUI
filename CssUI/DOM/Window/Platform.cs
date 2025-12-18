
namespace CssUI.DOM
{
    public abstract partial class Window
    {
        #region Platform Windowing Functionality
        /// <summary>
        /// Returns the location of this window relative to the top, left origin of the desktop
        /// </summary>
        protected abstract Point2i Get_Window_Location();
        /// <summary>
        /// Returns the size of this window on the desktop
        /// </summary>
        protected abstract Rect2i Get_Window_Size();


        /// <summary>
        /// Sets the location of this window relative to the top, left origin of the desktop
        /// </summary>
        protected abstract void Set_Window_Location(Point2i Pos);
        /// <summary>
        /// Sets the size of this window on the desktop
        /// </summary>
        protected abstract void Set_Window_Size(Rect2i Size);
        #endregion
    }
}

