using CssUI.DOM.Events;

namespace CssUI.DOM
{
    public interface INonDocumentTypeChildNode : IEventTarget
    {
        Element previousElementSibling { get; }
        Element nextElementSibling { get; }
    }
}
