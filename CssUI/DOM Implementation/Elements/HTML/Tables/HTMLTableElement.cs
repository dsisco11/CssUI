using CssUI.DOM.Nodes;
using System.Collections.Generic;

namespace CssUI.DOM
{
    public class HTMLTableElement : HTMLElement
    {
        #region Accessors

        /// <summary>
        /// Returns the table's caption element.
        /// Can be set, to replace the caption element.
        /// /// </summary>
        [CEReactions] HTMLTableCaptionElement caption
        {/* Docs: https://html.spec.whatwg.org/multipage/tables.html#dom-table-caption */
            get
            {
                Element child = firstElementChild;
                while (!ReferenceEquals(null, child))
                {
                    if (child is HTMLTableCaptionElement)
                        return child as HTMLTableCaptionElement;

                    child = child.nextElementSibling;
                }

                return null;
            }
            set
            {
                HTMLTableCaptionElement target = caption;
                if (!ReferenceEquals(null, target))
                {
                    removeChild(target);
                }

                Node first = firstChild;
                if (ReferenceEquals(null, first))
                {
                    appendChild(value);
                }
                else
                {
                    insertBefore(firstChild, value);
                }
            }
        }

        [CEReactions] HTMLTableSectionElement tHead;
        [CEReactions] HTMLTableSectionElement tFoot;

        IEnumerable<HTMLElement> tBodies
        {
            get
            {

            }
        }

        public IEnumerable<HTMLElement> rows
        {
            get
            {

            }
        }

        #endregion

        #region Constructor
        public HTMLTableElement(Document document, string localName, string prefix, string Namespace) : base(document, localName)
        {
        }
        #endregion

        HTMLTableCaptionElement createCaption();
        HTMLTableHeadElement createTHead();
        HTMLTableBodyElement createTBody();
        HTMLTableFootElement createTFoot();
        HTMLTableRowElement insertRow(long index = -1);

        [CEReactions] void deleteCaption();

        [CEReactions] void deleteTHead();

        [CEReactions] void deleteTFoot();

        [CEReactions] void deleteRow(long index);
    }
}
