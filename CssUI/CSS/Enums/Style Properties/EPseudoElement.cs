using EnumRecords;

namespace CssUI.CSS.Enums;

[EnumRecord<KeywordProperties>]
public enum EPseudoElement : int
{
    [EnumData("::before")]
    Before,

    [EnumData("::after")]
    After,

    [EnumData("::first-letter")]
    First_Letter,

    [EnumData("::first-line")]
    First_Line,

    [EnumData("::marker")]
    Marker,

    [EnumData("::placeholder")]
    Placeholder,
}

