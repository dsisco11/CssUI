using System.Collections.Generic;

namespace CssUI.DOM
{
    public class HTMLTableSectionElement : HTMLElement
    {/* Docs: https://html.spec.whatwg.org/multipage/tables.html#htmltablesectionelement */
        #region Accessors
        public IEnumerable<HTMLElement> rows;
        #endregion

        #region Constructor
        public HTMLTableSectionElement(Document document, string localName, string prefix, string Namespace) : base(document, localName)
        {
        }
        #endregion



        public HTMLTableRowElement insertRow(long index = -1);
        [CEReactions] void deleteRow(long index);
    }
}
