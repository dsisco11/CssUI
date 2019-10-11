using System;

namespace CssUI.CSS.Parser
{
    public sealed class BadStringToken : ValuedTokenBase
    {
        public BadStringToken(ReadOnlySpan<char> Value) : base(ECssTokenType.Bad_String, Value, false)
        {
        }

        public override string Encode() { return string.Empty; }
    }
}
