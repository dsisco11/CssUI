
namespace CssUI.CSS
{
    public sealed class SqBracketOpenToken : CssToken
    {
        public SqBracketOpenToken() : base(ECssTokenType.SqBracket_Open)
        {
        }

        public override string Encode() { return "["; }
    }
}
