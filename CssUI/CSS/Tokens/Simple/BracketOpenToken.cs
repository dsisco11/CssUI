
using CssUI.CSS.Parser;

namespace CssUI.CSS
{
    /// <summary>
    /// '{'
    /// </summary>
    public sealed class BracketOpenToken : CssToken
    {
        public static BracketOpenToken Instance = new BracketOpenToken();
        public BracketOpenToken() : base(ECssTokenType.Bracket_Open)
        {
        }

        public override string Encode() => "{";
    }
}
