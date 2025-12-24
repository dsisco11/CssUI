using System;

namespace CssUI.HTTP;


public class UrlHost : ISpanFormattable
{/* Docs: https://url.spec.whatwg.org/#concept-host */

    #region Properties
    public readonly dynamic Value;
    public readonly EHostType Type;
    #endregion

    #region Constructors
    public UrlHost(string host)
    {
        Value = host;
        if (host is null || host.Length <= 0)
        {
            Type = EHostType.Empty;
        }
        else
        {
            Type = EHostType.Opaque;
            if (StringCommon.Contains(host.AsSpan(), UnicodeCommon.CHAR_FULL_STOP))
            {
                Type = EHostType.Domain;
            }
        }
    }

    public UrlHost(IPV4Address address)
    {
        Value = address;
        Type = EHostType.IPV4Address;
    }

    public UrlHost(IPV6Address address)
    {
        Value = address;
        Type = EHostType.IPV6Address;
    }
    #endregion

    #region Accessors
    #endregion

    #region ISpanFormattable
    /// <summary>
    /// Serializes this host to its canonical string representation.
    /// </summary>
    /// <seealso href="https://url.spec.whatwg.org/#concept-host-serializer"/>
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
    {
        charsWritten = 0;
        switch (Type)
        {
            case EHostType.Domain:
            case EHostType.Opaque:
                {
                    string value = Value;
                    if (destination.Length < value.Length) return false;
                    value.AsSpan().CopyTo(destination);
                    charsWritten = value.Length;
                    return true;
                }
            case EHostType.IPV4Address:
                return ((IPV4Address)Value).TryFormat(destination, out charsWritten, format, provider);
            case EHostType.IPV6Address:
                {
                    if (destination.Length < 2) return false;
                    destination[0] = UnicodeCommon.CHAR_LEFT_SQUARE_BRACKET;
                    charsWritten = 1;
                    if (!((IPV6Address)Value).TryFormat(destination[1..], out int ipv6Written, format, provider))
                        return false;
                    charsWritten += ipv6Written;
                    if (destination.Length <= charsWritten) return false;
                    destination[charsWritten++] = UnicodeCommon.CHAR_RIGHT_SQUARE_BRACKET;
                    return true;
                }
            default:
                return true; // Empty string
        }
    }

    /// <inheritdoc/>
    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        Span<char> buffer = stackalloc char[256];
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

