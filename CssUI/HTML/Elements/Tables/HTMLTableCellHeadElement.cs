using CssUI.DOM;

namespace CssUI.HTML
{
    /// <summary>
    /// The th element represents a header cell in a table.
    /// </summary>
    [MetaElement("th")]
    public class HTMLTableCellHeadElement : HTMLTableCellElement
    {/* Docs: https://html.spec.whatwg.org/multipage/tables.html#the-th-element */

        #region Definition
        public override EContentCategories Categories => EContentCategories.None;
        #endregion

        #region Constructors
        public HTMLTableCellHeadElement(Document document) : base(document, "th")
        {
        }

        public HTMLTableCellHeadElement(Document document, string localName) : base(document, localName)
        {
        }
        #endregion

        #region Content Attribute
        [CEReactions]
        public string scope
        {
            get => getAttribute(EAttributeName.Scope)?.AsString();
            set => setAttribute(EAttributeName.Scope, AttributeValue.From(value));
        }
        [CEReactions]
        public string abbr
        {
            get => getAttribute(EAttributeName.Abbr).AsString();
            set => setAttribute(EAttributeName.Abbr, AttributeValue.From(value));
        }
        #endregion
    }
}
