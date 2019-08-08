using CssUI.DOM;

namespace CssUI.HTML
{
    /// <summary>
    /// The html element represents the root of an HTML document.
    /// </summary>
    [MetaElement("html")]
    public sealed class HTMLHtmlElement : HTMLElement
    {/* Docs: https://html.spec.whatwg.org/multipage/semantics.html#the-html-element */

        #region Definition
        public override EContentCategories Categories => throw new System.NotImplementedException();
        #endregion

        #region Constructors
        public HTMLHtmlElement(Document document) : base(document, "html")
        {
        }
        #endregion
    }
}
