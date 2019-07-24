using CssUI.DOM.CustomElements;

namespace CssUI.DOM
{
    public class HTMLOptGroupElement : HTMLElement
    {/* Docs: https://html.spec.whatwg.org/multipage/form-elements.html#the-optgroup-element */
        #region Accessors
        //[CEReactions] public bool disabled /* Redundant, HTMLElement already implements this */

        [CEReactions] public string label
        {
            get => getAttribute(EAttributeName.Label)?.Get_String();
            set => CEReactions.Wrap_CEReaction(this, () => setAttribute(EAttributeName.Label, AttributeValue.From_String(value)));
        }
        #endregion


        #region Constructors
        public HTMLOptGroupElement(Document document) : base(document, "optgroup")
        {
        }

        public HTMLOptGroupElement(Document document, string localName) : base(document, localName)
        {
        }
        #endregion
    }
}
