using EnumRecords;

namespace CssUI.DOM;

[EnumRecord<KeywordProperties>]
public enum ESelectionType : int
{
    [EnumData("None")]
    None,
    [EnumData("Caret")]
    Caret,
    [EnumData("Range")]
    Range,
}

