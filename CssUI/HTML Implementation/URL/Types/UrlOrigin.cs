﻿using System;
using System.Text;

namespace CssUI.HTML
{
    public class UrlOrigin
    {/* Docs: https://html.spec.whatwg.org/multipage/origin.html#concept-origin-opaque */

        #region Properties
        public readonly EOriginType Type;
        public readonly AtomicName<EUrlScheme> Scheme = null;
        public readonly UrlHost Host = null;
        public readonly ushort? Port = null;
        public readonly string Domain = null;
        #endregion

        #region Constructors
        public UrlOrigin()
        {
            Type = EOriginType.Opaque;
        }

        public UrlOrigin(AtomicName<EUrlScheme> scheme, UrlHost host, ushort? port, string domain)
        {
            Type = EOriginType.Tuple;
            Scheme = scheme;
            Host = host;
            Port = port;
            Domain = domain;
        }
        #endregion

        public bool IsSameOrigin(UrlOrigin other)
        {/* Docs: https://html.spec.whatwg.org/multipage/origin.html#same-origin */
            if (other == null) return false;
            if (Type == EOriginType.Opaque && other.Type == EOriginType.Opaque)
                return true;

            if (Type == EOriginType.Tuple && other.Type == EOriginType.Tuple)
            {
                return (Scheme == other.Scheme) && (Host == other.Host) && (Port == other.Port) && (Domain.AsSpan().Equals(other.Domain.AsSpan(), StringComparison.Ordinal));
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
                if (Scheme == other.Scheme && !ReferenceEquals(null, Domain) && !ReferenceEquals(null, other.Domain) && (Domain.AsSpan().Equals(other.Domain.AsSpan(), StringComparison.Ordinal)))
                {
                    return true;
                }
                else if (IsSameOrigin(other) && !ReferenceEquals(null, Domain) && !ReferenceEquals(null, other.Domain) && (Domain.AsSpan().Equals(other.Domain.AsSpan(), StringComparison.Ordinal)))
                {
                    return true;
                }
            }

            return false;
        }
    }

    public static class UrlOriginExt
    {
        public static string Serialize(this UrlOrigin origin)
        {
            if (origin == null || origin.Type == EOriginType.Opaque)
            {
                return "null";
            }

            StringBuilder result = new StringBuilder();
            result.Append(origin.Scheme.NameLower);
            result.Append("://");
            result.Append(origin.Host.Serialize());
            if (origin.Port.HasValue)
            {
                result.Append(":");
                result.Append(origin.Port.Value);
            }

            return result.ToString();
        }
    }

}