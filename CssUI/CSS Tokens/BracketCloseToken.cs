
namespace CssUI.CSS
{
    /// <summary>
    /// Represents '}'
    /// </summary>
    public sealed class BracketCloseToken : CssToken
    {
        public BracketCloseToken() : base(ECssTokenType.Bracket_Open)
        {
        }

        public override string Encode() { return "}"; }
    }
}
