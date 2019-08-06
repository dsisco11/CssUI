using CssUI.DOM.CustomElements;
using CssUI.DOM.Exceptions;
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

            get => DOMCommon.Get_First_Element_Child_OfType<HTMLTableCaptionElement>(this);
            set
            {
                CEReactions.Wrap_CEReaction(this, () =>
                {
                    HTMLTableCaptionElement target = caption;
                    if (target != null)
                    {
                        removeChild(target);
                    }

                    Node first = firstChild;
                    if (first == null)
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

        /// <summary>
        /// Returns the table's thead element.
        /// Can be set, to replace the thead element.If the new value is not a thead element, throws a "HierarchyRequestError" DOMException.
        /// </summary>
        [CEReactions] public HTMLTableHeadElement tHead
        {/* Docs: https://html.spec.whatwg.org/multipage/tables.html#dom-table-thead */

            get => DOMCommon.Get_First_Element_Child_OfType<HTMLTableHeadElement>(this);
            set
            {
                CEReactions.Wrap_CEReaction(this, () => 
                {
                    HTMLTableHeadElement firstOfType = tHead;
                    if (firstOfType != null)
                    {
                        removeChild(firstOfType);
                    }

                    /* the new value, if not null, must be inserted immediately before the first element in the table element that is neither a caption element nor a colgroup element, if any, 
                     * or at the end of the table if there are no such elements. */
                    if (value != null)
                    {
                        if (!(value is HTMLTableHeadElement))
                        {
                            throw new HierarchyRequestError();
                        }

                        Element before = firstElementChild;
                        while (before != null)
                        {
                            if (!(before is HTMLTableCaptionElement || before is HTMLTableColGroupElement))
                            {
                                insertBefore(value, before);
                                return;
                            }
                        }

                        /* ...or at the end of the table if there are no such elements. */
                        appendChild(value);
                    }
                });
            }
        }

        /// <summary>
        /// Returns the table's tfoot element.
        /// Can be set, to replace the tfoot element.If the new value is not a tfoot element, throws a "HierarchyRequestError" DOMException.
        /// </summary>
        [CEReactions] public HTMLTableFootElement tFoot
        {/* Docs: https://html.spec.whatwg.org/multipage/tables.html#dom-table-tfoot */
            get => DOMCommon.Get_First_Element_Child_OfType<HTMLTableFootElement>(this);
            set
            {
                CEReactions.Wrap_CEReaction(this, () =>
                {
                    /* On setting, if the new value is null or a tfoot element, the first tfoot element child of the table element, 
                     * if any, must be removed, and the new value, if not null, must be inserted at the end of the table. 
                     * If the new value is neither null nor a tfoot element, then a "HierarchyRequestError" DOMException must be thrown instead. */
                    var firstOfType = tFoot;
                    if (firstOfType != null)
                    {
                        removeChild(firstOfType);
                    }

                    if (value != null && !(value is HTMLTableFootElement))
                    {
                        throw new HierarchyRequestError();
                    }

                    appendChild(value);
                });
            }
        }

        /// <summary>
        /// Returns a collection of the tbody elements of the table.
        /// </summary>
        public IReadOnlyCollection<HTMLTableBodyElement> tBodies
        {/* Docs: https://html.spec.whatwg.org/multipage/tables.html#dom-table-tbodies */
            get => DOMCommon.Get_Children_OfType<HTMLTableBodyElement>(this);
        }

        /// <summary>
        /// Returns a collection of the tr elements of the table.
        /// </summary>
        public IReadOnlyCollection<HTMLTableRowElement> rows
        {/* Docs: https://html.spec.whatwg.org/multipage/tables.html#dom-table-rows */
            get => DOMCommon.Get_Children_OfType<HTMLTableRowElement>(this);
        }

        #endregion

        #region Constructor
        public HTMLTableElement(Document document, string localName, string prefix, string Namespace) : base(document, localName)
        {
        }
        #endregion

        /// <summary>
        /// Ensures the table has a caption element, and returns it.
        /// </summary>
        /// <returns></returns>
        public HTMLTableCaptionElement createCaption()
        {/* Docs: https://html.spec.whatwg.org/multipage/tables.html#dom-table-createcaption */
            var firstOfType = DOMCommon.Get_First_Element_Child_OfType<HTMLTableCaptionElement>(this);

            if (firstOfType != null)
            {
                return firstOfType;
            }

            var newChild = new HTMLTableCaptionElement(nodeDocument);
            insertFirst(newChild);

            return newChild;
        }

        /// <summary>
        /// Ensures the table has a thead element, and returns it.
        /// </summary>
        /// <returns></returns>
        public HTMLTableHeadElement createTHead()
        {/* Docs: https://html.spec.whatwg.org/multipage/tables.html#dom-table-createthead */

            var firstOfType = DOMCommon.Get_First_Element_Child_OfType<HTMLTableHeadElement>(this);

            if (firstOfType != null)
            {
                return firstOfType;
            }

            var newChild = new HTMLTableHeadElement(nodeDocument);

            Element before = firstElementChild;
            while (before != null)
            {
                if (!(before is HTMLTableCaptionElement || before is HTMLTableColGroupElement))
                {
                    insertBefore(newChild, before);
                    return newChild;
                }
            }

            appendChild(newChild);
            return newChild;
        }

        /// <summary>
        /// Ensures the table has a tfoot element, and returns it.
        /// </summary>
        /// <returns></returns>
        public HTMLTableFootElement createTFoot()
        {/* Docs: https://html.spec.whatwg.org/multipage/tables.html#dom-table-createtfoot */

            var firstOfType = DOMCommon.Get_First_Element_Child_OfType<HTMLTableFootElement>(this);

            if (firstOfType != null)
            {
                return firstOfType;
            }


            var newChild = new HTMLTableFootElement(nodeDocument);
            appendChild(newChild);
            return newChild;
        }

        /// <summary>
        /// Ensures the table has a tbody element, and returns it.
        /// </summary>
        /// <returns></returns>
        public HTMLTableBodyElement createTBody()
        {/* Docs: https://html.spec.whatwg.org/multipage/tables.html#dom-table-createtbody */

            /* The createTBody() method must table-create a new tbody element, insert it immediately after the last tbody element child in the table element, 
             * if any, or at the end of the table element if the table element has no tbody element children, 
             * and then must return the new tbody element. */
            var lastOfType = DOMCommon.Get_Last_Element_Child_OfType<HTMLTableBodyElement>(this);
            var newChild = new HTMLTableBodyElement(nodeDocument);

            if (lastOfType != null)
            {
                var nextToLast = lastOfType.nextElementSibling;
                if (nextToLast != null)
                {
                    insertBefore(newChild, nextToLast);
                    return newChild;
                }
            }

            appendChild(newChild);
            return newChild;
        }

        /// <summary>
        /// Creates a tr element, along with a tbody if required, inserts them into the table at the position given by the argument, and returns the tr.
        /// The position is relative to the rows in the table.The index −1, which is the default if the argument is omitted, is equivalent to inserting at the end of the table.
        /// If the given position is less than −1 or greater than the number of rows, throws an "IndexSizeError" DOMException.
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

        /// <summary>
        /// Ensures the table does not have a caption element.
        /// </summary>
        [CEReactions] public void deleteCaption()
        {/* Docs: https://html.spec.whatwg.org/multipage/tables.html#dom-table-deletecaption */

            CEReactions.Wrap_CEReaction(this, () =>
            {
                var firstOfType = DOMCommon.Get_First_Element_Child_OfType<HTMLTableCaptionElement>(this);
                if (firstOfType != null)
                {
                    removeChild(firstOfType);
                }
            });
        }

        /// <summary>
        /// Ensures the table does not have a thead element.
        /// </summary>
        [CEReactions] public void deleteTHead()
        {/* Docs: https://html.spec.whatwg.org/multipage/tables.html#dom-table-deletethead */

            CEReactions.Wrap_CEReaction(this, () =>
            {
                var firstOfType = DOMCommon.Get_First_Element_Child_OfType<HTMLTableHeadElement>(this);
                if (firstOfType != null)
                {
                    removeChild(firstOfType);
                }
            });
        }

        /// <summary>
        /// Ensures the table does not have a tfoot element.
        /// </summary>
        [CEReactions] public void deleteTFoot()
        {/* Docs: https://html.spec.whatwg.org/multipage/tables.html#dom-table-deletetfoot */

            CEReactions.Wrap_CEReaction(this, () =>
            {
                var firstOfType = DOMCommon.Get_First_Element_Child_OfType<HTMLTableFootElement>(this);
                if (firstOfType != null)
                {
                    removeChild(firstOfType);
                }
            });
        }

        /// <summary>
        /// Removes the tr element with the given position in the table.
        /// The position is relative to the rows in the table.The index −1 is equivalent to deleting the last row of the table.
        /// If the given position is less than −1 or greater than the index of the last row, or if there are no rows, throws an "IndexSizeError" DOMException.
        /// </summary>
        /// <param name="index"></param>
        [CEReactions] public void deleteRow(int index)
        {/* Docs: https://html.spec.whatwg.org/multipage/tables.html#dom-table-deleterow */

            CEReactions.Wrap_CEReaction(this, () =>
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
