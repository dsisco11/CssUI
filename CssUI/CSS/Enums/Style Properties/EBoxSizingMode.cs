using CssUI.Internal;

namespace CssUI.CSS
{
    /// <summary>
    /// Describes all of the CSS box sizing modes used by the 'box-sizing' property
    /// </summary>
    [MetaEnum]
    public enum EBoxSizingMode : int
    {
        /// <summary>
        /// Width/Height describe the size of the content area, not including the padding, border, or margins
        /// </summary>
        [MetaKeyword("content-box")]
        ContentBox,
        /// <summary>
        /// Width/Height describe the border area, including the content size, padding, and border size but not the margins
        /// </summary>
        [MetaKeyword("border-box")]
        BorderBox,
    }
}