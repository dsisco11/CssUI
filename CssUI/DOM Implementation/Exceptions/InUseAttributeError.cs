namespace CssUI.DOM.Exceptions
{
    public class InUseAttributeError : DOMException
    {
        public InUseAttributeError(string message = "")
            : base(message, "InUseAttributeError")
        {
        }
    }
}
