using EnumRecords;

namespace CssUI.HTML
{
    [EnumRecord<KeywordProperties>]
    public enum EContentEditable : short
    {/* Docs: https://html.spec.whatwg.org/multipage/interaction.html#attr-contenteditable */

        Invalid = -1,
        Missing = 0,
        [EnumData("true")]
        True,
        [EnumData("false")]
        False,
        [EnumData("inherit")]
        Inherit
    }
}

