namespace CssUI.CSS;

/// <summary>
/// Specifies the color space for CSS color values.
/// </summary>
/// <remarks>
/// Docs: https://www.w3.org/TR/css-color-4/#color-syntax
/// </remarks>
public enum EColorSpace : byte
{
    /// <summary>
    /// Standard sRGB color space (gamma-corrected).
    /// </summary>
    sRGB = 0,

    /// <summary>
    /// Linear-light sRGB color space.
    /// </summary>
    sRGBLinear = 1,

    /// <summary>
    /// Display P3 color space (wide gamut).
    /// </summary>
    DisplayP3 = 2,

    /// <summary>
    /// CIE Lab color space.
    /// </summary>
    Lab = 3,

    /// <summary>
    /// CIE LCH color space (cylindrical representation of Lab).
    /// </summary>
    Lch = 4,

    /// <summary>
    /// OkLab perceptually uniform color space.
    /// </summary>
    OkLab = 5,

    /// <summary>
    /// OkLCH color space (cylindrical representation of OkLab).
    /// </summary>
    OkLCh = 6,

    /// <summary>
    /// Adobe RGB (1998) color space (wide gamut).
    /// </summary>
    /// <remarks>
    /// Docs: https://www.w3.org/TR/css-color-4/#predefined-a98-rgb
    /// </remarks>
    A98Rgb = 7,

    /// <summary>
    /// ProPhoto RGB (ROMM RGB) color space (ultra-wide gamut, D50 white point).
    /// </summary>
    /// <remarks>
    /// Docs: https://www.w3.org/TR/css-color-4/#predefined-prophoto-rgb
    /// </remarks>
    ProPhotoRgb = 8,

    /// <summary>
    /// ITU-R BT.2020-2 (Rec. 2020) color space (ultra-wide gamut for UHDTV).
    /// </summary>
    /// <remarks>
    /// Docs: https://www.w3.org/TR/css-color-4/#predefined-rec2020
    /// </remarks>
    Rec2020 = 9,

    /// <summary>
    /// CIE XYZ color space with D50 white point.
    /// </summary>
    /// <remarks>
    /// Docs: https://www.w3.org/TR/css-color-4/#predefined-xyz
    /// </remarks>
    XyzD50 = 10,

    /// <summary>
    /// CIE XYZ color space with D65 white point.
    /// </summary>
    /// <remarks>
    /// Docs: https://www.w3.org/TR/css-color-4/#predefined-xyz
    /// xyz is an alias for xyz-d65.
    /// </remarks>
    XyzD65 = 11,
}
