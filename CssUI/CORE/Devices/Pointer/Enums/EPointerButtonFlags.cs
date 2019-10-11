using System;

namespace CssUI.Devices
{
    [Flags]
    public enum EPointerButtonFlags : short
    {
        None = 0x0,
        Left = (1 << 1), // 1
        Right = (1 << 2), // 2
        Middle = (1 << 3), // 4
        X1 = (1 << 4), // 8
        X2 = (1 << 5), // 16
        Eraser = (1 << 6), // 32
    }
}
