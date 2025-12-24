using System;

namespace CssUI.HTTP;

public struct IPV6Address : ISpanFormattable
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

