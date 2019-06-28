
namespace CssUI
{
    public class CssSyntaxErrorException : CssException
    {
        public CssSyntaxErrorException(string Message) : base("Syntax", Message)
        {
        }

        public CssSyntaxErrorException(params object[] Args) : base("Syntax", Args)
        {
        }
    }
}
