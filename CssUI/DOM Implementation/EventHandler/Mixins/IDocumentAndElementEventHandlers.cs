using CssUI.DOM.Events;

namespace CssUI.DOM
{
    public interface IDocumentAndElementEventHandlers
    {
        event EventCallback onCopy;
        event EventCallback onCut;
        event EventCallback onPaste;
    }
}
