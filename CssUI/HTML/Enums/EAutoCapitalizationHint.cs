using EnumRecords;

namespace CssUI.HTML
{
    /// <summary>
    /// Specifies the autocapitalization method for virtual keyboards (does not affect typing on physical keyboards)
    /// </summary>
    [EnumRecord<KeywordProperties>]
    public enum EAutoCapitalizationHint : int
    {/* Docs: https://html.spec.whatwg.org/multipage/interaction.html#autocapitalization */

        /// <summary>
        /// The user agent and input method should use make their own determination of whether or not to enable autocapitalization.
        /// </summary>
        [EnumRecordProperties("")]
        Default,

        /// <summary>
        /// No autocapitalization should be applied (all letters should default to lowercase).
        /// </summary>
        [EnumRecordProperties("off")]
        Off,

        /// <summary>
        /// No autocapitalization should be applied (all letters should default to lowercase).
        /// </summary>
        [EnumRecordProperties("none")]
        None,

        /// <summary>
        /// The first letter of each sentence should default to a capital letter; all other letters should default to lowercase.
        /// </summary>
        [EnumRecordProperties("on")]
        On,

        /// <summary>
        /// The first letter of each sentence should default to a capital letter; all other letters should default to lowercase.
        /// </summary>
        [EnumRecordProperties("sentences")]
        Sentences,

        /// <summary>
        /// The first letter of each word should default to a capital letter; all other letters should default to lowercase.
        /// </summary>
        [EnumRecordProperties("words")]
        Words,

        /// <summary>
        /// All letters should default to uppercase.
        /// </summary>
        [EnumRecordProperties("characters")]
        Characters,
    }
}

