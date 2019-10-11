using System;

namespace CssUI.HTTP
{

    public class UrlHost
    {/* Docs: https://url.spec.whatwg.org/#concept-host */

        #region Properties
        public readonly dynamic Value;
        public readonly EHostType Type;
        #endregion

        #region Constructors
        public UrlHost(string host)
        {
            Value = host;
            if (ReferenceEquals(null, host) || host.Length <= 0)
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

        public virtual string Serialize()
        {/* Docs: https://url.spec.whatwg.org/#concept-host-serializer */
            switch (Type)
            {
                case EHostType.Domain:
                case EHostType.Opaque:
                    return Value;
                case EHostType.IPV4Address:
                    return ((IPV4Address)Value).Serialize();
                case EHostType.IPV6Address:
                    return string.Concat(UnicodeCommon.CHAR_LEFT_SQUARE_BRACKET, ((IPV6Address)Value).Serialize(), UnicodeCommon.CHAR_RIGHT_SQUARE_BRACKET);
                default:
                    return string.Empty;
            }
        }

        public override string ToString()
        {
            return Serialize();
        }
    }
}
