﻿using CssUI.DOM;
using CssUI.DOM.Exceptions;

namespace CssUI.HTML
{
    /* XXX: I dont see too much of a point in the past names map aside from static code references being able to reference elements even after they change identities.
     * So I classify the past names map as low priority */
    /* Docs: https://html.spec.whatwg.org/multipage/forms.html#past-names-map */



    /// <summary>
    /// The form element represents a hyperlink that can be manipulated through a collection of form-associated elements, some of which can represent editable values that can be submitted to a server for processing.
    /// </summary>
    [MetaElement("form")]
    public class HTMLFormElement : HTMLElement
    {/* Docs: https://html.spec.whatwg.org/multipage/forms.html#htmlformelement */

        #region Definition
        public override EContentCategories Categories => EContentCategories.Flow | EContentCategories.Palpable;
        #endregion

        #region Properties
        private bool bLockedForReset = false;
        public bool bConstructingEntryList { get; internal set; } = false;
        public bool bFiringSubmissionEvents { get; internal set; } = false;
        public readonly DOMTokenList relList;
        public readonly HTMLFormControlsCollection elements;
        #endregion

        #region Constructor
        public HTMLFormElement(Document document) : this(document, "form")
        {
        }

        public HTMLFormElement(Document document, string localName) : base(document, localName)
        {
            relList = new DOMTokenList(this, EAttributeName.Rel);
            elements = new HTMLFormControlsCollection(this);
        }
        #endregion

        #region Content Attributes
        [CEReactions]
        public string acceptCharset
        {
            get => getAttribute(EAttributeName.AcceptCharset).AsString();
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.AcceptCharset, AttributeValue.From(value)));
        }

        [CEReactions]
        public string action
        {
            get => getAttribute(EAttributeName.Action)?.AsString() ?? nodeDocument.URL;
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.Action, AttributeValue.From(value)));
        }

        [CEReactions]
        public EAutoComplete autocomplete
        {
            get => getAttribute(EAttributeName.Autocomplete).AsEnum<EAutoComplete>();
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.Autocomplete, AttributeValue.From(value)));
        }

        [CEReactions]
        public EEncType enctype
        {
            get => getAttribute(EAttributeName.EncType).AsEnum<EEncType>();
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.EncType, AttributeValue.From(value)));
        }

        [CEReactions]
        public EEncType encoding
        {
            get => getAttribute(EAttributeName.EncType).AsEnum<EEncType>();
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.EncType, AttributeValue.From(value)));
        }

        [CEReactions]
        public EFormMethod method
        {
            get => getAttribute(EAttributeName.Method).AsEnum<EFormMethod>();
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.Method, AttributeValue.From(value)));
        }

        [CEReactions]
        public string name
        {
            get => getAttribute(EAttributeName.Name).AsString();
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.Name, AttributeValue.From(value)));
        }

        [CEReactions]
        public bool noValidate
        {
            get => hasAttribute(EAttributeName.NoValidate);
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => toggleAttribute(EAttributeName.NoValidate, value));
        }

        [CEReactions]
        public string target
        {
            get => getAttribute(EAttributeName.Target).AsString();
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.Target, AttributeValue.From(value)));
        }

        [CEReactions]
        public string rel
        {
            get => getAttribute(EAttributeName.Rel).AsString();
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.Rel, AttributeValue.From(value)));
        }
        #endregion

        #region Accessors
        public int length => elements.length;

        public Element this[int index] => elements[index];

        /// <summary>
        /// Returns the item with ID or name name from the collection.
        /// If there are multiple matching items, then a RadioNodeList object containing all those elements is returned.
        /// </summary>
        public dynamic this[string name] => elements[name];
        #endregion


        public void submit()
        {
            FormCommon.Submit_Form(this, this, true);
        }

        public void requestSubmit(HTMLElement submitter)
        {
            if (submitter != null)
            {
                if (!DOMCommon.Is_Submit_Button(submitter))
                {
                    throw new TypeError();
                }

                if (submitter is IListedElement formElement && !ReferenceEquals(this, formElement.form))
                {
                    throw new NotFoundError();
                }
            }
            else
            {
                submitter = this;
            }

            FormCommon.Submit_Form(this, submitter);
        }

        [CEReactions]
        public void reset()
        {
            CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () =>
            {
                if (bLockedForReset) return;

                bLockedForReset = true;
                FormCommon.Reset_Form(this);
                bLockedForReset = false;
            });
        }

        public bool checkValidity()
        {
            return FormCommon.Statically_Validate_Form_Constraints(this, out _);
        }

        public bool reportValidity()
        {
            return FormCommon.Interactively_Validate_Form_Constraints(this);
        }
    }
}
