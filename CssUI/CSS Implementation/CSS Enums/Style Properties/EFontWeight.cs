using CssUI.CSS.Internal;

namespace CssUI.CSS
{
    [CssEnum]
    public enum EFontWeight : int
    {/* DOcs: https://www.w3.org/TR/2018/REC-css-fonts-3-20180920/#font-weight-prop */
        /// <summary>
        /// Specifies a lighter weight than the inherited value.
        /// </summary>
        [CssKeyword("lighter")]
        Lighter,

        /// <summary>
        /// Same as '400'
        /// </summary>
        [CssKeyword("normal")]
        Normal,

        /// <summary>
        /// Same as '700'
        /// </summary>
        [CssKeyword("bold")]
        Bold,

        /// <summary>
        /// Specifies a bolder weight than the inherited value.
        /// </summary>
        [CssKeyword("bolder")]
        Bolder,
    }
}
