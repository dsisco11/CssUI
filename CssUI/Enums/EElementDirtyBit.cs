using System;

namespace CssUI
{
    [Flags]
    public enum EElementDirtyFlags : UInt16
    {
        Clean = 0,
        Text = (1 << 0),
        Font = (1 << 1),
        Visuals = (1 << 2),

    }
}