using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssUI.CSS
{
    public class CssSelectorError : CssException
    {
        public CssSelectorError(string Message) : base("Selector", Message)
        {
        }

        public CssSelectorError(params object[] Args) : base("Selector", Args)
        {
        }
    }
}
