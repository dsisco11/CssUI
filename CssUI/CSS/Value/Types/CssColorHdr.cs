using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CssUI.Rendering;

namespace CssUI.CSS;

/// <summary>
/// Represents a high dynamic range CSS color value with floating-point components
/// supporting wide-gamut color spaces.
/// </summary>
/// <remarks>
/// Docs: https://www.w3.org/TR/css-color-4/
/// </remarks>
[StructLayout(LayoutKind.Sequential)]
public readonly record struct CssColorHdr : IEquatable<CssColorHdr>, ISpanFormattable, IFormattable
{
    #region Constants
    private const float ByteMaxF = 255f;
    private const float Epsilon = 1e-6f;
    #endregion

    #region Static Instances
    /// <summary>Transparent black in sRGB</summary>
    public static readonly CssColorHdr Transparent = new(0f, 0f, 0f, 0f, EColorSpace.sRGB);

    /// <summary>Opaque black in sRGB</summary>
    public static readonly CssColorHdr Black = new(0f, 0f, 0f, 1f, EColorSpace.sRGB);

    /// <summary>Opaque white in sRGB</summary>
    public static readonly CssColorHdr White = new(1f, 1f, 1f, 1f, EColorSpace.sRGB);
    #endregion

    #region Fields
    /// <summary>
    /// First color component. Meaning depends on color space:
    /// - sRGB/sRGBLinear/DisplayP3: Red [0-1]
    /// - Lab/OkLab: Lightness L [0-1 for OkLab, 0-100 for Lab]
    /// - Lch/OkLCh: Lightness L
    /// </summary>
    private readonly float _c1;

    /// <summary>
    /// Second color component. Meaning depends on color space:
    /// - sRGB/sRGBLinear/DisplayP3: Green [0-1]
    /// - Lab/OkLab: a* axis (green-red)
    /// - Lch/OkLCh: Chroma C
    /// </summary>
    private readonly float _c2;

    /// <summary>
    /// Third color component. Meaning depends on color space:
    /// - sRGB/sRGBLinear/DisplayP3: Blue [0-1]
    /// - Lab/OkLab: b* axis (blue-yellow)
    /// - Lch/OkLCh: Hue H [0-360]
    /// </summary>
    private readonly float _c3;

    /// <summary>Alpha component in the range [0-1]</summary>
    private readonly float _alpha;

    /// <summary>The color space this color is defined in</summary>
    private readonly EColorSpace _colorSpace;
    #endregion

    #region Properties
    /// <summary>First color component (Red/Lightness depending on color space)</summary>
    public float C1 => _c1;

    /// <summary>Second color component (Green/a* depending on color space)</summary>
    public float C2 => _c2;

    /// <summary>Third color component (Blue/b*/Hue depending on color space)</summary>
    public float C3 => _c3;

    /// <summary>Alpha component in the range [0-1]</summary>
    public float Alpha => _alpha;

    /// <summary>The color space this color is defined in</summary>
    public EColorSpace ColorSpace => _colorSpace;

    /// <summary>Returns true if this color is in an RGB-based color space</summary>
    public bool IsRgbSpace => _colorSpace is EColorSpace.sRGB or EColorSpace.sRGBLinear or EColorSpace.DisplayP3;

    /// <summary>Returns true if this color is in a Lab-based color space</summary>
    public bool IsLabSpace => _colorSpace is EColorSpace.Lab or EColorSpace.OkLab;

    /// <summary>Returns true if this color is in a cylindrical (LCH) color space</summary>
    public bool IsCylindricalSpace => _colorSpace is EColorSpace.Lch or EColorSpace.OkLCh;
    #endregion

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="CssColorHdr"/> with the specified components and color space.
    /// </summary>
    public CssColorHdr(float c1, float c2, float c3, float alpha, EColorSpace colorSpace)
    {
        _c1 = c1;
        _c2 = c2;
        _c3 = c3;
        _alpha = alpha;
        _colorSpace = colorSpace;
    }
    #endregion

    #region Factory Methods
    /// <summary>
    /// Creates a <see cref="CssColorHdr"/> from sRGB values in the range [0-1].
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CssColorHdr FromSrgb(float r, float g, float b, float a = 1f)
        => new(r, g, b, a, EColorSpace.sRGB);

    /// <summary>
    /// Creates a <see cref="CssColorHdr"/> from linear sRGB values in the range [0-1].
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CssColorHdr FromLinearSrgb(float r, float g, float b, float a = 1f)
        => new(r, g, b, a, EColorSpace.sRGBLinear);

    /// <summary>
    /// Creates a <see cref="CssColorHdr"/> from Display P3 values in the range [0-1].
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CssColorHdr FromDisplayP3(float r, float g, float b, float a = 1f)
        => new(r, g, b, a, EColorSpace.DisplayP3);

    /// <summary>
    /// Creates a <see cref="CssColorHdr"/> from OkLab values.
    /// </summary>
    /// <param name="l">Lightness [0-1]</param>
    /// <param name="a">Green-red axis</param>
    /// <param name="b">Blue-yellow axis</param>
    /// <param name="alpha">Alpha [0-1]</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CssColorHdr FromOkLab(float l, float a, float b, float alpha = 1f)
        => new(l, a, b, alpha, EColorSpace.OkLab);

    /// <summary>
    /// Creates a <see cref="CssColorHdr"/> from OkLCH values.
    /// </summary>
    /// <param name="l">Lightness [0-1]</param>
    /// <param name="c">Chroma</param>
    /// <param name="h">Hue [0-360]</param>
    /// <param name="alpha">Alpha [0-1]</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CssColorHdr FromOkLCh(float l, float c, float h, float alpha = 1f)
        => new(l, c, h, alpha, EColorSpace.OkLCh);

    /// <summary>
    /// Creates a <see cref="CssColorHdr"/> from a <see cref="CssColor"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CssColorHdr FromCssColor(CssColor color)
    {
        var (r, g, b, a) = color.ToFloats();
        return new CssColorHdr(r, g, b, a, EColorSpace.sRGB);
    }
    #endregion

    #region Conversion Methods
    /// <summary>
    /// Converts this HDR color to a standard 8-bit <see cref="CssColor"/> with gamut mapping.
    /// </summary>
    /// <remarks>
    /// Colors outside the sRGB gamut will be clamped.
    /// For better quality gamut mapping, use <see cref="ColorConversion"/> utilities.
    /// </remarks>
    public CssColor ToCssColor()
    {
        // Convert to sRGB first if needed
        var srgb = ToSrgb();

        // Clamp to valid sRGB range and convert to bytes
        return CssColor.FromRgba(
            Math.Clamp(srgb._c1, 0f, 1f),
            Math.Clamp(srgb._c2, 0f, 1f),
            Math.Clamp(srgb._c3, 0f, 1f),
            Math.Clamp(srgb._alpha, 0f, 1f)
        );
    }

    /// <summary>
    /// Converts this HDR color to a rendering <see cref="Rgba"/> struct.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Rgba ToRenderColor() => ToCssColor().ToRenderColor();

    /// <summary>
    /// Converts this color to sRGB color space.
    /// </summary>
    public CssColorHdr ToSrgb()
    {
        return _colorSpace switch
        {
            EColorSpace.sRGB => this,
            EColorSpace.sRGBLinear => new CssColorHdr(
                ColorConversion.LinearToGamma(_c1),
                ColorConversion.LinearToGamma(_c2),
                ColorConversion.LinearToGamma(_c3),
                _alpha,
                EColorSpace.sRGB
            ),
            EColorSpace.OkLab => ColorConversion.OkLabToSrgb(_c1, _c2, _c3, _alpha),
            EColorSpace.OkLCh => ColorConversion.OkLChToSrgb(_c1, _c2, _c3, _alpha),
            // @todo: Implement other color space conversions
            _ => throw new NotSupportedException($"Conversion from {_colorSpace} to sRGB is not yet implemented.")
        };
    }

    /// <summary>
    /// Converts this color to linear sRGB color space.
    /// </summary>
    public CssColorHdr ToLinearSrgb()
    {
        var srgb = ToSrgb();
        return new CssColorHdr(
            ColorConversion.GammaToLinear(srgb._c1),
            ColorConversion.GammaToLinear(srgb._c2),
            ColorConversion.GammaToLinear(srgb._c3),
            srgb._alpha,
            EColorSpace.sRGBLinear
        );
    }

    /// <summary>
    /// Converts this color to OkLab color space.
    /// </summary>
    public CssColorHdr ToOkLab()
    {
        if (_colorSpace == EColorSpace.OkLab) return this;

        var linear = ToLinearSrgb();
        return ColorConversion.LinearSrgbToOkLab(linear._c1, linear._c2, linear._c3, linear._alpha);
    }

    /// <summary>
    /// Converts this color to OkLCH color space.
    /// </summary>
    public CssColorHdr ToOkLCh()
    {
        if (_colorSpace == EColorSpace.OkLCh) return this;

        var oklab = ToOkLab();
        return ColorConversion.OkLabToOkLCh(oklab._c1, oklab._c2, oklab._c3, oklab._alpha);
    }
    #endregion

    #region Utility Methods
    /// <summary>
    /// Returns a new <see cref="CssColorHdr"/> with the specified alpha value.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public CssColorHdr WithAlpha(float alpha) => new(_c1, _c2, _c3, alpha, _colorSpace);

    /// <summary>
    /// Linearly interpolates between two colors in the same color space.
    /// </summary>
    /// <param name="other">The target color.</param>
    /// <param name="t">Interpolation factor in the range [0.0-1.0].</param>
    public CssColorHdr Lerp(CssColorHdr other, float t)
    {
        t = Math.Clamp(t, 0f, 1f);

        // Convert other color to same color space if needed
        var target = other._colorSpace != _colorSpace ? ConvertTo(other, _colorSpace) : other;

        float oneMinusT = 1f - t;

        // Handle hue interpolation for cylindrical color spaces
        if (IsCylindricalSpace)
        {
            float h1 = _c3;
            float h2 = target._c3;

            // Take shortest path around the hue circle
            float hDiff = h2 - h1;
            if (hDiff > 180f) hDiff -= 360f;
            else if (hDiff < -180f) hDiff += 360f;

            float h = h1 + hDiff * t;
            if (h < 0f) h += 360f;
            else if (h >= 360f) h -= 360f;

            return new CssColorHdr(
                _c1 * oneMinusT + target._c1 * t,
                _c2 * oneMinusT + target._c2 * t,
                h,
                _alpha * oneMinusT + target._alpha * t,
                _colorSpace
            );
        }

        return new CssColorHdr(
            _c1 * oneMinusT + target._c1 * t,
            _c2 * oneMinusT + target._c2 * t,
            _c3 * oneMinusT + target._c3 * t,
            _alpha * oneMinusT + target._alpha * t,
            _colorSpace
        );
    }

    /// <summary>
    /// Returns true if this color is within the sRGB gamut.
    /// </summary>
    public bool IsInGamut()
    {
        var srgb = ToSrgb();
        return srgb._c1 >= -Epsilon && srgb._c1 <= 1f + Epsilon &&
               srgb._c2 >= -Epsilon && srgb._c2 <= 1f + Epsilon &&
               srgb._c3 >= -Epsilon && srgb._c3 <= 1f + Epsilon;
    }

    private static CssColorHdr ConvertTo(CssColorHdr color, EColorSpace targetSpace)
    {
        return targetSpace switch
        {
            EColorSpace.sRGB => color.ToSrgb(),
            EColorSpace.sRGBLinear => color.ToLinearSrgb(),
            EColorSpace.OkLab => color.ToOkLab(),
            EColorSpace.OkLCh => color.ToOkLCh(),
            _ => throw new NotSupportedException($"Conversion to {targetSpace} is not yet implemented.")
        };
    }
    #endregion

    #region Formatting
    /// <inheritdoc/>
    public override string ToString() => Serialization.CssColorSerializer.Serialize(this);

    /// <inheritdoc/>
    public string ToString(string? format, IFormatProvider? formatProvider) => ToString();

    /// <inheritdoc/>
    /// <remarks>
    /// Serializes HDR colors per CSS Color Level 4 §15:
    /// - lab(), lch() for Lab/LCH color spaces
    /// - oklab(), oklch() for OkLab/OkLCH color spaces
    /// - color() for other predefined color spaces
    /// </remarks>
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
    {
        charsWritten = 0;
        ReadOnlySpan<char> funcName = _colorSpace switch
        {
            EColorSpace.Lab => "lab(",
            EColorSpace.Lch => "lch(",
            EColorSpace.OkLab => "oklab(",
            EColorSpace.OkLCh => "oklch(",
            EColorSpace.sRGB => "color(srgb ",
            EColorSpace.sRGBLinear => "color(srgb-linear ",
            EColorSpace.DisplayP3 => "color(display-p3 ",
            _ => "color(unknown "
        };

        // Function name
        if (!funcName.TryCopyTo(destination))
            return false;
        charsWritten += funcName.Length;

        // C1 value
        if (!_c1.TryFormat(destination[charsWritten..], out int c1Written, "G6", CultureInfo.InvariantCulture))
            return false;
        charsWritten += c1Written;

        if (!" ".TryCopyTo(destination[charsWritten..]))
            return false;
        charsWritten += 1;

        // C2 value
        if (!_c2.TryFormat(destination[charsWritten..], out int c2Written, "G6", CultureInfo.InvariantCulture))
            return false;
        charsWritten += c2Written;

        if (!" ".TryCopyTo(destination[charsWritten..]))
            return false;
        charsWritten += 1;

        // C3 value
        if (!_c3.TryFormat(destination[charsWritten..], out int c3Written, "G6", CultureInfo.InvariantCulture))
            return false;
        charsWritten += c3Written;

        // Alpha (if not 1.0)
        if (Math.Abs(_alpha - 1f) > Epsilon)
        {
            if (!" / ".TryCopyTo(destination[charsWritten..]))
                return false;
            charsWritten += 3;

            if (!_alpha.TryFormat(destination[charsWritten..], out int alphaWritten, "G6", CultureInfo.InvariantCulture))
                return false;
            charsWritten += alphaWritten;
        }

        // Closing paren
        if (!")".TryCopyTo(destination[charsWritten..]))
            return false;
        charsWritten += 1;

        return true;
    }
    #endregion

    #region Equality
    /// <inheritdoc/>
    public bool Equals(CssColorHdr other)
    {
        return Math.Abs(_c1 - other._c1) < Epsilon &&
               Math.Abs(_c2 - other._c2) < Epsilon &&
               Math.Abs(_c3 - other._c3) < Epsilon &&
               Math.Abs(_alpha - other._alpha) < Epsilon &&
               _colorSpace == other._colorSpace;
    }

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(_c1, _c2, _c3, _alpha, _colorSpace);
    #endregion
}
