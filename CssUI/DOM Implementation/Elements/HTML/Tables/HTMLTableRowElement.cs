using System.Linq;
using System.Collections.Generic;
using CssUI.DOM.Exceptions;
using CssUI.DOM.Internal;
using CssUI.DOM.CustomElements;

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
        public IReadOnlyCollection<HTMLTableCellElement> cells
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

        /// <summary>
        /// Creates a td element, inserts it into the table row at the position given by the argument, and returns the td.
        /// The position is relative to the cells in the row.The index −1, which is the default if the argument is omitted, is equivalent to inserting at the end of the row.
        /// If the given position is less than −1 or greater than the number of cells, throws an "IndexSizeError" DOMException.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public HTMLTableCellElement insertCell(int index = -1)
        {/* Docs: https://html.spec.whatwg.org/multipage/tables.html#dom-tr-insertcell */
            int cellCount = cells.Count;
            if (index < -1 || index > cellCount)
            {
                throw new IndexSizeError();
            }

            var tableCell = new HTMLTableCellElement(nodeDocument);

            if (index == -1 || index == cellCount)
            {
                appendChild(tableCell);
            }
            else
            {
                var before = cells.ElementAt(index);
                insertBefore(tableCell, before);
            }

            return tableCell;
        }

        /// <summary>
        /// Removes the td or th element with the given position in the row.
        /// The position is relative to the cells in the row.The index −1 is equivalent to deleting the last cell of the row.
        /// If the given position is less than −1 or greater than the index of the last cell, or if there are no cells, throws an "IndexSizeError" DOMException.
        /// </summary>
        /// <param name="index"></param>
        [CEReactions] public void deleteCell(int index)
        {/* Docs: https://html.spec.whatwg.org/multipage/tables.html#dom-tr-deletecell */
            CEReactions.Wrap_CEReaction(this, () =>
            {
                int cellCount = cells.Count;
                if (index < -1 || index > cellCount)
                {
                    throw new IndexSizeError();
                }

                if (index == -1 && cellCount > 0)
                {
                    removeChild(cells.Last());
                }
                else
                {
                    var targetCell = cells.ElementAt(index);
                    removeChild(targetCell);
                }
            });
        }
    }
}
