
namespace CssUI
{
    public class CssParserException : CssException
    {
        public CssParserException(string Message) : base("Parser", Message)
        {
        }

        public CssParserException(params object[] Args) : base("Parser", Args)
        {
        }
    }
}
