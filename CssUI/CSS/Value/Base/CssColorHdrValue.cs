using System;
using CssUI.CSS.Serialization;

namespace CssUI.CSS;

/// <summary>
/// A specialized CSS value for HDR (wide-gamut) color values.
/// </summary>
/// <remarks>
/// <para>
/// HDR colors use floating-point components and can represent colors outside the standard sRGB gamut.
/// They are used in CSS Color Level 4 color spaces like display-p3, rec2020, lab, lch, etc.
/// </para>
/// <para>
/// Spec: https://www.w3.org/TR/css-color-4/#predefined
/// </para>
/// <para>
/// This subclass stores the <see cref="CssColorHdr"/> struct directly, eliminating
/// the boxing overhead of <see cref="CssValueData"/> and providing typed access.
/// </para>
/// </remarks>
public sealed record class CssColorHdrValue : CssValue
{
    private readonly CssColorHdr _value;

    /// <summary>
    /// Gets the HDR color value.
    /// </summary>
    public CssColorHdr Value => _value;

    /// <inheritdoc/>
    public override bool HasValue => true;

    /// <summary>
    /// Creates a new <see cref="CssColorHdrValue"/> with the specified HDR color.
    /// </summary>
    /// <param name="value">The HDR color value.</param>
    internal CssColorHdrValue(CssColorHdr value) : base(ECssValueTypes.COLOR_HDR)
    {
        _value = value;
    }

    /// <inheritdoc/>
    public override string ToString() => CssColorSerializer.Serialize(_value);

    /// <inheritdoc/>
    public override string ToString(string? format, IFormatProvider? formatProvider)
        => CssColorSerializer.Serialize(_value);

    /// <inheritdoc/>
    public override string Serialize() => CssColorSerializer.Serialize(_value);

    /// <inheritdoc/>
    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
    {
        var serialized = CssColorSerializer.Serialize(_value);
        if (destination.Length < serialized.Length)
        {
            charsWritten = 0;
            return false;
        }
        serialized.AsSpan().CopyTo(destination);
        charsWritten = serialized.Length;
        return true;
    }

    /// <inheritdoc/>
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override CssColorHdr AsCssColorHdr() => _value;

    /// <inheritdoc/>
    public override bool TryGetColor(out CssColor color)
    {
        // Convert HDR to 8-bit color by clamping/converting
        color = _value.ToCssColor();
        return true;
    }

    /// <inheritdoc/>
    public bool Equals(CssColorHdrValue? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return _value == other._value;
    }

    /// <inheritdoc/>
    public override bool Equals(CssValue? other)
    {
        if (other is CssColorHdrValue colorVal)
            return Equals(colorVal);
        return false;
    }

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Type, _value);
}
