using System.Collections.Generic;
using System.Text;

namespace CssUI.CSS
{
    public class CssQualifiedRule : CssComponent
    {
        //public List<CssComponent> Prelude = new List<CssComponent>();
        public List<CssToken> Prelude = new List<CssToken>();
        public CssSimpleBlock Block = new CssSimpleBlock(new BracketOpenToken());

        //public CssQualifiedRule() : base(ECssComponent.QualifiedRule)
        public CssQualifiedRule() : base(ECssTokenType.QualifiedRule)
        {
        }

        public override string Encode()
        {
            StringBuilder sb = new StringBuilder();
            foreach (CssToken t in Prelude) { sb.Append(t.Encode()); }
            sb.AppendLine();
            sb.Append(Block.Encode());

            return sb.ToString();
        }
    }
}
