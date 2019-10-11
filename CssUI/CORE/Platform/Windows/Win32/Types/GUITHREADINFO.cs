using System;
using System.Runtime.InteropServices;

namespace CssUI.Platform.Windows.Win32
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
    public class GUITHREADINFO
    {
        public readonly UInt32 cbSize = (UInt32)Marshal.SizeOf(typeof(GUITHREADINFO));
        /// <summary>
        /// The thread state. This member can be one or more of the following values: https://docs.microsoft.com/en-us/windows/desktop/api/winuser/ns-winuser-tagguithreadinfo
        /// </summary>
        public UInt32 flags;
        /// <summary>
        /// A handle to the active window within the thread.
        /// </summary>
        public IntPtr hwndActive;
        /// <summary>
        /// A handle to the window that has the keyboard focus.
        /// </summary>
        public IntPtr hwndFocus;
        /// <summary>
        /// A handle to the window that has captured the mouse.
        /// </summary>
        public IntPtr hwndCapture;
        /// <summary>
        /// A handle to the window that owns any active menus.
        /// </summary>
        public IntPtr hwndMenuOwner;
        /// <summary>
        /// A handle to the window in a move or size loop.
        /// </summary>
        public IntPtr hwndMoveSize;
        /// <summary>
        /// A handle to the window that is displaying the caret.
        /// </summary>
        public IntPtr hwndCaret;
        /// <summary>
        /// The caret's bounding rectangle, in client coordinates, relative to the window specified by the hwndCaret member.
        /// </summary>
        public RECT rcCaret;
    }
}
