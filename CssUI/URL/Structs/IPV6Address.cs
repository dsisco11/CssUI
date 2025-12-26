using System;
using System.Diagnostics.CodeAnalysis;

namespace CssUI.HTTP;

public struct IPV6Address : ISpanFormattable, IParsable<IPV6Address>, ISpanParsable<IPV6Address>
{/* Docs: https://url.spec.whatwg.org/#concept-ipv6 */

    #region Properties
    public readonly ushort[] Parts;
    #endregion

    #region Constructors

    public IPV6Address(ushort A, ushort B, ushort C, ushort D, ushort E, ushort F, ushort G, ushort H)
    {
        Parts = new ushort[8] { A, B, C, D, E, F, G, H };
    }

    public IPV6Address(ushort[] parts)
    {
        Parts = parts;
    }
    #endregion

    public byte[] GetAddressBytes()
    {
        byte[] address = new byte[16];

        address[0] = (byte)(Parts[0] & 0xFF);
        address[1] = (byte)(Parts[0] << 4 & 0xFF);

        address[2] = (byte)(Parts[1] & 0xFF);
        address[3] = (byte)(Parts[1] << 4 & 0xFF);

        address[4] = (byte)(Parts[2] & 0xFF);
        address[5] = (byte)(Parts[2] << 4 & 0xFF);

        address[6] = (byte)(Parts[3] & 0xFF);
        address[7] = (byte)(Parts[3] << 4 & 0xFF);

        address[8] = (byte)(Parts[4] & 0xFF);
        address[9] = (byte)(Parts[4] << 4 & 0xFF);

        address[10] = (byte)(Parts[5] & 0xFF);
        address[11] = (byte)(Parts[5] << 4 & 0xFF);

        address[12] = (byte)(Parts[6] & 0xFF);
        address[13] = (byte)(Parts[6] << 4 & 0xFF);

        address[14] = (byte)(Parts[7] & 0xFF);
        address[15] = (byte)(Parts[7] << 4 & 0xFF);

        return address;
    }

    #region Parsing (IParsable, ISpanParsable)
    /// <summary>
    /// Parses an IPv6 address from a string.
    /// </summary>
    /// <param name="s">The string to parse.</param>
    /// <param name="provider">The format provider (ignored).</param>
    /// <returns>The parsed IPv6 address.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="s"/> is null.</exception>
    /// <exception cref="FormatException">Thrown when the string is not a valid IPv6 address.</exception>
    public static IPV6Address Parse(string s, IFormatProvider? provider)
    {
        ArgumentNullException.ThrowIfNull(s);
        return Parse(s.AsSpan(), provider);
    }

    /// <summary>
    /// Parses an IPv6 address from a character span.
    /// </summary>
    /// <param name="s">The span to parse.</param>
    /// <param name="provider">The format provider (ignored).</param>
    /// <returns>The parsed IPv6 address.</returns>
    /// <exception cref="FormatException">Thrown when the span is not a valid IPv6 address.</exception>
    public static IPV6Address Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        if (!TryParse(s, provider, out IPV6Address result))
        {
            throw new FormatException($"Invalid IPv6 address: '{s.ToString()}'");
        }
        return result;
    }

    /// <summary>
    /// Tries to parse an IPv6 address from a string.
    /// </summary>
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out IPV6Address result)
    {
        result = default;
        if (string.IsNullOrWhiteSpace(s))
            return false;

        return TryParse(s.AsSpan(), provider, out result);
    }

    /// <summary>
    /// Tries to parse an IPv6 address from a character span.
    /// </summary>
    /// <remarks>
    /// Simplified parser for standard colon-hex notation. For full WHATWG URL spec compliance,
    /// use the Url.Parse_IPV6 method.
    /// </remarks>
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out IPV6Address result)
    {
        result = default;
        s = s.Trim();
        if (s.IsEmpty)
            return false;

        ushort[] parts = new ushort[8];
        int partIndex = 0;
        int compressIndex = -1;
        int currentValue = 0;
        int digitCount = 0;

        for (int i = 0; i < s.Length; i++)
        {
            char c = s[i];
            if (UnicodeCommon.Is_Ascii_Hex_Digit(c))
            {
                currentValue = currentValue * 16 + UnicodeCommon.Ascii_Hex_To_Value(c);
                if (currentValue > 0xFFFF)
                    return false;
                digitCount++;
                if (digitCount > 4)
                    return false;
            }
            else if (c == ':')
            {
                if (i + 1 < s.Length && s[i + 1] == ':')
                {
                    // Compression marker ::
                    if (compressIndex >= 0)
                        return false; // Only one :: allowed
                    if (digitCount > 0)
                    {
                        if (partIndex >= 8) return false;
                        parts[partIndex++] = (ushort)currentValue;
                    }
                    compressIndex = partIndex;
                    currentValue = 0;
                    digitCount = 0;
                    i++; // Skip second colon
                }
                else
                {
                    if (digitCount == 0 && compressIndex < 0)
                        return false;
                    if (digitCount > 0)
                    {
                        if (partIndex >= 8) return false;
                        parts[partIndex++] = (ushort)currentValue;
                    }
                    currentValue = 0;
                    digitCount = 0;
                }
            }
            else
            {
                return false;
            }
        }

        // Handle final part
        if (digitCount > 0)
        {
            if (partIndex >= 8) return false;
            parts[partIndex++] = (ushort)currentValue;
        }

        // Expand compression if present
        if (compressIndex >= 0)
        {
            int zerosNeeded = 8 - partIndex;
            if (zerosNeeded < 0) return false;

            // Shift parts after compress point
            for (int i = partIndex - 1; i >= compressIndex; i--)
            {
                parts[i + zerosNeeded] = parts[i];
                parts[i] = 0;
            }
        }
        else if (partIndex != 8)
        {
            return false;
        }

        result = new IPV6Address(parts);
        return true;
    }
    #endregion

    #region ISpanFormattable
    /// <summary>
    /// Serializes this IPv6 address to its canonical string representation.
    /// </summary>
    /// <seealso href="https://url.spec.whatwg.org/#concept-ipv6-serializer"/>
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
    {
        charsWritten = 0;
        int? compress = null;
        bool bCompress = false;

        // Find compression point (longest run of zeros)
        for (int i = 0; i < Parts.Length; i++)
        {
            var part = Parts[i];
            if (part == 0)
            {
                if (bCompress)
                {
                    compress = i;
                    break;
                }
            }
            else if (!bCompress)
            {
                bCompress = true;
                continue;
            }
        }

        bool ignore0 = false;
        for (int pieceIndex = 0; pieceIndex < Parts.Length; pieceIndex++)
        {
            if (ignore0 && Parts[pieceIndex] == 0) continue;
            else if (ignore0)
            {
                ignore0 = false;
            }

            if (compress == pieceIndex)
            {
                if (pieceIndex == 0)
                {
                    if (destination.Length < charsWritten + 2) return false;
                    destination[charsWritten++] = UnicodeCommon.CHAR_COLON;
                    destination[charsWritten++] = UnicodeCommon.CHAR_COLON;
                }
                else
                {
                    if (destination.Length <= charsWritten) return false;
                    destination[charsWritten++] = UnicodeCommon.CHAR_COLON;
                }

                ignore0 = true;
                continue;
            }

            var hexValue = UnicodeCommon.Ascii_Value_To_Hex(Parts[pieceIndex]);
            if (destination.Length < charsWritten + hexValue.Length) return false;
            hexValue.AsSpan().CopyTo(destination[charsWritten..]);
            charsWritten += hexValue.Length;

            if (pieceIndex != 7)
            {
                if (destination.Length <= charsWritten) return false;
                destination[charsWritten++] = UnicodeCommon.CHAR_COLON;
            }
        }

        return true;
    }

    /// <inheritdoc/>
    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        Span<char> buffer = stackalloc char[39]; // Max: "ffff:ffff:ffff:ffff:ffff:ffff:ffff:ffff"
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

