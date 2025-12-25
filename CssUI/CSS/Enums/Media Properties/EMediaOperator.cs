using EnumRecords;

namespace CssUI.CSS.Media;

/// <summary>
/// Describes the comparison logic for media a feature
/// </summary>
[EnumRecord<KeywordProperties>]
public enum EMediaOperator : int
{
    [EnumData("<")]
    LessThan,
    [EnumData("=")]
    EqualTo,
    [EnumData(">")]
    GreaterThan,

    [EnumData("<=")]
    LessThanEq,
    [EnumData(">=")]
    GreaterThanEq,
}

