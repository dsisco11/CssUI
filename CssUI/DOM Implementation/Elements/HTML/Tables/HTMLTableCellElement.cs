namespace CssUI.DOM
{
    public class HTMLTableCellElement : HTMLElement
    {/* Docs: https://html.spec.whatwg.org/multipage/tables.html#htmltablecellelement */
        #region Properties
        /// <summary>
        /// Number of columns that the cell is to span
        /// </summary>
        [CEReactions] public ulong colSpan
        {/* The td and th elements may have a colspan content attribute specified, whose value must be a valid non-negative integer greater than zero and less than or equal to 1000. */
            get => (ulong)MathExt.Clamp(getAttribute_Numeric(EAttributeName.ColSpan), 0, 1000);
            set => setAttribute(EAttributeName.ColSpan, MathExt.Clamp(value, 0, 1000).ToString());
        }
        /// <summary>
        /// Number of rows that the cell is to span
        /// </summary>
        [CEReactions] public ulong rowSpan
        {
            /* The td and th elements may also have a rowspan content attribute specified, whose value must be a valid non-negative integer less than or equal to 65534. 
             * For this attribute, the value zero means that the cell is to span all the remaining rows in the row group. */
            get => (ulong)MathExt.Clamp(getAttribute_Numeric(EAttributeName.RowSpan), 0, ushort.MaxValue);
            set => setAttribute(EAttributeName.RowSpan, MathExt.Clamp(value, 0, ushort.MaxValue).ToString());
        }
        /// <summary>
        /// The header cells for this cell
        /// </summary>
        [CEReactions] public string headers;
        public readonly long cellIndex;
        #endregion

        #region Constructor
        public HTMLTableCellElement(Document document) : base(document, "td")
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="document">The <see cref="Document"/> object this element resides in</param>
        /// <param name="localName">The local part of the qualified name (the html tag)</param>
        public HTMLTableCellElement(Document document, string localName) : base(document, localName)
        {
        }
        #endregion
    }
}
