using System;

namespace CssUI.CSS;

/// <summary>
/// A specialized CSS value for integer numbers.
/// </summary>
/// <remarks>
/// <para>
/// CSS integers are whole numbers without a fractional component.
/// They are used in properties like <c>z-index</c>, <c>order</c>, <c>column-count</c>, etc.
/// </para>
/// <para>
/// Spec: https://www.w3.org/TR/css-values-4/#integers
/// </para>
/// <para>
/// This subclass stores the integer directly as a <see cref="long"/>, eliminating
/// the boxing overhead of <see cref="CssValueData"/>.
/// </para>
/// </remarks>
public sealed record class CssIntegerValue : CssValue
{
    private readonly long _value;

    /// <summary>
    /// Gets the integer value.
    /// </summary>
    public long Value => _value;

    /// <inheritdoc/>
    public override bool HasValue => true;

    /// <summary>
    /// Creates a new <see cref="CssIntegerValue"/> with the specified value.
    /// </summary>
    /// <param name="value">The integer value.</param>
    internal CssIntegerValue(long value) : base(ECssValueTypes.INTEGER)
    {
        _value = value;
    }

    /// <inheritdoc/>
    public override string ToString() => _value.ToString();

    /// <inheritdoc/>
    public override string ToString(string? format, IFormatProvider? formatProvider)
        => _value.ToString(format, formatProvider);

    /// <inheritdoc/>
    public override string Serialize() => _value.ToString();

    /// <inheritdoc/>
    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
        => _value.TryFormat(destination, out charsWritten, format, provider);

    /// <inheritdoc/>
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override long AsInteger() => _value;

    /// <inheritdoc/>
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override double AsDecimal() => _value;
}
