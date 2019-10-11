
using CssUI.CSS.Parser;

namespace CssUI.CSS
{
    public sealed class SqBracketOpenToken : CssToken
    {
        public static SqBracketOpenToken Instance = new SqBracketOpenToken();
        public SqBracketOpenToken() : base(ECssTokenType.SqBracket_Open)
        {
        }

        public override string Encode() => "[";
    }
}
