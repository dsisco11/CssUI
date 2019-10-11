using CssUI.DOM;

namespace CssUI.HTML
{
    /// <summary>
    /// The tbody element represents a block of rows that consist of a body of data for the parent table element, if the tbody element has a parent and it is a table.
    /// </summary>
    [MetaElement("tbody")]
    public class HTMLTableBodyElement : HTMLTableSectionElement
    {/* Docs: https://html.spec.whatwg.org/multipage/tables.html#the-tbody-element */

        #region Definition
        public override EContentCategories Categories => EContentCategories.None;
        #endregion

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
