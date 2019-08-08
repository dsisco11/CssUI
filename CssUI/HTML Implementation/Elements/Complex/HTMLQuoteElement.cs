using CssUI.DOM;
using CssUI.DOM.CustomElements;

namespace CssUI.HTML
{
    /// <summary>
    /// The blockquote element represents a section that is quoted from another source.
    /// </summary>
    [MetaElement("blockquote")]
    public class HTMLQuoteElement : HTMLElement
    {/* Docs: https://html.spec.whatwg.org/multipage/grouping-content.html#the-blockquote-element */

        #region Definition
        public override EContentCategories Categories => EContentCategories.Flow | EContentCategories.SectioningRoot | EContentCategories.Palpable;
        #endregion

        #region Constructors
        public HTMLQuoteElement(Document document) : this(document, "blockquote")
        {
        }

        public HTMLQuoteElement(Document document, string localName) : base(document, localName)
        {
        }
        #endregion

        #region Content Attributes
        /// <summary>
        /// Link to the source of the quotation or more information about the edit
        /// </summary>
        [CEReactions]
        public string cite
        {
            get => getAttribute(EAttributeName.Cite).Get_String();
            set => CEReactions.Wrap_CEReaction(this, () => setAttribute(EAttributeName.Cite, AttributeValue.From_String(value)));
        }
        #endregion
    }
}
