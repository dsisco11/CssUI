using System;

namespace CssUI.CSS.Parser
{
    public sealed class FunctionNameToken : ValuedTokenBase
    {
        public FunctionNameToken(ReadOnlySpan<char> Value) : base(ECssTokenType.FunctionName, Value)
        {
        }

        public override string Encode()
        {
            return string.Concat(Value, UnicodeCommon.CHAR_LEFT_PARENTHESES);
        }
    }
}
