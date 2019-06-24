using System;
using System.Numerics;

namespace CssUI.Platform
{
    /// <summary>
    /// The base implementation for System-Metrics implementations
    /// </summary>
    public interface ISystemMetricsHandler
    {
        /// <summary>
        /// Returns the minimum distance the mouse needs to click down on something and then move for it to turn into a 'drag' event
        /// </summary>
        Vector2 Get_Drag_Event_Distance();

        /// <summary>
        /// Get the system parameters for vertical scrollbars
        /// </summary>
        ScrollBar_Params Get_Vertical_Scrollbar_Params();


        /// <summary>
        /// Get the system parameters for horizontal scrollbars
        /// </summary>
        ScrollBar_Params Get_Horizontal_Scrollbar_Params();


        /// <summary>
        /// Returns the maximum distance from the first location that a second mouse down event can be for it to fire a DoubleClick event
        /// </summary>
        Vector2 Get_DoubleClick_Distance_Threshold();

        /// <summary>
        /// Returns the maximum delay between two consecutive mouse down events before the 'click' event turns into a 'double click' event
        /// </summary>
        /// <returns>Double-click time threshold, in seconds</returns>
        float Get_Double_Click_Time();

        /// <summary>
        /// Returns the DPI this system is currently running at
        /// </summary>
        /// <returns>System Dpi X/Y values</returns>
        Vector2 Get_DPI(IntPtr window);
    }
}
