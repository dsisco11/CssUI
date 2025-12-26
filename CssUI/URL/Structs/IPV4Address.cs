using System;
using System.Diagnostics.CodeAnalysis;

namespace CssUI.HTTP;

/// <summary>
/// Contains 8-bit integers making up a single 32-bit integer which indicates a network address
/// </summary>
public struct IPV4Address : ISpanFormattable, IParsable<IPV4Address>, ISpanParsable<IPV4Address>
{/* Docs: https://url.spec.whatwg.org/#concept-ipv4 */

    #region Properties
    public readonly byte[] Part;
    #endregion

    #region Constructors

    public IPV4Address(byte A, byte B, byte C, byte D)
    {
        Part = new byte[4] { A, B, C, D };
    }
    #endregion

    public uint Address
    {
        get => (uint)(Part[0] << 0 | Part[1] << 8 | Part[2] << 16 | Part[3] << 24);
    }

    #region Parsing (IParsable, ISpanParsable)
    /// <summary>
    /// Parses an IPv4 address from a string.
    /// </summary>
    /// <param name="s">The string to parse.</param>
    /// <param name="provider">The format provider (ignored).</param>
    /// <returns>The parsed IPv4 address.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="s"/> is null.</exception>
    /// <exception cref="FormatException">Thrown when the string is not a valid IPv4 address.</exception>
    public static IPV4Address Parse(string s, IFormatProvider? provider)
    {
        ArgumentNullException.ThrowIfNull(s);
        return Parse(s.AsSpan(), provider);
    }

    /// <summary>
    /// Parses an IPv4 address from a character span.
    /// </summary>
    /// <param name="s">The span to parse.</param>
    /// <param name="provider">The format provider (ignored).</param>
    /// <returns>The parsed IPv4 address.</returns>
    /// <exception cref="FormatException">Thrown when the span is not a valid IPv4 address.</exception>
    public static IPV4Address Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        if (!TryParse(s, provider, out IPV4Address result))
        {
            throw new FormatException($"Invalid IPv4 address: '{s.ToString()}'");
        }
        return result;
    }

    /// <summary>
    /// Tries to parse an IPv4 address from a string.
    /// </summary>
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out IPV4Address result)
    {
        result = default;
        if (string.IsNullOrWhiteSpace(s))
            return false;

        return TryParse(s.AsSpan(), provider, out result);
    }

    /// <summary>
    /// Tries to parse an IPv4 address from a character span.
    /// </summary>
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out IPV4Address result)
    {
        result = default;
        s = s.Trim();
        if (s.IsEmpty)
            return false;

        // Simple IPv4 parsing: a.b.c.d where each part is 0-255
        Span<byte> parts = stackalloc byte[4];
        int partIndex = 0;
        int currentValue = 0;
        bool hasDigit = false;

        for (int i = 0; i < s.Length; i++)
        {
            char c = s[i];
            if (c >= '0' && c <= '9')
            {
                currentValue = currentValue * 10 + (c - '0');
                if (currentValue > 255)
                    return false;
                hasDigit = true;
            }
            else if (c == '.')
            {
                if (!hasDigit || partIndex >= 3)
                    return false;
                parts[partIndex++] = (byte)currentValue;
                currentValue = 0;
                hasDigit = false;
            }
            else
            {
                return false;
            }
        }

        if (!hasDigit || partIndex != 3)
            return false;

        parts[3] = (byte)currentValue;
        result = new IPV4Address(parts[0], parts[1], parts[2], parts[3]);
        return true;
    }
    #endregion

    #region ISpanFormattable
    /// <summary>
    /// Serializes this IPv4 address to its canonical string representation.
    /// </summary>
    /// <seealso href="https://url.spec.whatwg.org/#concept-ipv4-serializer"/>
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
    {
        charsWritten = 0;
        for (int i = 0; i < 4; i++)
        {
            int partValue = Part[i] % 256;
            if (!partValue.TryFormat(destination[charsWritten..], out int partWritten, default, provider))
                return false;
            charsWritten += partWritten;

            if (i != 3)
            {
                if (destination.Length <= charsWritten) return false;
                destination[charsWritten] = UnicodeCommon.CHAR_FULL_STOP;
                charsWritten++;
            }
        }
        return true;
    }

    /// <inheritdoc/>
    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        Span<char> buffer = stackalloc char[15]; // Max: "255.255.255.255"
        if (TryFormat(buffer, out int charsWritten, format.AsSpan(), formatProvider))
        {
            return buffer[..charsWritten].ToString();
        }
        return string.Empty;
    }

    /// <inheritdoc/>
    public override string ToString() => ToString(null, null);
    #endregion
}

