using System;

namespace CssUI.CSS.Enums
{
    /// <summary>
    /// Specififes important flags for a box which distinguish it's flow-behaviour and certain element types
    /// </summary>
    [Flags]
    public enum EBoxFlags : int
    {
        None = 0x0,
        /// <summary>
        /// This box belongs to a replaced-type element
        /// </summary>
        IsReplaced = (1 << 1),
    }
}
