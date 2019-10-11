#if WINDOWS
using System;
using System.Numerics;
using System.Threading.Tasks;

namespace CssUI.Platform.Windows
{
    /// <summary>
    /// Windows system metrics implementation
    /// </summary>
    public class WindowsMetricsHandler : ISystemMetricsHandler
    {
        public Vector2 Get_Drag_Event_Distance()
        {
            int dX = 0;
            int dY = 0;

            dX = Win32.User32.GetSystemMetrics((int)Win32.SYSTEM_METRICS.SM_CXDRAG);
            dY = Win32.User32.GetSystemMetrics((int)Win32.SYSTEM_METRICS.SM_CYDRAG);
            return new Vector2(dX, dY);
        }

        public ScrollBar_Params Get_Vertical_Scrollbar_Params()
        {
            return new ScrollBar_Params()
            {
                Size = Win32.User32.GetSystemMetrics((int)Win32.SYSTEM_METRICS.SM_CXVSCROLL),
                ThumbSize = Win32.User32.GetSystemMetrics((int)Win32.SYSTEM_METRICS.SM_CYVTHUMB),
                BtnArrowSize = Win32.User32.GetSystemMetrics((int)Win32.SYSTEM_METRICS.SM_CYVSCROLL),
            };
        }

        public ScrollBar_Params Get_Horizontal_Scrollbar_Params()
        {
            return new ScrollBar_Params()
            {
                Size = Win32.User32.GetSystemMetrics((int)Win32.SYSTEM_METRICS.SM_CYHSCROLL),
                ThumbSize = Win32.User32.GetSystemMetrics((int)Win32.SYSTEM_METRICS.SM_CXHTHUMB),
                BtnArrowSize = Win32.User32.GetSystemMetrics((int)Win32.SYSTEM_METRICS.SM_CXHSCROLL),
            };
        }

        public Vector2 Get_DoubleClick_Distance_Threshold()
        {
            int dX = 0;
            int dY = 0;

            dX = Win32.User32.GetSystemMetrics((int)Win32.SYSTEM_METRICS.SM_CXDOUBLECLK);
            dY = Win32.User32.GetSystemMetrics((int)Win32.SYSTEM_METRICS.SM_CYDOUBLECLK);
            return new Vector2(dX, dY);
        }

        public float Get_Double_Click_Time()
        {
            uint ms = Win32.User32.GetDoubleClickTime();
            return ((float)ms / 1000.0f);
        }

        public Vector2 Get_DPI(IntPtr window)
        {
            // First try and use the fancy new windows 10 function for screen-specific dpi
            try
            {
                if (window == null) throw new ArgumentNullException();

                int winDpi = Win32.User32.GetDpiForWindow((IntPtr)window);
                return new Vector2(winDpi);
            }
            catch
            {// Ok we couldnt get screen specific dpi, we will settle for system-wide dpi

                IntPtr desktop = Win32.User32.GetDesktopWindow();
                int LogicalScreenHeight = Win32.Gdi32.GetDeviceCaps(desktop, (int)Win32.DeviceCap.VERTRES);
                int PhysicalScreenHeight = Win32.Gdi32.GetDeviceCaps(desktop, (int)Win32.DeviceCap.DESKTOPVERTRES);
                int logpixelsy = Win32.Gdi32.GetDeviceCaps(desktop, (int)Win32.DeviceCap.LOGPIXELSY);

                float screenScalingFactor = (float)PhysicalScreenHeight / (float)LogicalScreenHeight;
                float dpiScalingFactor = (float)logpixelsy / (float)96;

                return new Vector2(dpiScalingFactor);
            }
        }
    }
}
#endif