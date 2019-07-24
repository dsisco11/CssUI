using CssUI.DOM.CustomElements;
using CssUI.DOM.Nodes;
using System.Collections.Generic;

namespace CssUI.DOM
{
    public class HTMLSelectElement : HTMLElement, IFormAssociatedElement
    {/* Docs: https://html.spec.whatwg.org/multipage/form-elements.html#htmlselectelement */
        #region Properties
        readonly HTMLFormElement form;
        #endregion

        #region Constructor
        public HTMLSelectElement(Document document) : base(document, "select")
        {
        }
        public HTMLSelectElement(Document document, string localName) : base(document, localName)
        {
        }
        #endregion

        [CEReactions] public string autocomplete;
        [CEReactions] public bool autofocus;
        [CEReactions] public bool disabled;
        [CEReactions] public bool multiple;
        [CEReactions] public string name;
        [CEReactions] public bool required;
        [CEReactions] public ulong size;

        public readonly string type;

        public readonly IReadOnlyCollection<HTMLOptionElement> options;
        [CEReactions] public ulong length;
        public Element item(ulong index);
        public HTMLOptionElement namedItem(string name);
        [CEReactions] public void add((HTMLOptionElement or HTMLOptGroupElement) element, optional(HTMLElement or long)? before = null);
        [CEReactions] public void remove(); // ChildNode overload
        [CEReactions] public void remove(long index);
        [CEReactions] public setter void (ulong index, HTMLOptionElement? option);

        public readonly IReadOnlyCollection<Element> selectedOptions;
        public long selectedIndex;
        public string value;

        public readonly bool willValidate;
        public readonly EValidityState validity;
        public readonly string validationMessage;
        public bool checkValidity();
        public bool reportValidity();
        public void setCustomValidity(string error);

        public IReadOnlyCollection<Node> labels;
    }
}
