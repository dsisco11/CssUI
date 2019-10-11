using CssUI.Internal;

namespace CssUI.CSS
{
    /// <summary>
    /// </summary>
    [MetaEnum]
    public enum EPosition : int
    {/* Docs: https://www.w3.org/TR/css-backgrounds-3/#propdef-background-position */

        /// <summary>
        /// </summary>
        [MetaKeyword("left")]
        Left,
        /// <summary>
        /// </summary>
        [MetaKeyword("center")]
        Center,
        /// <summary>
        /// </summary>
        [MetaKeyword("right")]
        Right,
        /// <summary>
        /// </summary>
        [MetaKeyword("top")]
        Top,
        /// <summary>
        /// </summary>
        [MetaKeyword("bottom")]
        Bottom,
    }
}