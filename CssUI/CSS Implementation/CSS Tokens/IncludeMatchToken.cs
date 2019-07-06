

namespace CssUI.CSS
{
    public sealed class IncludeMatchToken : CssToken
    {
        public IncludeMatchToken() : base(ECssTokenType.Include_Match)
        {
        }

        public override string Encode() { return "~="; }
    }
}
