using System;

namespace CssUI
{
    [Flags]
    public enum EBlockFlags : byte
    {
        /// <summary>
        /// Block is clean
        /// </summary>
        Clean = (1 << 0),
        /// <summary>
        /// Block is locked and cannot be updated
        /// </summary>
        Locked = (1 << 1),
        /// <summary>
        /// Block is dirty and needs to be updated/resolved
        /// </summary>
        Dirty = (1 << 2),
    }
}
