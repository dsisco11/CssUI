
namespace CssUI.CSS
{
    public sealed class ParenthesisOpenToken : CssToken
    {
        public ParenthesisOpenToken() : base(ECssTokenType.Parenth_Open)
        {
        }

        public override string Encode() { return "("; }
    }
}
