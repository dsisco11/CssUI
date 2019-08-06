namespace CssUI.DOM
{
    /// <summary>
    /// The tbody element represents a block of rows that consist of a body of data for the parent table element, if the tbody element has a parent and it is a table.
    /// </summary>
    public class HTMLTableBodyElement : HTMLTableSectionElement
    {
        #region Constructors
        public HTMLTableBodyElement(Document document) : base(document, "tbody")
        {
        }

        public HTMLTableBodyElement(Document document, string localName) : base(document, localName)
        {
        }
        #endregion
    }
}
