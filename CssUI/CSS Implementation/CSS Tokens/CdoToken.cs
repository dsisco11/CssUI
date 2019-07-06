
namespace CssUI.CSS
{
    public sealed class CdoToken : CssToken
    {
        public CdoToken() : base(ECssTokenType.CDO)
        {
        }

        public override string Encode() { return "<!--"; }
    }
}
