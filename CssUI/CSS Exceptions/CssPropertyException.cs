namespace CssUI
{
    public class CssPropertyException : CssException
    {
        public CssPropertyException(string Message) : base("CssProperty", Message)
        {
        }

        public CssPropertyException(params object[] Args) : base("CssProperty", Args)
        {
        }
    }
}
