using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssUI.CSS
{
    public class CssSyntaxError : CssException
    {
        public CssSyntaxError(string Message) : base("Syntax", Message)
        {
        }

        public CssSyntaxError(params object[] Args) : base("Syntax", Args)
        {
        }
    }
}
