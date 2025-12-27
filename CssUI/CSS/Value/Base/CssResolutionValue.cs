using System;
using System.Globalization;

namespace CssUI.CSS;

/// <summary>
/// A specialized CSS value for resolution values (dpi, dpcm, dppx).
/// </summary>
/// <remarks>
/// <para>
/// CSS resolution values specify the pixel density of an output device.
/// They are used in media queries like <c>@media (min-resolution: 2dppx)</c>.
/// </para>
/// <para>
/// Spec: https://www.w3.org/TR/css-values-4/#resolution
/// </para>
/// <para>
/// This subclass stores the numeric value and resolution unit directly, eliminating
/// the boxing overhead of <see cref="CssValueData"/> and providing typed access.
/// </para>
/// </remarks>
public sealed record class CssResolutionValue : CssValue
{
    private readonly double _value;
    private readonly ECssUnit _unit;

    /// <summary>
    /// Gets the numeric value.
    /// </summary>
    public double Value => _value;

    /// <summary>
    /// Gets the CSS resolution unit (dpi, dpcm, or dppx).
    /// </summary>
    public override ECssUnit Unit => _unit;

    /// <inheritdoc/>
    public override bool HasValue => true;

    /// <summary>
    /// Creates a new <see cref="CssResolutionValue"/> with the specified value and unit.
    /// </summary>
    /// <param name="value">The numeric value.</param>
    /// <param name="unit">The CSS resolution unit (must be DPI, DPCM, or DPPX).</param>
    internal CssResolutionValue(double value, ECssUnit unit) : base(ECssValueTypes.RESOLUTION)
    {
        _value = value;
        _unit = unit;
    }

    /// <inheritdoc/>
    public override string ToString()
        => $"{_value.ToString(CultureInfo.InvariantCulture)}{_unit.Keyword()}";

    /// <inheritdoc/>
    public override string ToString(string? format, IFormatProvider? formatProvider)
        => $"{_value.ToString(format, formatProvider ?? CultureInfo.InvariantCulture)}{_unit.Keyword()}";

    /// <inheritdoc/>
    public override string Serialize()
        => $"{_value.ToString(CultureInfo.InvariantCulture)}{_unit.Keyword()}";

    /// <inheritdoc/>
    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
    {
        // Format: {value}{unit}
        Span<char> buffer = stackalloc char[32];
        if (!_value.TryFormat(buffer, out int numWritten, format, provider ?? CultureInfo.InvariantCulture))
        {
            charsWritten = 0;
            return false;
        }

        var unitStr = _unit.Keyword();
        int totalLength = numWritten + unitStr.Length;
        if (destination.Length < totalLength)
        {
            charsWritten = 0;
            return false;
        }

        buffer[..numWritten].CopyTo(destination);
        unitStr.AsSpan().CopyTo(destination[numWritten..]);
        charsWritten = totalLength;
        return true;
    }

    /// <inheritdoc/>
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override long AsInteger() => (long)_value;

    /// <inheritdoc/>
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override double AsDecimal() => _value;

    /// <inheritdoc/>
    public bool Equals(CssResolutionValue? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return _value == other._value && _unit == other._unit;
    }

    /// <inheritdoc/>
    public override bool Equals(CssValue? other)
    {
        if (other is CssResolutionValue resVal)
            return Equals(resVal);
        return false;
    }

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Type, _value, _unit);
}
