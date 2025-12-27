using System;
using System.Globalization;

namespace CssUI.CSS;

/// <summary>
/// A specialized CSS value for floating-point numbers.
/// </summary>
/// <remarks>
/// <para>
/// CSS numbers are decimal values that may have a fractional component.
/// They are used in properties like <c>opacity</c>, <c>flex-grow</c>, <c>line-height</c> (unitless), etc.
/// </para>
/// <para>
/// Spec: https://www.w3.org/TR/css-values-4/#numbers
/// </para>
/// <para>
/// This subclass stores the number directly as a <see cref="double"/>, eliminating
/// the boxing overhead of <see cref="CssValueData"/>.
/// </para>
/// </remarks>
public sealed record class CssNumberValue : CssValue
{
    private readonly double _value;

    /// <summary>
    /// Gets the numeric value.
    /// </summary>
    public double Value => _value;

    /// <inheritdoc/>
    public override bool HasValue => true;

    /// <summary>
    /// Creates a new <see cref="CssNumberValue"/> with the specified value.
    /// </summary>
    /// <param name="value">The numeric value.</param>
    internal CssNumberValue(double value) : base(ECssValueTypes.NUMBER)
    {
        _value = value;
    }

    /// <inheritdoc/>
    public override string ToString() => _value.ToString(CultureInfo.InvariantCulture);

    /// <inheritdoc/>
    public override string ToString(string? format, IFormatProvider? formatProvider)
        => _value.ToString(format, formatProvider ?? CultureInfo.InvariantCulture);

    /// <inheritdoc/>
    public override string Serialize() => _value.ToString(CultureInfo.InvariantCulture);

    /// <inheritdoc/>
    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
        => _value.TryFormat(destination, out charsWritten, format, provider ?? CultureInfo.InvariantCulture);

    /// <inheritdoc/>
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override long AsInteger() => (long)Math.Round(_value);

    /// <inheritdoc/>
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override double AsDecimal() => _value;
}
