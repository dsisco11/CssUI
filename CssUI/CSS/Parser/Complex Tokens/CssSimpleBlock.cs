using CssUI.CSS.Parser;
using System.Collections.Generic;
using System.Text;

namespace CssUI.CSS
{
    public class CssSimpleBlock : CssComponent
    {
        public readonly CssToken StartToken;
        //public List<CssComponent> Values = new List<CssComponent>();
        public List<CssToken> Values = new List<CssToken>();

        //public CssSimpleBlock(CssToken StartToken) : base(ECssComponent.SimpleBlock)
        public CssSimpleBlock(CssToken StartToken) : base(ECssTokenType.SimpleBlock)
        {
            this.StartToken = StartToken;
        }


        public override string Encode()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(StartToken.Encode());
            sb.AppendLine();
            foreach (CssToken t in Values) { sb.Append(t.Encode()); }
            sb.AppendLine();
            switch(StartToken.Type)
            {
                case ECssTokenType.Parenth_Open:
                    sb.Append(")");
                    break;
                case ECssTokenType.Bracket_Close:
                    sb.Append("}");
                    break;
                case ECssTokenType.SqBracket_Close:
                    sb.Append("]");
                    break;
            }

            return sb.ToString();
        }
    }

}
