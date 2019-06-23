
namespace CssUI.CSS
{
    public class BadUrlToken : UrlToken
    {
        public BadUrlToken() : base(string.Empty, ECssTokenType.Bad_Url)
        {
        }

        public override string Encode() { return string.Empty; }
    }
}
