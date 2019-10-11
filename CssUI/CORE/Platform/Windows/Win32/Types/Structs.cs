#if WINDOWS
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;


namespace CssUI.Platform.Windows.Win32
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
    public class MONITORINFOEX
    {
        public int cbSize = Marshal.SizeOf(typeof(MONITORINFOEX));
        /// <summary>
        /// A RECT structure that specifies the display monitor rectangle, expressed in virtual-screen coordinates. Note that if the monitor is not the primary display monitor, some of the rectangle's coordinates may be negative values.
        /// </summary>
        public RECT rcMonitor = new RECT();
        /// <summary>
        /// A RECT structure that specifies the work area rectangle of the display monitor, expressed in virtual-screen coordinates. Note that if the monitor is not the primary display monitor, some of the rectangle's coordinates may be negative values.
        /// </summary>
        public RECT rcWork = new RECT();
        public int dwFlags = 0;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public char[] szDevice = new char[32];
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINTSTRUCT
    {
        public int x;
        public int y;
        public POINTSTRUCT(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
    
    [StructLayout(LayoutKind.Sequential)]
    internal struct PSIZE
    {
        public int x;
        public int y;
    }

    #region TreeView Structs
    [StructLayout(LayoutKind.Sequential)]
    public struct TVHITTESTINFO
    {
        public Point pt;
        public int flags;
        public IntPtr hItem;
    }
    #endregion
        
}
#endif