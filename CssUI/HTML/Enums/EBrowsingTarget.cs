using EnumRecords;

namespace CssUI.HTML
{
    [EnumRecord<KeywordProperties>]
    public enum EBrowsingTarget : int
    {/* Docs:  */

        /// <summary>
        /// </summary>
        [EnumData("_blank")]
        Blank,
        /// <summary>
        /// </summary>
        [EnumData("_self")]
        Self,
        /// <summary>
        /// </summary>
        [EnumData("_parent")]
        Parent,
        /// <summary>
        /// </summary>
        [EnumData("_top")]
        Top,

    }
}

