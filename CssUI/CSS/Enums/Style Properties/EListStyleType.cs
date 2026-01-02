using EnumRecords;

namespace CssUI.CSS.Enums;

/// <summary>
/// Specifies the marker string, which is used to fill the list item's marker.
/// Spec: https://www.w3.org/TR/css-lists-3/#text-markers
/// </summary>
[EnumRecord<KeywordProperties>]
public enum EListStyleType : int
{
    /// <summary>
    /// The element has no marker string.
    /// </summary>
    [EnumData("none")]
    None,

    #region Unordered List Styles (Bullets)
    /// <summary>
    /// A filled circle (•)
    /// </summary>
    [EnumData("disc")]
    Disc,

    /// <summary>
    /// A hollow circle (○)
    /// </summary>
    [EnumData("circle")]
    Circle,

    /// <summary>
    /// A filled square (■)
    /// </summary>
    [EnumData("square")]
    Square,
    #endregion

    #region Ordered List Styles (Numbers)
    /// <summary>
    /// Western decimal numbers (1, 2, 3, ...)
    /// </summary>
    [EnumData("decimal")]
    Decimal,

    /// <summary>
    /// Decimal numbers padded with initial zeros (01, 02, 03, ...)
    /// </summary>
    [EnumData("decimal-leading-zero")]
    DecimalLeadingZero,

    /// <summary>
    /// Lowercase ASCII letters (a, b, c, ... z, aa, ab, ...)
    /// </summary>
    [EnumData("lower-alpha")]
    LowerAlpha,

    /// <summary>
    /// Lowercase ASCII letters (a, b, c, ... z, aa, ab, ...)
    /// Same as lower-alpha
    /// </summary>
    [EnumData("lower-latin")]
    LowerLatin,

    /// <summary>
    /// Uppercase ASCII letters (A, B, C, ... Z, AA, AB, ...)
    /// </summary>
    [EnumData("upper-alpha")]
    UpperAlpha,

    /// <summary>
    /// Uppercase ASCII letters (A, B, C, ... Z, AA, AB, ...)
    /// Same as upper-alpha
    /// </summary>
    [EnumData("upper-latin")]
    UpperLatin,

    /// <summary>
    /// Lowercase Roman numerals (i, ii, iii, iv, v, ...)
    /// </summary>
    [EnumData("lower-roman")]
    LowerRoman,

    /// <summary>
    /// Uppercase Roman numerals (I, II, III, IV, V, ...)
    /// </summary>
    [EnumData("upper-roman")]
    UpperRoman,

    /// <summary>
    /// Lowercase classical Greek (α, β, γ, ...)
    /// </summary>
    [EnumData("lower-greek")]
    LowerGreek,
    #endregion
}
