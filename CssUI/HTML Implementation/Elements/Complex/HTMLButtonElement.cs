using CssUI.DOM;
using CssUI.DOM.CustomElements;
using CssUI.DOM.Enums;
using CssUI.DOM.Events;
using CssUI.DOM.Nodes;
using System.Collections.Generic;

namespace CssUI.HTML
{
    /// <summary>
    /// The button element represents a button labeled by its contents.
    /// </summary>
    [MetaElement("button")]
    public class HTMLButtonElement : FormAssociatedElement, IListedElement, ILableableElement, ISubmittableElement, IAutoCapitalizeInheritingElement
    {/* Docs: https://html.spec.whatwg.org/multipage/form-elements.html#the-button-element */

        #region Definition
        public override EContentCategories Categories => EContentCategories.Flow | EContentCategories.Phrasing | EContentCategories.Interactive | EContentCategories.Palpable;
        #endregion

        #region Properties
        private readonly NodeFilter labelFilter;
        #endregion

        #region Constructors
        public HTMLButtonElement(Document document) : this(document, "button")
        {
        }

        public HTMLButtonElement(Document document, string localName) : base(document, localName)
        {
            labelFilter = new FilterLabelFor(this);
        }
        #endregion

        #region Accessors
        public IReadOnlyCollection<HTMLLabelElement> labels => DOMCommon.Get_Descendents_OfType<HTMLLabelElement>(form, labelFilter, ENodeFilterMask.SHOW_ELEMENT);
        #endregion

        #region Content Attributes
        [CEReactions] public bool autofocus
        {
            get => hasAttribute(EAttributeName.Autofocus);
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => toggleAttribute(EAttributeName.Autofocus, value));
        }

        [CEReactions] public string formAction
        {
            get
            {
                if (!hasAttribute(EAttributeName.FormAction, out Attr attr))
                {
                    return nodeDocument.URL;
                }
                
                return attr.Value.Get_String();
            }
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.FormAction, AttributeValue.From_String(value)));
        }

        [CEReactions] public EEncType formEnctype
        {
            get => getAttribute(EAttributeName.FormEncType).Get_Enum<EEncType>();
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.FormEncType, AttributeValue.From_Enum(value)));
        }

        [CEReactions] public EFormMethod formMethod
        {
            get => getAttribute(EAttributeName.FormMethod).Get_Enum<EFormMethod>();
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.FormMethod, AttributeValue.From_Enum(value)));
        }

        [CEReactions] public bool formNoValidate
        {
            get => hasAttribute(EAttributeName.FormNoValidate);
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => toggleAttribute(EAttributeName.FormNoValidate, value));
        }

        [CEReactions] public string formTarget
        {
            get => getAttribute(EAttributeName.FormTarget).Get_String();
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.FormTarget, AttributeValue.From_String(value)));
        }

        [CEReactions] public string name
        {
            get => getAttribute(EAttributeName.Name).Get_String();
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.Name, AttributeValue.From_String(value)));
        }

        [CEReactions] public new EButtonType type
        {
            get => getAttribute(EAttributeName.Type).Get_Enum<EButtonType>();
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => { setAttribute(EAttributeName.Type, AttributeValue.From_Enum(value)); });
        }

        #endregion

        internal override bool has_activation_behaviour => true;
        internal override void activation_behaviour(Event @event)
        {
            base.activation_behaviour(@event);

            switch (type)
            {
                case EButtonType.Reset:
                    {
                        if (form != null && nodeDocument.Is_FullyActive)
                        {
                            FormCommon.Reset_Form(form);
                        }
                    }
                    break;
                case EButtonType.Submit:
                    {
                        if (form != null && nodeDocument.Is_FullyActive)
                        {
                            FormCommon.Submit_Form(form, this);
                        }
                    }
                    break;

                default:
                    break;
            }
        }
    }
}
