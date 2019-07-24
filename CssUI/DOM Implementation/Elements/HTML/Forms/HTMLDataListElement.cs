using System.Collections.Generic;

namespace CssUI.DOM
{
    /// <summary>
    /// The datalist element represents a set of option elements that represent predefined options for other controls.
    /// </summary>
    public class HTMLDataListElement : HTMLElement
    {/* Docs: https://html.spec.whatwg.org/multipage/form-elements.html#the-datalist-element */
        #region Properties
        public IReadOnlyCollection<Element> options
        {/* Docs: https://html.spec.whatwg.org/multipage/form-elements.html#dom-datalist-options */
            get
            {
                return (IReadOnlyCollection<Element>)DOMCommon.Get_Descendents(this, new FilterElementType(typeof(HTMLOptionElement)), Enums.ENodeFilterMask.SHOW_ELEMENT);
            }
        }
        #endregion

        #region Constructor
        public HTMLDataListElement(Document document) : base(document, "datalist")
        {
        }

        public HTMLDataListElement(Document document, string localName) : base(document, localName)
        {
        }
        #endregion
    }
}
