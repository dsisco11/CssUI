using CssUI.DOM.Internal;

namespace CssUI.DOM
{
    /// <summary>
    /// Specifies the autocapitalization method for virtual keyboards (does not affect typing on physical keyboards)
    /// </summary>
    [DomEnum]
    public enum EAutoCapitalizationHint : int
    {/* Docs: https://html.spec.whatwg.org/multipage/interaction.html#autocapitalization */

        /// <summary>
        /// The user agent and input method should use make their own determination of whether or not to enable autocapitalization.
        /// </summary>
        [DomKeyword("")]
        Default,

        /// <summary>
        /// No autocapitalization should be applied (all letters should default to lowercase).
        /// </summary>
        [DomKeyword("off")]
        Off,

        /// <summary>
        /// No autocapitalization should be applied (all letters should default to lowercase).
        /// </summary>
        [DomKeyword("none")]
        None,

        /// <summary>
        /// The first letter of each sentence should default to a capital letter; all other letters should default to lowercase.
        /// </summary>
        [DomKeyword("on")]
        On,

        /// <summary>
        /// The first letter of each sentence should default to a capital letter; all other letters should default to lowercase.
        /// </summary>
        [DomKeyword("sentences")]
        Sentences,

        /// <summary>
        /// The first letter of each word should default to a capital letter; all other letters should default to lowercase.
        /// </summary>
        [DomKeyword("words")]
        Words,

        /// <summary>
        /// All letters should default to uppercase.
        /// </summary>
        [DomKeyword("characters")]
        Characters,
    }
}
