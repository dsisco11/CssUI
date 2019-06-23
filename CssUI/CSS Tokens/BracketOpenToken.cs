
namespace CssUI.CSS
{
    /// <summary>
    /// '{'
    /// </summary>
    public sealed class BracketOpenToken : CssToken
    {
        public BracketOpenToken() : base(ECssTokenType.Bracket_Open)
        {
        }

        public override string Encode() { return "{"; }
    }
}
