using CssUI.DOM.Events;
using System.Collections.Generic;

namespace CssUI.HTML
{
    /// <summary>
    /// Some elements, not all of them form-associated, are categorized as labelable elements. These are elements that can be associated with a label element.
    /// </summary>
    public interface ILableableElement : IEventTarget
    {
        HTMLFormElement form { get; set; }

        /// <summary>
        /// Returns a NodeList of all the label elements that the form control is associated with.
        /// </summary>
        IReadOnlyCollection<HTMLLabelElement> labels { get; }
    }
}
