using CssUI.DOM;
using CssUI.DOM.CustomElements;

namespace CssUI.HTML
{
    /// <summary>
    /// The colgroup element represents a group of one or more columns in the table that is its parent, if it has a parent and that is a table element.
    /// </summary>
    [MetaElement("colgroup")]
    public class HTMLTableColGroupElement : HTMLTableSectionElement
    {/* Docs: https://html.spec.whatwg.org/multipage/tables.html#the-colgroup-element */

        #region Definition
        public override EContentCategories Categories => EContentCategories.None;
        #endregion

        #region Constructors
        public HTMLTableColGroupElement(Document document) : base(document, "colgroup")
        {
        }

        public HTMLTableColGroupElement(Document document, string localName) : base(document, localName)
        {
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Number of columns spanned by the element
        /// </summary>
        [CEReactions]
        public uint span
        {
            get => MathExt.Clamp(getAttribute(EAttributeName.Span).Get_UInt(), 1, 1000);
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.Span, AttributeValue.From_Integer(value)));
        }
        #endregion
    }
}
