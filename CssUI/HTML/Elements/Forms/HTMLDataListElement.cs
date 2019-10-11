using CssUI.DOM;

namespace CssUI.HTML
{
    /// <summary>
    /// The datalist element represents a set of option elements that represent predefined options for other controls.
    /// </summary>
    [MetaElement("datalist")]
    public class HTMLDataListElement : HTMLElement
    {/* Docs: https://html.spec.whatwg.org/multipage/form-elements.html#the-datalist-element */

        #region Definition
        public override EContentCategories Categories => EContentCategories.Flow | EContentCategories.Phrasing;
        #endregion

        #region Properties
        /* Docs: https://html.spec.whatwg.org/multipage/form-elements.html#dom-datalist-options */
        public readonly HTMLCollection<HTMLOptionElement> options;
        #endregion

        #region Constructor
        public HTMLDataListElement(Document document) : this(document, "datalist")
        {
        }

        public HTMLDataListElement(Document document, string localName) : base(document, localName)
        {
            options = new HTMLCollection<HTMLOptionElement>(this);
        }
        #endregion
    }
}
