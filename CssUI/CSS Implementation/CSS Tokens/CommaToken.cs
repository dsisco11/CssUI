
using CssUI.CSS.Parser;

namespace CssUI.CSS
{
    public sealed class CommaToken : CssToken
    {
        public CommaToken() : base(ECssTokenType.Comma)
        {
        }

        public override string Encode() { return ","; }
    }
}
