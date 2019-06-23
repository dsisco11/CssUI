using System;
using System.Runtime.InteropServices;

namespace CssUI.Platform.Win32
{
    public static class User32
    {
        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, ref TVHITTESTINFO lParam);
        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern int GetSystemMetrics(int ID);

        /// <summary>
        /// Retrieves the current double-click time for the mouse. A double-click is a series of two clicks of the mouse button, the second occurring within a specified time after the first. The double-click time is the maximum number of milliseconds that may occur between the first and second click of a double-click. The maximum double-click time is 5000 milliseconds.
        /// </summary>
        /// <returns>Double-click time threshold, in milliseconds</returns>
        [DllImport("user32.dll")]
        public static extern uint GetDoubleClickTime();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetMonitorInfo(IntPtr hmonitor, [In, Out]MONITORINFOEX info);

        [DllImport("User32.dll", ExactSpelling = true)]
        public static extern IntPtr MonitorFromPoint(POINTSTRUCT pt, int flags);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);

        /// <summary>
        /// https://msdn.microsoft.com/en-us/library/dd145064(v=vs.85).aspx
        /// <para>Retrieves a handle to the display monitor that has the largest area of intersection with the bounding rectangle of a specified window.</para>
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        [DllImport("user32.dll", ExactSpelling = true)]
        public static extern IntPtr MonitorFromWindow(IntPtr hWnd, int flags);



        /// <summary>
        /// Used with SendMessage to perform a hit test at a given location and return which TreeNode (if any) was hit
        /// </summary>
        public const int TVM_HITTEST = 0x1111;
        public const int WM_VSCROLL = 277; // Vertical scroll
        public const int SB_LINEUP = 0; // Scrolls one line up
        public const int SB_LINEDOWN = 1; // Scrolls one line down
    }

    /// <summary>
    /// Windows system-metrics item ID's.
    /// (Used with <see cref="User32.GetSystemMetrics(int)"/>)
    /// </summary>
    enum SYSTEM_METRICS : int
    {
        /// <summary>The width of a vertical scroll bar, in pixels.</summary>
        SM_CXVSCROLL = 2,
        /// <summary>The height of a horizontal scroll bar, in pixels.</summary>
        SM_CYHSCROLL = 3,
        /// <summary>The height of the thumb box in a vertical scroll bar, in pixels.</summary>
        SM_CYVTHUMB = 9,
        /// <summary>The width of the thumb box in a horizontal scroll bar, in pixels.</summary>
        SM_CXHTHUMB = 10,
        /// <summary>The height of the arrow bitmap on a vertical scroll bar, in pixels.</summary>
        SM_CYVSCROLL = 20,
        /// <summary>The width of the arrow bitmap on a horizontal scroll bar, in pixels.</summary>
        SM_CXHSCROLL = 21,
        /// <summary>The width of a button in a window caption or title bar, in pixels.</summary>
        SM_CXSIZE = 30,
        /// <summary>The height of a button in a window caption or title bar, in pixels.</summary>
        SM_CYSIZE = 31,
        /// <summary>double-click sequence distance X-Axis threshold, in pixels</summary>
        SM_CXDOUBLECLK = 36,
        /// <summary>double-click sequence distance Y-Axis threshold, in pixels</summary>
        SM_CYDOUBLECLK = 37,
        /// <summary>Mouse drag distance X-Axis threshold, in pixels</summary>
        SM_CXDRAG = 68,
        /// <summary>Mouse drag distance Y-Axis threshold, in pixels</summary>
        SM_CYDRAG = 69,
    }


    [Flags]
    enum MONITOR_FLAG : uint
    {
        DEFAULTTOPRIMARY = 0x00000001,
        DEFAULTTONEAREST = 0x00000002
    }
}
