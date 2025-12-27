using System;

namespace CssUI.HTTP;

public class UrlOrigin : ISpanFormattable
{/* Docs: https://html.spec.whatwg.org/multipage/origin.html#concept-origin-opaque */

    #region Static
    public static UrlOrigin Opaque = new UrlOrigin();
    public static UrlOrigin Default = new UrlOrigin(null, null, null, "CSSUI");
    #endregion

    #region Properties
    public readonly EOriginType Type;
    public readonly string? Scheme = null;
    public readonly UrlHost? Host = null;
    public readonly ushort? Port = null;
    public readonly string? Domain = null;
    #endregion

    #region Constructors
    public UrlOrigin()
    {
        Type = EOriginType.Opaque;
    }

    public UrlOrigin(string? scheme, UrlHost? host, ushort? port, string? domain)
    {
        Type = EOriginType.Tuple;
        Scheme = scheme;
        Host = host;
        Port = port;
        Domain = domain;
    }
    #endregion

    #region ISpanFormattable
    /// <summary>
    /// Serializes this origin to its ASCII representation.
    /// </summary>
    /// <seealso href="https://html.spec.whatwg.org/multipage/origin.html#ascii-serialisation-of-an-origin"/>
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
    {
        charsWritten = 0;

        if (Type == EOriginType.Opaque)
        {
            if (destination.Length < 4) return false;
            "null".AsSpan().CopyTo(destination);
            charsWritten = 4;
            return true;
        }

        // Append scheme
        var schemeName = Scheme!;
        if (destination.Length < schemeName.Length) return false;
        schemeName.AsSpan().CopyTo(destination);
        charsWritten = schemeName.Length;

        // Append "://"
        if (destination.Length < charsWritten + 3) return false;
        destination[charsWritten++] = ':';
        destination[charsWritten++] = '/';
        destination[charsWritten++] = '/';

        // Append host
        if (Host is not null)
        {
            if (!Host.TryFormat(destination[charsWritten..], out int hostWritten, format, provider))
                return false;
            charsWritten += hostWritten;
        }

        // Append port if present
        if (Port.HasValue)
        {
            if (destination.Length <= charsWritten) return false;
            destination[charsWritten++] = ':';
            if (!Port.Value.TryFormat(destination[charsWritten..], out int portWritten, default, provider))
                return false;
            charsWritten += portWritten;
        }

        return true;
    }

    /// <inheritdoc/>
    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        Span<char> buffer = stackalloc char[512];
        if (TryFormat(buffer, out int charsWritten, format.AsSpan(), formatProvider))
        {
            return buffer[..charsWritten].ToString();
        }
        return string.Empty;
    }

    /// <inheritdoc/>
    public override string ToString() => ToString(null, null);
    #endregion

    #region Equality

    public bool IsSameOrigin(UrlOrigin other)
    {/* Docs: https://html.spec.whatwg.org/multipage/origin.html#same-origin */
        if (other == null) return false;
        if (Type == EOriginType.Opaque && other.Type == EOriginType.Opaque)
            return true;

        if (Type == EOriginType.Tuple && other.Type == EOriginType.Tuple)
        {
            return Scheme == other.Scheme && Host == other.Host && Port == other.Port && Domain.AsSpan().Equals(other.Domain.AsSpan(), StringComparison.Ordinal);
        }

        return false;
    }

    public bool IsSameOriginDomain(UrlOrigin other)
    {/* Docs: https://html.spec.whatwg.org/multipage/origin.html#same-origin-domain */
        if (other == null) return false;
        if (Type == EOriginType.Opaque && other.Type == EOriginType.Opaque)
            return true;

        if (Type == EOriginType.Tuple && other.Type == EOriginType.Tuple)
        {
            if (Scheme == other.Scheme && Domain is not null && other.Domain is not null && Domain.AsSpan().Equals(other.Domain.AsSpan(), StringComparison.Ordinal))
            {
                return true;
            }
            else if (IsSameOrigin(other) && Domain is not null && other.Domain is not null && Domain.AsSpan().Equals(other.Domain.AsSpan(), StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }



    public static bool IsSameOrigin(string A, string B)
    {
        var urlA = new Url(A);
        var urlB = new Url(B);
        var originA = urlA.Origin;
        var originB = urlB.Origin;

        return originA.IsSameOrigin(originB);
    }

    public static bool IsSameOriginDomain(string A, string B)
    {
        var urlA = new Url(A);
        var urlB = new Url(B);
        var originA = urlA.Origin;
        var originB = urlB.Origin;

        return originA.IsSameOriginDomain(originB);
    }
    #endregion
}

