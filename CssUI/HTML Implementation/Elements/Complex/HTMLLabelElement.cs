using CssUI.DOM;
using CssUI.DOM.CustomElements;
using CssUI.DOM.Enums;

namespace CssUI.HTML
{
    /// <summary>
    /// The label element represents a caption in a user interface. The caption can be associated with a specific form control, known as the label element's labeled control, either using the for attribute, or by putting the form control inside the label element itself.
    /// </summary>
    [MetaElement("label")]
    public class HTMLLabelElement : HTMLElement
    {/* Docs: https://html.spec.whatwg.org/multipage/forms.html#the-label-element */

        #region Definition
        public override EContentCategories Categories => EContentCategories.Flow | EContentCategories.Phrasing | EContentCategories.Interactive | EContentCategories.Palpable;
        #endregion

        #region Constructors
        public HTMLLabelElement(Document document) : base(document, "label")
        {
        }

        public HTMLLabelElement(Document document, string localName) : base(document, localName)
        {
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Returns the form owner of the form control that is associated with this element.
        /// Returns null if there isn't one.
        /// </summary>
        public HTMLFormElement form
        {/* Docs: https://html.spec.whatwg.org/multipage/forms.html#dom-label-form */
            get
            {
                if (control == null) return null;
                if (!(control is FormAssociatedElement formElement)) return null;

                return formElement.form;
            }
        }

        /// <summary>
        /// Returns the form control that is associated with this element.
        /// </summary>
        public HTMLElement control
        {/* Docs: https://html.spec.whatwg.org/multipage/forms.html#attr-label-for */
            get
            {
                if (hasAttribute(EAttributeName.For, out Attr forAttr) && !ReferenceEquals(null, forAttr.Value))
                {
                    return (HTMLElement)ownerDocument.getElementByID(forAttr?.Value?.Get_Atomic());
                }

                return (HTMLElement)DOMCommon.Get_Nth_Descendant(this, 1, FilterIsLableable.Instance, ENodeFilterMask.SHOW_ELEMENT);
            }
        }
        #endregion

        #region Content Attributes
        [CEReactions]
        public string htmlFor
        {/* Docs: https://html.spec.whatwg.org/multipage/forms.html#dom-label-htmlfor */
            get => getAttribute(EAttributeName.For)?.Get_String();
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.For, AttributeValue.From_String(value)));
        }
        #endregion

    }
}
