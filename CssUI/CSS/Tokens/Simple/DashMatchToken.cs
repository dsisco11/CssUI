
using CssUI.CSS.Parser;

namespace CssUI.CSS
{
    public sealed class DashMatchToken : CssToken
    {
        public static DashMatchToken Instance = new DashMatchToken();
        public DashMatchToken() : base(ECssTokenType.Dash_Match)
        {
        }

        public override string Encode() => "|=";
    }
}
