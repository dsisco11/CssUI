using CssUI.DOM;

namespace CssUI.HTML
{
    /// <summary>
    /// The q element represents some phrasing content quoted from another source.
    /// </summary>
    [MetaElement("q")]
    public class HTMLQElement : HTMLQuoteElement
    {/* Docs: https://html.spec.whatwg.org/multipage/text-level-semantics.html#the-q-element */

        #region Definition
        public override EContentCategories Categories => EContentCategories.Flow | EContentCategories.Phrasing | EContentCategories.Palpable;
        #endregion

        #region Constructors
        public HTMLQElement(Document document) : this(document, "q")
        {
        }
        public HTMLQElement(Document document, string localName) : base(document, localName)
        {
        }
        #endregion
    }
}
