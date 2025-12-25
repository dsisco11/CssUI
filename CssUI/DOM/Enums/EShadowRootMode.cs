using EnumRecords;

namespace CssUI.DOM.Enums;

[EnumRecord<KeywordProperties>]
public enum EShadowRootMode : int
{
    [EnumData("open")]
    Open,
    [EnumData("closed")]
    Closed,
}

