namespace CssUI.DOM.Exceptions
{
    public class DomSyntaxError : DOMException
    {
        public DomSyntaxError(string message = "")
            : base(message, "SyntaxError")
        {
        }
    }
}
