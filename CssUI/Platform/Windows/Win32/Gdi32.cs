#if WINDOWS
using System;
using System.Runtime.InteropServices;

namespace CssUI.Platform.Win32
{
    public static class Gdi32
    {
        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern int GetDeviceCaps(IntPtr hDC, int nIndex);
    }

    public enum DeviceCap : int
    {
        VERTRES = 10,
        DESKTOPVERTRES = 117,
        /// <summary>
        /// Logical pixels inch in X
        /// </summary>
        LOGPIXELSX = 88,
        /// <summary>
        /// Logical pixels inch in Y
        /// </summary>
        LOGPIXELSY = 90
    };
}
#endif