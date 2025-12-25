using EnumRecords;

namespace CssUI.CSS;

/// <summary>
/// Values for the flex-direction property.
/// Spec: https://www.w3.org/TR/css-flexbox-1/#propdef-flex-direction
/// </summary>
[EnumRecord<KeywordProperties>]
public enum EFlexDirection
{
    /// <summary>
    /// The flex container's main axis has the same orientation as the inline axis of the current writing mode.
    /// </summary>
    [EnumData("row")]
    Row,

    /// <summary>
    /// Same as row, but the main-start and main-end directions are swapped.
    /// </summary>
    [EnumData("row-reverse")]
    RowReverse,

    /// <summary>
    /// The flex container's main axis has the same orientation as the block axis of the current writing mode.
    /// </summary>
    [EnumData("column")]
    Column,

    /// <summary>
    /// Same as column, but the main-start and main-end directions are swapped.
    /// </summary>
    [EnumData("column-reverse")]
    ColumnReverse
}

