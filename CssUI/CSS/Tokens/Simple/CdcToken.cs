
using CssUI.CSS.Parser;

namespace CssUI.CSS
{
    public sealed class CdcToken : CssToken
    {
        public static CdcToken Instance = new CdcToken();
        public CdcToken() : base(ECssTokenType.CDC)
        {
        }

        public override string Encode() => "-->";
    }
}
