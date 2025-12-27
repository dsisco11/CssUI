using System;
using System.Globalization;

namespace CssUI.CSS;

/// <summary>
/// A specialized CSS value for percentage values.
/// </summary>
/// <remarks>
/// <para>
/// CSS percentages represent a fraction of some other reference value, specified in the range 0-100.
/// They are used in properties like <c>width</c>, <c>margin</c>, <c>font-size</c>, etc.
/// </para>
/// <para>
/// Spec: https://www.w3.org/TR/css-values-4/#percentages
/// </para>
/// <para>
/// This subclass stores the percentage directly as a <see cref="double"/> (0-100 range),
/// providing type-safe access without boxing.
/// </para>
/// </remarks>
public sealed record class CssPercentValue : CssValue
{
    private readonly double _value;

    /// <summary>
    /// Gets the percentage value in the range 0-100.
    /// </summary>
    public double Value => _value;

    /// <inheritdoc/>
    public override bool HasValue => true;

    /// <summary>
    /// Creates a new <see cref="CssPercentValue"/> with the specified value.
    /// </summary>
    /// <param name="value">The percentage value (0-100 range).</param>
    internal CssPercentValue(double value) : base(ECssValueTypes.PERCENT)
    {
        _value = value;
    }

    /// <inheritdoc/>
    public override string ToString()
        => $"{_value.ToString(CultureInfo.InvariantCulture)}%";

    /// <inheritdoc/>
    public override string ToString(string? format, IFormatProvider? formatProvider)
        => $"{_value.ToString(format, formatProvider ?? CultureInfo.InvariantCulture)}%";

    /// <inheritdoc/>
    public override string Serialize()
        => $"{_value.ToString(CultureInfo.InvariantCulture)}%";

    /// <inheritdoc/>
    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
    {
        // Format: {value}%
        Span<char> buffer = stackalloc char[32];
        if (!_value.TryFormat(buffer, out int numWritten, format, provider ?? CultureInfo.InvariantCulture))
        {
            charsWritten = 0;
            return false;
        }

        int totalLength = numWritten + 1; // +1 for '%'
        if (destination.Length < totalLength)
        {
            charsWritten = 0;
            return false;
        }

        buffer[..numWritten].CopyTo(destination);
        destination[numWritten] = '%';
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
    public bool Equals(CssPercentValue? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return _value == other._value;
    }

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Type, _value);
}
