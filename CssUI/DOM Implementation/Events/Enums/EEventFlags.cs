using System;

namespace CssUI.DOM
{
    [Flags]
    public enum EEventFlags : ushort
    {
        StopPropogation = (1 << 1),
        StopImmediatePropogation = (1 << 2),
        Canceled = (1 << 3),
        InPassiveListener = (1 << 4),
        Composed = (1 << 5),
        Initialized = (1 << 6),
        Dispatch = (1 << 7)
    }
}
