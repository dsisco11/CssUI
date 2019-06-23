
namespace CssUI.CSS
{
    public sealed class AtToken : ValuedTokenBase
    {
        public AtToken(string Value) : base(ECssTokenType.At_Keyword, Value)
        {
        }

        public override string Encode()
        {
            return this.Value;
        }
    }
}
