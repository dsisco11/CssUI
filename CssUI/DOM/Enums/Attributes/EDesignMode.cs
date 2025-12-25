using EnumRecords;

namespace CssUI.DOM;

[EnumRecord<KeywordProperties>]
public enum EDesignMode : int
{/* Docs: https://html.spec.whatwg.org/multipage/interaction.html#designMode */

    [EnumRecordProperties("on")]
    ON,

    [EnumRecordProperties("off")]
    OFF,
}

