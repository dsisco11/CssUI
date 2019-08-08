using CssUI.DOM;

namespace CssUI.HTML
{
    /// <summary>
    /// The caption element represents the title of the table that is its parent, if it has a parent and that is a table element.
    /// </summary>
    [MetaElement("caption")]
    public class HTMLTableCaptionElement : HTMLElement
    {/* Docs: https://html.spec.whatwg.org/multipage/tables.html#the-caption-element */

        #region Definition
        public override EContentCategories Categories => EContentCategories.None;
        #endregion

        #region Constructors
        public HTMLTableCaptionElement(Document document) : base(document, "caption")
        {
        }
        public HTMLTableCaptionElement(Document document, string localName) : base(document, localName)
        {
        }
        #endregion
    }
}
