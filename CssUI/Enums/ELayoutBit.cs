using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssUI
{
    [Flags]
    public enum ELayoutBit
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
