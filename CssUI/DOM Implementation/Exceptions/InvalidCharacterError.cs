namespace CssUI.DOM.Exceptions
{
    public class InvalidCharacterError : DOMException
    {
        public InvalidCharacterError(string message = "")
            : base(message, "InvalidCharacterError")
        {
        }
    }
}
