using System.Linq;
using System.Collections.Generic;

namespace CssUI.DOM
{
    public class HTMLTableRowElement : HTMLElement
    {/* Docs: https://html.spec.whatwg.org/multipage/tables.html#htmltablerowelement */

        #region Accessors
        /// <summary>
        /// Returns the position of the row in the table's rows list.
        /// Returns −1 if the element isn't in a table.
        /// </summary>
        public int rowIndex
        {/* Docs: https://html.spec.whatwg.org/multipage/tables.html#dom-tr-rowindex */
            get
            {
                /* The rowIndex attribute must, if this element has a parent table element, 
                 * or a parent tbody, thead, or tfoot element and a grandparent table element, 
                 * return the index of this tr element in that table element's rows collection. 
                 * If there is no such table element, then the attribute must return −1. */
                if (parentElement is HTMLTableElement parentTable)
                {
                    return parentTable.rows.ToList().IndexOf(this);
                }
                else if (parentElement is HTMLTableSectionElement && parentElement.parentElement is HTMLTableElement grandparentTable)
                {
                    return grandparentTable.rows.ToList().IndexOf(this);
                }

                return -1;
            }
        }

        /// <summary>
        /// Returns the position of the row in the table section's rows list.
        /// Returns −1 if the element isn't in a table section.
        /// </summary>
        public int sectionRowIndex
        {
            get
            {
                /* The sectionRowIndex attribute must, if this element has a parent table, tbody, thead, or tfoot element, 
                 * return the index of the tr element in the parent element's rows collection (for tables, that's HTMLTableElement's rows collection; for table sections, that's HTMLTableSectionElement's rows collection). 
                 * If there is no such parent element, then the attribute must return −1. */

                if (parentElement is HTMLTableElement parentTable)
                {
                    return parentTable.rows.ToList().IndexOf(this);
                }
                else if (parentElement is HTMLTableSectionElement parentSection)
                {
                    return parentSection.rows.ToList().IndexOf(this);
                }

                return -1;
            }
        }

        /// <summary>
        /// Returns an HTMLCollection of the td and th elements of the row.
        /// </summary>
        public ICollection<HTMLTableCellElement> cells
        {/* Docs: https://html.spec.whatwg.org/multipage/tables.html#dom-tr-cells */
            get
            {
                /* The cells attribute must return an HTMLCollection rooted at this tr element, whose filter matches only td and th elements that are children of the tr element. */
                LinkedList<HTMLTableCellElement> cellList = new LinkedList<HTMLTableCellElement>();
                Element child = firstElementChild;
                while (!ReferenceEquals(null, child))
                {
                    if (child is HTMLTableCellElement childCell)
                    {
                        cellList.AddLast(childCell);
                    }

                    child = child.nextElementSibling;
                }

                return cellList;
            }
        }
        #endregion

        #region Constructor
        public HTMLTableRowElement(Document document) : base(document, "tr")
        {
        }

        public HTMLTableRowElement(Document document, string localName) : base(document, localName)
        {
        }
        #endregion


        public HTMLTableCellElement insertCell(long index = -1);
        [CEReactions] public void deleteCell(long index);
    }
}
