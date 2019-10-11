
using CssUI.CSS.Parser;

namespace CssUI.CSS
{
    public sealed class ColumnToken : CssToken
    {
        public static ColumnToken Instance = new ColumnToken();
        public ColumnToken() : base(ECssTokenType.Column)
        {
        }

        public override string Encode() => "||";
    }
}
