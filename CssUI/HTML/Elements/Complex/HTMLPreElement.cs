using CssUI.DOM;

namespace CssUI.HTML
{
    /// <summary>
    /// The pre element represents a block of preformatted text, in which structure is represented by typographic conventions rather than by elements.
    /// </summary>
    [MetaElement("pre")]
    public class HTMLPreElement : HTMLElement
    {/* Docs: https://html.spec.whatwg.org/multipage/grouping-content.html#htmlpreelement */

        #region Definition
        public override EContentCategories Categories => EContentCategories.Flow | EContentCategories.Palpable;
        #endregion

        #region Constructors
        public HTMLPreElement(Document document) : this(document, "pre")
        {
        }
        
        public HTMLPreElement(Document document, string localName) : base(document, localName)
        {
        }
        #endregion

    }
}
