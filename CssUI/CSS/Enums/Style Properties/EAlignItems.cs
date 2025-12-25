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
    [EnumRecordProperties("auto")]
    Auto,

    /// <summary>
    /// Default alignment for the layout mode.
    /// </summary>
    [EnumRecordProperties("normal")]
    Normal,

    /// <summary>
    /// Pack items toward the start of the alignment container.
    /// </summary>
    [EnumRecordProperties("start")]
    Start,

    /// <summary>
    /// Pack items toward the end of the alignment container.
    /// </summary>
    [EnumRecordProperties("end")]
    End,

    /// <summary>
    /// Align to the item's own start edge.
    /// </summary>
    [EnumRecordProperties("self-start")]
    SelfStart,

    /// <summary>
    /// Align to the item's own end edge.
    /// </summary>
    [EnumRecordProperties("self-end")]
    SelfEnd,

    /// <summary>
    /// Pack items toward the start of the flex container (flex-specific).
    /// </summary>
    [EnumRecordProperties("flex-start")]
    FlexStart,

    /// <summary>
    /// Pack items toward the end of the flex container (flex-specific).
    /// </summary>
    [EnumRecordProperties("flex-end")]
    FlexEnd,

    /// <summary>
    /// Pack items around the center.
    /// </summary>
    [EnumRecordProperties("center")]
    Center,

    /// <summary>
    /// Stretch items to fill the container.
    /// </summary>
    [EnumRecordProperties("stretch")]
    Stretch,

    /// <summary>
    /// Align items to their baseline.
    /// </summary>
    [EnumRecordProperties("baseline")]
    Baseline
}

