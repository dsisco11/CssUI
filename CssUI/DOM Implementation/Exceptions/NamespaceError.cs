namespace CssUI.DOM.Exceptions
{
    public class NamespaceError : DOMException
    {
        public NamespaceError(string message = "")
            : base(message, "NamespaceError")
        {
        }
    }
}
