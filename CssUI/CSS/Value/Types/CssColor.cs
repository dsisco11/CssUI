using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CssUI.Rendering;

namespace CssUI.CSS;

/// <summary>
/// Represents an immutable 8-bit RGBA CSS color value with explicit layout for zero-cost uint reinterpretation.
/// </summary>
/// <remarks>
/// Docs: https://www.w3.org/TR/css-color-4/
/// </remarks>
[StructLayout(LayoutKind.Explicit, Size = 4)]
public readonly record struct CssColor : IEquatable<CssColor>
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
    /// Creates a <see cref="CssColor"/> from a hexadecimal color string.
    /// </summary>
    /// <param name="hex">Hex string in format: #RGB, #RGBA, #RRGGBB, or #RRGGBBAA</param>
    public static CssColor FromHex(ReadOnlySpan<char> hex)
    {
        if (hex.IsEmpty)
            return Transparent;

        // Skip leading '#' if present
        if (hex[0] == '#')
            hex = hex[1..];

        return hex.Length switch
        {
            3 => ParseHex3(hex),    // #RGB
            4 => ParseHex4(hex),    // #RGBA
            6 => ParseHex6(hex),    // #RRGGBB
            8 => ParseHex8(hex),    // #RRGGBBAA
            _ => throw new FormatException($"Invalid hex color format: #{hex.ToString()}")
        };
    }

    /// <summary>
    /// Attempts to parse a hexadecimal color string.
    /// </summary>
    public static bool TryFromHex(ReadOnlySpan<char> hex, out CssColor color)
    {
        try
        {
            color = FromHex(hex);
            return true;
        }
        catch
        {
            color = Transparent;
            return false;
        }
    }

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

        // Get the RGB values from the MetaKeyword attribute data
        if (Lookup.TryData(ecolor, out EnumData? data) && data is { Length: >= 4 } enumData)
        {
            // MetaKeyword format: keyword, hex, r, g, b
            // Data[0] = hex value (unused), Data[1] = R, Data[2] = G, Data[3] = B
            return new CssColor(
                (byte)(int)enumData.Data[1],
                (byte)(int)enumData.Data[2],
                (byte)(int)enumData.Data[3],
                255
            );
        }

        return Transparent;
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
        var atomicKeyword = new AtomicString(loweredString);

        if (Lookup.TryEnum<EColor>(atomicKeyword, out EColor ecolor))
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

            color = FromEColor(ecolor);
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

    #region Parsing Helpers
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static CssColor ParseHex3(ReadOnlySpan<char> hex)
    {
        byte r = ParseHexNibble(hex[0]);
        byte g = ParseHexNibble(hex[1]);
        byte b = ParseHexNibble(hex[2]);
        return new CssColor((byte)(r | (r << 4)), (byte)(g | (g << 4)), (byte)(b | (b << 4)), 255);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static CssColor ParseHex4(ReadOnlySpan<char> hex)
    {
        byte r = ParseHexNibble(hex[0]);
        byte g = ParseHexNibble(hex[1]);
        byte b = ParseHexNibble(hex[2]);
        byte a = ParseHexNibble(hex[3]);
        return new CssColor((byte)(r | (r << 4)), (byte)(g | (g << 4)), (byte)(b | (b << 4)), (byte)(a | (a << 4)));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static CssColor ParseHex6(ReadOnlySpan<char> hex)
    {
        byte r = ParseHexByte(hex[0], hex[1]);
        byte g = ParseHexByte(hex[2], hex[3]);
        byte b = ParseHexByte(hex[4], hex[5]);
        return new CssColor(r, g, b, 255);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static CssColor ParseHex8(ReadOnlySpan<char> hex)
    {
        byte r = ParseHexByte(hex[0], hex[1]);
        byte g = ParseHexByte(hex[2], hex[3]);
        byte b = ParseHexByte(hex[4], hex[5]);
        byte a = ParseHexByte(hex[6], hex[7]);
        return new CssColor(r, g, b, a);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static byte ParseHexNibble(char c)
    {
        return c switch
        {
            >= '0' and <= '9' => (byte)(c - '0'),
            >= 'a' and <= 'f' => (byte)(c - 'a' + 10),
            >= 'A' and <= 'F' => (byte)(c - 'A' + 10),
            _ => throw new FormatException($"Invalid hex character: {c}")
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static byte ParseHexByte(char high, char low)
    {
        return (byte)((ParseHexNibble(high) << 4) | ParseHexNibble(low));
    }
    #endregion

    #region Formatting
    /// <summary>
    /// Returns the hex string representation of this color (e.g., "#FF0000FF").
    /// </summary>
    public string ToHexString()
    {
        return _a == 255
            ? $"#{_r:X2}{_g:X2}{_b:X2}"
            : $"#{_r:X2}{_g:X2}{_b:X2}{_a:X2}";
    }

    /// <summary>
    /// Returns the CSS rgb() or rgba() function representation.
    /// </summary>
    public string ToCssString()
    {
        return _a == 255
            ? $"rgb({_r}, {_g}, {_b})"
            : $"rgba({_r}, {_g}, {_b}, {(_a / ByteMaxF).ToString("F3", CultureInfo.InvariantCulture)})";
    }

    /// <inheritdoc/>
    public override string ToString() => ToCssString();
    #endregion

    #region Equality
    /// <inheritdoc/>
    public bool Equals(CssColor other) => _packed == other._packed;

    /// <inheritdoc/>
    public override int GetHashCode() => (int)_packed;
    #endregion
}
