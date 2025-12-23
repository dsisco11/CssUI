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
}
