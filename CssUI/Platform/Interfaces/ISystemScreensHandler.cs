
using System;

namespace CssUI.Platform
{
    public interface ISystemScreensHandler
    {
        /// <summary>
        /// Returns the system screen which the given application window resides in.
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        ISystemScreen Get_Screen_From_window(IntPtr window);
    }
}
