
namespace CssUI.Enums
{
    /// <summary>
    /// Describes how to calculate the boundaries of a <see cref="CssBox"/>
    /// </summary>
    internal enum EBoxDisplayGroup : int
    {
        INVALID = 0,
        /// <summary>
        /// This box does not break flow, it's size is determined by it's contents
        /// </summary>
        INLINE,
        /// <summary>
        /// This box breaks flow, and it's size is determined by CSS properties
        /// </summary>
        BLOCK,
        /// <summary>
        /// This box does not break flow, but it's size is determined by CSS properties
        /// </summary>
        INLINE_BLOCK,

        FLOATING,
        /// <summary>
        /// This box's position is not determines by the flow system, it is specified by CSS properties
        /// </summary>
        ABSOLUTELY_POSITIONED

    }
}
