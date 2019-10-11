
using CssUI.CSS.Parser;

namespace CssUI.CSS
{
    public sealed class CdoToken : CssToken
    {
        public static CdoToken Instance = new CdoToken();
        public CdoToken() : base(ECssTokenType.CDO)
        {
        }

        public override string Encode() => "<!--";
    }
}
