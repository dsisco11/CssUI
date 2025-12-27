using System;

namespace CssUI.CSS;

/// <summary>
/// A specialized CSS value for unicode-range types, avoiding boxing of the <see cref="CssUnicodeRange"/> struct.
/// </summary>
/// <remarks>
/// <para>
/// CSS unicode-range values are used in @font-face rules to specify which characters
/// a font supports. They use the U+XXXX or U+XXXX-YYYY syntax.
/// </para>
/// <para>
/// Spec: https://www.w3.org/TR/css-syntax-3/#urange
/// </para>
/// <para>
/// This subclass stores the <see cref="CssUnicodeRange"/> directly, eliminating the boxing
/// overhead of <see cref="CssValueData.ObjectValue"/>.
/// </para>
/// </remarks>
public sealed record class CssUnicodeRangeValue : CssValue
{
    private readonly CssUnicodeRange _value;

    /// <summary>
    /// Gets the unicode-range value.
    /// </summary>
    public CssUnicodeRange Value => _value;

    /// <inheritdoc/>
    public override bool HasValue => true; // Struct always has a value

    /// <summary>
    /// Creates a new <see cref="CssUnicodeRangeValue"/> with the specified unicode range.
    /// </summary>
    /// <param name="range">The unicode range.</param>
    internal CssUnicodeRangeValue(CssUnicodeRange range) : base(ECssValueTypes.UNICODE_RANGE)
    {
        _value = range;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Returns the CSS serialization: U+XXXX or U+XXXX-YYYY
    /// </remarks>
    public override string ToString() => _value.Serialize();

    /// <inheritdoc/>
    public override string ToString(string? format, IFormatProvider? formatProvider) => _value.Serialize();

    /// <inheritdoc/>
    /// <remarks>
    /// Serializes as a CSS unicode-range per CSS Syntax Level 3.
    /// </remarks>
    public override string Serialize() => _value.Serialize();

    /// <summary>
    /// Returns the unicode-range value directly (without boxing).
    /// </summary>
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override CssUnicodeRange AsUnicodeRange() => _value;

    /// <inheritdoc/>
    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
    {
        return _value.TryFormat(destination, out charsWritten, format, provider);
    }
}
