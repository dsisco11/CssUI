using CssUI.DOM.Internal;

namespace CssUI.DOM
{
    [DomEnum]
    public enum EContentEditable : int
    {
        [DomKeyword("true")]
        True,

        [DomKeyword("false")]
        False,

        [DomKeyword("inherit")]
        Inherit
    }
}
