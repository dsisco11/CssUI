using EnumRecords;

namespace CssUI.HTML
{
    [EnumRecord<KeywordProperties>]
    public enum EContentEditable : short
    {/* Docs: https://html.spec.whatwg.org/multipage/interaction.html#attr-contenteditable */

        Invalid = -1,
        Missing = 0,
        [EnumRecordProperties("true")]
        True,
        [EnumRecordProperties("false")]
        False,
        [EnumRecordProperties("inherit")]
        Inherit
    }
}

