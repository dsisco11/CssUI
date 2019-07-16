using CssUI.CSS.Parser;

namespace CssUI.CSS.Selectors
{
    public class CombinatorToken : CssToken
    {
        public readonly string Value;
        public CombinatorToken(string Value) : base(ECssTokenType.Combinator)
        {
            this.Value = Value;
        }

        public override string Encode()
        {
            return Value;
        }
    }
}
