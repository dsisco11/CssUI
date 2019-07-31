using CssUI.DOM.Events;

namespace CssUI.DOM
{
    public interface IViewport : IEventTarget
    {
        Document document { get; }
    }
}
