
using System.Collections.Generic;

namespace CssUI.DOM
{
    public class HTMLTableRowElement : HTMLElement
    {/* Docs: https://html.spec.whatwg.org/multipage/tables.html#htmltablerowelement */
        #region Properties
        public readonly long rowIndex;
        public readonly long sectionRowIndex;
        public readonly IEnumerable<HTMLElement> cells;
        #endregion

        #region Constructor
        public HTMLTableRowElement(Document document, string localName) : base(document, localName)
        {
        }
        #endregion


        public HTMLTableCellElement insertCell(long index = -1);
        [CEReactions] public void deleteCell(long index);
    }
}
