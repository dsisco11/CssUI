namespace CssUI.DOM.Exceptions
{
    public class EncodingError : DOMException
    {
        public EncodingError(string message = "")
            : base(message, "EncodingError")
        {
        }
    }
}
