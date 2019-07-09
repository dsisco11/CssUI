using CssUI.DOM.Internal;

namespace CssUI.DOM.Enums
{
    [DomEnum]
    public enum EContentEditable : short
    {/* Docs: https://html.spec.whatwg.org/multipage/interaction.html#attr-contenteditable */

        Invalid = -1,
        Missing = 0,
        [DomKeyword("true")]
        True,
        [DomKeyword("false")]
        False,
        [DomKeyword("inherit")]
        Inherit
    }
}
