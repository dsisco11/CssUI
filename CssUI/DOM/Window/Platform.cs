
namespace CssUI.DOM
{
    public abstract partial class Window
    {
        #region Platform Windowing Functionality
        /// <summary>
        /// Returns the location of this window relative to the top, left origin of the desktop
        /// </summary>
        protected abstract ReadOnlyPoint2i Get_Window_Location();
        /// <summary>
        /// Returns the size of this window on the desktop
        /// </summary>
        protected abstract ReadOnlyRect2i Get_Window_Size();


        /// <summary>
        /// Sets the location of this window relative to the top, left origin of the desktop
        /// </summary>
        protected abstract void Set_Window_Location(ReadOnlyPoint2i Pos);
        /// <summary>
        /// Sets the size of this window on the desktop
        /// </summary>
        protected abstract void Set_Window_Size(ReadOnlyRect2i Size);
        #endregion
    }
}
