using System;

namespace CssUI.HTTP;

/// <summary>
/// Contains 8-bit integers making up a single 32-bit integer which indicates a network address
/// </summary>
public struct IPV4Address : ISpanFormattable
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

