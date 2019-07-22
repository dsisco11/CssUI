using CssUI.DOM.Internal;

namespace CssUI.DOM
{
    /// <summary>
    /// The enterkeyhint content attribute is an enumerated attribute that specifies what action label (or icon) to present for the enter key on virtual keyboards. 
    /// This allows authors to customize the presentation of the enter key in order to make it more helpful for users.
    /// </summary>
    [DomEnum]
    public enum EEnterKeyHint : int
    {
        /// <summary>
        /// The user agent should present a cue for the operation 'enter', typically inserting a new line.
        /// </summary>
        [DomKeyword("enter")]
        Enter,

        /// <summary>
        /// The user agent should present a cue for the operation 'done', typically meaning there is nothing more to input and the IME will be closed.
        /// </summary>
        [DomKeyword("done")]
        Done,

        /// <summary>
        /// The user agent should present a cue for the operation 'go', typically meaning to take the user to the target of the text they typed.
        /// </summary>
        [DomKeyword("go")]
        Go,

        /// <summary>
        /// The user agent should present a cue for the operation 'next', typically taking the user to the next field that will accept text.
        /// </summary>
        [DomKeyword("next")]
        Next,

        /// <summary>
        /// The user agent should present a cue for the operation 'previous', typically taking the user to the previous field that will accept text.
        /// </summary>
        [DomKeyword("previous")]
        Previous,

        /// <summary>
        /// The user agent should present a cue for the operation 'search', typically taking the user to the results of searching for the text they have typed.
        /// </summary>
        [DomKeyword("search")]
        Search,

        /// <summary>
        /// The user agent should present a cue for the operation 'send', typically delivering the text to its target.
        /// </summary>
        [DomKeyword("send")]
        Send,
    }
}
