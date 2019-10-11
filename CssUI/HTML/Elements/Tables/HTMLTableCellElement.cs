using CssUI.DOM;
using System.Linq;

namespace CssUI.HTML
{
    /// <summary>
    /// The td element represents a data cell in a table.
    /// </summary>
    [MetaElement("td")]
    public class HTMLTableCellElement : HTMLElement
    {/* Docs: https://html.spec.whatwg.org/multipage/tables.html#htmltablecellelement */

        #region Definition
        public override EContentCategories Categories => EContentCategories.SectioningRoot;
        #endregion

        #region Constructors
        public HTMLTableCellElement(Document document) : base(document, "td")
        {
        }

        public HTMLTableCellElement(Document document, string localName) : base(document, localName)
        {
        }
        #endregion

        #region Content Attributes
        /// <summary>
        /// Number of columns that the cell is to span
        /// </summary>
        [CEReactions]
        public uint colSpan
        {/* The td and th elements may have a colspan content attribute specified, whose value must be a valid non-negative integer greater than zero and less than or equal to 1000. */
            get => MathExt.Clamp(getAttribute(EAttributeName.ColSpan)?.AsUInt() ?? 0, 0, 1000);
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.ColSpan, AttributeValue.From(MathExt.Clamp(value, 0, 1000))));
        }
        /// <summary>
        /// Number of rows that the cell is to span
        /// </summary>
        [CEReactions]
        public uint rowSpan
        {
            /* The td and th elements may also have a rowspan content attribute specified, whose value must be a valid non-negative integer less than or equal to 65534. 
             * For this attribute, the value zero means that the cell is to span all the remaining rows in the row group. */
            get => MathExt.Clamp(getAttribute(EAttributeName.RowSpan)?.AsUInt() ?? 0, 0, ushort.MaxValue);
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.RowSpan, AttributeValue.From(MathExt.Clamp(value, 0, ushort.MaxValue))));
        }
        /// <summary>
        /// The header cells for this cell
        /// </summary>
        [CEReactions]
        public string headers
        {/* Docs: https://html.spec.whatwg.org/multipage/tables.html#attr-tdth-headers */
            get => getAttribute(EAttributeName.Headers)?.AsString();
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.Headers, AttributeValue.From(value)));
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
    }
}
