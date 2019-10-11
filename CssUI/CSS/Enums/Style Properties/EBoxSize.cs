using CssUI.Internal;

namespace CssUI.CSS
{
    [MetaEnum]
    public enum EBoxSize
    {
        //Auto,// This is already just defined as a special, reserved CssValue TYPE
        //None,// This is already just defined as a special, reserved CssValue TYPE
        /// <summary>
        /// 
        /// </summary>
        [MetaKeyword("min-content")]
        Min_Content,

        /// <summary>
        /// 
        /// </summary>
        [MetaKeyword("max-content")]
        Max_Content,

        /// <summary>
        /// 
        /// </summary>
        [MetaKeyword("fit-content")]
        Fit_Content,
    }
}
