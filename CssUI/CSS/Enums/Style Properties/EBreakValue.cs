using EnumRecords;

namespace CssUI.CSS;

/// <summary>
/// Values for break-before, break-after, and break-inside properties.
/// Docs: https://www.w3.org/TR/css-break-3/#break-between
/// </summary>
[EnumRecord<KeywordProperties>]
public enum EBreakValue
{
    /// <summary>
    /// Neither force nor forbid a break before/after/inside the principal box.
    /// </summary>
    [EnumRecordProperties("auto")]
    Auto,

    /// <summary>
    /// Avoid a break before/after/inside the principal box.
    /// </summary>
    [EnumRecordProperties("avoid")]
    Avoid,

    /// <summary>
    /// Avoid a page break before/after/inside the principal box.
    /// </summary>
    [EnumRecordProperties("avoid-page")]
    AvoidPage,

    /// <summary>
    /// Avoid a column break before/after/inside the principal box.
    /// </summary>
    [EnumRecordProperties("avoid-column")]
    AvoidColumn,

    /// <summary>
    /// Avoid a region break before/after/inside the principal box.
    /// </summary>
    [EnumRecordProperties("avoid-region")]
    AvoidRegion,

    /// <summary>
    /// Always force a page break before/after the principal box.
    /// Only valid for break-before/break-after.
    /// </summary>
    [EnumRecordProperties("page")]
    Page,

    /// <summary>
    /// Always force a column break before/after the principal box.
    /// Only valid for break-before/break-after.
    /// </summary>
    [EnumRecordProperties("column")]
    Column,

    /// <summary>
    /// Always force a region break before/after the principal box.
    /// Only valid for break-before/break-after.
    /// </summary>
    [EnumRecordProperties("region")]
    Region,

    /// <summary>
    /// Force a page break before/after so the next page is a left page.
    /// Only valid for break-before/break-after.
    /// </summary>
    [EnumRecordProperties("left")]
    Left,

    /// <summary>
    /// Force a page break before/after so the next page is a right page.
    /// Only valid for break-before/break-after.
    /// </summary>
    [EnumRecordProperties("right")]
    Right,

    /// <summary>
    /// Force one or two page breaks before/after so the next page is a recto page.
    /// Only valid for break-before/break-after.
    /// </summary>
    [EnumRecordProperties("recto")]
    Recto,

    /// <summary>
    /// Force one or two page breaks before/after so the next page is a verso page.
    /// Only valid for break-before/break-after.
    /// </summary>
    [EnumRecordProperties("verso")]
    Verso
}
