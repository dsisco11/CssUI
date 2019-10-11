namespace CssUI.DOM.Exceptions
{
    public class HierarchyRequestError : DOMException
    {
        public HierarchyRequestError(string message = "")
            : base(message, "HierarchyRequestError")
        {
        }
    }
}
