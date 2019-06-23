using CssUI.Platform;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssUI.Platform.Windows
{
    /// <summary>
    /// Windows system metrics implementation
    /// </summary>
    class WinMetrics : SystemMetricsBase
    {
        public override Point Get_Drag_Event_Distance()
        {
            int dX = 0;
            int dY = 0;

            dX = Platform.Win32.User32.GetSystemMetrics((int)Platform.Win32.SYSTEM_METRICS.SM_CXDRAG);
            dY = Platform.Win32.User32.GetSystemMetrics((int)Platform.Win32.SYSTEM_METRICS.SM_CYDRAG);
            return new Point(dX, dY);
        }

        public override ScrollBar_Params Get_Vertical_Scrollbar_Params()
        {
            return new ScrollBar_Params()
            {
                Size = Platform.Win32.User32.GetSystemMetrics((int)Platform.Win32.SYSTEM_METRICS.SM_CXVSCROLL),
                ThumbSize = Platform.Win32.User32.GetSystemMetrics((int)Platform.Win32.SYSTEM_METRICS.SM_CYVTHUMB),
                BtnArrowSize = Platform.Win32.User32.GetSystemMetrics((int)Platform.Win32.SYSTEM_METRICS.SM_CYVSCROLL),
            };
        }

        public override ScrollBar_Params Get_Horizontal_Scrollbar_Params()
        {
            return new ScrollBar_Params()
            {
                Size = Platform.Win32.User32.GetSystemMetrics((int)Platform.Win32.SYSTEM_METRICS.SM_CYHSCROLL),
                ThumbSize = Platform.Win32.User32.GetSystemMetrics((int)Platform.Win32.SYSTEM_METRICS.SM_CXHTHUMB),
                BtnArrowSize = Platform.Win32.User32.GetSystemMetrics((int)Platform.Win32.SYSTEM_METRICS.SM_CXHSCROLL),
            };
        }

        public override Point Get_DoubleClick_Distance_Threshold()
        {
            int dX = 0;
            int dY = 0;

            dX = Platform.Win32.User32.GetSystemMetrics((int)Platform.Win32.SYSTEM_METRICS.SM_CXDOUBLECLK);
            dY = Platform.Win32.User32.GetSystemMetrics((int)Platform.Win32.SYSTEM_METRICS.SM_CYDOUBLECLK);
            return new Point(dX, dY);
        }

        public override float Get_Double_Click_Time()
        {
            uint ms = Win32.User32.GetDoubleClickTime();
            return ((float)ms / 1000.0f);
        }
    }
}
