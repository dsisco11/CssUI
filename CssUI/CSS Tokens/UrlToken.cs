
namespace CssUI.CSS
{
    public class UrlToken : ValuedTokenBase
    {
        #region Constructors
        public UrlToken(string Value, ECssTokenType Type) : base(Type, Value, false)
        {
        }
        public UrlToken(string Value) : base(ECssTokenType.Url, Value, false)
        {
        }
        #endregion

        public override string Encode()
        {
            return string.Concat("url(", Value, ")");
        }
    }
}
