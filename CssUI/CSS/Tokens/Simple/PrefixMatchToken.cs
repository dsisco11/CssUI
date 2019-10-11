
using CssUI.CSS.Parser;

namespace CssUI.CSS
{
    public sealed class PrefixMatchToken : CssToken
    {
        public static PrefixMatchToken Instance = new PrefixMatchToken();
        public PrefixMatchToken() : base(ECssTokenType.Prefix_Match)
        {
        }

        public override string Encode() => "^=";
    }
}
