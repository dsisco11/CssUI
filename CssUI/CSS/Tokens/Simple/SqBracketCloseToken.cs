
using CssUI.CSS.Parser;

namespace CssUI.CSS
{
    public sealed class SqBracketCloseToken : CssToken
    {
        public static SqBracketCloseToken Instance = new SqBracketCloseToken();
        public SqBracketCloseToken() : base(ECssTokenType.SqBracket_Close)
        {
        }

        public override string Encode() => "]";
    }
}
