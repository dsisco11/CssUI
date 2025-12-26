using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using CssUI.CSS.Parser;

namespace CssUI.CSS;

/// <summary>
/// Represents a unicode-range value from the CSS Unicode-Range microsyntax.
/// </summary>
/// <remarks>
/// <para>
/// Some constructs, such as the 'unicode-range' descriptor for the '@font-face' rule,
/// need a way to describe one or more unicode code points. The '&lt;urange&gt;' production
/// represents a range of one or more unicode code points.
/// </para>
/// <para>
/// Informally, the &lt;urange&gt; production has three forms:
/// <list type="bullet">
/// <item><description>U+0001 - Defines a range consisting of a single code point.</description></item>
/// <item><description>U+0001-00ff - Defines a range of codepoints between the first and second value inclusive.</description></item>
/// <item><description>U+00?? - Defines a range using wildcards, where '?' characters range over all hex digits.</description></item>
/// </list>
/// </para>
/// <para>
/// In each form, a maximum of 6 digits is allowed for each hexadecimal number.
/// </para>
/// </remarks>
/// <seealso href="https://www.w3.org/TR/css-syntax-3/#urange"/>
public readonly struct CssUnicodeRange : IEquatable<CssUnicodeRange>, ISpanFormattable, IFormattable, IParsable<CssUnicodeRange>, ISpanParsable<CssUnicodeRange>
{
    /// <summary>
    /// The maximum allowed code point defined by Unicode: U+10FFFF.
    /// </summary>
    public const int MaxCodePoint = 0x10FFFF;

    /// <summary>
    /// The start value of the unicode range (inclusive).
    /// </summary>
    public readonly int Start;

    /// <summary>
    /// The end value of the unicode range (inclusive).
    /// </summary>
    public readonly int End;

    /// <summary>
    /// Creates a new unicode range with the specified start and end values.
    /// </summary>
    /// <param name="start">The start value of the range (inclusive).</param>
    /// <param name="end">The end value of the range (inclusive).</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when end is greater than the maximum allowed code point,
    /// or when start is greater than end.
    /// </exception>
    public CssUnicodeRange(int start, int end)
    {
        if (end > MaxCodePoint)
        {
            throw new ArgumentOutOfRangeException(nameof(end), $"End value {end:X} exceeds maximum allowed code point U+{MaxCodePoint:X}.");
        }
        if (start > end)
        {
            throw new ArgumentOutOfRangeException(nameof(start), $"Start value U+{start:X} is greater than end value U+{end:X}.");
        }
        if (start < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(start), "Start value cannot be negative.");
        }

        Start = start;
        End = end;
    }

    /// <summary>
    /// Creates a new unicode range for a single code point.
    /// </summary>
    /// <param name="codePoint">The single code point.</param>
    /// <returns>A unicode range representing the single code point.</returns>
    public static CssUnicodeRange SingleCodePoint(int codePoint)
    {
        return new CssUnicodeRange(codePoint, codePoint);
    }

    /// <summary>
    /// Gets whether this range represents a single code point.
    /// </summary>
    public bool IsSingleCodePoint => Start == End;

    /// <summary>
    /// Gets the number of code points in this range.
    /// </summary>
    public int Count => End - Start + 1;

    /// <summary>
    /// Checks if a given code point is within this unicode range.
    /// </summary>
    /// <param name="codePoint">The code point to check.</param>
    /// <returns>True if the code point is within this range; otherwise, false.</returns>
    public bool Contains(int codePoint)
    {
        return codePoint >= Start && codePoint <= End;
    }

    #region Parsing (IParsable, ISpanParsable)
    /// <summary>
    /// Parses a unicode-range value from a CSS string.
    /// </summary>
    /// <param name="s">The string to parse.</param>
    /// <param name="provider">The format provider (ignored - CSS is locale-independent).</param>
    /// <returns>The parsed unicode range.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="s"/> is null.</exception>
    /// <exception cref="FormatException">Thrown when the string is not a valid unicode-range.</exception>
    public static CssUnicodeRange Parse(string s, IFormatProvider? provider)
    {
        ArgumentNullException.ThrowIfNull(s);
        return Parse(s.AsSpan(), provider);
    }

    /// <summary>
    /// Parses a unicode-range value from a character span.
    /// </summary>
    /// <param name="s">The span to parse.</param>
    /// <param name="provider">The format provider (ignored - CSS is locale-independent).</param>
    /// <returns>The parsed unicode range.</returns>
    /// <exception cref="FormatException">Thrown when the span is not a valid unicode-range.</exception>
    public static CssUnicodeRange Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        if (!TryParse(s, provider, out CssUnicodeRange result))
        {
            throw new FormatException($"Invalid unicode-range syntax: '{s.ToString()}'");
        }
        return result;
    }

    /// <summary>
    /// Tries to parse a unicode-range value from a CSS string.
    /// </summary>
    /// <param name="s">The string to parse.</param>
    /// <param name="provider">The format provider (ignored - CSS is locale-independent).</param>
    /// <param name="result">The parsed unicode range if successful.</param>
    /// <returns>True if parsing succeeded; otherwise, false.</returns>
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out CssUnicodeRange result)
    {
        result = default;
        if (string.IsNullOrWhiteSpace(s))
            return false;

        return TryParse(s.AsSpan(), provider, out result);
    }

    /// <summary>
    /// Tries to parse a unicode-range value from a character span.
    /// </summary>
    /// <param name="s">The span to parse.</param>
    /// <param name="provider">The format provider (ignored - CSS is locale-independent).</param>
    /// <param name="result">The parsed unicode range if successful.</param>
    /// <returns>True if parsing succeeded; otherwise, false.</returns>
    /// <remarks>
    /// Delegates to <see cref="CssUnicodeRangeParser"/> internally.
    /// </remarks>
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out CssUnicodeRange result)
    {
        result = default;

        if (s.IsEmpty || s.IsWhiteSpace())
            return false;

        // Delegate to the existing parser
        if (CssUnicodeRangeParser.TryParse(s.ToString(), out CssUnicodeRange? parsed))
        {
            result = parsed.Value;
            return true;
        }

        return false;
    }
    #endregion

    #region Serialization
    /// <summary>
    /// Serializes this unicode range to its canonical CSS string representation.
    /// </summary>
    /// <returns>The canonical serialization of this unicode range.</returns>
    /// <remarks>
    /// <para>
    /// For a single code point, returns "U+XXXX" where XXXX is the hexadecimal value.
    /// For a range, returns "U+XXXX-YYYY" where XXXX and YYYY are the start and end values.
    /// </para>
    /// <para>
    /// Note: This method does not attempt to serialize using the wildcard (?) syntax,
    /// as the canonical form uses explicit ranges.
    /// </para>
    /// </remarks>
    public string Serialize()
    {
        if (IsSingleCodePoint)
        {
            return $"U+{Start:X}";
        }
        return $"U+{Start:X}-{End:X}";
    }

    /// <inheritdoc/>
    public override string ToString() => Serialize();

    /// <inheritdoc/>
    public string ToString(string? format, IFormatProvider? formatProvider) => Serialize();

    /// <inheritdoc/>
    /// <remarks>
    /// For a single code point, formats as "U+XXXX".
    /// For a range, formats as "U+XXXX-YYYY".
    /// Note: This does not use wildcard (?) syntax; canonical form uses explicit ranges.
    /// </remarks>
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
    {
        charsWritten = 0;

        // U+
        if (!"U+".TryCopyTo(destination))
            return false;
        charsWritten += 2;

        // Start value (uppercase hex)
        if (!Start.TryFormat(destination[charsWritten..], out int startWritten, "X", CultureInfo.InvariantCulture))
            return false;
        charsWritten += startWritten;

        // If range (not single code point), add -End
        if (!IsSingleCodePoint)
        {
            if (!"-".TryCopyTo(destination[charsWritten..]))
                return false;
            charsWritten += 1;

            if (!End.TryFormat(destination[charsWritten..], out int endWritten, "X", CultureInfo.InvariantCulture))
                return false;
            charsWritten += endWritten;
        }

        return true;
    }
    #endregion

    #region Equality
    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return obj is CssUnicodeRange other && Equals(other);
    }

    /// <inheritdoc/>
    public bool Equals(CssUnicodeRange other)
    {
        return Start == other.Start && End == other.End;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(Start, End);
    }

    public static bool operator ==(CssUnicodeRange left, CssUnicodeRange right) => left.Equals(right);
    public static bool operator !=(CssUnicodeRange left, CssUnicodeRange right) => !left.Equals(right);
    #endregion
}
