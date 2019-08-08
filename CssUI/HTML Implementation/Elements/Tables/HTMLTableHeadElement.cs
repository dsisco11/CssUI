using CssUI.DOM;

namespace CssUI.HTML
{
    /// <summary>
    /// The thead element represents the block of rows that consist of the column labels (headers) for the parent table element, if the thead element has a parent and it is a table.
    /// </summary>
    [MetaElement("thead")]
    public class HTMLTableHeadElement : HTMLTableSectionElement
    {/* Docs: https://html.spec.whatwg.org/multipage/tables.html#the-thead-element */

        #region Definition
        public override EContentCategories Categories => EContentCategories.None;
        #endregion

        #region Constructors
        public HTMLTableHeadElement(Document document) : base(document, "thead")
        {
        }

        public HTMLTableHeadElement(Document document, string localName) : base(document, localName)
        {
        }
        #endregion
    }
}
