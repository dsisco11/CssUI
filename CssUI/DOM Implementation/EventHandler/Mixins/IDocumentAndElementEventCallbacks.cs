using CssUI.DOM.Events;

namespace CssUI.DOM
{
    public interface IDocumentAndElementEventCallbacks
    {
        event EventCallback onCopy;
        event EventCallback onCut;
        event EventCallback onPaste;
    }
}
