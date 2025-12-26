using EnumRecords;

namespace CssUI.HTML
{
    [EnumRecord<KeywordProperties>]
    public enum EContentEditable : short
    {/* Docs: https://html.spec.whatwg.org/multipage/interaction.html#attr-contenteditable */

        [Ignore]
        Invalid = -1,
        [Ignore]
        Missing = 0,
        [EnumData("true")]
        True,
        [EnumData("false")]
        False,
        [EnumData("inherit")]
        Inherit
    }
}

