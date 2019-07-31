
using CssUI.DOM.Events;

namespace CssUI.DOM
{
    /// <summary>
    /// Some elements, not all of them form-associated, are categorized as labelable elements. These are elements that can be associated with a label element.
    /// </summary>
    public interface ILableableElement : IEventTarget
    {
        HTMLFormElement form { get; set; }
    }
}
