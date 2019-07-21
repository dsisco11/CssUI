﻿using CssUI.DOM.Internal;
using System.Linq;

namespace CssUI.DOM
{
    public class HTMLTableCellElement : HTMLElement
    {/* Docs: https://html.spec.whatwg.org/multipage/tables.html#htmltablecellelement */
        #region Properties
        /// <summary>
        /// Number of columns that the cell is to span
        /// </summary>
        [CEReactions] public uint colSpan
        {/* The td and th elements may have a colspan content attribute specified, whose value must be a valid non-negative integer greater than zero and less than or equal to 1000. */
            get => (uint)MathExt.Clamp(getAttribute_Numeric(EAttributeName.ColSpan), 0, 1000);
            set => ReactionCommon.Wrap_CEReaction(this, () => setAttribute(EAttributeName.ColSpan, MathExt.Clamp(value, 0, 1000).ToString()));
        }
        /// <summary>
        /// Number of rows that the cell is to span
        /// </summary>
        [CEReactions] public uint rowSpan
        {
            /* The td and th elements may also have a rowspan content attribute specified, whose value must be a valid non-negative integer less than or equal to 65534. 
             * For this attribute, the value zero means that the cell is to span all the remaining rows in the row group. */
            get => (uint)MathExt.Clamp(getAttribute_Numeric(EAttributeName.RowSpan), 0, ushort.MaxValue);
            set => ReactionCommon.Wrap_CEReaction(this, () => setAttribute(EAttributeName.RowSpan, MathExt.Clamp(value, 0, ushort.MaxValue).ToString()));
        }
        /// <summary>
        /// The header cells for this cell
        /// </summary>
        [CEReactions]
        public string headers
        {/* Docs: https://html.spec.whatwg.org/multipage/tables.html#attr-tdth-headers */
            get => getAttribute(EAttributeName.Headers);
            set => ReactionCommon.Wrap_CEReaction(this, () => setAttribute(EAttributeName.Headers, value));
        }

        /// <summary>
        /// Returns the position of the cell in the row's cells list. This does not necessarily correspond to the x-position of the cell in the table, since earlier cells might cover multiple rows or columns.
        /// Returns −1 if the element isn't in a row.
        /// </summary>
        public int cellIndex
        {/* Docs:  */
            get
            {
                /* The cellIndex IDL attribute must, if the element has a parent tr element, return the index of the cell's element in the parent element's cells collection. 
                 * If there is no such parent element, then the attribute must return −1. */
                if (parentElement is HTMLTableRowElement parentRow)
                {
                    return parentRow.cells.ToList().IndexOf(this);
                }

                return -1;
            }
        }
        #endregion

        #region Constructors
        public HTMLTableCellElement(Document document) : base(document, "td")
        {
        }

        public HTMLTableCellElement(Document document, string localName) : base(document, localName)
        {
        }
        #endregion
    }
}