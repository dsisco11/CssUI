
namespace CssUI.CSS
{
    public sealed class PrefixMatchToken : CssToken
    {
        public PrefixMatchToken() : base(ECssTokenType.Prefix_Match)
        {
        }

        public override string Encode() { return "^="; }
    }
}
