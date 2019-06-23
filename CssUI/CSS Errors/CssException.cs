using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssUI.CSS
{
    public class CssException : Exception
    {
        protected CssException(string Prefix, params object[] Args) : base(string.Concat("[CSS]", Prefix, string.Join("", Args.Select(o => o.ToString()))))
        {
        }

        public CssException(string message) : base(string.Concat("[CSS]", message))
        {
        }

        public CssException(params object[] args) : base(string.Join("", args.Select(o => o.ToString())))
        {
        }
    }
}
