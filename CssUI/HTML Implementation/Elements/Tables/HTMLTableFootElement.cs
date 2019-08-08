using CssUI.DOM;

namespace CssUI.HTML
{
    /// <summary>
    /// The tfoot element represents the block of rows that consist of the column summaries (footers) for the parent table element, if the tfoot element has a parent and it is a table.
    /// </summary>
    [MetaElement("tfoot")]
    public class HTMLTableFootElement : HTMLTableSectionElement
    {/* Docs: https://html.spec.whatwg.org/multipage/tables.html#the-tfoot-element */

        #region Definition
        public override EContentCategories Categories => EContentCategories.None;
        #endregion

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
