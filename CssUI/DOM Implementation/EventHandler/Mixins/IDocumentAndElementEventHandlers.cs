using CssUI.DOM.Events;

namespace CssUI.DOM
{
    public interface IDocumentAndElementEventHandlers
    {
        event EventHandler oncopy;
        event EventHandler oncut;
        event EventHandler onpaste;
    }
}
