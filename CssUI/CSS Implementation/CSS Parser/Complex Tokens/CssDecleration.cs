using CssUI.CSS.Parser;
using System.Collections.Generic;
using System.Text;

namespace CssUI.CSS
{
    public class CssDecleration : CssComponent
    {
        public readonly string Name;
        //public List<CssComponent> Values = new List<CssComponent>();
        public List<CssToken> Values = new List<CssToken>();
        public bool Important = false;

        //public CssDecleration(string Name) : base(ECssComponent.Decleration)
        public CssDecleration(string Name) : base(ECssTokenType.Decleration)
        {
            this.Name = Name;
        }
        
        public override string Encode()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Name);
            sb.Append(": ");
            foreach (CssToken t in Values) { sb.Append(t.Encode()); }
            if (Important) sb.Append("!important");
            sb.Append(";");

            return sb.ToString();
        }
    }
}
