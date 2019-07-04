using System;

namespace CssUI.Enums
{
    /// <summary>
    /// Specififes important flags for a box which distinguish it's flow-behaviour and certain element types
    /// </summary>
    [Flags]
    public enum EBoxFlags : int
    {
        /// <summary>
        /// This element's content specifys its own size
        /// </summary>
        REPLACED_ELEMENT = (1 << 1),
    }
}
