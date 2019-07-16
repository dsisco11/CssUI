
using CssUI.CSS.Parser;

namespace CssUI.CSS
{
    public sealed class WhitespaceToken : CssToken
    {
        public WhitespaceToken(string Value) : base(ECssTokenType.Whitespace)
        {
        }

        public override string Encode() { return " "; }
    }
}
