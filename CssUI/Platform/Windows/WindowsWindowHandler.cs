#if WINDOWS
using System;
using System.Runtime.InteropServices;

namespace CssUI.Platform.Windows
{
    public class WindowsWindowHandler : ISystemWindowHandler
    {
        public IntPtr Get_Window()
        {
            uint threadId = Win32.User32.GetWindowThreadProcessId(IntPtr.Zero, out int processId);
            Win32.GUITHREADINFO gui = new Win32.GUITHREADINFO();
            if (Win32.User32.GetGUIThreadInfo(threadId, gui))
            {
                return gui.hwndActive;
            }

            //return Platform.Win32.User32.GetActiveWindow();
            return IntPtr.Zero;
        }
    }
}
#endif