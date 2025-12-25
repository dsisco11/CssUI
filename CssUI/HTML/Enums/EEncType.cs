using EnumRecords;

namespace CssUI.HTML
{
    [EnumRecord<KeywordProperties>]
    public enum EEncType : int
    {/* Docs: https://html.spec.whatwg.org/multipage/form-control-infrastructure.html#attr-fs-formenctype */

        /// <summary>
        /// 
        /// </summary>
        [EnumRecordProperties("application/x-www-form-urlencoded")]
        UrlEncoded,
        /// <summary>
        /// 
        /// </summary>
        [EnumRecordProperties("multipart/form-data")]
        FormData,
        /// <summary>
        /// 
        /// </summary>
        [EnumRecordProperties("text/plain")]
        Plain,

    }
}

