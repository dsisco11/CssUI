using EnumRecords;

namespace CssUI.CSS;

/// <summary>
/// </summary>
[EnumRecord<KeywordProperties>]
public enum EPosition : int
{/* Docs: https://www.w3.org/TR/css-backgrounds-3/#propdef-background-position */

    /// <summary>
    /// </summary>
    [EnumData("left")]
    Left,
    /// <summary>
    /// </summary>
    [EnumData("center")]
    Center,
    /// <summary>
    /// </summary>
    [EnumData("right")]
    Right,
    /// <summary>
    /// </summary>
    [EnumData("top")]
    Top,
    /// <summary>
    /// </summary>
    [EnumData("bottom")]
    Bottom,
}
