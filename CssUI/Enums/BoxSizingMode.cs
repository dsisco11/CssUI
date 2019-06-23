

namespace CssUI
{
    /// <summary>
    /// Describes all of the box sizing modes <see cref="uiElement"/>'s can use
    /// </summary>
    public enum EBoxSizingMode : int
    {
        /// <summary>
        /// Width/Height describe the size of the content area, not including the padding, border, or margins
        /// </summary>
        CONTENT,
        /// <summary>
        /// Width/Height describe the border area, including the content size, padding, and border size but not the margins
        /// </summary>
        BORDER,
    }
}