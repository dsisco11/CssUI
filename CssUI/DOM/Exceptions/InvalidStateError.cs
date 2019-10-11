namespace CssUI.DOM.Exceptions
{
    public class InvalidStateError : DOMException
    {
        public InvalidStateError(string message = "")
            : base(message, "InvalidStateError")
        {
        }
    }
}
