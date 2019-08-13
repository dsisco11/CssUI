using CssUI.Internal;

namespace CssUI.CSS
{
    [MetaEnum]
    public enum EFontWeight : int
    {/* DOcs: https://www.w3.org/TR/2018/REC-css-fonts-3-20180920/#font-weight-prop */
        /// <summary>
        /// Specifies a lighter weight than the inherited value.
        /// </summary>
        [MetaKeyword("lighter")]
        Lighter,

        /// <summary>
        /// Same as '400'
        /// </summary>
        [MetaKeyword("normal")]
        Normal,

        /// <summary>
        /// Same as '700'
        /// </summary>
        [MetaKeyword("bold")]
        Bold,

        /// <summary>
        /// Specifies a bolder weight than the inherited value.
        /// </summary>
        [MetaKeyword("bolder")]
        Bolder,
    }
}
