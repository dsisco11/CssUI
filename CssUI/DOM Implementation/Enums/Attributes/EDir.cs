using CssUI.DOM.Internal;

namespace CssUI.DOM
{
    [DomEnum]
    public enum EDir : int
    {
        /// <summary>
        /// Indicates that the contents of the element are explicitly directionally isolated left-to-right text.
        /// </summary>
        [DomKeyword("ltr")]
        Ltr = 1,

        /// <summary>
        /// Indicates that the contents of the element are explicitly directionally isolated right-to-left text.
        /// </summary>
        [DomKeyword("rtl")]
        Rtl,

        /// <summary>
        /// Indicates that the contents of the element are explicitly directionally isolated text, but that the direction is to be determined programmatically using the contents of the element
        /// </summary>
        [DomKeyword("auto")]
        Auto,

    }
}
