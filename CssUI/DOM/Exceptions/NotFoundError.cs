namespace CssUI.DOM.Exceptions
{
    public class NotFoundError : DOMException
    {
        public NotFoundError(string message = "")
            : base(message, "NotFoundError")
        {
        }
    }
}
