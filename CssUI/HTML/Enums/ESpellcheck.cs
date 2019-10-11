using CssUI.Internal;

namespace CssUI.HTML
{
    [MetaEnum]
    public enum ESpellcheck : int
    {/* Docs: https://html.spec.whatwg.org/multipage/interaction.html#spelling-and-grammar-checking */
        /// <summary>
        /// 
        /// </summary>
        [MetaKeyword("")]
        Default = 0,

        /// <summary>
        /// 
        /// </summary>
        [MetaKeyword("true")]
        True,

        /// <summary>
        /// 
        /// </summary>
        [MetaKeyword("false")]
        False,
    }
}
