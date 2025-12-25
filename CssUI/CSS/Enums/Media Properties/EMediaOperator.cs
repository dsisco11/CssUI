using EnumRecords;

namespace CssUI.CSS.Media;

/// <summary>
/// Describes the comparison logic for media a feature
/// </summary>
[EnumRecord<KeywordProperties>]
public enum EMediaOperator : int
{
    [EnumRecordProperties("<")]
    LessThan,
    [EnumRecordProperties("=")]
    EqualTo,
    [EnumRecordProperties(">")]
    GreaterThan,

    [EnumRecordProperties("<=")]
    LessThanEq,
    [EnumRecordProperties(">=")]
    GreaterThanEq,
}

