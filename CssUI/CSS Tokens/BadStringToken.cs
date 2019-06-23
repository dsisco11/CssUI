
namespace CssUI.CSS
{
    public sealed class BadStringToken : ValuedTokenBase
    {
        public BadStringToken(string Value) : base(ECssTokenType.Bad_String, Value, false)
        {
        }

        public override string Encode() { return string.Empty; }
    }
}
