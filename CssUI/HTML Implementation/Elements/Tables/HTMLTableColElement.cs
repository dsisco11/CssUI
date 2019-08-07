using CssUI.DOM;
using CssUI.DOM.CustomElements;

namespace CssUI.HTML
{
    /// <summary>
    /// If a col element has a parent and that is a colgroup element that itself has a parent that is a table element, then the col element represents one or more columns in the column group represented by that colgroup.
    /// </summary>
    public class HTMLTableColElement : HTMLTableSectionElement
    {
        #region Accessors
        /// <summary>
        /// Number of columns spanned by the element
        /// </summary>
        [CEReactions]
        public uint span
        {
            get => MathExt.Clamp(getAttribute(EAttributeName.Span).Get_UInt(), 1, 1000);
            set => CEReactions.Wrap_CEReaction(this, () => setAttribute(EAttributeName.Span, AttributeValue.From_Integer(value)));
        }
        #endregion

        #region Constructors
        public HTMLTableColElement(Document document) : base(document, "col")
        {
        }

        public HTMLTableColElement(Document document, string localName) : base(document, localName)
        {
        }
        #endregion
    }
}
