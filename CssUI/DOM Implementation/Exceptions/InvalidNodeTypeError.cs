namespace CssUI.DOM.Exceptions
{
    public class InvalidNodeTypeError : DOMException
    {
        public InvalidNodeTypeError(string message = "")
            : base(message, "InvalidNodeTypeError")
        {
        }
    }
}
