using System;
using System.Collections.Generic;
using System.Text;

namespace CssUI.CSS
{
    internal class CssFunction : CssComponent
    {
        public readonly string Name;
        //public List<CssComponent> Value = new List<CssComponent>();
        public List<CssToken> Arguments = new List<CssToken>();

        #region Constructors
        //public CssFunction(string Name) : base(ECssComponent.Function)
        public CssFunction(string Name) : base(ECssTokenType.Function)
        {
            this.Name = Name;
        }
        #endregion

        public override string Encode()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Name);
            sb.Append("(");
            foreach (CssToken t in Arguments) { sb.Append(t.Encode()); }
            sb.Append(")");

            return sb.ToString();
        }
    }
}
