using EnumRecords;

namespace CssUI.CSS.Enums;

[EnumRecord<KeywordProperties>]
public enum EPseudoElement : int
{
    [EnumRecordProperties("::before")]
    Before,

    [EnumRecordProperties("::after")]
    After,

    [EnumRecordProperties("::first-letter")]
    First_Letter,

    [EnumRecordProperties("::first-line")]
    First_Line,

    [EnumRecordProperties("::marker")]
    Marker,

    [EnumRecordProperties("::placeholder")]
    Placeholder,
}

