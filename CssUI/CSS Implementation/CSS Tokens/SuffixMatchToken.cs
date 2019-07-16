
using CssUI.CSS.Parser;

namespace CssUI.CSS
{
    public sealed class SuffixMatchToken : CssToken
    {
        public SuffixMatchToken() : base(ECssTokenType.Suffix_Match)
        {
        }

        public override string Encode() { return "$="; }
    }
}
