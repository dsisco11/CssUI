
using CssUI.CSS.Parser;

namespace CssUI.CSS
{
    public sealed class SubstringMatchToken : CssToken
    {
        public static SubstringMatchToken Instance = new SubstringMatchToken();
        public SubstringMatchToken() : base(ECssTokenType.Substring_Match)
        {
        }

        public override string Encode() => "*=";
    }
}
