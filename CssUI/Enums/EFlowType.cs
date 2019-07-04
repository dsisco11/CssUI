
namespace CssUI.Enums
{
    public enum EFlowType : int
    {
        /// <summary>
        /// Ignored by layout
        /// </summary>
        INVALID = 0,
        /// <summary>
        /// During layout this element interrupts flow
        /// </summary>
        BLOCK = 1,
        /// <summary>
        /// During layout this element doesnt interrupt flow
        /// </summary>
        INLINE = 2,
    }
}
