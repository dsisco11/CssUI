using EnumRecords;

namespace CssUI.CSS;

/// <summary>
/// 
/// </summary>
/// Docs: https://www.w3.org/TR/css-writing-modes-4/#logical-to-physical
[EnumRecord<KeywordProperties>]
public enum EWritingMode : int
{
    [EnumRecordProperties("horizontal-tb")]
    Horizontal_TB = 1,
    [EnumRecordProperties("vertical-rl")]
    Vertical_RL,
    [EnumRecordProperties("vertical-lr")]
    Vertical_LR,
    [EnumRecordProperties("sideways-rl")]
    Sideways_RL,
    [EnumRecordProperties("sideways-lr")]
    Sideways_LR,
}

