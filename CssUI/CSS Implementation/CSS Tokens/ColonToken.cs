
using CssUI.CSS.Parser;

namespace CssUI.CSS
{
    public sealed class ColonToken : CssToken
    {
        public ColonToken() : base(ECssTokenType.Colon)
        {
        }

        public override string Encode() { return ":"; }
    }
}
