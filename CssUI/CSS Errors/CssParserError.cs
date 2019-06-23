using System;

namespace CssUI.CSS
{
    public class CssParserError : CssException
    {
        public CssParserError(string Message) : base("Parser", Message)
        {
        }

        public CssParserError(params object[] Args) : base("Parser", Args)
        {
        }
    }
}
