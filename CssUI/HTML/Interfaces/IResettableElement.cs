using CssUI.DOM.Events;

namespace CssUI.HTML
{
    /// <summary>
    /// Denotes elements that can be affected when a form element is reset.
    /// </summary>
    public interface IResettableElement : IEventTarget
    {
        void Reset();
    }
}
