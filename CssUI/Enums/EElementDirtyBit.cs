using System;

namespace CssUI
{
    [Flags]
    public enum EElementDirtyBit : UInt16
    {
        None = 0,
        Text = (1 << 0),
        Font = (1 << 1),
        Visuals = (1 << 2),

    }
}