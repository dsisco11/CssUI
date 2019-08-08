using CssUI.DOM;

namespace CssUI.HTML
{
    [MetaElement("body")]
    public sealed class HTMLBodyElement : HTMLElement
    {/* Docs: https://html.spec.whatwg.org/multipage/sections.html#the-body-element */

        #region Definition
        public override EContentCategories Categories => EContentCategories.Flow;
        #endregion

        #region Constructor
        public HTMLBodyElement(Document document) : base(document, "body")
        {
        }
        #endregion

    }
}
