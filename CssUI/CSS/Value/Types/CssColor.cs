using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CssUI.CSS.Parser;
using CssUI.Rendering;

namespace CssUI.CSS;

/// <summary>
/// Represents an immutable 8-bit RGBA CSS color value with explicit layout for zero-cost uint reinterpretation.
/// </summary>
/// <remarks>
/// Docs: https://www.w3.org/TR/css-color-4/
/// </remarks>
[StructLayout(LayoutKind.Explicit, Size = 4)]
public readonly record struct CssColor : IEquatable<CssColor>, ISpanFormattable, IFormattable, IParsable<CssColor>, ISpanParsable<CssColor>
{
    #region Constants
    private const float ByteMaxF = 255f;
    #endregion

    #region Static Instances
    /// <summary>Transparent black (rgba(0, 0, 0, 0))</summary>
    public static readonly CssColor Transparent = new(0, 0, 0, 0);

    /// <summary>Opaque black (rgba(0, 0, 0, 255))</summary>
    public static readonly CssColor Black = new(0, 0, 0, 255);

    /// <summary>Opaque white (rgba(255, 255, 255, 255))</summary>
    public static readonly CssColor White = new(255, 255, 255, 255);

    /// <summary>Current color keyword value marker</summary>
    public static readonly CssColor CurrentColor = new(0, 0, 0, 0);
    #endregion

    #region Fields
    /// <summary>Red channel value in the range [0-255]</summary>
    [FieldOffset(0)]
    private readonly byte _r;

    /// <summary>Green channel value in the range [0-255]</summary>
    [FieldOffset(1)]
    private readonly byte _g;

    /// <summary>Blue channel value in the range [0-255]</summary>
    [FieldOffset(2)]
    private readonly byte _b;

    /// <summary>Alpha channel value in the range [0-255]</summary>
    [FieldOffset(3)]
    private readonly byte _a;

    /// <summary>Packed 32-bit RGBA value (overlaps R, G, B, A)</summary>
    [FieldOffset(0)]
    private readonly uint _packed;
    #endregion

    #region Properties
    /// <summary>Red channel value in the range [0-255]</summary>
    public byte R => _r;

    /// <summary>Green channel value in the range [0-255]</summary>
    public byte G => _g;

    /// <summary>Blue channel value in the range [0-255]</summary>
    public byte B => _b;

    /// <summary>Alpha channel value in the range [0-255]</summary>
    public byte A => _a;

    /// <summary>Packed 32-bit RGBA value</summary>
    public uint Packed => _packed;
    #endregion

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="CssColor"/> from individual RGBA byte values.
    /// </summary>
    public CssColor(byte r, byte g, byte b, byte a)
    {
        // Initialize packed to 0 first to satisfy definite assignment
        _packed = 0;
        _r = r;
        _g = g;
        _b = b;
        _a = a;
    }

    /// <summary>
    /// Creates a new <see cref="CssColor"/> from a packed 32-bit RGBA value.
    /// </summary>
    public CssColor(uint packed)
    {
        // Initialize individual bytes to 0 first to satisfy definite assignment
        _r = 0;
        _g = 0;
        _b = 0;
        _a = 0;
        _packed = packed;
    }
    #endregion

    #region Factory Methods
    /// <summary>
    /// Creates a <see cref="CssColor"/> from RGBA byte values.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CssColor FromRgba(byte r, byte g, byte b, byte a = 255) => new(r, g, b, a);

    /// <summary>
    /// Creates a <see cref="CssColor"/> from RGB byte values with full opacity.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CssColor FromRgb(byte r, byte g, byte b) => new(r, g, b, 255);

    /// <summary>
    /// Creates a <see cref="CssColor"/> from floating-point RGBA values in the range [0.0-1.0].
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CssColor FromRgba(float r, float g, float b, float a = 1.0f)
    {
        return new CssColor(
            (byte)Math.Clamp(r * ByteMaxF + 0.5f, 0f, ByteMaxF),
            (byte)Math.Clamp(g * ByteMaxF + 0.5f, 0f, ByteMaxF),
            (byte)Math.Clamp(b * ByteMaxF + 0.5f, 0f, ByteMaxF),
            (byte)Math.Clamp(a * ByteMaxF + 0.5f, 0f, ByteMaxF)
        );
    }

    /// <summary>
    /// Creates a <see cref="CssColor"/> from floating-point RGBA values in the range [0.0-1.0].
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CssColor FromRgba(double r, double g, double b, double a = 1.0)
    {
        return FromRgba((float)r, (float)g, (float)b, (float)a);
    }

    /// <summary>
    /// Creates a <see cref="CssColor"/> from a packed 32-bit RGBA value.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CssColor FromPacked(uint packed) => new(packed);

    /// <summary>
    /// Creates a <see cref="CssColor"/> from a named CSS color keyword.
    /// </summary>
    /// <param name="ecolor">The CSS named color enum value.</param>
    /// <returns>The corresponding color, or <see cref="Transparent"/> for special keywords like currentColor.</returns>
    /// <remarks>
    /// For <see cref="EColor.CurrentColor"/>, returns <see cref="Transparent"/> as it requires context resolution.
    /// For <see cref="EColor.Transparent"/>, returns rgba(0, 0, 0, 0).
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CssColor FromEColor(EColor ecolor)
    {
        // Handle special keywords
        if (ecolor == EColor.CurrentColor || ecolor == EColor.Transparent)
        {
            return Transparent;
        }

        // Use the generated extension methods for R, G, B
        // Cast from int to byte (EnumRecords workaround for byte type limitation)
        return new CssColor(
            (byte)ecolor.R(),
            (byte)ecolor.G(),
            (byte)ecolor.B(),
            255
        );
    }

    /// <summary>
    /// Attempts to create a <see cref="CssColor"/> from a named CSS color keyword string.
    /// </summary>
    /// <param name="keyword">The CSS color keyword (e.g., "red", "blue", "transparent").</param>
    /// <param name="color">The resulting color if successful.</param>
    /// <param name="isCurrentColor">True if the keyword was "currentColor" (requires context resolution).</param>
    /// <returns>True if the keyword is a valid CSS named color.</returns>
    /// <remarks>
    /// Per CSS Color Level 4 spec: https://www.w3.org/TR/css-color-4/#named-colors
    /// Named colors are case-insensitive.
    /// </remarks>
    public static bool TryFromNamedColor(ReadOnlySpan<char> keyword, out CssColor color, out bool isCurrentColor)
    {
        color = Transparent;
        isCurrentColor = false;

        if (keyword.IsEmpty)
        {
            return false;
        }

        // CSS color keywords are case-insensitive
        // Create a lowercased string for lookup
        var loweredString = new string(keyword).ToLowerInvariant();

        if (EColorExtensions.TryFromKeyword(loweredString, out EColor? ecolor))
        {
            // Check for special keywords
            if (ecolor == EColor.CurrentColor)
            {
                isCurrentColor = true;
                color = CurrentColor;
                return true;
            }

            if (ecolor == EColor.Transparent)
            {
                color = Transparent;
                return true;
            }

            color = FromEColor(ecolor.Value);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Attempts to create a <see cref="CssColor"/> from a named CSS color keyword string.
    /// </summary>
    /// <param name="keyword">The CSS color keyword (e.g., "red", "blue", "transparent").</param>
    /// <param name="color">The resulting color if successful.</param>
    /// <returns>True if the keyword is a valid CSS named color.</returns>
    public static bool TryFromNamedColor(ReadOnlySpan<char> keyword, out CssColor color)
    {
        return TryFromNamedColor(keyword, out color, out _);
    }
    #endregion

    #region Conversion Methods
    /// <summary>
    /// Converts this CSS color to a rendering <see cref="Rgba"/> struct.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Rgba ToRenderColor() => new(_r, _g, _b, _a);

    /// <summary>
    /// Returns the RGBA values as a tuple of floats in the range [0.0-1.0].
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public (float R, float G, float B, float A) ToFloats()
    {
        return (_r / ByteMaxF, _g / ByteMaxF, _b / ByteMaxF, _a / ByteMaxF);
    }

    /// <summary>
    /// Returns the RGBA values as a <see cref="System.Numerics.Vector4"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public System.Numerics.Vector4 ToVector4()
    {
        return new System.Numerics.Vector4(_r / ByteMaxF, _g / ByteMaxF, _b / ByteMaxF, _a / ByteMaxF);
    }
    #endregion

    #region Utility Methods
    /// <summary>
    /// Returns a new <see cref="CssColor"/> with the specified alpha value.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public CssColor WithAlpha(byte alpha) => new(_r, _g, _b, alpha);

    /// <summary>
    /// Returns a new <see cref="CssColor"/> with the specified alpha value in the range [0.0-1.0].
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public CssColor WithAlpha(float alpha) => new(_r, _g, _b, (byte)Math.Clamp(alpha * ByteMaxF + 0.5f, 0f, ByteMaxF));

    /// <summary>
    /// Returns this color pre-multiplied by its alpha value.
    /// </summary>
    public CssColor ToPremultiplied()
    {
        if (_a == 255) return this;
        if (_a == 0) return Transparent;

        float alphaFactor = _a / ByteMaxF;
        return new CssColor(
            (byte)(_r * alphaFactor + 0.5f),
            (byte)(_g * alphaFactor + 0.5f),
            (byte)(_b * alphaFactor + 0.5f),
            _a
        );
    }

    /// <summary>
    /// Linearly interpolates between two colors.
    /// </summary>
    /// <param name="other">The target color.</param>
    /// <param name="t">Interpolation factor in the range [0.0-1.0].</param>
    public CssColor Lerp(CssColor other, float t)
    {
        t = Math.Clamp(t, 0f, 1f);
        float oneMinusT = 1f - t;

        return new CssColor(
            (byte)(_r * oneMinusT + other._r * t + 0.5f),
            (byte)(_g * oneMinusT + other._g * t + 0.5f),
            (byte)(_b * oneMinusT + other._b * t + 0.5f),
            (byte)(_a * oneMinusT + other._a * t + 0.5f)
        );
    }
    #endregion

    #region Formatting (ISpanFormattable, IFormattable)
    /// <inheritdoc/>
    public override string ToString() => Serialization.CssColorSerializer.Serialize(this);

    /// <inheritdoc/>
    public string ToString(string? format, IFormatProvider? formatProvider) => ToString();

    /// <summary>
    /// Tries to format the color into the provided span.
    /// </summary>
    /// <param name="destination">The span to write to.</param>
    /// <param name="charsWritten">The number of characters written.</param>
    /// <param name="format">The format string (ignored).</param>
    /// <param name="provider">The format provider (ignored).</param>
    /// <returns>True if formatting succeeded; otherwise, false.</returns>
    /// <remarks>
    /// Per CSS Color 4 §15.2.2:
    /// - Uses rgb() for opaque colors, rgba() for non-opaque
    /// - Legacy comma-separated syntax for compatibility
    /// - Component values as numbers in [0-255] range
    /// - Alpha omitted when 1, else serialized as a number
    /// </remarks>
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
    {
        charsWritten = 0;

        // Opaque: rgb(R, G, B) → max 16 chars "rgb(255, 255, 255)"
        // Non-opaque: rgba(R, G, B, A) → max 26 chars "rgba(255, 255, 255, 0.999)"
        if (_a == 255)
        {
            // Opaque: rgb(r, g, b)
            return TryFormatRgb(destination, out charsWritten);
        }
        else
        {
            // Non-opaque: rgba(r, g, b, a)
            return TryFormatRgba(destination, out charsWritten);
        }
    }

    /// <summary>
    /// Formats an opaque color as rgb(r, g, b).
    /// </summary>
    private bool TryFormatRgb(Span<char> destination, out int charsWritten)
    {
        charsWritten = 0;

        // "rgb(" = 4 chars
        if (destination.Length < 4)
            return false;

        "rgb(".AsSpan().CopyTo(destination);
        int pos = 4;

        // Red component
        if (!_r.TryFormat(destination[pos..], out int written, default, CultureInfo.InvariantCulture))
            return false;
        pos += written;

        // ", " = 2 chars
        if (destination.Length < pos + 2)
            return false;
        ", ".AsSpan().CopyTo(destination[pos..]);
        pos += 2;

        // Green component
        if (!_g.TryFormat(destination[pos..], out written, default, CultureInfo.InvariantCulture))
            return false;
        pos += written;

        // ", " = 2 chars
        if (destination.Length < pos + 2)
            return false;
        ", ".AsSpan().CopyTo(destination[pos..]);
        pos += 2;

        // Blue component
        if (!_b.TryFormat(destination[pos..], out written, default, CultureInfo.InvariantCulture))
            return false;
        pos += written;

        // ")" = 1 char
        if (destination.Length < pos + 1)
            return false;
        destination[pos] = ')';
        pos++;

        charsWritten = pos;
        return true;
    }

    /// <summary>
    /// Formats a non-opaque color as rgba(r, g, b, a).
    /// </summary>
    private bool TryFormatRgba(Span<char> destination, out int charsWritten)
    {
        charsWritten = 0;

        // "rgba(" = 5 chars
        if (destination.Length < 5)
            return false;

        "rgba(".AsSpan().CopyTo(destination);
        int pos = 5;

        // Red component
        if (!_r.TryFormat(destination[pos..], out int written, default, CultureInfo.InvariantCulture))
            return false;
        pos += written;

        // ", " = 2 chars
        if (destination.Length < pos + 2)
            return false;
        ", ".AsSpan().CopyTo(destination[pos..]);
        pos += 2;

        // Green component
        if (!_g.TryFormat(destination[pos..], out written, default, CultureInfo.InvariantCulture))
            return false;
        pos += written;

        // ", " = 2 chars
        if (destination.Length < pos + 2)
            return false;
        ", ".AsSpan().CopyTo(destination[pos..]);
        pos += 2;

        // Blue component
        if (!_b.TryFormat(destination[pos..], out written, default, CultureInfo.InvariantCulture))
            return false;
        pos += written;

        // ", " = 2 chars
        if (destination.Length < pos + 2)
            return false;
        ", ".AsSpan().CopyTo(destination[pos..]);
        pos += 2;

        // Alpha component - serialize per CSS Color 4 algorithm
        if (!TryFormatAlpha(_a, destination[pos..], out written))
            return false;
        pos += written;

        // ")" = 1 char
        if (destination.Length < pos + 1)
            return false;
        destination[pos] = ')';
        pos++;

        charsWritten = pos;
        return true;
    }

    /// <summary>
    /// Formats an 8-bit alpha value following the CSS Color 4 algorithm.
    /// </summary>
    /// <remarks>
    /// Per CSS Color 4 §15.1:
    /// 1. If there exists an integer 0-100 that when multiplied by 2.55 and rounded equals alpha, return that integer / 100
    /// 2. Otherwise, return alpha / 0.255 rounded to nearest integer / 1000
    /// </remarks>
    private static bool TryFormatAlpha(byte alpha, Span<char> destination, out int charsWritten)
    {
        charsWritten = 0;

        if (alpha == 255)
        {
            if (destination.Length < 1)
                return false;
            destination[0] = '1';
            charsWritten = 1;
            return true;
        }

        if (alpha == 0)
        {
            if (destination.Length < 1)
                return false;
            destination[0] = '0';
            charsWritten = 1;
            return true;
        }

        // Try to find an integer percentage that rounds to this alpha value
        for (int i = 1; i < 100; i++)
        {
            int rounded = (int)Math.Round(i * 2.55);
            if (rounded == alpha)
            {
                // Found a clean percentage representation (i / 100)
                float result = i / 100f;
                return result.TryFormat(destination, out charsWritten, "0.##", CultureInfo.InvariantCulture);
            }
        }

        // No clean percentage, use the fallback algorithm
        float value = alpha / 255f;
        return value.TryFormat(destination, out charsWritten, "0.#####", CultureInfo.InvariantCulture);
    }
    #endregion

    #region Parsing (IParsable, ISpanParsable)
    /// <summary>
    /// Parses a CSS color string into a <see cref="CssColor"/>.
    /// </summary>
    /// <param name="s">The string to parse.</param>
    /// <param name="provider">The format provider (ignored).</param>
    /// <returns>The parsed color.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="s"/> is null.</exception>
    /// <exception cref="FormatException">Thrown when the string is not a valid CSS color.</exception>
    public static CssColor Parse(string s, IFormatProvider? provider)
    {
        ArgumentNullException.ThrowIfNull(s);
        return Parse(s.AsSpan(), provider);
    }

    /// <summary>
    /// Parses a CSS color span into a <see cref="CssColor"/>.
    /// </summary>
    /// <param name="s">The span to parse.</param>
    /// <param name="provider">The format provider (ignored).</param>
    /// <returns>The parsed color.</returns>
    /// <exception cref="FormatException">Thrown when the span is not a valid CSS color.</exception>
    public static CssColor Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        if (!TryParse(s, provider, out CssColor result))
        {
            throw new FormatException($"Invalid CSS color format: '{s.ToString()}'");
        }
        return result;
    }

    /// <summary>
    /// Tries to parse a CSS color string into a <see cref="CssColor"/>.
    /// </summary>
    /// <param name="s">The string to parse.</param>
    /// <param name="provider">The format provider (ignored).</param>
    /// <param name="result">The parsed color if successful.</param>
    /// <returns>True if parsing succeeded; otherwise, false.</returns>
    public static bool TryParse(string? s, IFormatProvider? provider, out CssColor result)
    {
        result = Transparent;
        if (string.IsNullOrWhiteSpace(s))
            return false;

        return TryParse(s.AsSpan(), provider, out result);
    }

    /// <summary>
    /// Tries to parse a CSS color span into a <see cref="CssColor"/>.
    /// </summary>
    /// <param name="s">The span to parse.</param>
    /// <param name="provider">The format provider (ignored).</param>
    /// <param name="result">The parsed color if successful.</param>
    /// <returns>True if parsing succeeded; otherwise, false.</returns>
    /// <remarks>
    /// Supports hex colors (#RGB, #RRGGBB, #RGBA, #RRGGBBAA) and named colors.
    /// For function-based colors (rgb, hsl, etc.), use the CSS parser infrastructure.
    /// </remarks>
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out CssColor result)
    {
        result = Transparent;

        s = s.Trim();
        if (s.IsEmpty)
            return false;

        // Try hex color first (#RGB, #RRGGBB, etc.)
        if (s[0] == '#')
        {
            return CssHexColorParser.TryParse(s, out result);
        }

        // Try named color
        if (TryFromNamedColor(s, out result))
        {
            return true;
        }

        return false;
    }
    #endregion

    #region Equality
    /// <inheritdoc/>
    public bool Equals(CssColor other) => _packed == other._packed;

    /// <inheritdoc/>
    public override int GetHashCode() => (int)_packed;
    #endregion
}
