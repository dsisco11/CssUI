
namespace CssUI.DOM
{
    public interface IFormAssociatedElement
    {/* Docs: https://html.spec.whatwg.org/multipage/forms.html#form-associated-element */

        /// <summary>
        /// A form-associated element can have a relationship with a form element, which is called the element's form owner. If a form-associated element is not associated with a form element, its form owner is said to be null.
        /// </summary>
        HTMLFormElement form { get; set; }
        bool bParserInserted { get; set; }
    }
}
