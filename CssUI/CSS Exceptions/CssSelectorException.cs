
namespace CssUI
{
    public class CssSelectorException : CssException
    {
        public CssSelectorException(string Message) : base("Selector", Message)
        {
        }

        public CssSelectorException(params object[] Args) : base("Selector", Args)
        {
        }
    }
}
