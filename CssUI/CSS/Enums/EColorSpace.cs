using EnumRecords;

namespace CssUI.CSS;

/// <summary>
/// Specifies the color space for CSS color values.
/// </summary>
/// <remarks>
/// Docs: https://www.w3.org/TR/css-color-4/#color-syntax
/// </remarks>
[EnumRecord<KeywordProperties>]
public enum EColorSpace : byte
{
    /// <summary>
    /// Standard sRGB color space (gamma-corrected).
    /// </summary>
    [EnumRecordProperties("srgb")]
    sRGB = 0,

    /// <summary>
    /// Linear-light sRGB color space.
    /// </summary>
    [EnumRecordProperties("srgb-linear")]
    sRGBLinear = 1,

    /// <summary>
    /// Display P3 color space (wide gamut).
    /// </summary>
    [EnumRecordProperties("display-p3")]
    DisplayP3 = 2,

    /// <summary>
    /// CIE Lab color space.
    /// </summary>
    [EnumRecordProperties("lab")]
    Lab = 3,

    /// <summary>
    /// CIE LCH color space (cylindrical representation of Lab).
    /// </summary>
    [EnumRecordProperties("lch")]
    Lch = 4,

    /// <summary>
    /// OkLab perceptually uniform color space.
    /// </summary>
    [EnumRecordProperties("oklab")]
    OkLab = 5,

    /// <summary>
    /// OkLCH color space (cylindrical representation of OkLab).
    /// </summary>
    [EnumRecordProperties("oklch")]
    OkLCh = 6,

    /// <summary>
    /// Adobe RGB (1998) color space (wide gamut).
    /// </summary>
    /// <remarks>
    /// Docs: https://www.w3.org/TR/css-color-4/#predefined-a98-rgb
    /// </remarks>
    [EnumRecordProperties("a98-rgb")]
    A98Rgb = 7,

    /// <summary>
    /// ProPhoto RGB (ROMM RGB) color space (ultra-wide gamut, D50 white point).
    /// </summary>
    /// <remarks>
    /// Docs: https://www.w3.org/TR/css-color-4/#predefined-prophoto-rgb
    /// </remarks>
    [EnumRecordProperties("prophoto-rgb")]
    ProPhotoRgb = 8,

    /// <summary>
    /// ITU-R BT.2020-2 (Rec. 2020) color space (ultra-wide gamut for UHDTV).
    /// </summary>
    /// <remarks>
    /// Docs: https://www.w3.org/TR/css-color-4/#predefined-rec2020
    /// </remarks>
    [EnumRecordProperties("rec2020")]
    Rec2020 = 9,

    /// <summary>
    /// CIE XYZ color space with D50 white point.
    /// </summary>
    /// <remarks>
    /// Docs: https://www.w3.org/TR/css-color-4/#predefined-xyz
    /// </remarks>
    [EnumRecordProperties("xyz-d50")]
    XyzD50 = 10,

    /// <summary>
    /// CIE XYZ color space with D65 white point.
    /// </summary>
    /// <remarks>
    /// Docs: https://www.w3.org/TR/css-color-4/#predefined-xyz
    /// xyz is an alias for xyz-d65.
    /// </remarks>
    [EnumRecordProperties("xyz-d65")]
    XyzD65 = 11,
}
