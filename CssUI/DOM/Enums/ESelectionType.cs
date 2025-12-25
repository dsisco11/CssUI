using EnumRecords;

namespace CssUI.DOM;

[EnumRecord<KeywordProperties>]
public enum ESelectionType : int
{
    [EnumRecordProperties("None")]
    None,
    [EnumRecordProperties("Caret")]
    Caret,
    [EnumRecordProperties("Range")]
    Range,
}

