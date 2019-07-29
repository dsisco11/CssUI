using CssUI.Internal;

namespace CssUI.DOM
{
    [MetaEnum]
    public enum EEncType : int
    {/* Docs: https://html.spec.whatwg.org/multipage/form-control-infrastructure.html#attr-fs-formenctype */

        /// <summary>
        /// 
        /// </summary>
        [MetaKeyword("application/x-www-form-urlencoded")]
        UrlEncoded,
        /// <summary>
        /// 
        /// </summary>
        [MetaKeyword("multipart/form-data")]
        FormData,
        /// <summary>
        /// 
        /// </summary>
        [MetaKeyword("text/plain")]
        Plain,

    }
}
