using EnumRecords;

namespace CssUI.CSS;

/// <summary>
/// Values for the align-content property.
/// Spec: https://www.w3.org/TR/css-align-3/#propdef-align-content
/// </summary>
[EnumRecord<KeywordProperties>]
public enum EAlignContent
{
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
    /// Distribute items evenly, first item at start, last at end.
    /// </summary>
    [EnumData("space-between")]
    SpaceBetween,

    /// <summary>
    /// Distribute items evenly with equal space around them.
    /// </summary>
    [EnumData("space-around")]
    SpaceAround,

    /// <summary>
    /// Distribute items evenly with equal space between them.
    /// </summary>
    [EnumData("space-evenly")]
    SpaceEvenly,

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

