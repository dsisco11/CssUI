using EnumRecords;

namespace CssUI.CSS;

/// <summary>
/// </summary>
[EnumRecord<KeywordProperties>]
public enum EPosition : int
{/* Docs: https://www.w3.org/TR/css-backgrounds-3/#propdef-background-position */

    /// <summary>
    /// </summary>
    [EnumRecordProperties("left")]
    Left,
    /// <summary>
    /// </summary>
    [EnumRecordProperties("center")]
    Center,
    /// <summary>
    /// </summary>
    [EnumRecordProperties("right")]
    Right,
    /// <summary>
    /// </summary>
    [EnumRecordProperties("top")]
    Top,
    /// <summary>
    /// </summary>
    [EnumRecordProperties("bottom")]
    Bottom,
}
