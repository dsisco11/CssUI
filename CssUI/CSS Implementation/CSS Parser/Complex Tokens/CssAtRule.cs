using CssUI.CSS.Parser;
using System.Collections.Generic;
using System.Text;

namespace CssUI.CSS
{
    public class CssAtRule : CssComponent
    {
        public readonly string Name;
        //public List<CssComponent> Prelude = new List<CssComponent>();
        public List<CssToken> Prelude = new List<CssToken>();
        public CssSimpleBlock Block = null;

        //public CssAtRule(string Name) : base(ECssComponent.AtRule)
        public CssAtRule(string Name) : base(ECssTokenType.AtRule)
        {
            this.Name = Name;
        }

        public override string Encode()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("@");
            sb.Append(Name);
            foreach (CssToken t in Prelude) { sb.Append(t.Encode()); }
            sb.Append(Block.Encode());
            sb.Append(";");

            return sb.ToString();
        }
    }
}
