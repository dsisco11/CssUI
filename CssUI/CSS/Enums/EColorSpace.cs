using CssUI.Internal;

namespace CssUI.CSS;

/// <summary>
/// Specifies the color space for CSS color values.
/// </summary>
/// <remarks>
/// Docs: https://www.w3.org/TR/css-color-4/#color-syntax
/// </remarks>
[MetaEnum]
public enum EColorSpace : byte
{
    /// <summary>
    /// Standard sRGB color space (gamma-corrected).
    /// </summary>
    [MetaKeyword("srgb")]
    sRGB = 0,

    /// <summary>
    /// Linear-light sRGB color space.
    /// </summary>
    [MetaKeyword("srgb-linear")]
    sRGBLinear = 1,

    /// <summary>
    /// Display P3 color space (wide gamut).
    /// </summary>
    [MetaKeyword("display-p3")]
    DisplayP3 = 2,

    /// <summary>
    /// CIE Lab color space.
    /// </summary>
    [MetaKeyword("lab")]
    Lab = 3,

    /// <summary>
    /// CIE LCH color space (cylindrical representation of Lab).
    /// </summary>
    [MetaKeyword("lch")]
    Lch = 4,

    /// <summary>
    /// OkLab perceptually uniform color space.
    /// </summary>
    [MetaKeyword("oklab")]
    OkLab = 5,

    /// <summary>
    /// OkLCH color space (cylindrical representation of OkLab).
    /// </summary>
    [MetaKeyword("oklch")]
    OkLCh = 6,

    /// <summary>
    /// Adobe RGB (1998) color space (wide gamut).
    /// </summary>
    /// <remarks>
    /// Docs: https://www.w3.org/TR/css-color-4/#predefined-a98-rgb
    /// </remarks>
    [MetaKeyword("a98-rgb")]
    A98Rgb = 7,

    /// <summary>
    /// ProPhoto RGB (ROMM RGB) color space (ultra-wide gamut, D50 white point).
    /// </summary>
    /// <remarks>
    /// Docs: https://www.w3.org/TR/css-color-4/#predefined-prophoto-rgb
    /// </remarks>
    [MetaKeyword("prophoto-rgb")]
    ProPhotoRgb = 8,

    /// <summary>
    /// ITU-R BT.2020-2 (Rec. 2020) color space (ultra-wide gamut for UHDTV).
    /// </summary>
    /// <remarks>
    /// Docs: https://www.w3.org/TR/css-color-4/#predefined-rec2020
    /// </remarks>
    [MetaKeyword("rec2020")]
    Rec2020 = 9,

    /// <summary>
    /// CIE XYZ color space with D50 white point.
    /// </summary>
    /// <remarks>
    /// Docs: https://www.w3.org/TR/css-color-4/#predefined-xyz
    /// </remarks>
    [MetaKeyword("xyz-d50")]
    XyzD50 = 10,

    /// <summary>
    /// CIE XYZ color space with D65 white point.
    /// </summary>
    /// <remarks>
    /// Docs: https://www.w3.org/TR/css-color-4/#predefined-xyz
    /// xyz is an alias for xyz-d65.
    /// </remarks>
    [MetaKeyword("xyz-d65")]
    XyzD65 = 11,
}
