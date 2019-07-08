namespace CssUI.DOM.Exceptions
{
    public class IndexSizeError : DOMException
    {
        public IndexSizeError(string message = "")
            : base(message, "IndexSizeError")
        {
        }
    }
}
