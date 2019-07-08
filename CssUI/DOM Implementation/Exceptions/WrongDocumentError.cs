namespace CssUI.DOM.Exceptions
{
    public class WrongDocumentError : DOMException
    {
        public WrongDocumentError(string message = "")
            : base(message, "WrongDocumentError")
        {
        }
    }
}
