using System;

namespace CssUI.CSS.Parser
{
    public class UrlToken : ValuedTokenBase
    {
        #region Constructors
        public UrlToken(ReadOnlySpan<char> Value, ECssTokenType Type) : base(Type, Value, false)
        {
        }
        public UrlToken(ReadOnlySpan<char> Value) : base(ECssTokenType.Url, Value, false)
        {
        }
        #endregion

        public override string Encode()
        {
            return string.Concat("url(", Value, ")");
        }
    }
}
