using System;

namespace CssUI.Platform
{
    public class SystemScreen : ISystemScreen
    {
        public IntPtr Handle { get; set; } = IntPtr.Zero;

        public SystemScreen(IntPtr hwnd)
        {
            Handle = hwnd;
        }
    }
}
