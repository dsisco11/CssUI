using System;

namespace CssUI.CSS
{
    [Flags]
    public enum EBoxDirtFlags : byte
    {
        Clean = 0x0,
        Dirty = 0x1 << 2,
    }
}
