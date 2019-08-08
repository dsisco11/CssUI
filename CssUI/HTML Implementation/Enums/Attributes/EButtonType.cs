using CssUI.Internal;

namespace CssUI.DOM
{
    [MetaEnum]
    public enum EButtonType : int
    {/* Docs: https://html.spec.whatwg.org/multipage/form-elements.html#attr-button-type */
        /// <summary>
        /// Submits the form
        /// </summary>
        [MetaKeyword("submit")]
        Submit,

        /// <summary>
        /// Resets the form
        /// </summary>
        [MetaKeyword("reset")]
        Reset,

        /// <summary>
        /// Does nothing
        /// </summary>
        [MetaKeyword("button")]
        Button,
    }
}
