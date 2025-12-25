using EnumRecords;

namespace CssUI.DOM.Enums;

[EnumRecord<KeywordProperties>]
public enum EShadowRootMode : int
{
    [EnumRecordProperties("open")]
    Open,
    [EnumRecordProperties("closed")]
    Closed,
}

