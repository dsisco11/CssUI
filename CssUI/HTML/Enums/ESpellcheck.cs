using EnumRecords;

namespace CssUI.HTML
{
    [EnumRecord<KeywordProperties>]
    public enum ESpellcheck : int
    {/* Docs: https://html.spec.whatwg.org/multipage/interaction.html#spelling-and-grammar-checking */
        /// <summary>
        ///
        /// </summary>
        [EnumData("")]
        Default = 0,

        /// <summary>
        ///
        /// </summary>
        [EnumData("true")]
        True,

        /// <summary>
        ///
        /// </summary>
        [EnumData("false")]
        False,
    }
}

