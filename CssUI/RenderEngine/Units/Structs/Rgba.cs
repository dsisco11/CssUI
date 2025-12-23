using System;
using System.Diagnostics.Contracts;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CssUI.CSS;

namespace CssUI.Rendering;

/// <summary>
/// Represents an immutable 8-bit RGBA color value for rendering operations.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public readonly record struct Rgba : IColorObject
{
    #region Constants
    private const float ByteMaxF = 255f;
    #endregion

    #region Properties
    private readonly byte red;
    private readonly byte green;
    private readonly byte blue;
    private readonly byte alpha;
    #endregion

    #region Accessors
    public byte Red => red;
    public byte Green => green;
    public byte Blue => blue;
    public byte Alpha => alpha;
    #endregion

    #region Constructors
    public Rgba(uint packed)
    {
        var rgb = Rgba.Unpack(packed);
        red = rgb[0];
        green = rgb[1];
        blue = rgb[2];
        alpha = rgb[3];
    }

    public Rgba(byte red, byte green, byte blue, byte alpha)
    {
        this.red = red;
        this.green = green;
        this.blue = blue;
        this.alpha = alpha;
    }
    #endregion

    #region Factory Methods
    /// <summary>
    /// Creates an <see cref="Rgba"/> from a <see cref="CssColor"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Rgba FromCss(CssColor color) => new(color.R, color.G, color.B, color.A);

    /// <summary>
    /// Creates an <see cref="Rgba"/> from a <see cref="CssColorHdr"/> (with gamut mapping).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Rgba FromCss(CssColorHdr color) => FromCss(color.ToCssColor());

    /// <summary>
    /// Creates an <see cref="Rgba"/> from floating-point values in the range [0-1].
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Rgba FromFloat(float r, float g, float b, float a = 1f)
    {
        return new Rgba(
            (byte)Math.Clamp(r * ByteMaxF + 0.5f, 0f, ByteMaxF),
            (byte)Math.Clamp(g * ByteMaxF + 0.5f, 0f, ByteMaxF),
            (byte)Math.Clamp(b * ByteMaxF + 0.5f, 0f, ByteMaxF),
            (byte)Math.Clamp(a * ByteMaxF + 0.5f, 0f, ByteMaxF)
        );
    }
    #endregion

    #region Pack/Unpack
    /// <summary>
    /// Views the 8-bit RGBA values at this objects address as a 32-bit integer.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint Pack()
    {
        unsafe
        {
            fixed (byte* ptr = &red)
            {
                return *((uint*)ptr);
            }
        }
    }

    /// <summary>
    /// Unpacks a 32-bit integer into 8-bit RGBA values.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe byte[] Unpack(uint packed)
    {
        Contract.Ensures(Contract.Result<byte[]>() != null);
        Contract.Ensures(Contract.Result<byte[]>().Length == 4);

        byte[] bytes = new byte[4];
        fixed (byte* ptr = bytes)
            *((uint*)ptr) = packed;

        return bytes;
    }
    #endregion

    #region IColorObject Implementation
    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector4 GetVector() => new(red / ByteMaxF, green / ByteMaxF, blue / ByteMaxF, alpha / ByteMaxF);

    /// <inheritdoc/>
    /// <remarks>This method is a no-op for readonly structs. Use <see cref="FromFloat"/> to create a new instance.</remarks>
    void IColorObject.SetVector(Vector4 RGBA)
    {
        // No-op: Rgba is immutable. This is here for interface compliance.
        // Callers should use FromFloat() factory method instead.
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint AsInteger() => Pack();
    #endregion

    #region Conversion
    /// <summary>
    /// Converts this rendering color to a <see cref="CssColor"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public CssColor ToCssColor() => new(red, green, blue, alpha);
    #endregion

    #region Utility
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint ReverseBytes(uint value)
    {
        return (value & 0x000000FFU) << 24 | (value & 0x0000FF00U) << 8 |
            (value & 0x00FF0000U) >> 8 | (value & 0xFF000000U) >> 24;
    }
    #endregion
}

