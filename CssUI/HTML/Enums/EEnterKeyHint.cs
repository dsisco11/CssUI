using CssUI.Internal;

namespace CssUI.HTML
{
    /// <summary>
    /// The enterkeyhint content attribute is an enumerated attribute that specifies what action label (or icon) to present for the enter key on virtual keyboards. 
    /// This allows authors to customize the presentation of the enter key in order to make it more helpful for users.
    /// </summary>
    [MetaEnum]
    public enum EEnterKeyHint : int
    {/* Docs: https://html.spec.whatwg.org/multipage/interaction.html#input-modalities:-the-enterkeyhint-attribute */
        /// <summary>
        /// The user agent should present a cue for the operation 'enter', typically inserting a new line.
        /// </summary>
        [MetaKeyword("enter")]
        Enter,

        /// <summary>
        /// The user agent should present a cue for the operation 'done', typically meaning there is nothing more to input and the IME will be closed.
        /// </summary>
        [MetaKeyword("done")]
        Done,

        /// <summary>
        /// The user agent should present a cue for the operation 'go', typically meaning to take the user to the target of the text they typed.
        /// </summary>
        [MetaKeyword("go")]
        Go,

        /// <summary>
        /// The user agent should present a cue for the operation 'next', typically taking the user to the next field that will accept text.
        /// </summary>
        [MetaKeyword("next")]
        Next,

        /// <summary>
        /// The user agent should present a cue for the operation 'previous', typically taking the user to the previous field that will accept text.
        /// </summary>
        [MetaKeyword("previous")]
        Previous,

        /// <summary>
        /// The user agent should present a cue for the operation 'search', typically taking the user to the results of searching for the text they have typed.
        /// </summary>
        [MetaKeyword("search")]
        Search,

        /// <summary>
        /// The user agent should present a cue for the operation 'send', typically delivering the text to its target.
        /// </summary>
        [MetaKeyword("send")]
        Send,
    }
}
