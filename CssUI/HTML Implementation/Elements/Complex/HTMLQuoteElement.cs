using CssUI.DOM;
using CssUI.DOM.CustomElements;

namespace CssUI.HTML
{
    public class HTMLQuoteElement : HTMLElement
    {/* Docs: https://html.spec.whatwg.org/multipage/grouping-content.html#the-blockquote-element */
        #region Properties
        #endregion

        #region Constructors
        public HTMLQuoteElement(Document document) : this(document, "quote")
        {
        }

        public HTMLQuoteElement(Document document, string localName) : base(document, localName)
        {
        }
        #endregion

        #region Content Attributes
        [CEReactions]
        public string cite
        {
            get => getAttribute(EAttributeName.Cite).Get_String();
            set => CEReactions.Wrap_CEReaction(this, () => setAttribute(EAttributeName.Cite, AttributeValue.From_String(value)));
        }
        #endregion
    }
}
