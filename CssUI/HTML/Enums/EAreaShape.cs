using EnumRecords;

namespace CssUI.HTML
{

    [EnumRecord<KeywordProperties>]
    public enum EAreaShape : int
    {/* Docs: https://html.spec.whatwg.org/multipage/image-maps.html#attr-area-shape */

        [EnumRecordProperties("default")]
        Default,

        [EnumRecordProperties("circle")]
        Circle,

        [EnumRecordProperties("poly")]
        Polygon,

        [EnumRecordProperties("rect")]
        Rectangle,
    }
}

