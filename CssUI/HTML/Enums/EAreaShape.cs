using CssUI.Internal;

namespace CssUI.HTML
{

    [MetaEnum]
    public enum EAreaShape : int
    {/* Docs: https://html.spec.whatwg.org/multipage/image-maps.html#attr-area-shape */

        [MetaKeyword("default")]
        Default,

        [MetaKeyword("circle")]
        Circle,

        [MetaKeyword("poly")]
        Polygon,

        [MetaKeyword("rect")]
        Rectangle,
    }
}
