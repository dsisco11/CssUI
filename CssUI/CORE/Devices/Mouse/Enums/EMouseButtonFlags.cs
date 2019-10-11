using System;

namespace CssUI.Devices
{
    [Flags]
    public enum EMouseButtonFlags : ushort
    {
        None = 0x0,
        Left = 1 << 1,
        Right = 1 << 2,
        Middle = 1 << 3,
        X1 = 1 << 4,
        X2 = 1 << 5,
    }
}
