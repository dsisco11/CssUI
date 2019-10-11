using CssUI.DOM.Events;

namespace CssUI.HTML
{
    /// <summary>
    /// Denotes elements that are listed in the form.elements and fieldset.elements APIs. These elements also have a form content attribute, and a matching form IDL attribute, that allow authors to specify an explicit form owner.
    /// </summary>
    public interface IListedElement : IEventTarget
    {
        HTMLFormElement form { get; set; }
    }
}
