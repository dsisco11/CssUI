using System;

namespace CssUI.Internal
{
    [Flags]
    public enum EBoxDirtFlags : byte
    {
        Clean = (1 << 1),
        Dirty = (1 << 2),
    }
}
