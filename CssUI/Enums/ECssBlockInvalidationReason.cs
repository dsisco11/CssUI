
namespace CssUI
{
    /// <summary>
    /// Specifies all of the reasons for which a cssElement's block may be flagged as dirty.
    /// </summary>
    public enum ECssBlockInvalidationReason
    {
        /// <summary>
        /// </summary>
        Unknown,
        /// <summary>
        /// The element has changed parents.
        /// </summary>
        Reparented,
        /// <summary>
        /// The elements containing block changed and it has properties which depend on it.
        /// </summary>
        Containing_Block_Changed,
        /// <summary>
        /// The element has been repositioned by it's parent's layout system.
        /// </summary>
        Layout_Pos_Changed,
        /// <summary>
        /// The elements scroll-offset changed.
        /// </summary>
        Scroll_Offset_Change,
        /// <summary>
        /// The element's block values changed.
        /// </summary>
        Block_Changed,
    }
}