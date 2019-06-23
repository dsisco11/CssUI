
namespace CssUI.CSS
{
    public sealed class FunctionNameToken : ValuedTokenBase
    {
        public FunctionNameToken(string Value) : base(ECssTokenType.FunctionName, Value)
        {
        }

        public override string Encode()
        {
            return this.Value;
        }
    }
}
