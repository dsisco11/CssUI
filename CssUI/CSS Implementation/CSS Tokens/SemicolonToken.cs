
namespace CssUI.CSS
{
    public sealed class SemicolonToken : CssToken
    {
        public SemicolonToken() : base(ECssTokenType.Semicolon)
        {
        }

        public override string Encode() { return ";"; }
    }
}
