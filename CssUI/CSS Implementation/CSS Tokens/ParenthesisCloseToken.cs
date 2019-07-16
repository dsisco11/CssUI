
using CssUI.CSS.Parser;

namespace CssUI.CSS
{
    public sealed class ParenthesisCloseToken : CssToken
    {
        public ParenthesisCloseToken() : base(ECssTokenType.Parenth_Close)
        {
        }

        public override string Encode() { return ")"; }
    }
}
