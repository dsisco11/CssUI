
using CssUI.CSS.Parser;

namespace CssUI.CSS
{
    public sealed class SemicolonToken : CssToken
    {
        public static SemicolonToken Instance = new SemicolonToken();

        public SemicolonToken() : base(ECssTokenType.Semicolon)
        {
        }

        public override string Encode() => ";";
    }
}
