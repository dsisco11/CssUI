using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace CssUI.CSS;

/// <summary>
/// Represents a CSS value that is a list of sub-values.
/// </summary>
/// <remarks>
/// <para>
/// This subclass stores the values array directly without boxing through <c>CssValueData.ObjectValue</c>,
/// providing allocation-free access via <see cref="Values"/> and efficient iteration.
/// </para>
/// <para>
/// CSS list values are used extensively for properties like:
/// <list type="bullet">
/// <item><description><c>font-family</c> - comma-separated list of font names</description></item>
/// <item><description><c>background</c> - multiple background layers</description></item>
/// <item><description><c>transition</c> - multiple transition definitions</description></item>
/// <item><description><c>transform</c> - space-separated list of transform functions</description></item>
/// <item><description><c>box-shadow</c> - comma-separated shadow definitions</description></item>
/// </list>
/// </para>
/// </remarks>
public sealed record class CssListValue : CssValue
{
    #region Fields

    private readonly CssValue[] _values;
    private readonly ECssListSeparator _separator;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the values in this list as a span for allocation-free iteration.
    /// </summary>
    public ReadOnlySpan<CssValue> Values => _values.AsSpan();

    /// <summary>
    /// Gets the number of values in this list.
    /// </summary>
    public int Count => _values.Length;

    /// <summary>
    /// Gets the separator used between values when serializing.
    /// </summary>
    public ECssListSeparator Separator => _separator;

    /// <summary>
    /// Gets a value at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the value to get.</param>
    /// <returns>The value at the specified index.</returns>
    /// <exception cref="IndexOutOfRangeException">Thrown when the index is out of range.</exception>
    public CssValue this[int index] => _values[index];

    /// <inheritdoc/>
    public override bool HasValue => _values.Length > 0;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a new <see cref="CssListValue"/> with the specified values.
    /// </summary>
    /// <param name="values">The array of values. This array is stored directly (not copied).</param>
    /// <param name="separator">The separator to use when serializing. Default is space.</param>
    internal CssListValue(CssValue[] values, ECssListSeparator separator = ECssListSeparator.Space)
        : base(ECssValueTypes.COLLECTION)
    {
        _values = values ?? Array.Empty<CssValue>();
        _separator = separator;
    }

    /// <summary>
    /// Creates a new <see cref="CssListValue"/> by copying values from a span.
    /// </summary>
    /// <param name="values">The span of values to copy.</param>
    /// <param name="separator">The separator to use when serializing. Default is space.</param>
    internal CssListValue(ReadOnlySpan<CssValue> values, ECssListSeparator separator = ECssListSeparator.Space)
        : base(ECssValueTypes.COLLECTION)
    {
        _values = values.ToArray();
        _separator = separator;
    }

    #endregion

    #region Accessors

    /// <summary>
    /// Returns the values as a <see cref="ReadOnlyCollection{T}"/>.
    /// </summary>
    /// <remarks>
    /// Creates a new wrapper each call. Prefer using <see cref="Values"/> span for iteration when possible.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlyCollection<CssValue> AsReadOnlyCollection()
    {
        return new ReadOnlyCollection<CssValue>(_values);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Creates a new wrapper each call. Prefer using <see cref="Values"/> span for iteration when possible.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ReadOnlyCollection<CssValue> AsCollection() => AsReadOnlyCollection();

    #endregion

    #region Serialization

    /// <inheritdoc/>
    public override string Serialize()
    {
        if (_values.Length == 0)
            return string.Empty;

        if (_values.Length == 1)
            return _values[0].Serialize();

        var separatorStr = _separator switch
        {
            ECssListSeparator.Comma => ", ",
            ECssListSeparator.Slash => " / ",
            _ => " " // Space is default
        };

        return string.Join(separatorStr, Array.ConvertAll(_values, v => v.Serialize()));
    }

    /// <inheritdoc/>
    public override string ToString() => Serialize();

    /// <inheritdoc/>
    public override string ToString(string? format, IFormatProvider? formatProvider) => Serialize();

    /// <inheritdoc/>
    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
    {
        charsWritten = 0;

        if (_values.Length == 0)
            return true;

        ReadOnlySpan<char> separatorSpan = _separator switch
        {
            ECssListSeparator.Comma => ", ".AsSpan(),
            ECssListSeparator.Slash => " / ".AsSpan(),
            _ => " ".AsSpan()
        };

        for (int i = 0; i < _values.Length; i++)
        {
            // Add separator before all items except the first
            if (i > 0)
            {
                if (destination.Length - charsWritten < separatorSpan.Length)
                    return false;

                separatorSpan.CopyTo(destination.Slice(charsWritten));
                charsWritten += separatorSpan.Length;
            }

            // Format the value
            if (!_values[i].TryFormat(destination.Slice(charsWritten), out int valueChars, format, provider))
                return false;

            charsWritten += valueChars;
        }

        return true;
    }

    #endregion
}

/// <summary>
/// Specifies the separator used between values in a CSS list.
/// </summary>
public enum ECssListSeparator
{
    /// <summary>
    /// Values are separated by spaces (default for most properties).
    /// </summary>
    Space = 0,

    /// <summary>
    /// Values are separated by commas (e.g., font-family, background layers).
    /// </summary>
    Comma = 1,

    /// <summary>
    /// Values are separated by slashes (e.g., font shorthand, border-radius).
    /// </summary>
    Slash = 2
}
