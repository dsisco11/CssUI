
using CssUI.CSS.Parser;

namespace CssUI.CSS
{
    public sealed class ParenthesisOpenToken : CssToken
    {
        public static ParenthesisOpenToken Instance = new ParenthesisOpenToken();
        public ParenthesisOpenToken() : base(ECssTokenType.Parenth_Open)
        {
        }

        public override string Encode() => "(";
    }
}
