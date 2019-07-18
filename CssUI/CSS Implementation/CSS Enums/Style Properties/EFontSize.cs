using CssUI.CSS.Internal;

namespace CssUI.CSS
{
    [CssEnum]
    public enum EFontSize : int
    {/* Docs: https://www.w3.org/TR/css-fonts-3/#font-size-prop */

        /* ABSOLUTE SIZES */
        /// <summary>
        /// 
        /// </summary>
        [CssKeyword("xx-small")]
        XXSmall = 0,

        /// <summary>
        /// 
        /// </summary>
        [CssKeyword("x-small")]
        XSmall,

        /// <summary>
        /// 
        /// </summary>
        [CssKeyword("small")]
        Small,

        /// <summary>
        /// 
        /// </summary>
        [CssKeyword("medium")]
        Medium,

        /// <summary>
        /// 
        /// </summary>
        [CssKeyword("large")]
        Large,

        /// <summary>
        /// 
        /// </summary>
        [CssKeyword("x-large")]
        XLarge,

        /// <summary>
        /// 
        /// </summary>
        [CssKeyword("xx-large")]
        XXLarge,


        /* RELATIVE SIZES */


        /// <summary>
        /// 
        /// </summary>
        [CssKeyword("smaller")]
        Smaller,

        /// <summary>
        /// 
        /// </summary>
        [CssKeyword("larger")]
        Larger,

    }
}
