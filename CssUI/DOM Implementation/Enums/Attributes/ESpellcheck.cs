using CssUI.DOM.Internal;

namespace CssUI.DOM
{
    [DomEnum]
    public enum ESpellcheck : int
    {/* Docs: https://html.spec.whatwg.org/multipage/interaction.html#spelling-and-grammar-checking */
        /// <summary>
        /// 
        /// </summary>
        [DomKeyword("")]
        Default = 0,

        /// <summary>
        /// 
        /// </summary>
        [DomKeyword("true")]
        True,

        /// <summary>
        /// 
        /// </summary>
        [DomKeyword("false")]
        False,
    }
}
