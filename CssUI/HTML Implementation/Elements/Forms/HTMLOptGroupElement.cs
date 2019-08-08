using CssUI.DOM;
using CssUI.DOM.CustomElements;

namespace CssUI.HTML
{
    /// <summary>
    /// The optgroup element represents a group of option elements with a common label.
    /// </summary>
    [MetaElement("optgroup")]
    public class HTMLOptGroupElement : HTMLElement
    {/* Docs: https://html.spec.whatwg.org/multipage/form-elements.html#the-optgroup-element */

        #region Definition
        public override EContentCategories Categories => EContentCategories.None;
        #endregion

        #region Constructors
        public HTMLOptGroupElement(Document document) : base(document, "optgroup")
        {
        }

        public HTMLOptGroupElement(Document document, string localName) : base(document, localName)
        {
        }
        #endregion

        #region Content Attributes
        /// <summary>
        /// The label attribute must be specified. Its value gives the name of the group, for the purposes of the user interface.
        /// </summary>
        [CEReactions]
        public string label
        {
            get => getAttribute(EAttributeName.Label)?.Get_String();
            set => CEReactions.Wrap_CEReaction(this, () => setAttribute(EAttributeName.Label, AttributeValue.From_String(value)));
        }
        #endregion
    }
}
