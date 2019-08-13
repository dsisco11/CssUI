using CssUI.Internal;

namespace CssUI.CSS.Media
{
    /// <summary>
    /// Combinators specify how a media query determines if it's set of features cause it to match a given document
    /// </summary>
    [MetaEnum]
    public enum EMediaCombinator
    {
        None = 0x0,
        /// <summary>
        /// </summary>
        [MetaKeyword("and")]
        AND,

        /// <summary>
        /// Instantly returns true for a match
        /// </summary>
        [MetaKeyword("or")]
        OR,

        /// <summary>
        /// Negates the comparison result for a match
        /// </summary>
        [MetaKeyword("not")]
        NOT,
    }
}
