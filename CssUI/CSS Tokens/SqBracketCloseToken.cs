
namespace CssUI.CSS
{
    public sealed class SqBracketCloseToken : CssToken
    {
        public SqBracketCloseToken() : base(ECssTokenType.SqBracket_Close)
        {
        }

        public override string Encode() { return "]"; }
    }
}
