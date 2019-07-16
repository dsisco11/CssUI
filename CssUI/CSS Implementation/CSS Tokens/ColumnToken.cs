
using CssUI.CSS.Parser;

namespace CssUI.CSS
{
    public sealed class ColumnToken : CssToken
    {
        public ColumnToken() : base(ECssTokenType.Column)
        {
        }

        public override string Encode() { return "||"; }
    }
}
