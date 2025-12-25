using EnumRecords;

namespace CssUI.DOM;

[EnumRecord<KeywordProperties>]
public enum EDesignMode : int
{/* Docs: https://html.spec.whatwg.org/multipage/interaction.html#designMode */

    [EnumData("on")]
    ON,

    [EnumData("off")]
    OFF,
}

