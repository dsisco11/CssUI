using CssUI.Internal;

namespace CssUI.DOM
{
    [MetaEnum]
    public enum EDesignMode : int
    {/* Docs: https://html.spec.whatwg.org/multipage/interaction.html#designMode */

        [MetaKeyword("on")]
        ON,

        [MetaKeyword("off")]
        OFF,
    }
}
