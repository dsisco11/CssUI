namespace CssUI.CSS.Parser
{
    public sealed class IdentToken : ValuedTokenBase
    {
        public IdentToken(string Value) : base(ECssTokenType.Ident, Value)
        {
        }

        public override string Encode()
        {
            return Value;
        }
    }
}
