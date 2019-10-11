using CssUI.DOM;
using CssUI.DOM.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace CssUI.HTML
{
    public abstract class HTMLTableSectionElement : HTMLElement
    {/* Docs: https://html.spec.whatwg.org/multipage/tables.html#htmltablesectionelement */

        #region Constructor
        public HTMLTableSectionElement(Document document, string localName) : base(document, localName)
        {
        }
        #endregion

        #region Accessors
        public IReadOnlyCollection<HTMLElement> rows
        {/* The rows attribute must return an HTMLCollection rooted at this element, whose filter matches only tr elements that are children of this element. */
            get
            {
                LinkedList<HTMLElement> rowList = new LinkedList<HTMLElement>();
                Element child = firstElementChild;
                while (child != null)
                {
                    if (child is HTMLTableRowElement rowElement)
                        rowList.AddLast(rowElement);

                    child = child.nextElementSibling;
                }

                return rowList;
            }
        }
        #endregion


        /// <summary>
        /// Creates a tr element, inserts it into the table section at the position given by the argument, and returns the tr.
        /// The position is relative to the rows in the table section.The index −1, which is the default if the argument is omitted, is equivalent to inserting at the end of the table section.
        /// If the given position is less than −1 or greater than the number of rows, throws an "IndexSizeError" DOMException.
        /// </summary>
        /// <param name="index">Index to insert new row at</param>
        /// <exception cref="IndexSizeError"></exception>
        /// <returns></returns>
        public HTMLTableRowElement insertRow(int index = -1)
        {/* Docs: https://html.spec.whatwg.org/multipage/tables.html#dom-tbody-insertrow */
            int rowCount = rows.Count;
            if (index < -1 || index > rowCount)
            {
                throw new IndexSizeError();
            }

            var tableRow = new HTMLTableRowElement(nodeDocument);

            if (index == -1 || index == rowCount)
            {
                appendChild(tableRow);
            }
            else
            {
                var before = rows.ElementAt(index);
                insertBefore(tableRow, before);
            }

            return tableRow;
        }

        /// <summary>
        /// Removes the tr element with the given position in the table section.
        /// The position is relative to the rows in the table section.The index −1 is equivalent to deleting the last row of the table section.
        /// If the given position is less than −1 or greater than the index of the last row, or if there are no rows, throws an "IndexSizeError" DOMException.
        /// </summary>
        /// <exception cref="IndexSizeError"></exception>
        /// <param name="index"></param>
        [CEReactions]
        public void deleteRow(int index)
        {/* Docs: https://html.spec.whatwg.org/multipage/tables.html#dom-tbody-deleterow */
            CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () =>
            {
                int rowCount = rows.Count;
                if (index < -1 || index > rowCount)
                {
                    throw new IndexSizeError();
                }

                if (index == -1)
                {
                    var row = rows.Last();
                    removeChild(row);
                }
                else
                {
                    var row = rows.ElementAt(index);
                    removeChild(row);
                }
            });
        }

        #region Internal Utilities
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void _delete_row(int index)
        {
        }
        #endregion
    }
}
