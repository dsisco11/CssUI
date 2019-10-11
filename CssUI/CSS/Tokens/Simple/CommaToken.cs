
using CssUI.CSS.Parser;

namespace CssUI.CSS
{
    public sealed class CommaToken : CssToken
    {
        public static CommaToken Instance = new CommaToken();
        public CommaToken() : base(ECssTokenType.Comma)
        {
        }

        public override string Encode() => ",";
    }
}
