using EnumRecords;

namespace CssUI.HTML
{

    [EnumRecord<KeywordProperties>]
    public enum EAreaShape : int
    {/* Docs: https://html.spec.whatwg.org/multipage/image-maps.html#attr-area-shape */

        [EnumData("default")]
        Default,

        [EnumData("circle")]
        Circle,

        [EnumData("poly")]
        Polygon,

        [EnumData("rect")]
        Rectangle,
    }
}

