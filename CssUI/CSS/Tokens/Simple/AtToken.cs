using System;

namespace CssUI.CSS.Parser
{
    public sealed class AtToken : ValuedTokenBase
    {
        public AtToken(ReadOnlySpan<char> Value) : base(ECssTokenType.At_Keyword, Value)
        {
        }

        public override string Encode()
        {
            return Value;
        }
    }
}
