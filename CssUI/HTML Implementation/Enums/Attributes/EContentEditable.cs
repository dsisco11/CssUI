using CssUI.Internal;

namespace CssUI.DOM
{
    [MetaEnum]
    public enum EContentEditable : short
    {/* Docs: https://html.spec.whatwg.org/multipage/interaction.html#attr-contenteditable */

        Invalid = -1,
        Missing = 0,
        [MetaKeyword("true")]
        True,
        [MetaKeyword("false")]
        False,
        [MetaKeyword("inherit")]
        Inherit
    }
}
