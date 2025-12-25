using EnumRecords;

namespace CssUI.HTML
{
    [EnumRecord<KeywordProperties>]
    public enum EBrowsingTarget : int
    {/* Docs:  */

        /// <summary>
        /// </summary>
        [EnumRecordProperties("_blank")]
        Blank,
        /// <summary>
        /// </summary>
        [EnumRecordProperties("_self")]
        Self,
        /// <summary>
        /// </summary>
        [EnumRecordProperties("_parent")]
        Parent,
        /// <summary>
        /// </summary>
        [EnumRecordProperties("_top")]
        Top,

    }
}

