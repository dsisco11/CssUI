﻿namespace CssUI.DOM
{
    /// <summary>
    /// The tfoot element represents the block of rows that consist of the column summaries (footers) for the parent table element, if the tfoot element has a parent and it is a table.
    /// </summary>
    public class HTMLTableFootElement : HTMLTableSectionElement
    {
        #region Constructors
        public HTMLTableFootElement(Document document) : base(document, "tfoot")
        {
        }
        
        public HTMLTableFootElement(Document document, string localName) : base(document, localName)
        {
        }
        #endregion
    }
}
