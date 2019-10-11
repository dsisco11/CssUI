namespace CssUI.DOM.Exceptions
{
    public class NotSupportedError : DOMException
    {
        public NotSupportedError(string message = "")
            : base(message, "NotSupportedError")
        {
        }
    }
}
