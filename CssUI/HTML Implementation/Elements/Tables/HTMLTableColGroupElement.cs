using CssUI.DOM;
using CssUI.DOM.CustomElements;

namespace CssUI.HTML
{
    /// <summary>
    /// The colgroup element represents a group of one or more columns in the table that is its parent, if it has a parent and that is a table element.
    /// </summary>
    public class HTMLTableColGroupElement : HTMLTableSectionElement
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
        public HTMLTableColGroupElement(Document document) : base(document, "colgroup")
        {
        }

        public HTMLTableColGroupElement(Document document, string localName) : base(document, localName)
        {
        }
        #endregion
    }
}
