using EnumRecords;

namespace CssUI.CSS;

/// <summary>
///
/// </summary>
/// Docs: https://www.w3.org/TR/css-writing-modes-4/#logical-to-physical
[EnumRecord<KeywordProperties>]
public enum EWritingMode : int
{
    [EnumData("horizontal-tb")]
    Horizontal_TB = 1,
    [EnumData("vertical-rl")]
    Vertical_RL,
    [EnumData("vertical-lr")]
    Vertical_LR,
    [EnumData("sideways-rl")]
    Sideways_RL,
    [EnumData("sideways-lr")]
    Sideways_LR,
}

