using System;

namespace CssUI
{
    /// <summary>
    /// Specifies all of the reasons for which a cssElement's block may be flagged as dirty.
    /// </summary>
    [Flags]
    public enum EBoxInvalidationReason : ushort
    {
        /// <summary>
        /// This block is still valid
        /// </summary>
        NotInvalid = 0,
        /// <summary>
        /// </summary>
        Unknown = (1 << 1),
        /// <summary>
        /// The element has changed parents.
        /// </summary>
        Reparented = (1 << 2),
        /// <summary>
        /// The elements containing block changed and it has properties which depend on it.
        /// </summary>
        Containing_Block_Changed = (1 << 3),
        /// <summary>
        /// The element has been repositioned by it's parent's layout system.
        /// </summary>
        Layout_Pos_Changed = (1 << 4),
        /// <summary>
        /// The elements scroll-offset changed.
        /// </summary>
        Scroll_Offset_Change = (1 << 5),
        /// <summary>
        /// The element's block values changed.
        /// </summary>
        Block_Changed = (1 << 6),
        /// <summary>
        /// A property value affecting the block changed
        /// </summary>
        Property_Changed = (1 << 7),
        /// <summary>
        /// The contents of the elemnt have changed
        /// </summary>
        Content_Changed = (1 << 8),
    }
}