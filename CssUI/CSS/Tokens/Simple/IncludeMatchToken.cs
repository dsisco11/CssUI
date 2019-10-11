

using CssUI.CSS.Parser;

namespace CssUI.CSS
{
    public sealed class IncludeMatchToken : CssToken
    {
        public static IncludeMatchToken Instance = new IncludeMatchToken();
        public IncludeMatchToken() : base(ECssTokenType.Include_Match)
        {
        }

        public override string Encode() => "~=";
    }
}
