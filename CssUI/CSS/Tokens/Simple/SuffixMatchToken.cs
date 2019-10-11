
using CssUI.CSS.Parser;

namespace CssUI.CSS
{
    public sealed class SuffixMatchToken : CssToken
    {
        public static SuffixMatchToken Instance = new SuffixMatchToken();
        public SuffixMatchToken() : base(ECssTokenType.Suffix_Match)
        {
        }

        public override string Encode() => "$=";
    }
}
