using CssUI.DOM.CustomElements;
using CssUI.DOM.Nodes;
using System.Collections.Generic;
using System.Linq;

namespace CssUI.DOM
{
    public class HTMLSelectElement : FormAssociatedElement
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

        public readonly IReadOnlyCollection<HTMLOptionElement> selectedOptions;
        public long selectedIndex;

        public override string value
        {/* Docs: https://html.spec.whatwg.org/multipage/form-elements.html#dom-select-value */
            get
            {
                var selected = selectedOptions;
                if (selected.Count <= 0)
                {
                    return string.Empty;
                }

                return selected.First()?.value;
            }
        }

        public IReadOnlyCollection<HTMLLabelElement> labels
        {
            get => (IReadOnlyCollection<HTMLLabelElement>)DOMCommon.Get_Descendents(form, new FilterLabelFor(this), Enums.ENodeFilterMask.SHOW_ELEMENT);
        }
    }
}
