
namespace CssUI.CSS
{
    public sealed class SubstringMatchToken : CssToken
    {
        public SubstringMatchToken() : base(ECssTokenType.Substring_Match)
        {
        }

        public override string Encode() { return "*="; }
    }
}
