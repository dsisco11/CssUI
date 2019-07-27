using CssUI.Internal;

namespace CssUI.DOM
{
    [MetaEnum]
    public enum EDirName : int
    {
        /// <summary>
        /// Indicates that the contents of the element are explicitly directionally isolated left-to-right text.
        /// </summary>
        [MetaKeyword("ltr")]
        Ltr = 1,

        /// <summary>
        /// Indicates that the contents of the element are explicitly directionally isolated right-to-left text.
        /// </summary>
        [MetaKeyword("rtl")]
        Rtl,

    }
}
