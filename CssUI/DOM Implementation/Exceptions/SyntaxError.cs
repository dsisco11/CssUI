namespace CssUI.DOM.Exceptions
{
    public class SyntaxError : DOMException
    {
        public SyntaxError(string message = "")
            : base(message, "SyntaxError")
        {
        }
    }
}
