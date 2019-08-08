using CssUI.DOM;

namespace CssUI.HTML
{
    /// <summary>
    /// The head element represents a collection of metadata for the Document.
    /// </summary>
    [MetaElement("head")]
    public sealed class HTMLHeadElement : HTMLElement
    {/* Docs:https://html.spec.whatwg.org/multipage/semantics.html#the-head-element */

        #region Definition
        public override EContentCategories Categories => EContentCategories.None;
        #endregion

        #region Constructors
        public HTMLHeadElement(Document document) : base(document, "head")
        {
        }
        #endregion
    }
}
