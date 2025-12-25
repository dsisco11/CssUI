using EnumRecords;

namespace CssUI.CSS;

/// <summary>
/// Values for box-decoration-break property.
/// Docs: https://www.w3.org/TR/css-break-3/#break-decoration
/// </summary>
[EnumRecord<KeywordProperties>]
public enum EBoxDecorationBreak
{
    /// <summary>
    /// Box decorations are sliced at fragment boundaries.
    /// Each fragment is rendered as if the box were sliced at the break.
    /// </summary>
    [EnumRecordProperties("slice")]
    Slice,

    /// <summary>
    /// Each box fragment is independently wrapped with the border, padding, and background.
    /// </summary>
    [EnumRecordProperties("clone")]
    Clone
}
