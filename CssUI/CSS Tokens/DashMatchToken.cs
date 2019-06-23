
namespace CssUI.CSS
{
    public sealed class DashMatchToken : CssToken
    {
        public DashMatchToken() : base(ECssTokenType.Dash_Match)
        {
        }

        public override string Encode() { return "|="; }
    }
}
