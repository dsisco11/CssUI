using EnumRecords;

namespace CssUI.CSS;

/// <summary>
/// Values for the flex-wrap property.
/// Spec: https://www.w3.org/TR/css-flexbox-1/#propdef-flex-wrap
/// </summary>
[EnumRecord<KeywordProperties>]
public enum EFlexWrap
{
    /// <summary>
    /// The flex container is single-line.
    /// </summary>
    [EnumRecordProperties("nowrap")]
    NoWrap,

    /// <summary>
    /// The flex container is multi-line.
    /// </summary>
    [EnumRecordProperties("wrap")]
    Wrap,

    /// <summary>
    /// Same as wrap, but the cross-start and cross-end directions are swapped.
    /// </summary>
    [EnumRecordProperties("wrap-reverse")]
    WrapReverse
}

