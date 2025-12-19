using CssUI.Internal;

namespace CssUI.CSS;

/// <summary>
/// Values for break-before, break-after, and break-inside properties.
/// Docs: https://www.w3.org/TR/css-break-3/#break-between
/// </summary>
[MetaEnum]
public enum EBreakValue
{
    /// <summary>
    /// Neither force nor forbid a break before/after/inside the principal box.
    /// </summary>
    [MetaKeyword("auto")]
    Auto,

    /// <summary>
    /// Avoid a break before/after/inside the principal box.
    /// </summary>
    [MetaKeyword("avoid")]
    Avoid,

    /// <summary>
    /// Avoid a page break before/after/inside the principal box.
    /// </summary>
    [MetaKeyword("avoid-page")]
    AvoidPage,

    /// <summary>
    /// Avoid a column break before/after/inside the principal box.
    /// </summary>
    [MetaKeyword("avoid-column")]
    AvoidColumn,

    /// <summary>
    /// Avoid a region break before/after/inside the principal box.
    /// </summary>
    [MetaKeyword("avoid-region")]
    AvoidRegion,

    /// <summary>
    /// Always force a page break before/after the principal box.
    /// Only valid for break-before/break-after.
    /// </summary>
    [MetaKeyword("page")]
    Page,

    /// <summary>
    /// Always force a column break before/after the principal box.
    /// Only valid for break-before/break-after.
    /// </summary>
    [MetaKeyword("column")]
    Column,

    /// <summary>
    /// Always force a region break before/after the principal box.
    /// Only valid for break-before/break-after.
    /// </summary>
    [MetaKeyword("region")]
    Region,

    /// <summary>
    /// Force a page break before/after so the next page is a left page.
    /// Only valid for break-before/break-after.
    /// </summary>
    [MetaKeyword("left")]
    Left,

    /// <summary>
    /// Force a page break before/after so the next page is a right page.
    /// Only valid for break-before/break-after.
    /// </summary>
    [MetaKeyword("right")]
    Right,

    /// <summary>
    /// Force one or two page breaks before/after so the next page is a recto page.
    /// Only valid for break-before/break-after.
    /// </summary>
    [MetaKeyword("recto")]
    Recto,

    /// <summary>
    /// Force one or two page breaks before/after so the next page is a verso page.
    /// Only valid for break-before/break-after.
    /// </summary>
    [MetaKeyword("verso")]
    Verso
}
