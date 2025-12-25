using EnumRecords;

namespace CssUI.CSS;

/// <summary>
/// Values for the align-items and align-self properties.
/// Spec: https://www.w3.org/TR/css-align-3/#propdef-align-items
/// </summary>
[EnumRecord<KeywordProperties>]
public enum EAlignItems
{
    /// <summary>
    /// Behaves as 'stretch' for flex items, 'start' for grid items.
    /// For align-self, computes to the parent's align-items value.
    /// </summary>
    [EnumData("auto")]
    Auto,

    /// <summary>
    /// Default alignment for the layout mode.
    /// </summary>
    [EnumData("normal")]
    Normal,

    /// <summary>
    /// Pack items toward the start of the alignment container.
    /// </summary>
    [EnumData("start")]
    Start,

    /// <summary>
    /// Pack items toward the end of the alignment container.
    /// </summary>
    [EnumData("end")]
    End,

    /// <summary>
    /// Align to the item's own start edge.
    /// </summary>
    [EnumData("self-start")]
    SelfStart,

    /// <summary>
    /// Align to the item's own end edge.
    /// </summary>
    [EnumData("self-end")]
    SelfEnd,

    /// <summary>
    /// Pack items toward the start of the flex container (flex-specific).
    /// </summary>
    [EnumData("flex-start")]
    FlexStart,

    /// <summary>
    /// Pack items toward the end of the flex container (flex-specific).
    /// </summary>
    [EnumData("flex-end")]
    FlexEnd,

    /// <summary>
    /// Pack items around the center.
    /// </summary>
    [EnumData("center")]
    Center,

    /// <summary>
    /// Stretch items to fill the container.
    /// </summary>
    [EnumData("stretch")]
    Stretch,

    /// <summary>
    /// Align items to their baseline.
    /// </summary>
    [EnumData("baseline")]
    Baseline
}

