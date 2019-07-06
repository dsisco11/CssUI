
namespace CssUI.CSS
{
    public sealed class CdcToken : CssToken
    {
        public CdcToken() : base(ECssTokenType.CDC)
        {
        }

        public override string Encode() { return "-->"; }
    }
}
