using System;

namespace CssUI
{
    [Flags]
    public enum ELayoutDirt : byte
    {
        /// <summary>
        /// The layout is completely clean, abort any traversal.
        /// </summary>
        Clean = 0,
        /// <summary>
        /// The layout is dirty
        /// </summary>
        Dirty = (1 << 0),
        /// <summary>
        /// The layout might not be dirty itself, but it has children which ARE dirty.
        /// </summary>
        Dirty_Children = (1 << 1),
    }
}
