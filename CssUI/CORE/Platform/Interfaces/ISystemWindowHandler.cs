
using System;

namespace CssUI.Platform
{
    public interface ISystemWindowHandler
    {
        /// <summary>
        /// Returns a handle to the window belonging calling threads message queue
        /// </summary>
        /// <returns></returns>
        IntPtr Get_Window();
    }
}
