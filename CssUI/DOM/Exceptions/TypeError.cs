namespace CssUI.DOM.Exceptions
{
    public class TypeError : DOMException
    {
        public TypeError(string message = "")
            : base(message, "TypeError")
        {
        }
    }
}
