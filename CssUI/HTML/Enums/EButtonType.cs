using EnumRecords;

namespace CssUI.HTML
{
    [EnumRecord<KeywordProperties>]
    public enum EButtonType : int
    {/* Docs: https://html.spec.whatwg.org/multipage/form-elements.html#attr-button-type */
        /// <summary>
        /// Submits the form
        /// </summary>
        [EnumRecordProperties("submit")]
        Submit,

        /// <summary>
        /// Resets the form
        /// </summary>
        [EnumRecordProperties("reset")]
        Reset,

        /// <summary>
        /// Does nothing
        /// </summary>
        [EnumRecordProperties("button")]
        Button,
    }
}

