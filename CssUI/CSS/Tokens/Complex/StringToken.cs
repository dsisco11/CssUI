using System;

namespace CssUI.CSS.Parser
{
    public sealed class StringToken : ValuedTokenBase
    {
        public StringToken(ReadOnlySpan<char> Value) : base(ECssTokenType.String, Value, false)
        {
        }

    }
}
