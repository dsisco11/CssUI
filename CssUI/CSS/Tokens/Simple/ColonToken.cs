
using CssUI.CSS.Parser;

namespace CssUI.CSS
{
    public sealed class ColonToken : CssToken
    {
        public static ColonToken Instance = new ColonToken();
        public ColonToken() : base(ECssTokenType.Colon)
        {
        }

        public override string Encode() => ":";
    }
}
