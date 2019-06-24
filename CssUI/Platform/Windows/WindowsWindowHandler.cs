#if WINDOWS
using System;

namespace CssUI.Platform
{
    public class WindowsWindowHandler : ISystemWindowHandler
    {
        public IntPtr Get_Window()
        {
            return Platform.Win32.User32.GetActiveWindow();
        }
    }
}
#endif