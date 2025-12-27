using System;
using CssUI.CSS.Serialization;

namespace CssUI.CSS;

/// <summary>
/// A specialized CSS value for 8-bit RGBA color values.
/// </summary>
/// <remarks>
/// <para>
/// CSS colors can be specified in various formats: named colors, hex notation (#rgb, #rrggbb, #rrggbbaa),
/// RGB/RGBA functions, HSL/HSLA functions, etc.
/// </para>
/// <para>
/// Spec: https://www.w3.org/TR/css-color-4/
/// </para>
/// <para>
/// This subclass stores the <see cref="CssColor"/> struct directly, eliminating
/// the boxing overhead of <see cref="CssValueData"/> and providing typed access.
/// </para>
/// </remarks>
public sealed record class CssColorValue : CssValue
{
    private readonly CssColor _value;

    /// <summary>
    /// Gets the 8-bit RGBA color value.
    /// </summary>
    public CssColor Value => _value;

    /// <inheritdoc/>
    public override bool HasValue => true;

    /// <summary>
    /// Creates a new <see cref="CssColorValue"/> with the specified color.
    /// </summary>
    /// <param name="value">The color value.</param>
    internal CssColorValue(CssColor value) : base(ECssValueTypes.COLOR)
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
        => _value.TryFormat(destination, out charsWritten, format, provider);

    /// <inheritdoc/>
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override CssColor AsCssColor() => _value;

    /// <inheritdoc/>
    public override bool TryGetColor(out CssColor color)
    {
        color = _value;
        return true;
    }

    /// <inheritdoc/>
    public bool Equals(CssColorValue? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return _value == other._value;
    }

    /// <inheritdoc/>
    public override bool Equals(CssValue? other)
    {
        if (other is CssColorValue colorVal)
            return Equals(colorVal);
        return false;
    }

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Type, _value);
}
