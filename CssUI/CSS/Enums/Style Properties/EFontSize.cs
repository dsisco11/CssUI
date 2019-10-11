using CssUI.Internal;

namespace CssUI.CSS
{
    [MetaEnum]
    public enum EFontSize : int
    {/* Docs: https://www.w3.org/TR/css-fonts-3/#font-size-prop */

        /* ABSOLUTE SIZES */
        /// <summary>
        /// 
        /// </summary>
        [MetaKeyword("xx-small")]
        XXSmall = 0,

        /// <summary>
        /// 
        /// </summary>
        [MetaKeyword("x-small")]
        XSmall,

        /// <summary>
        /// 
        /// </summary>
        [MetaKeyword("small")]
        Small,

        /// <summary>
        /// 
        /// </summary>
        [MetaKeyword("medium")]
        Medium,

        /// <summary>
        /// 
        /// </summary>
        [MetaKeyword("large")]
        Large,

        /// <summary>
        /// 
        /// </summary>
        [MetaKeyword("x-large")]
        XLarge,

        /// <summary>
        /// 
        /// </summary>
        [MetaKeyword("xx-large")]
        XXLarge,


        /* RELATIVE SIZES */


        /// <summary>
        /// 
        /// </summary>
        [MetaKeyword("smaller")]
        Smaller,

        /// <summary>
        /// 
        /// </summary>
        [MetaKeyword("larger")]
        Larger,

    }
}
