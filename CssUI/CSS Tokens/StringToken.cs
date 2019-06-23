
namespace CssUI.CSS
{
    public sealed class StringToken : ValuedTokenBase
    {
        public StringToken(string Value) : base(ECssTokenType.String, Value, false)
        {
        }

        public override string Encode()
        {
            return this.Value;
        }
    }
}
