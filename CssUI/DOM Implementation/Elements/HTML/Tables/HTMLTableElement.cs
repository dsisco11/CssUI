using CssUI.DOM.Exceptions;
using CssUI.DOM.Internal;
using CssUI.DOM.Nodes;
using System.Collections.Generic;
using System.Linq;

namespace CssUI.DOM
{
    public class HTMLTableElement : HTMLElement
    {/* Docs: https://html.spec.whatwg.org/multipage/tables.html#the-table-element */
        #region Accessors

        /// <summary>
        /// Returns the table's caption element.
        /// Can be set, to replace the caption element.
        /// /// </summary>
        [CEReactions] public HTMLTableCaptionElement caption
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
                ReactionsCommon.Wrap_CEReaction(this, () =>
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
                });
            }
        }

        [CEReactions] public HTMLTableSectionElement tHead
        {/* Docs:  */
            get
            {

            }
        }

        [CEReactions] public HTMLTableSectionElement tFoot;

        public IReadOnlyCollection<HTMLElement> tBodies
        {
            get
            {

            }
        }

        public IReadOnlyCollection<HTMLElement> rows
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

        public HTMLTableCaptionElement createCaption();
        public HTMLTableHeadElement createTHead();
        public HTMLTableBodyElement createTBody();
        public HTMLTableFootElement createTFoot();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index">Index to insert new row at</param>
        /// <exception cref="IndexSizeError"></exception>
        /// <returns></returns>
        public HTMLTableRowElement insertRow(int index = -1)
        {/* Docs: https://html.spec.whatwg.org/multipage/tables.html#dom-table-insertrow */
            int rowCount = rows.Count;
            if (index < -1 || index > rowCount)
            {
                throw new IndexSizeError();
            }

            if (rowCount <= 0 && tBodies.Count <= 0)
            {
                var body = new HTMLTableBodyElement(nodeDocument);
                var row = new HTMLTableRowElement(nodeDocument);

                body.appendChild(row);
                appendChild(body);

                return row;
            }

            if (rowCount <= 0)
            {
                var row = new HTMLTableRowElement(nodeDocument);
                tBodies.Last().appendChild(row);
                return row;
            }

            if (index == -1 || index == rowCount)
            {
                var row = new HTMLTableRowElement(nodeDocument);
                var parent = rows.Last().parentNode;
                parent.appendChild(row);
                return row;
            }

            var newRow = new HTMLTableRowElement(nodeDocument);
            var indexRow = rows.ElementAt(index);
            indexRow.parentNode.insertBefore(newRow, indexRow);
            return newRow;
        }


        [CEReactions] public void deleteCaption();

        [CEReactions] public void deleteTHead();

        [CEReactions] public void deleteTFoot();

        [CEReactions] public void deleteRow(int index)
        {/* Docs: https://html.spec.whatwg.org/multipage/tables.html#dom-table-deleterow */
            ReactionsCommon.Wrap_CEReaction(this, () =>
            {
                int rowCount = rows.Count;
                if (index < -1 || index > rowCount)
                {
                    throw new IndexSizeError();
                }

                if (index == -1)
                {
                    if (rowCount > 0)
                    {
                        var row = rows.Last();
                        row.parentElement.removeChild(row);
                    }
                }
                else
                {
                    var row = rows.ElementAt(index);
                    row.parentElement.removeChild(row);
                }
            });
        }

    }
}
