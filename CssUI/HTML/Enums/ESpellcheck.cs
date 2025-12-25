using EnumRecords;

namespace CssUI.HTML
{
    [EnumRecord<KeywordProperties>]
    public enum ESpellcheck : int
    {/* Docs: https://html.spec.whatwg.org/multipage/interaction.html#spelling-and-grammar-checking */
        /// <summary>
        /// 
        /// </summary>
        [EnumRecordProperties("")]
        Default = 0,

        /// <summary>
        /// 
        /// </summary>
        [EnumRecordProperties("true")]
        True,

        /// <summary>
        /// 
        /// </summary>
        [EnumRecordProperties("false")]
        False,
    }
}

