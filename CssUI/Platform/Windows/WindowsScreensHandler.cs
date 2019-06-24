#if WINDOWS
using System;

namespace CssUI.Platform.Windows
{
    public class WindowsScreensHandler : ISystemScreensHandler
    {
        public ISystemScreen Get_Screen_From_window(IntPtr window)
        {
            var hwnd = Platform.Win32.User32.MonitorFromWindow(window, Win32.MONITOR_FLAG.MONITOR_DEFAULTTONEAREST);
            return new SystemScreen(hwnd);
        }
    }
}
#endif