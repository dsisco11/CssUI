using System;
using System.Globalization;

namespace CssUI.CSS;

/// <summary>
/// A specialized CSS value for dimension values (length, angle, time, frequency).
/// </summary>
/// <remarks>
/// <para>
/// CSS dimensions are numbers with an associated unit like <c>px</c>, <c>em</c>, <c>deg</c>, <c>s</c>, etc.
/// They are used in properties like <c>width</c>, <c>transform</c>, <c>animation-duration</c>, etc.
/// </para>
/// <para>
/// Spec: https://www.w3.org/TR/css-values-4/#dimensions
/// </para>
/// <para>
/// This subclass stores the numeric value and unit directly, eliminating
/// the boxing overhead of <see cref="CssValueData"/> and providing typed access.
/// </para>
/// </remarks>
public sealed record class CssDimensionValue : CssValue
{
    private readonly double _value;
    private readonly ECssUnit _unit;

    /// <summary>
    /// Gets the numeric value.
    /// </summary>
    public double Value => _value;

    /// <summary>
    /// Gets the CSS unit for this dimension.
    /// </summary>
    public override ECssUnit Unit => _unit;

    /// <inheritdoc/>
    public override bool HasValue => true;

    /// <summary>
    /// Creates a new <see cref="CssDimensionValue"/> with the specified value and unit.
    /// </summary>
    /// <param name="value">The numeric value.</param>
    /// <param name="unit">The CSS unit.</param>
    internal CssDimensionValue(double value, ECssUnit unit) : base(ECssValueTypes.DIMENSION)
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
        // Use CSS-standard format: max 3 decimal places, no trailing zeros
        var effectiveFormat = format.IsEmpty ? "0.###".AsSpan() : format;
        Span<char> buffer = stackalloc char[32];
        if (!_value.TryFormat(buffer, out int numWritten, effectiveFormat, provider ?? CultureInfo.InvariantCulture))
        {
            charsWritten = 0;
            return false;
        }

        // Special case: ECssUnit.None uses "<none>" marker for debugging
        var unitStr = _unit == ECssUnit.None ? "<none>" : _unit.Keyword();
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
    public bool Equals(CssDimensionValue? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return _value == other._value && _unit == other._unit;
    }

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Type, _value, _unit);
}
